using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CsvHelper;

namespace Ica.StackIt.Application.Billing
{
	public class LineItemCsvParser : IDisposable
	{
		private readonly TextReader _streamReader;
		private CsvReader _csvReader;

		public LineItemCsvParser(TextReader streamReader)
		{
			_streamReader = streamReader;
		}

		public IEnumerable<LineItem> GetLineItems()
		{
			if (_csvReader != null)
			{
				throw new InvalidOperationException("GetLineItems may be called only once per instance");
			}
			_csvReader = new CsvReader(_streamReader);
			return _csvReader.GetRecords<LineItem>().Where(li => li.RecordType == "LineItem");
		}

		public void Dispose()
		{
			if (_csvReader != null)
			{
				_csvReader.Dispose();
			}
		}
	}
}