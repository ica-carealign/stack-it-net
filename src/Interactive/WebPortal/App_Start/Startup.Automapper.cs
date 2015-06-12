using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		public static void ConfigureAutomapper()
		{
			Mapper.CreateMap<Stack, StackOverviewViewModel>()
			      .ForMember(dest => dest.StackPowerViewModel, opt => opt.Ignore())
			      .ForMember(dest => dest.StackInstanceViewModel, opt => opt.Ignore())
				  .ForMember(dest => dest.TotalCost, opt => opt.Ignore());

			Mapper.CreateMap<Instance, InstanceOverviewViewModel>()
			      .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.InstanceType))
			      .ForMember(dest => dest.PrivateIp, opt => opt.MapFrom(src => GetFirstIpAddress(src.PrivateAddresses)))
			      .ForMember(dest => dest.PublicIp, opt => opt.MapFrom(src => GetFirstIpAddress(src.PublicAddresses)))
			      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StateToNullableBool(src.State)))
			      .ForMember(dest => dest.Image, opt => opt.Ignore())
			      .ForMember(dest => dest.Platform, opt => opt.Ignore())
			      .ForMember(dest => dest.HttpAccessability, opt => opt.Ignore());

			Mapper.AssertConfigurationIsValid();
		}

		private static string GetFirstIpAddress(IEnumerable<string> addresses)
		{
			string address = addresses.FirstOrDefault();
			return address;
		}

		private static bool? StateToNullableBool(string state)
		{
			if (state == null) return null;
			if (state.Equals("running", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}

			if (state.Equals("stopped", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			return null;
		}
	}
}