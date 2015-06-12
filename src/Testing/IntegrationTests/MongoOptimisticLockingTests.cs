using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Ica.StackIt.Core;
using Ica.StackIt.Infrastructure;
using Ica.StackIt.Infrastructure.Mongo;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	// To run these tests, change the connection string in MongoDbTest.cs
	[Ignore]
	public class MongoOptimisticLockingTests : MongoDbTest
	{
		#region Setup/Teardown

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			SprocketCollection = GetCollection<VersionedSprocket>();
			SprocketRepo = new SprocketRepository(SprocketCollection);
		}

		#endregion

		private MongoCollection<VersionedSprocket> SprocketCollection { get; set; }
		private SprocketRepository SprocketRepo { get; set; }

		private class VersionedSprocket : IEntity<Guid>, IVersioned
		{
			public string Name { get; set; }
			public ICollection<VersionedSprocket> Sprockets { get; set; }
			public Guid Id { get; set; }
			public int Version { get; set; }
		}

		private class SprocketRepository : MongoRepositoryBase<VersionedSprocket>
		{
			public SprocketRepository(MongoCollection<VersionedSprocket> collection) : base(collection) {}
		}

		[Test]
		public void Add_Inserts()
		{
			var sprocket = new VersionedSprocket
			{
				Id = Guid.NewGuid(),
				Name = "Outer",
				Sprockets = new[] {new VersionedSprocket {Id = Guid.NewGuid(), Name = "Inner"}}
			};
			SprocketRepo.Add(sprocket);

			VersionedSprocket found = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			found.Name.Should().Be("Outer");
			found.Sprockets.Single().Name.Should().Be("Inner");
			found.Version.Should().Be(0);
		}

		[Test]
		public void DeleteEntity_Removes()
		{
			var sprocket = new VersionedSprocket {Id = Guid.NewGuid(), Name = "DeleteMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Delete(sprocket);
			SprocketCollection.FindAll().Count(s => s.Id == sprocket.Id).Should().Be(0);
		}

		[Test]
		public void Delete_Removes()
		{
			var sprocket = new VersionedSprocket {Id = Guid.NewGuid(), Name = "DeleteMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Delete(sprocket.Id);
			SprocketCollection.FindAll().Count(s => s.Id == sprocket.Id).Should().Be(0);
		}

		[Test]
		public void FindAll_ReturnsAll()
		{
			MongoCollection<VersionedSprocket> sprocketCollection = GetCollection<VersionedSprocket>();
			VersionedSprocket[] expectedSprockets =
			{
				new VersionedSprocket {Name = "One"},
				new VersionedSprocket {Name = "Two"}
			};

			sprocketCollection.InsertBatch(expectedSprockets);

			var repo = new SprocketRepository(sprocketCollection);
			repo.FindAll().Select(x => x.Name).ShouldBeEquivalentTo(new[] {"One", "Two"});
		}

		[Test]
		public void FindById_Finds()
		{
			var sprocket = new VersionedSprocket {Id = Guid.NewGuid(), Name = "FindMe"};
			SprocketCollection.Insert(sprocket);
			SprocketRepo.Find(sprocket.Id).Name.Should().Be("FindMe");
		}

		[Test]
		public void Update_Saves()
		{
			var sprocket = new VersionedSprocket
			{
				Id = Guid.NewGuid(),
				Name = "Outer",
				Sprockets = new[] {new VersionedSprocket {Id = Guid.NewGuid(), Name = "Inner"}}
			};
			SprocketCollection.Insert(sprocket);
			sprocket.Version.Should().Be(0);

			sprocket = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			sprocket.Name = "Saved";
			SprocketRepo.Update(sprocket);
			sprocket.Version.Should().Be(1);

			VersionedSprocket found = SprocketCollection.AsQueryable().Single(s => s.Id == sprocket.Id);
			found.Name.Should().Be("Saved");
			found.Version.Should().Be(1);
		}

		[Test]
		public void StaleUpdate_ThrowsException()
		{
			var sprocket = new VersionedSprocket {Id = Guid.NewGuid(), Version = 1};
			SprocketCollection.Insert(sprocket);

			sprocket.Version = 0;
			sprocket.Name = "Saved";
			SprocketRepo.Invoking(r => r.Update(sprocket))
			            .ShouldThrow<StaleObjectException>()
			            .WithMessage("*VersionedSprocket*")
			            .WithMessage("*updated by another transaction*");

			sprocket.Version.Should().Be(0);
		}

		[Test]
		public void VersionlessUpdate_Succeeds()
		{
			// Mimic the scenario where an object does not start out versioned but a version field is added later.
			// The repository should behave as if a document with no version has version number 0.
			var rawCollection = MongoDatabase.GetCollection(SprocketCollection.Name);
			Guid id = Guid.NewGuid();
			var doc = new BsonDocument(new Dictionary<string, object>()
			{
				{ "_id", id },
				{ "Name", "Versionless"}
			});
			rawCollection.Insert(doc);

			// sanity check
			var sprocket = SprocketCollection.AsQueryable().Single(s => s.Id == id);
			sprocket.Version.Should().Be(0);

			sprocket.Name = "Now versioned";
			SprocketRepo.Update(sprocket);
			sprocket.Version.Should().Be(1);
		}
	}
}