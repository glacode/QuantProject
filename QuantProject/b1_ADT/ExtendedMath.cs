using System;

namespace QuantProject.ADT
{
	/// <summary>
	/// Mathematical function provider
	/// </summary>
	public class ExtendedMath
	{
		public ExtendedMath()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static long Factorial( int n )
		{
			if ( n < 0 )
				throw new Exception( "Factorial is undefined for n<0!" );
			long factorial;
			if ( ( n == 0 ) || ( n == 1 ) )
				factorial = 1;
			else
				factorial = n * Factorial( n - 1 );
			return factorial;
		}
	}
}
