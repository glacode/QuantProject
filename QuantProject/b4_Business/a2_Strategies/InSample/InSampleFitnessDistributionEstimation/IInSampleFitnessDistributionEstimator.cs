/*
QuantProject - Quantitative Finance Library

IInSampleFitnessDistributionEstimator.cs
Copyright (C) 2009
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

//using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.InSample;

namespace QuantProject.Business.Strategies.InSample.InSampleFitnessDistributionEstimation
{
	/// <summary>
	/// Interface for classes performing
	/// estimations of the distribution's features (mainly, 
	/// average and variance) for the fitness of the population
	/// regarded by an optimization process.
	/// The classes should be used for Hypothesis tests
	/// on given fitnesses 
	/// </summary>
	public interface IInSampleFitnessDistributionEstimator
	{
		/// <summary>Returns an estimation of the average fitness
		/// for a given population of fitnesses	
		/// </summary>
		/// <param name="geneticChooser">Genetic chooser that targets the population
		/// through the way fitness has to be computed</param>
		/// <param name="eligibleTickers">eligible tickers used for the in sample optimization</param>
		/// <param name="returnsManager">manager to efficiently handle in sample optimization</param>
		/// <param name="sampleLength">number of samples that have to be drawn from
		/// the population for the estimation</param>
		/// <returns>Estimation of the average fitness of the population</returns>
		double GetAverage( GeneticChooser geneticChooser,
		                   EligibleTickers eligibleTickers ,
		                   ReturnsManager returnsManager,
		                   int sampleLength );
		
		double GetVariance( GeneticChooser geneticChooser,
		                    EligibleTickers eligibleTickers ,
		                    ReturnsManager returnsManager,
		                    int sampleLength );
	}
}
