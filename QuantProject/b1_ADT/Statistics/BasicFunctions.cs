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
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      
      double stdDevOfFirst = BasicFunctions.StdDev(firstDataVariable);
      double stdDevOfSecond = BasicFunctions.StdDev(secondDataVariable);
      double coVariance = BasicFunctions.CoVariance(firstDataVariable,
                                                    secondDataVariable);
      
      return	(coVariance)/(stdDevOfFirst*stdDevOfSecond);
    }
    static public double PearsonCorrelationCoefficient( float[] firstDataVariable,
                                                        float[] secondDataVariable ) 
    {
      BasicFunctions.checkLengthOfDataVariables(firstDataVariable, secondDataVariable);
      
      double stdDevOfFirst = BasicFunctions.StdDev(firstDataVariable);
      double stdDevOfSecond = BasicFunctions.StdDev(secondDataVariable);
      double coVariance = BasicFunctions.CoVariance(firstDataVariable,
                                                    secondDataVariable);
      
      return	(coVariance)/(stdDevOfFirst*stdDevOfSecond);
    }

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

		
