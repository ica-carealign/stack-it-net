using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests
{
	internal class NumberedStringGeneratorTests
	{
		private NumberedStringGenerator Generator { get; set; }

		[SetUp]
		public void SetUp()
		{
			Generator = new NumberedStringGenerator();
		}

		[Test]
		public void GeneratesSequence_Ok()
		{
			const string testString = "hello";
			var actual = new List<string>();

			for (int x = 0; x < 3; x++)
			{
				string res = Generator.GetNextString(testString);
				actual.Add(res);
			}

			actual.ShouldBeEquivalentTo(new List<string> {"hello0", "hello1", "hello2"});
		}
	}
}