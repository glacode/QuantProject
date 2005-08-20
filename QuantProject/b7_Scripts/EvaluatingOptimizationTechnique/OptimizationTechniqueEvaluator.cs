/*
QuantProject - Quantitative Finance Library

OptimizationTechniqueEvaluator.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Statistics;

namespace QuantProject.Scripts.EvaluatingOptimizationTechnique
{
	/// <summary>
	/// Class for evaluating a given optimization technique
	/// </summary>
	[Serializable]
  public class OptimizationTechniqueEvaluator 
	{
  	double[] fitnessesInSample;
  	double[] fitnessesOutOfSample;
    
  	/// <summary>
		/// Creates a new instance of OptimizationTechniqueEvaluator 
		/// </summary> 
		/// <param name="fitnessesInSample">The array has to contain the fitnesses in sample (the values computed by the optimizer). Fitnesses in sample have to be ordered!</param>
    /// <param name="fitnessesOutOfSample">The array has to contain the fitnesses out of sample
    /// 	 (the values evaluated by the trading simpulation). Fitnesses out of sample have to
    /// 	 corresponde one by one to fitnesses in sample</param>
  	public OptimizationTechniqueEvaluator(double[] fitnessesInSample,
  	                                      double[] fitnessesOutOfSample)
    {
  		this.optimizationTechniqueEvaluator_checkParameters(fitnessesInSample,
    	                                                    fitnessesOutOfSample);
  		this.fitnessesInSample = fitnessesInSample;
	  	this.fitnessesOutOfSample = fitnessesOutOfSample;
		}
    
    private void optimizationTechniqueEvaluator_checkParameters(double[] fitnessesInSample,
  	                                      											double[] fitnessesOutOfSample)
    {
	  	if(fitnessesInSample.Length != fitnessesOutOfSample.Length)
  			throw new Exception("fitnessesInSample and fitnessesOutOfSample " +
  			                    "can't have a different size!");
   	}   
    
    /// <summary>
    /// Returns the average values for the given sub-sets of
    /// fitnesses in sample. Fitnesses in sample are ordered!
    /// </summary>
    /// <param name="numberOfSubsets">Number of sub-sets fitnessesInSample are divided into</param>
	  public double[] GetAveragesOfSubsetsInSample(int numberOfSubsets)
    {
	  	return this.getSubsetAverage(this.fitnessesInSample, numberOfSubsets);
   	}
    
	  /// <summary>
    /// Returns the average values for the given sub-sets of
    /// fitnesses out of sample
    /// </summary>
    /// <param name="numberOfSubsets">Number of sub-sets fitnessesOutOfSample are divided into</param>
	  public double[] GetAveragesOfSubsetsOutOfSample(int numberOfSubsets)
    {
	  	return this.getSubsetAverage(this.fitnessesOutOfSample, numberOfSubsets);
		}
		
	  private void getSubsetAverage_checkParameters(int numberOfSubsets)
    {
	  	if(numberOfSubsets > this.fitnessesInSample.Length)
	  	//fitnessesInSample have the same length as fitnessesOutOfSample
    		throw new Exception("numberOfSubsets can't be greater than the number of data points!");
   	}   
	  
	  /// <summary>
    /// Returns the average values for the given sub-set of
    /// a given set of data
    /// </summary>
    /// <param name="data">The set of data points(inSample or OutOfSample)</param>
    /// <param name="numberOfSubsets">Number of sub-sets the
    /// 															given set of data is divided into</param>
    private double[] getSubsetAverage(double[] data,
                                   		int numberOfSubsets)
    {
    	this.getSubsetAverage_checkParameters(numberOfSubsets);
    	int numberOfDataPointsInEachSubset = 
    		(int)Math.Floor(Convert.ToDouble(data.Length)/Convert.ToDouble(numberOfSubsets));
    	double[] returnValue = new double[numberOfSubsets];
    	
    	for(int i = 0; i<numberOfSubsets; i++)
    	{
    		for(int j = i * numberOfDataPointsInEachSubset;
    		    j<(i+1)* numberOfDataPointsInEachSubset;
    		    j++)
	    	{
    			returnValue[i]+= data[j]/numberOfDataPointsInEachSubset;
	    	}
    	}
    	
    	return returnValue;
    }
    
    /// <summary>
    /// Returns the Pearson correlation coefficient between fitnesses in sample
    /// and fitnesses out of sample
    /// </summary>
    public double GetCorrelationBetweenFitnesses()
    {
	  	return BasicFunctions.PearsonCorrelationCoefficient(this.fitnessesInSample,
    	                                                    this.fitnessesOutOfSample);
		}
	}
}
