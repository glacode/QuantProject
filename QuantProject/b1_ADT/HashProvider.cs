using System;
using System.Security.Cryptography;

namespace QuantProject.ADT
{
	/// <summary>
	/// Provides hashing values
	/// </summary>
	public class HashProvider
	{
		public HashProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// Returns an hash value for the string toBeHashed
		/// </summary>
		/// <param name="toBeHashed">parameter for which the hash value must be computed</param>
		/// <returns>Computed hash value</returns>
		public static string GetHashValue( string toBeHashed )
		{
			SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();

			// converts the original string to an array of bytes
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes( toBeHashed );

			// computes the Hash, and returns an array of bytes
			byte[] hashBytes = sha1CryptoServiceProvider.ComputeHash( bytes );

			sha1CryptoServiceProvider.Clear();

			// returns a base 64 encoded string of the hash value
			return Convert.ToBase64String( hashBytes );
		}
	}
}
