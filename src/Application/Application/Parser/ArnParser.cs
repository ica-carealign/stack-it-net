using System;
using System.Text.RegularExpressions;

namespace Ica.StackIt.Application.Parser
{
	public class ArnParser : IArnParser
	{
		private static readonly Regex _iamPattern = new Regex("arn:aws:(iam|sts)::(.+?):", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public string GetAccountNumber(string iamArn)
		{
			Match regexResult = _iamPattern.Match(iamArn);

			if (!regexResult.Success)
			{
				throw new ArgumentException("Not a valid Identity and Access Management ARN", "iamArn");
			}

			return regexResult.Groups[2].ToString();
		}
	}
}