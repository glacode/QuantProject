using System;
using System.Collections;

namespace QuantProject.ADT.Statistics.Combinatorial
{
	/// <summary>
	/// Enumerates all combinations of integer values within a given
	/// range of integers
	/// </summary>
	public class Combination : IEnumerator
	{
		private int[] currentCombinationValues;

		private int minValue;
		private int maxValue;
		private int length;
		
		/// <summary>
		/// each combination length
		/// </summary>
		public int Length
		{
			get { return this.length; }
		}
		public long TotalNumberOfCombinations
		{
			get
			{
				return this.totalNumberOfCombinations();
			}
		}

		/// <summary>
		/// Enumerates all combinations of integer values within a given
		/// range of integers
		/// </summary>
		/// <param name="minValue">min value for combination elements</param>
		/// <param name="maxValue">max value for combination elements</param>
		/// <param name="length">combination length</param>
		public Combination( int minValue , int maxValue ,
			int length )
		{
			this.checkParameters( minValue , maxValue , length );
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.length = length;

			this.currentCombinationValues =
				new int[ length ];
		}
		private void checkParameters( int minValue , int maxValue ,
			int length )
		{
			if ( minValue >= maxValue )
				throw new Exception( "minValue is not less then maxValue!" );
			if ( length <= 0 )
				throw new Exception( "length is not greater than zero!" );
		}
		#region Current
		public object Current
		{
			get
			{
				return this.getCurrent();
			}
		}
		private int[] getCurrent()
		{
			return this.currentCombinationValues;
		}
		#endregion
		public void Reset()
		{
			for ( int currentComponentIndex = 0 ;
				currentComponentIndex < this.length ;
				currentComponentIndex ++ )
				this.currentCombinationValues[ currentComponentIndex ] =
					this.minValue + currentComponentIndex;
		}
		#region MoveNext
		private int getMaxValueForElement( int elementIndex )
		{
			int maxValueForElement = this.maxValue - ( this.length - 1 - elementIndex );
			return maxValueForElement;
		}
		private bool hasNotReachedItsMaxValue( int indexToCheck )
		{
			int maxValueToReach = this.getMaxValueForElement( indexToCheck );
			return ( this.currentCombinationValues[ indexToCheck ] <
				maxValueToReach );
		}
		private int getIndexOfTheFirstItemThatHasNotReachedItsMaxValue()
		{
			int indexOfTheFirstItemThatHasNotReachedItsMaxValue = -1 ;
			bool isIndexFound = false;
			int indexToCheck = this.length - 1;
			while ( ( !isIndexFound ) && ( indexToCheck >= 0 ) )
			{
				if ( this.hasNotReachedItsMaxValue( indexToCheck ) )
				{
					indexOfTheFirstItemThatHasNotReachedItsMaxValue =
						indexToCheck;
					isIndexFound = true;
				}
				else
					indexToCheck -- ;
			}
			return indexOfTheFirstItemThatHasNotReachedItsMaxValue;
		}
		private void moveNextActually(
			int indexOfTheFirstItemThatHasNotReachedItsMaxValue )
		{
			this.currentCombinationValues[
				indexOfTheFirstItemThatHasNotReachedItsMaxValue ] ++ ;
			for ( int i = indexOfTheFirstItemThatHasNotReachedItsMaxValue + 1 ;
				i < this.currentCombinationValues.Length; i++ ) 
			{
				this.currentCombinationValues[ i ] =
					this.currentCombinationValues[ i - 1 ] + 1;
			}
		}
		public bool MoveNext()
		{
			bool isNotLastCombination = true;
			int indexOfTheFirstItemThatHasNotReachedItsMaxValue =
				this.getIndexOfTheFirstItemThatHasNotReachedItsMaxValue();
			if ( indexOfTheFirstItemThatHasNotReachedItsMaxValue == -1 )
				// no more combinations to be generated: every item has
				// reached its max value
				isNotLastCombination = false;
			else
				this.moveNextActually(
					indexOfTheFirstItemThatHasNotReachedItsMaxValue );
			return isNotLastCombination;
		}
		#endregion
		public int GetValue( int elementIndex )
		{
			return this.currentCombinationValues[ elementIndex ];
		}
		#region totalNumberOfCombinations
		private long totalNumberOfCombinations_getNumerator()
		{
			int firstFactor = this.maxValue - this.minValue + 1;
			long numerator = 1;
			for ( int factor = firstFactor ; factor > firstFactor - this.length ;
				factor -- )
				numerator *= factor;
			return numerator;
		}
		private long totalNumberOfCombinations_getDenominator()
		{
			return ExtendedMath.Factorial( this.length );
		}
		private long totalNumberOfCombinations()
		{
			long numerator = this.totalNumberOfCombinations_getNumerator();
			long denominator = this.totalNumberOfCombinations_getDenominator();
			return numerator / denominator;
		}
		#endregion
	}
}
