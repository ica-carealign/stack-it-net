using System.Linq;

using Ica.StackIt.Application.Billing;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Billing
{
	public class BillingManagerTests
	{
		[Test]
		public void LoadsRecords()
		{
			// Arrange
			var repo = new Mock<IRepository<ResourceLedger>>();
			var billingManager = new BillingManager(repo.Object);

			var oldLineItem = new LineItem {ResourceId = "abc", BlendedCost = 1.0m, RecordId = "abc-rec"};
			var newLineItem = new LineItem {ResourceId = "def", BlendedCost = 2.0m, RecordId = "def-rec"};

			var ledger = new ResourceLedger {ResourceId = "abc"};
			ledger.GetPeriod("2015-03")[oldLineItem.RecordId] = oldLineItem.BlendedCost;

			repo.Setup(x => x.CreateQuery()).Returns(new[] {ledger}.AsQueryable());

			// Act
			billingManager.LoadLineItems(new[] {oldLineItem, newLineItem}, "2015-04");

			// Assert
			repo.Verify(x => x.Add(It.Is((ResourceLedger l) => l.ResourceId == "abc")), Times.Never);
			repo.Verify(x => x.Add(It.Is((ResourceLedger l) => l.ResourceId == "def")), Times.Once);

			repo.Verify(x => x.Update(It.Is((ResourceLedger l) =>
				l.ResourceId == "abc" && l.Periods["2015-04"]["abc-rec"] == 1.0m)), Times.Once);
			repo.Verify(x => x.Update(It.Is((ResourceLedger l) =>
				l.ResourceId == "def" && l.Periods["2015-04"]["def-rec"] == 2.0m)), Times.Once);
		}
	}
}