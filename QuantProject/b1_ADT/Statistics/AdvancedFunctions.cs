/*
QuantProject - Quantitative Finance Library

AdvancedFunctions.cs
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
	/// Implements advanced statistical functions
	/// </summary>
	public class AdvancedFunctions
	{
    /// <summary>
    /// Returns a new matrix in which each element is equal to the corresponding
    /// element of the given data[], powered by the given exponential value
    /// </summary>
    public static double[] Pow( double[] data, double exponentialValue) 
    {
      double[] returnValue = new double[data.Length];
      for(int i = 0; i < data.Length; i++)
      {
        returnValue[i] = Math.Pow(data[i],exponentialValue);
      }
      return returnValue;
    }

    /// <summary>
    /// Returns a DownsideData[] from a given data[], with
    /// respect to a given target
    /// If data[i] >= target, then the corresponding DownsideData[i] is 
    /// equal to 0; else, it is equal to data[i]
    /// </summary>
    /// <param name="data">Data for which the corresponding DownsideData is returned</param>
    /// <param name="target">The target used for computation of the DownsideData</param>
    public static double[] DownsideData( double[] data, double target) 
    {
      double[] returnValue = new double[data.Length];
      for(int i = 0; i < data.Length; i++)
      {
          if(data[i] >= target)
            returnValue[i] = 0.0;
          else
            returnValue[i] = data[i];
      }
      return returnValue;
    }
    /// <summary>
    /// Returns an UpsideData[] from a given data[], with
    /// respect to a given target
    /// If data[i] > target, then the corresponding UpsideData[i] is 
    /// equal to data[i]; else, it is equal to 0 
    /// </summary>
    /// <param name="data">Data for which the corresponding UpsideData is returned</param>
    /// <param name="target">The target used for computation of the UpsideData</param>
    public static double[] UpsideData( double[] data, double target) 
    {
      double[] returnValue = new double[data.Length];
      for(int i = 0; i < data.Length; i++)
      {
        if(data[i] > target)
          returnValue[i] = data[i];
        else
          returnValue[i] = 0.0;
      }
      return returnValue;
    }

    /// <summary>
    /// Returns a mean measure of deviations of given data 
    /// below a given target
    /// (Useful in finance for measure of downside risk)
    /// </summary>
    /// <param name="data">Data for which the LPM has to be computed</param>
    /// <param name="targetMean">The target from which below deviations are computed</param>
     /// <param name="moment">The exp value for the computation of deviations</param>
    public static double LowerPartialMoment( double[] data, double target, 
                                             double moment ) 
    {
      double[] downsideData = DownsideData(data, target);
      double[] negativeDeviations = new double[data.Length];
      for(int i = 0; i < data.Length; i++)
      {
        negativeDeviations[i] = target - downsideData[i];
      }
      return BasicFunctions.SimpleAverage(Pow(negativeDeviations,moment));
    }

    /// <summary>
    /// Returns the negative semivariance
    /// </summary>
    /// <param name="data">Data for which the negative semivariance has to be computed</param>
    public static double NegativeSemiVariance( double[] data ) 
    {
      return LowerPartialMoment(data, BasicFunctions.SimpleAverage(data),
                                2.0);
      //negative semivariance is a special case of Lower partial moment
    }

  }
  

}		

		