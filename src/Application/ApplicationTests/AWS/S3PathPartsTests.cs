using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using Ica.StackIt.Application.Parser;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	public class S3PathPartsTests
	{

		[Test]
		public void ToString_S3Url()
		{
			var parts = new S3PathParts("mybucket", "mykey");
			parts.ToString().Should().Be("s3://mybucket/mykey");
		}
	}
}
