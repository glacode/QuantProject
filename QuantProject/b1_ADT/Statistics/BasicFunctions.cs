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

namespace QuantProject.ADT.Statistics
{
	/// <summary>
	/// Implements some basic statistical functions
	/// </summary>
	public class BasicFunctions
	{
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
	}	
}		

		