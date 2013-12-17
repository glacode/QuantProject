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
		#region ArrayOfAbs
		public static double[] ArrayOfAbs( double[] sourceArray )
		{
			double[] arrayOfAbs = new double[sourceArray.Length];
			for(int i = 0; i < sourceArray.Length; i++)
				arrayOfAbs[i] = Math.Abs( sourceArray[i] );
			return arrayOfAbs;
		}
		public static float[] ArrayOfAbs( float[] sourceArray )
		{
			float[] arrayOfAbs = new float[sourceArray.Length];
			for(int i = 0; i < sourceArray.Length; i++)
				arrayOfAbs[i] = Math.Abs( sourceArray[i] );
			return arrayOfAbs;
		}
		public static int[] ArrayOfAbs( int[] sourceArray )
		{
			int[] arrayOfAbs = new int[sourceArray.Length];
			for(int i = 0; i < sourceArray.Length; i++)
				arrayOfAbs[i] = Math.Abs( sourceArray[i] );
			return arrayOfAbs;
		}
		#endregion
	}
}
