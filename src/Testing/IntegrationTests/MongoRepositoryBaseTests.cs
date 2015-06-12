using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Ica.StackIt.Core;
using Ica.StackIt.Infrastructure.Mongo;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	// To run these tests, change the connection string in MongoDbTest.cs

	[Ignore]
	public class MongoRepositoryBaseTests : MongoDbTest
	{
		#region Setup/Teardown

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			SprocketCollection = GetCollection<Sprocket>();
			SprocketRepo = new SprocketRepository(SprocketCollection);
		}

		#endregion

		private MongoCollection<Sprocket> SprocketCollection { get; set; }
		private SprocketRepository SprocketRepo { get; set; }

		private class Sprocket : IEntity<Guid>
		{
			public string Name { get; set; }
			public ICollection<Sprocket> Sprockets { get; set; }
			public Guid Id { get; set; }
		}

		private class SprocketRepository : MongoRepositoryBase<Sprocket>
		{
			public SprocketRepository(MongoCollection<Sprocket> collection) : base(collection) {}
		}

		[Test]
		public void Add_Inserts()
		{
			var sprocket = new Sprocket
			{
				Id = Guid.NewGuid(),
				Name = "Outer",
				Sprockets = new[] {new Sprocket {Id = Guid.NewGuid(), Name = "Inner"}}
			};
			SprocketRepo.Add(sprocket);

			Sprocket found = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			found.Name.Should().Be("Outer");
			found.Sprockets.Single().Name.Should().Be("Inner");
		}

		[Test]
		public void DeleteEntity_Removes()
		{
			var sprocket = new Sprocket {Id = Guid.NewGuid(), Name = "DeleteMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Delete(sprocket);
			SprocketCollection.FindAll().Count(s => s.Id == sprocket.Id).Should().Be(0);
		}

		[Test]
		public void Delete_Removes()
		{
			var sprocket = new Sprocket {Id = Guid.NewGuid(), Name = "DeleteMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Delete(sprocket.Id);
			SprocketCollection.FindAll().Count(s => s.Id == sprocket.Id).Should().Be(0);
		}

		[Test]
		public void FindAll_ReturnsAll()
		{
			MongoCollection<Sprocket> sprocketCollection = GetCollection<Sprocket>();
			Sprocket[] expectedSprockets =
			{
				new Sprocket {Name = "One"},
				new Sprocket {Name = "Two"}
			};

			sprocketCollection.InsertBatch(expectedSprockets);

			var repo = new SprocketRepository(sprocketCollection);
			repo.FindAll().Select(x => x.Name).ShouldBeEquivalentTo(new[] {"One", "Two"});
		}

		[Test]
		public void FindById_Finds()
		{
			var sprocket = new Sprocket {Id = Guid.NewGuid(), Name = "FindMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Find(sprocket.Id).Name.Should().Be("FindMe");
		}

		[Test]
		public void Update_Saves()
		{
			var sprocket = new Sprocket
			{
				Id = Guid.NewGuid(),
				Name = "Outer",
				Sprockets = new[] {new Sprocket {Id = Guid.NewGuid(), Name = "Inner"}}
			};
			SprocketCollection.Insert(sprocket);

			sprocket = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			sprocket.Name = "Saved";
			SprocketRepo.Update(sprocket);

			Sprocket found = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			found.Name.Should().Be("Saved");
		}
	}
}