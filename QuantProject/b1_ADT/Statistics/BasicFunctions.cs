/*
QuantProject - Quantitative Finance Library

BasicFunctions.cs
Copyright (C) 2003 
Marco Milletti

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections;

namespace QuantProject.ADT.Statistics
{
	/// <summary>
	/// Implements some basic statistical functions
	/// </summary>
	public class BasicFunctions
	{
   	private static double[] orderedData;
    
    static private void setOrderedData(double[] data)
    {
  	 	orderedData = new double[data.Length];
      data.CopyTo(orderedData,0);
      Array.Sort(orderedData);
    }
		
		static public  double Sum( double[] data )
    {
      double sum = 0;
      for( int i = 0; i < data.Length ; i ++ ) 
      {
        sum += data[ i ];
      }
      return	sum;
    }
		
		static public  double Sum( float[] data ) 
		{
			double sum = 0;
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				sum += data[ i ];
			}
			return	sum;
		}
		/// <summary>
		/// Returns the max value of the given data
		/// </summary>
		/// <param name="data">each data item must be convertible to a double</param>
		/// <returns></returns>
		static public double GetMax( ICollection data ) 
		{
			if ( data.Count < 1 )
				throw new Exception( "The data collection does not contain " +
					"any value!" );
			double max = Double.MinValue;
			foreach( object obj in data ) 
			{
				double valueToBeCompared;
				try
				{
					valueToBeCompared =	Convert.ToDouble( obj );
				}
				catch
				{
					throw new Exception( "The data collection contains " +
						"a data that cannot be converted to double!" );
				}
				max = Math.Max( max , valueToBeCompared );
			}
			return max;
		}
		static public double GetSum( ICollection data ) 
		{
			double sum = 0;
			foreach( object obj in data ) 
			{
				double valueToBeAdded;
				try
				{
					valueToBeAdded =	Convert.ToDouble( obj );
				}
				catch
				{
					throw new Exception( "The data collection contains " +
						"a data that cannot be converted to double!" );
				}
				sum += valueToBeAdded;
			}
			return sum;
		}
		static public  double SumOfSquares( double[] data ) 
    {
      double sumOfSquares = 0;
      for( int i = 0; i < data.Length ; i ++ ) 
      {
        sumOfSquares += data[ i ]*data[ i ];
      }
      return	sumOfSquares;
    }
		
    static public  double SumOfSquares( float[] data ) 
		{
			double sumOfSquares = 0;
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				sumOfSquares += data[ i ]*data[ i ];
			}
			return	sumOfSquares;
		}
    
		static public  double SumOfAbs( double[] data )
		{
			double sum = 0;
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				sum += Math.Abs( data[ i ] );
			}
			return	sum;
		}
		static public double[] MultiplyBy( double[] data , double multiplier )
		{
			double[] returnData = new double[ data.Length ];
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				returnData[ i ] = data[ i ] * multiplier;
			}
			return returnData;
		}
		
		static public double SimpleAverage( double[] data ) 
		{
			return	BasicFunctions.Sum(data)/data.Length;
		}
		
    static public double SimpleAverage( float[] data ) 
		{
			return	BasicFunctions.Sum(data)/data.Length;
		}
		static public double GetSimpleAverage( ICollection data ) 
		{
			return BasicFunctions.GetSum( data )/data.Count;
		}
		static public double Variance( double[] data ) 
    {
      double sum = BasicFunctions.Sum(data);
      double sumOfSquares = BasicFunctions.SumOfSquares(data);
      return	(sumOfSquares - sum*sum/data.Length)/data.Length;
    }
		static public double Variance( float[] data ) 
		{
			double sum = BasicFunctions.Sum(data);
			double sumOfSquares = BasicFunctions.SumOfSquares(data);
			return	(sumOfSquares - sum*sum/data.Length)/data.Length;
		}

		static public double GetVariance( ICollection data ) 
		{
			double simpleAverage = BasicFunctions.GetSimpleAverage(data);
			double sum = 0;
			foreach ( object obj in data )
				sum += ( Convert.ToDouble( obj ) - simpleAverage ) * 
					( Convert.ToDouble( obj ) - simpleAverage );
			return sum / ( data.Count - 1 );
		}

		static public double CoVariance( double[] firstDataVariable,
                                      double[] secondDataVariable ) 
    {
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      double simpleAvgOfProduct = BasicFunctions.SimpleAverageOfProduct(firstDataVariable, secondDataVariable);
      double productOfSimpleAvgs = BasicFunctions.SimpleAverage(firstDataVariable) * 
                                   BasicFunctions.SimpleAverage(secondDataVariable);
     
      return	(simpleAvgOfProduct - productOfSimpleAvgs);
    }
    
    static public double CoVariance( float[] firstDataVariable,
                                      float[] secondDataVariable ) 
    {
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      double simpleAvgOfProduct = BasicFunctions.SimpleAverageOfProduct(firstDataVariable, secondDataVariable);
      double productOfSimpleAvgs = BasicFunctions.SimpleAverage(firstDataVariable) * 
        BasicFunctions.SimpleAverage(secondDataVariable);
     
      return	(simpleAvgOfProduct - productOfSimpleAvgs);
    }

    static public double StdDev( double[] data ) 
    {
      return	System.Math.Sqrt(BasicFunctions.Variance(data));
    }
		static public double StdDev( float[] data ) 
		{
			return	System.Math.Sqrt(BasicFunctions.Variance(data));
		}
		static public double GetStdDev( ICollection data ) 
		{
			return	System.Math.Sqrt( BasicFunctions.GetVariance( data ) );
		}
    
    static public double PearsonCorrelationCoefficient( double[] firstDataVariable,
                                                        double[] secondDataVariable ) 
    {
//      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
//      
//      double stdDevOfFirst = BasicFunctions.StdDev(firstDataVariable);
//      double stdDevOfSecond = BasicFunctions.StdDev(secondDataVariable);
//      double coVariance = BasicFunctions.CoVariance(firstDataVariable,
//                                                    secondDataVariable);
//      
//      return	(coVariance)/(stdDevOfFirst*stdDevOfSecond);
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      int n = firstDataVariable.Length;
      double sumOfProduct = 0.0, sumOfFirst = 0.0, sumOfSecond = 0.0,
        sumOfSquaredFirst = 0.0, sumOfSquaredSecond = 0.0;
      for(int i = 0; i < n; i++)
      {
        sumOfFirst += firstDataVariable[i];
        sumOfSecond += secondDataVariable[i];
        sumOfProduct += firstDataVariable[i]*secondDataVariable[i];
        sumOfSquaredFirst += firstDataVariable[i]*firstDataVariable[i];
        sumOfSquaredSecond += secondDataVariable[i]*secondDataVariable[i];
      }
      return (n*sumOfProduct - sumOfFirst*sumOfSecond)/
        Math.Sqrt( (n*sumOfSquaredFirst - sumOfFirst*sumOfFirst)*
        (n*sumOfSquaredSecond - sumOfSecond*sumOfSecond) ); 

    }
    static public double PearsonCorrelationCoefficient( float[] firstDataVariable,
                                                        float[] secondDataVariable ) 
    {
//  OLD Computation way
//      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
//      
//      double stdDevOfFirst = BasicFunctions.StdDev(firstDataVariable);
//      double stdDevOfSecond = BasicFunctions.StdDev(secondDataVariable);
//      double coVariance = BasicFunctions.CoVariance(firstDataVariable,
//                                                    secondDataVariable);
//      
//      return	(coVariance)/(stdDevOfFirst*stdDevOfSecond);
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      int n = firstDataVariable.Length;
      double sumOfProduct = 0.0, sumOfFirst = 0.0, sumOfSecond = 0.0,
             sumOfSquaredFirst = 0.0, sumOfSquaredSecond = 0.0;
      for(int i = 0; i < n; i++)
      {
        sumOfFirst += firstDataVariable[i];
        sumOfSecond += secondDataVariable[i];
        sumOfProduct += firstDataVariable[i]*secondDataVariable[i];
        sumOfSquaredFirst += firstDataVariable[i]*firstDataVariable[i];
        sumOfSquaredSecond += secondDataVariable[i]*secondDataVariable[i];
      }
      return (n*sumOfProduct - sumOfFirst*sumOfSecond)/
              Math.Sqrt( (n*sumOfSquaredFirst - sumOfFirst*sumOfFirst)*
                         (n*sumOfSquaredSecond - sumOfSecond*sumOfSecond) );
      

    }

		#region PearsonCorrelationCoefficient with multipliers
		static private void checkMultiplierIsNonZero( double multiplier )
		{
			if ( multiplier == 0 )
				throw new Exception( "A multiplier cannot be zero!" );
		}
		static private void pearsonCorrelationCoefficient_checkParameters(
			double firstArrayMultiplier , float[] firstDataVariable,
			double secondArrayMultiplier , float[] secondDataVariable )
		{
			BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
			checkMultiplierIsNonZero( firstArrayMultiplier );
			checkMultiplierIsNonZero( secondArrayMultiplier );
		}
		/// <summary>
		/// Computes the Pearson's Correlation Coefficient for two weighted arrays
		/// </summary>
		/// <param name="firstMultiplier">multiplies each value of the
		/// first data set</param>
		/// <param name="firstDataVariable">first data set</param>
		/// <param name="secondMultiplier">multiplies each value of the second
		/// data set</param>
		/// <param name="secondDataVariable">second data set</param>
		/// <returns></returns>
		static public double PearsonCorrelationCoefficient(
			double firstMultiplier , float[] firstDataVariable,
			double secondMultiplier , float[] secondDataVariable ) 
		{
			pearsonCorrelationCoefficient_checkParameters(
				firstMultiplier , firstDataVariable,
				secondMultiplier , secondDataVariable );
			int n = firstDataVariable.Length;
			double sumOfProduct = 0.0, sumOfFirst = 0.0, sumOfSecond = 0.0,
				sumOfSquaredFirst = 0.0, sumOfSquaredSecond = 0.0;
			double currentValueFromFirstDataSet , currentValueFromSecondDataSet;
			for(int i = 0; i < n; i++)
			{
				currentValueFromFirstDataSet = firstMultiplier * firstDataVariable[i];
				currentValueFromSecondDataSet = secondMultiplier * secondDataVariable[i];
				sumOfFirst += currentValueFromFirstDataSet;
				sumOfSecond += currentValueFromSecondDataSet;
				sumOfProduct += currentValueFromFirstDataSet * currentValueFromSecondDataSet;
				sumOfSquaredFirst +=
					currentValueFromFirstDataSet * currentValueFromFirstDataSet;
				sumOfSquaredSecond +=
					currentValueFromSecondDataSet * currentValueFromSecondDataSet;
			}
			double n1 = n*sumOfProduct;
			double n2 = sumOfFirst*sumOfSecond;
			double numerator = n*sumOfProduct - sumOfFirst*sumOfSecond;
			double d1 = n*sumOfSquaredFirst;
			double d2 = sumOfFirst*sumOfFirst;
			double d3 = n*sumOfSquaredSecond;
			double d4 = sumOfSecond*sumOfSecond;
			double d5 = d1 - d2;
			double d6 = d3 - d4;
			return (n*sumOfProduct - sumOfFirst*sumOfSecond)/
				Math.Sqrt( (n*sumOfSquaredFirst - sumOfFirst*sumOfFirst)*
				(n*sumOfSquaredSecond - sumOfSecond*sumOfSecond) );
		}
		#endregion PearsonCorrelationCoefficient with multipliers


    static public double SimpleAverageOfProduct( double[] firstDataVariable,
                                                 double[] secondDataVariable ) 
    {
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      double[] productDataVariable = new double[firstDataVariable.Length];
      
      for( int i = 0; i < firstDataVariable.Length ; i ++ ) 
      {
        productDataVariable[i]= firstDataVariable[i]*secondDataVariable[i];
      }

      return	BasicFunctions.SimpleAverage(productDataVariable);
    }
    static public double SimpleAverageOfProduct( float[] firstDataVariable,
                                                 float[] secondDataVariable ) 
    {
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      double[] productDataVariable = new double[firstDataVariable.Length];
      
      for( int i = 0; i < firstDataVariable.Length ; i ++ ) 
      {
        productDataVariable[i]= firstDataVariable[i]*secondDataVariable[i];
      }

      return	BasicFunctions.SimpleAverage(productDataVariable);
    }
    static private void checkLengthOfDataVariables(float[] firstDataVariable,
                                                    float[] secondDataVariable)
    {
      if(firstDataVariable.Length !=secondDataVariable.Length)
        throw new Exception("The two variables haven't the same length!");
    }
    static private void checkLengthOfDataVariables(double[] firstDataVariable,
                                                  double[] secondDataVariable)
    {
      if(firstDataVariable.Length !=secondDataVariable.Length)
        throw new Exception("The two variables haven't the same length!");
    }
		
		/// <summary>
    /// Returns the t-th percentile for a given set of data
    /// </summary>
    /// <param name="data">Data for which t-th percentile is to be computed </param>
    /// <param name="percentileNumber">A number between 0 and 100, corresponding
    ///  to the requested t-th percentile	</param>
    static public double Percentile(double[] data,
                                    int percentileNumber)
    {
    	if(percentileNumber<0 || percentileNumber >100)
    		throw new Exception("percentileNumber has to be a number between 0 and 100!");
    	setOrderedData(data);
    	int n = orderedData.Length;
    	double np = ((double)n+1) * (double)percentileNumber/100.0;
      if(np<=1.0)
        return orderedData[0];
      if(np>=n)
        return orderedData[n-1];
      
      int integerPart_np = (int)Math.Floor(np);
      double fractionalPart_np = np - integerPart_np;
      //if np is not a whole number, it has to be returned a
      // number between two data points
      // In this implementation the method used to return such number
      // is based on a simple interpolation 
      return (orderedData[integerPart_np-1] +
              fractionalPart_np* (orderedData[integerPart_np] - orderedData[integerPart_np-1]));
     
    }
	    
    /// <summary>
    /// Returns the median for a given set of  data
    /// (Median is the value, belonging or not to the given set of data, such that
    /// number of data points below that value is equal to the number of data points
    /// above that value)
    /// </summary>
    /// <param name="data">Data for which median is to be computed</param>
    static public double Median(double[] data)
    {
    	return Percentile(data, 50);
    }
    
    /// <summary>
    /// Returns the average value for the given decile of
    /// a given set of data
    /// </summary>
    /// <param name="data">The set of data</param>
    /// <param name="decileNumber">The decile for which the average has to be computed</param>
    static public double GetDecileAverage(double[] data,
                                         	int decileNumber)
    {
    	if(decileNumber<1 || decileNumber >9)
    		throw new Exception("decileNumber has to be a number between 1 and 9!");
    	
    	double firstCorrespondingPercentile =
    		Percentile(data, decileNumber * 10 - 10);
    	double secondCorrespondingPercentile = 
    		Percentile(data, decileNumber * 10);
    	double totalBetweenPercentiles = 0.0;
    	int numberOfDataPointsBetweenPercentiles = 0;
    	for(int i = 0; i<orderedData.Length; i++)
    	{
    		if(orderedData[i]>= firstCorrespondingPercentile &&
    		   orderedData[i]< secondCorrespondingPercentile)
    		{
    			numberOfDataPointsBetweenPercentiles++;
    			totalBetweenPercentiles += orderedData[i];
    		}
    		
    	}
    	
    	return totalBetweenPercentiles/numberOfDataPointsBetweenPercentiles;
    }
    
    
	}	
}		

		
