using System;

using FluentAssertions;

using Ica.StackIt.Core;
using Ica.StackIt.Infrastructure;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.InfrastructureTests
{
	public class QuiescingRepositoryTests
	{
		[Test]
		public void Add_Quiesces()
		{
			// Arrange
			var repoMock = new Mock<IRepository<QuiesceableEntity>>();
			var quiescingRepo = new QuiescingRepository<QuiesceableEntity>(repoMock.Object);
			var entity = new QuiesceableEntity();

			// Act
			quiescingRepo.Add(entity);

			// Assert
			repoMock.Verify(x => x.Add(It.Is((QuiesceableEntity e) => e.IsQuiesced)), Times.Once);
			entity.IsQuiesced.Should().BeTrue();
		}

		[Test]
		public void Update_Quiesces()
		{
			// Arrange
			var repoMock = new Mock<IRepository<QuiesceableEntity>>();
			var quiescingRepo = new QuiescingRepository<QuiesceableEntity>(repoMock.Object);
			var entity = new QuiesceableEntity();

			// Act
			quiescingRepo.Update(entity);

			// Assert
			repoMock.Verify(x => x.Update(It.Is((QuiesceableEntity e) => e.IsQuiesced)), Times.Once);
			entity.IsQuiesced.Should().BeTrue();
		}

		[Test]
		public void Nonquiesceable_NotQuiesced()
		{
			// Arrange
			var repoMock = new Mock<IRepository<NonquiesceableEntity>>();
			var quiescingRepo = new QuiescingRepository<NonquiesceableEntity>(repoMock.Object);
			var entity = new NonquiesceableEntity();

			// Act
			quiescingRepo.Update(entity);

			// Assert
			repoMock.Verify(x => x.Update(It.Is((NonquiesceableEntity e) => ! e.IsQuiesced)), Times.Once);
			entity.IsQuiesced.Should().BeFalse();
		}

		public class QuiesceableEntity : IEntity<Guid>, IQuiesceable
		{
			public Guid Id { get; private set; }
			public bool IsQuiesced { get; set; }

			public void Quiesce()
			{
				IsQuiesced = true;
			}
		}

		public class NonquiesceableEntity : IEntity<Guid>
		{
			public Guid Id { get; private set; }
			public bool IsQuiesced { get; set; }

			public void Quiesce()
			{
				IsQuiesced = true;
			}
		}
	}
}