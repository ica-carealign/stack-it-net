using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.EC2.Model;

using Hangfire;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using TagResource = Amazon.EC2.Model.Tag;
using TagEntity = Ica.StackIt.Core.Entities.Tag;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateAllImages : CommandBase
	{
		private readonly IRepository<BaseImage> _imageRepository;

		public UpdateAllImages(
			IAwsClientFactory awsClientFactory,
			IRepository<BaseImage> imageRepository,
			IRepository<AwsProfile> profileRepository) : base(profileRepository, awsClientFactory)
		{
			_imageRepository = imageRepository;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			IImageService imageService = awsClient.ImageService;

			List<Image> images = imageService.GetImagesByTagName(Conventions.BaseImageTag);
			foreach (Image image in images)
			{
				var imageName = image.Tags.Single(x => x.Key == Conventions.BaseImageTag).Value;

				BaseImage baseImage = _imageRepository.FindAll().FirstOrDefault(x => x.Name == imageName);
				SaveMechanic saveMechanic = baseImage == null
					? SaveMechanic.Add
					: SaveMechanic.Update;

				baseImage = baseImage ?? new BaseImage();
				MapToBaseImage(image, baseImage);

				// TODO: Still need a way to remove stale AMI's
				// like when someone deletes an AMI whose id was already stored locally with a profile
				if (imageName.ToLower().EndsWith("-edge"))
				{
					baseImage.EdgeAmi[profileId] = image.ImageId;
				}
				else
				{
					baseImage.ProductionAmi[profileId] = image.ImageId;
				}

				if (saveMechanic == SaveMechanic.Add)
				{
					_imageRepository.Add(baseImage);
				}
				else
				{
					_imageRepository.Update(baseImage);
				}
			}
		}

		public void MapToBaseImage(Image src, BaseImage dest)
		{
			List<TagResource> imageTags = src.Tags;
			List<TagEntity> tagEntities = imageTags.Select(x => new TagEntity {Name = x.Key, Value = x.Value}).ToList();

			dest.Name = tagEntities.Single(x => x.Name == Conventions.BaseImageTag).Value;
			dest.ResourceId = src.ImageId;
			dest.Tags = tagEntities;
			dest.Platform = src.Platform.ToNonNull();
			dest.RootDeviceName = src.RootDeviceName;
		}
	}
}