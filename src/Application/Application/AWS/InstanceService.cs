using System.Collections.Generic;
using System.Linq;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public class InstanceService : IInstanceService
	{
		private readonly IAmazonEC2 _ec2Client;

		public InstanceService(IAmazonEC2 ec2Client)
		{
			_ec2Client = ec2Client;
		}

		/// <summary>
		/// Get all instances that have not been terminated
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Instance> GetAllInstances()
		{
			var response = _ec2Client.DescribeInstances();
			return response.Reservations.SelectMany(x => x.Instances).Where(x => x.State.Name != InstanceStateName.Terminated);
		}

		public Instance GetInstance(string instanceId)
		{
			var request = new DescribeInstancesRequest {InstanceIds = new List<string> {instanceId}};
			var instance = _ec2Client.DescribeInstances(request).Reservations.SelectMany(x => x.Instances).FirstOrDefault();
			return instance;
		}

		public IEnumerable<SecurityGroup> GetSecurityGroups(Instance instance)
		{
			var securityGroupIds = instance.SecurityGroups.Select(x => x.GroupId);
			var request = new DescribeSecurityGroupsRequest {GroupIds = securityGroupIds.ToList()};
			var response = _ec2Client.DescribeSecurityGroups(request);
			return response.SecurityGroups;
		}

		public Image GetImage(Instance instance)
		{
			var imageId = instance.ImageId;
			var request = new DescribeImagesRequest {ImageIds = new List<string> {imageId}};
			var response = _ec2Client.DescribeImages(request);
			return response.Images.FirstOrDefault();
		}

		public void StopInstances(List<string> instanceIds)
		{
			var request = new StopInstancesRequest {InstanceIds = instanceIds};
			_ec2Client.StopInstances(request);
		}

		public void StartInstances(List<string> instanceIds)
		{
			var request = new StartInstancesRequest {InstanceIds = instanceIds};
			_ec2Client.StartInstances(request);
		}
	}
}