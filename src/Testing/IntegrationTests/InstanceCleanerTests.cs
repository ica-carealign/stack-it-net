using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using FluentAssertions;

using Ica.StackIt.Application.Command;
using Ica.StackIt.Core.Entities;

using Microsoft.Owin.Hosting;

using Newtonsoft.Json;

using NUnit.Framework;

using Owin;

namespace Ica.StackIt.Testing.IntegrationTests
{
	public class InstanceCleanerTests
	{
		private IDisposable _server;
		private const string _baseUrl = "http://localhost:43821";
		private HttpClient HttpClient { get; set; }
		private CleanUpPuppet CleanUpPuppet { get; set; }

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			HttpClient = new HttpClient {BaseAddress = new Uri(_baseUrl)};
			CleanUpPuppet = new CleanUpPuppet(HttpClient);
			_server = WebApp.Start<Startup>(_baseUrl);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			if (_server != null)
			{
				_server.Dispose();
			}
		}

		[Test]
		public void CleanUpInstance_Ok()
		{
			// Arrange
			var instance = new Instance
			{
				PrivateAddresses = new List<string> {"10.0.0.1"}
			};

			// Act
			Action act = () => CleanUpPuppet.CleanUp(instance);

			// Assert
			act.ShouldNotThrow();
		}
	}

	internal class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.Use((ctx, nextTaskFactory) =>
			{
				string url = ctx.Request.Uri.AbsoluteUri;
				string method = ctx.Request.Method;
				if (method == "POST")
				{
					string contentType = ctx.Request.ContentType;
					string postBody;

					using (var reader = new StreamReader(ctx.Request.Body))
					{
						postBody = reader.ReadToEnd();
					}

					ctx.Response.StatusCode = 200;
					ctx.Response.WriteAsync(JsonConvert.SerializeObject(new
					{
						url,
						contentType,
						method,
						postBody
					}));
				}

				return nextTaskFactory();
			});
		}
	}
}