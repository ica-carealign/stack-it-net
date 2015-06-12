using System;
using System.Collections.Generic;
using System.Web.Mvc;

using AutoMapper;

using FluentAssertions;

using Hangfire;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Controllers;
using Ica.StackIt.Interactive.WebPortal.Models;
using Ica.StackIt.Interactive.WebPortal.ViewModelHelpers;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Interactive.WebPortalTests
{
	internal class InstanceControllerTests
	{
		public Mock<IRepository<Stack>> StackRepositoryMock { get; set; }
		public Mock<IRepository<Instance>> InstanceRepositoryMock { get; set; }
		public Mock<IRepository<BaseImage>> BaseImageMock { get; set; }
		public Mock<IMappingEngine> MappingEngineMock { get; set; }
		public Mock<IBackgroundJobClient> JobClientMock { get; set; }
		public Mock<IStackViewModelHelper> ViewModelHelperMock { get; set; }
		private InstanceController Controller { get; set; }

		[SetUp]
		public void SetUp()
		{
			StackRepositoryMock = new Mock<IRepository<Stack>>();
			InstanceRepositoryMock = new Mock<IRepository<Instance>>();
			BaseImageMock = new Mock<IRepository<BaseImage>>();
			MappingEngineMock = new Mock<IMappingEngine>();
			JobClientMock = new Mock<IBackgroundJobClient>();
			ViewModelHelperMock = new Mock<IStackViewModelHelper>();
			Controller = new InstanceController(
				StackRepositoryMock.Object,
				InstanceRepositoryMock.Object,
				BaseImageMock.Object,
				MappingEngineMock.Object,
				JobClientMock.Object,
				ViewModelHelperMock.Object);
		}

		[Test]
		public void Index()
		{
			// Arrange
			Guid stackId = Guid.NewGuid();
			Guid instanceIdOne = Guid.NewGuid();
			var instanceOne = new Instance {Name = "InstanceOne"};

			Guid instanceIdTwo = Guid.NewGuid();
			var instanceTwo = new Instance {Name = "InstanceTwo"};
			var instanceIds = new List<Guid> {instanceIdOne, instanceIdTwo};
			var stack = new Stack
			{
				Id = stackId,
				Name = "hoojey",
				InstanceIds = instanceIds
			};

			StackRepositoryMock.Setup(x => x.Find(stackId)).Returns(stack);
			InstanceRepositoryMock.Setup(x => x.Find(instanceIdOne)).Returns(instanceOne);
			InstanceRepositoryMock.Setup(x => x.Find(instanceIdTwo)).Returns(instanceTwo);

			var instanceModels = new List<InstanceOverviewViewModel>
			{
				new InstanceOverviewViewModel {Name = instanceOne.Name},
				new InstanceOverviewViewModel {Name = instanceTwo.Name}
			};

			var expectedModel = new StackInstancesViewModel
			{
				StackName = stack.Name,
				Instances = instanceModels
			};

			MappingEngineMock.Setup(engine => engine
				.Map<Instance, InstanceOverviewViewModel>(instanceOne))
			                 .Returns(instanceModels[0]);

			MappingEngineMock.Setup(engine => engine
				.Map<Instance, InstanceOverviewViewModel>(instanceTwo))
			                 .Returns(instanceModels[1]);

			// Act
			var actionResult = Controller.Index(stack.Id) as ViewResultBase;

			// Assert
			(actionResult.Model as StackInstancesViewModel).ShouldBeEquivalentTo(expectedModel, options => options.Excluding(vm => vm.StackRecordId));
		}
	}
}