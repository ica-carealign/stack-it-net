using System.Collections.Generic;

namespace Ica.StackIt.Application
{
	public class NumberedStringGenerator : INumberedStringGenerator
	{
		private readonly Dictionary<string, int> _nameDictionary = new Dictionary<string, int>();

		public string GetNextString(string str)
		{
			bool isInDictionary = _nameDictionary.ContainsKey(str);

			if (!isInDictionary)
			{
				_nameDictionary[str] = 0;
			}

			return string.Format("{0}{1}", str, _nameDictionary[str]++);
		}
	}
}