using System.Text;

namespace Ica.StackIt.Application
{
	public static class BinaryEncoding
	{
		private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

		public static Encoding Encoding
		{
			get { return _encoding; }
		}

		/// <summary>
		///     Converts the input byte array into a <see cref="string" /> where each character
		///     represents a single byte in the array.
		/// </summary>
		/// <param name="input">the bytes to convert</param>
		/// <returns>the encoded string</returns>
		public static string Encode(byte[] input)
		{
			return _encoding.GetString(input);
		}

		/// <summary>
		///     Converts the input string into a <see cref="byte[]" /> where each byte represents
		///     a character in the string.
		/// </summary>
		/// <param name="input">the string to convert</param>
		/// <returns>the byte array</returns>
		public static byte[] Decode(string input)
		{
			return _encoding.GetBytes(input);
		}
	}
}