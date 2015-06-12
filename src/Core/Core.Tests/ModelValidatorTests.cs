using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Range = System.ComponentModel.DataAnnotations.RangeAttribute;

namespace Ica.StackIt.Core.Tests
{
	public class ModelValidatorTests
	{
		private interface IBinocular
		{
			[Range(2, 2)]
			int EyeCount { get; }
		}

		private interface IBipedal
		{
			[Range(2, 2)]
			int FootCount { get; }
		}

		private interface IHuman : IBinocular, IBipedal {}

		private class Person
		{
			[Required]
			public string Name { get; set; }

			public int EyeCount { get; set; }
			public int FootCount { get; set; }
		}

		private class BinocularPerson : Person, IBinocular {}

		private class HumanPerson : Person, IHuman {}

		private List<ValidationResult> Results { get; set; }

		[SetUp]
		public void SetUp()
		{
			Results = new List<ValidationResult>();
		}

		[Test]
		public void Concrete_TryValidate_Ok()
		{
			var person = new Person {Name = "Helen"};
			ModelValidator.TryValidateObject(person, Results).Should().BeTrue();
		}

		[Test]
		public void Concrete_TryValidate_Fail()
		{
			var person = new Person {Name = null};
			ModelValidator.TryValidateObject(person, Results).Should().BeFalse();
			Results.Count.Should().BePositive();
			Results[0].ErrorMessage.Should().Match("*Name*required*");
		}

		[Test]
		public void Concrete_Validate_Ok()
		{
			var person = new Person {Name = "Helen"};
			person.Invoking(p => ModelValidator.ValidateObject(person))
			      .ShouldNotThrow();
		}

		[Test]
		public void Concrete_Validate_Fail()
		{
			var person = new Person {Name = null};
			person.Invoking(p => ModelValidator.ValidateObject(person))
			      .ShouldThrow<ValidationException>()
			      .Which.Message.Should().Contain("The Name field is required");
		}

		[Test]
		public void Subclass_TryValidate_Ok()
		{
			var person = new BinocularPerson {Name = "Helga", EyeCount = 2, FootCount = 2};
			ModelValidator.TryValidateObject(person, Results).Should().BeTrue();
		}

		[Test]
		public void Subclass_TryValidate_Fail()
		{
			var person = new BinocularPerson {Name = null, EyeCount = 2, FootCount = 2};
			ModelValidator.TryValidateObject(person, Results).Should().BeFalse();
			Results.Count.Should().BePositive();
			Results[0].ErrorMessage.Should().Match("*Name*required*");
		}

		[Test]
		public void OneInterface_TryValidate_Ok()
		{
			var person = new BinocularPerson {Name = "Hattie", EyeCount = 2};
			ModelValidator.TryValidateObject(person, Results).Should().BeTrue();
		}

		[Test]
		public void OneInterface_TryValidate_Fail()
		{
			var person = new BinocularPerson {Name = "Cyclops", EyeCount = 1};
			ModelValidator.TryValidateObject(person, Results).Should().BeFalse();
			Results[0].ErrorMessage.Should().Match("*EyeCount*2*");
		}

		[Test]
		public void TwoInterfaces_TryValidate_Ok()
		{
			var person = new HumanPerson {Name = "Hyacinth", EyeCount = 2, FootCount = 2};
			ModelValidator.TryValidateObject(person, Results).Should().BeTrue();
			Results.Should().BeEmpty();
		}

		[Test]
		public void TwoInterfaces_TryValidate_Fail()
		{
			var person = new HumanPerson {Name = "Clumsy Cyclops", EyeCount = 1, FootCount = 1};
			ModelValidator.TryValidateObject(person, Results).Should().BeFalse();
			Results.Select(x => x.ErrorMessage).Should().Contain(m => m.Contains("EyeCount"));
			Results.Select(x => x.ErrorMessage).Should().Contain(m => m.Contains("FootCount"));
			Results.Count.Should().Be(2);
		}

		[Test]
		public void TwoInterfaces_Validate_Fail()
		{
			var person = new HumanPerson {Name = "Clumsy Cyclops", EyeCount = 1, FootCount = 1};
			person.Invoking(p => ModelValidator.ValidateObject(person))
			      .ShouldThrow<ValidationException>()
			      .Which.Message.Should().Match(m => m.Contains("EyeCount") || m.Contains("FootCount"));

			person.EyeCount = 2;
			person.Invoking(p => ModelValidator.ValidateObject(person))
			      .ShouldThrow<ValidationException>()
			      .WithMessage("*FootCount*");

			person.Name = null;
			person.Invoking(p => ModelValidator.ValidateObject(person))
			      .ShouldThrow<ValidationException>()
			      .WithMessage("*Name*required*");
		}
	}
}