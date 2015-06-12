using System.Collections.Generic;
using System.IO;
using System.Linq;

using CsvHelper;

using FluentAssertions;

using Ica.StackIt.Application.ApplicationTests.Properties;
using Ica.StackIt.Application.Billing;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Billing
{
	public class LineItemCsvParserTests
	{
		[Test]
		public void Parse_Ok()
		{
			using (var stream = new MemoryStream(Resources.LineItems))
			{
				using (var streamReader = new StreamReader(stream))
				{
					using (var parser = new LineItemCsvParser(streamReader))
					{
						List<LineItem> items = parser.GetLineItems().Take(5).ToList();
						items.Count.Should().Be(5);
						foreach (LineItem lineItem in items)
						{
							lineItem.RecordType.Should().Be("LineItem");
							lineItem.RecordId.Should().NotBeEmpty();
							lineItem.ResourceId.Should().NotBeEmpty();
							lineItem.UsageStartDate.Year.Should().BeGreaterOrEqualTo(2014);
							lineItem.UsageEndDate.Year.Should().BeGreaterOrEqualTo(2014);
						}
					}
				}
			}
		}

		[Test]
		public void FiltersForLineItemType()
		{
			// Arrange
			var sw = new StringWriter();
			using (var csv = new CsvWriter(sw))
			{
				var lineItems = new[]
				{
					new LineItem {RecordType = "Dummy"},
					new LineItem {RecordType = "LineItem", RecordId = "Hoojey"}
				};
				csv.WriteHeader<LineItem>();
				csv.WriteRecords(lineItems);
			}

			// Act
			List<LineItem> parsedItems;
			using (var reader = new StringReader(sw.ToString()))
			{
				using (var parser = new LineItemCsvParser(reader))
				{
					parsedItems = parser.GetLineItems().ToList();
				}
			}

			// Assert
			parsedItems.Count.Should().Be(1);
			parsedItems[0].RecordId.Should().Be("Hoojey");
		}
	}
}