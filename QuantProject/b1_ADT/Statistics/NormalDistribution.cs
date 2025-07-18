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
	  const int numIntForInfinityApproximation = 15;
	  const int minimumIntervalsForAreaApproximation = 25;
	  private double average;
		private double stdDeviation;
		
		private double infinity;
		private int numOfIntervalsForPDFIntegralApproximation;
	  
	  public NormalDistribution(double average, double stdDeviation)
    {
    	if(stdDeviation <= 0)
    		throw new Exception("Standard deviation must be > 0!");
    	
    	this.average = average;
    	this.stdDeviation = stdDeviation;
    	this.infinity = this.average + 
    									numIntForInfinityApproximation * stdDeviation ;
    	this.numOfIntervalsForPDFIntegralApproximation =
    			Math.Max(Convert.ToInt32(this.infinity*minimumIntervalsForAreaApproximation),
    		           minimumIntervalsForAreaApproximation);
    }
    /// <summary>
    /// gets the probability density function at point x
    /// for the current instance of normal distribution
    /// </summary>
    public double GetProbabilityDensityValue(double x)
    {
    	if(Double.IsInfinity(x) || Double.IsNaN(x))
    		throw new Exception("Density value of x is not computable!");
    	
    	double y = 0;
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
     	if(Double.IsInfinity(y) || Double.IsNaN(y))
    		throw new Exception("Prob(Y<y) is not computable!");
 			return this.GetProbability(-this.infinity, y);
    }
    
    /// <summary>
    /// gets the probability that the current normal random variable
    /// is between a and b
    /// </summary>
 		public double GetProbability(double a, double b)
    {
     	if( Double.IsInfinity(a) || Double.IsNaN(a) ||
 			    Double.IsInfinity(b) || Double.IsNaN(b) )
    				throw new Exception("Prob(a<Y<b) is not computable!");
 			
 			return
     	CalculusApproximation.GetArea((IPdfDefiner)this,a,b,
     	                           		this.numOfIntervalsForPDFIntegralApproximation);
    }
    
	}
}
