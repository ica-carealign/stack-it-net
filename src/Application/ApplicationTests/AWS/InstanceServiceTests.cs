using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

using FluentAssertions;

using Ica.StackIt.Application.AWS;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	internal class InstanceServiceTests
	{
		private Mock<IAmazonEC2> Ec2ClientMock { get; set; }
		private InstanceService InstanceService { get; set; }

		[SetUp]
		public void SetUp()
		{
			Ec2ClientMock = new Mock<IAmazonEC2>();
			InstanceService = new InstanceService(Ec2ClientMock.Object);

			SetUpResponse();
		}

		[Test]
		public void GetAllInstances()
		{
			// Arrange

			// Act
			IEnumerable<Instance> instances = InstanceService.GetAllInstances();

			// Assert
			instances.Select(x => x.InstanceId).Should().BeEquivalentTo(new List<string> {"one", "two", "six", "seven"});
		}

		[Test]
		public void GetInstance()
		{
			// Arrange
			const string instanceId = "eight";
			var request = new DescribeInstancesRequest { InstanceIds = new List<string> { instanceId } };
			var response = new DescribeInstancesResponse
			{
				Reservations = new List<Reservation>
				{
					new Reservation
					{
						Instances = new List<Instance>
						{
							GenerateInstance(instanceId, InstanceStateName.Running)
						}
					}
				}
			};
			Ec2ClientMock.Setup(x => x.DescribeInstances(It.Is<DescribeInstancesRequest>(
				req => req.InstanceIds.First() == request.InstanceIds.First()
			))).Returns(response);

			// Act
			Instance instance = InstanceService.GetInstance(instanceId);

			// Assert
			instance.State.Name.Should().Be(InstanceStateName.Running);
		}

		private void SetUpResponse()
		{
			var response = new DescribeInstancesResponse();
			var reservationOne = new Reservation
			{
				Instances = new List<Instance>
				{
					GenerateInstance("one", InstanceStateName.Running),
					GenerateInstance("two", InstanceStateName.Stopped),
					GenerateInstance("three", InstanceStateName.Terminated)
				}
			};

			var reservationTwo = new Reservation
			{
				Instances = new List<Instance>
				{
					GenerateInstance("four", InstanceStateName.Terminated),
					GenerateInstance("five", InstanceStateName.Terminated),
					GenerateInstance("six", InstanceStateName.Running),
					GenerateInstance("seven", InstanceStateName.Stopping)
				}
			};

			response.Reservations = new List<Reservation> {reservationOne, reservationTwo};
			Ec2ClientMock.Setup(x => x.DescribeInstances()).Returns(response);
		}

		private static Instance GenerateInstance(string instanceId, InstanceStateName stateName)
		{
			return new Instance {InstanceId = instanceId, State = new InstanceState {Name = stateName}};
		}
	}
}