using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AutoMapper;

using FluentAssertions;

using Ica.StackIt.Application.Billing;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal;
using Ica.StackIt.Interactive.WebPortal.Controllers;
using Ica.StackIt.Interactive.WebPortal.Models;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Interactive.WebPortalTests
{
	internal class HomeControllerTests
	{
		private Mock<IRepository<Stack>> StackRepository { get; set; }
		private HomeController HomeController { get; set; }

		[SetUp]
		public void SetUp()
		{
			Startup.ConfigureAutomapper();
			StackRepository = new Mock<IRepository<Stack>>();
			var costCalculator = new CostCalculator(new Mock<IRepository<Instance>>().Object, new Mock<IRepository<ResourceLedger>>().Object);
			HomeController = new HomeController(StackRepository.Object, null, null, null, null, null, Mapper.Engine, null, null, null, null, costCalculator, null);
		}

		[Test]
		[Ignore("Needs session mock")]
		public void IndexListStacks()
		{
			// Arrange
			var halloweenStack = new Stack {Id = Guid.NewGuid()};
			var christmasStack = new Stack {Id = Guid.NewGuid()};

			var stacks = new List<Stack> {halloweenStack, christmasStack};
			StackRepository.Setup(x => x.FindAll()).Returns(stacks);

			// Act
			var actionResult = (ViewResultBase) HomeController.Index();
			List<StackOverviewViewModel> model = ((IEnumerable<StackOverviewViewModel>) actionResult.Model).ToList();

			// Assert
			model.Should().Contain(x => x.Id == halloweenStack.Id);
			model.Should().Contain(x => x.Id == christmasStack.Id);
		}
	}
}