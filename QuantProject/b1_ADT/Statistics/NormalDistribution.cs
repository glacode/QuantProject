/*
QuantProject - Quantitative Finance Library

NormalDistribution.cs
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
using QuantProject.ADT.Calculus;


namespace QuantProject.ADT.Statistics
{
	/// <summary>
	/// Class for the normal distribution
	/// </summary>
  public class NormalDistribution : IPdfDefiner
	{
	  
	  private double average;
		private double stdDeviation;
		
		private double infinity;
		private int numOfIntervalsForPDFIntegralApproximation;
	  
	  public NormalDistribution(double average, double stdDeviation)
    {
    	if(stdDeviation < 0)
    		throw new Exception("Standard deviation must be > 0!");
    	
    	this.average = average;
    	this.stdDeviation = stdDeviation;
    	this.infinity = 15 * stdDeviation;
    	this.numOfIntervalsForPDFIntegralApproximation =
    					Convert.ToInt32(25*this.infinity);
    }
    /// <summary>
    /// gets the probability density function at point x
    /// for the current instance of normal distribution
    /// </summary>
    public double GetProbabilityDensityValue(double x)
    {
    	double y;
    	y = Math.Pow(Math.E,(-Math.Pow(x-this.average,2)
    	             /(2*this.stdDeviation*this.stdDeviation)))
    	    /(Math.Sqrt(2*Math.PI)*this.stdDeviation);
    	return y;
    }
    // end of IPdfDefiner implementation
    
    /// <summary>
    /// gets the probability that the normal random variable with the
    /// current features is less or equal to y
    /// </summary>
 		public double GetProbability(double y)
    {
     	return this.GetProbability(-this.infinity, y);
    }
    
    /// <summary>
    /// gets the probability that the current normal random variable
    /// is between a and b
    /// </summary>
 		public double GetProbability(double a, double b)
    {
     	return 
     	CalculusApproximation.GetArea((IPdfDefiner)this,a,b,
     	                           this.numOfIntervalsForPDFIntegralApproximation);
    }
    
	}
}
