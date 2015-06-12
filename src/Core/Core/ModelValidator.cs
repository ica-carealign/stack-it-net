using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Ica.StackIt.Core
{
	public class ModelValidator
	{
		private class PropertyValidator
		{
			public PropertyInfo Property { get; private set; }
			public ValidationAttribute[] ValidationAttributes { get; private set; }

			public PropertyValidator(PropertyInfo property, ValidationAttribute[] validationAttributes)
			{
				Property = property;
				ValidationAttributes = validationAttributes;
			}
		}

		public static bool TryValidateObject<TObject>(
			TObject instance,
			ICollection<ValidationResult> results
			)
		{
			IEnumerable<PropertyValidator> validators = typeof (TObject).GetInterfaces().SelectMany(GetPropertyValidators);

			var context = new ValidationContext(instance);
			bool ok = Validator.TryValidateObject(instance, context, results);
			// do not short circuit this.
			if (! TryValidateProperties(instance, validators, results))
			{
				ok = false;
			}
			return ok;
		}

		public static void ValidateObject<TObject>(TObject instance)
		{
			var context = new ValidationContext(instance);
			Validator.ValidateObject(instance, context);

			IEnumerable<PropertyValidator> validators = typeof (TObject).GetInterfaces().SelectMany(GetPropertyValidators);
			foreach (var val in validators)
			{
				object value = val.Property.GetValue(instance);
				context = new ValidationContext(instance) {MemberName = val.Property.Name};
				Validator.ValidateValue(value, context, val.ValidationAttributes);
			}
		}

		private static bool TryValidateProperties<TObject>(
			TObject instance,
			IEnumerable<PropertyValidator> validators,
			ICollection<ValidationResult> results
			)
		{
			bool ok = true;
			// avoid validators.All(...) because it can exit early, which we don't want.
			foreach (PropertyValidator val in validators)
			{
				object value = val.Property.GetValue(instance);
				var context = new ValidationContext(instance) {MemberName = val.Property.Name};
				if (! Validator.TryValidateValue(value, context, results, val.ValidationAttributes))
				{
					ok = false;
				}
			}
			return ok;
		}

		private static IEnumerable<PropertyValidator> GetPropertyValidators(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
			foreach (PropertyInfo prop in properties)
			{
				Attribute[] validators = Attribute.GetCustomAttributes(prop, typeof (ValidationAttribute));
				yield return new PropertyValidator(prop, validators.Cast<ValidationAttribute>().ToArray());
			}
		}
	}
}