/*
QuantProject - Quantitative Finance Library

BasicInSampleFitnessDistributionEstimator.cs
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies.InSample;
//using QuantProject.Business.DataProviders;
//using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.InSample.InSampleFitnessDistributionEstimation
{
	/// <summary>
	/// Basic IInSampleFitnessDistributionEstimator for estimation
	/// of average and variance of the fitness for the population from
	/// which best fitnesses are retrieved through an optimization process 
	/// </summary>
	[Serializable]
	public class BasicInSampleFitnessDistributionEstimator : IInSampleFitnessDistributionEstimator
	{
		protected IGenomeManager genomeManager;
		protected GeneticOptimizer geneticOptimizer;
		protected ArrayList sampleFitnesses;
		protected EligibleTickers currentEligibleTickers;
		protected ReturnsManager currentReturnsManager;
		protected int sampleLength;
		
		/// <summary>
		/// Basic IInSampleFitnessDistributionEstimator for estimation
		/// of average and variance of the fitness for the population from
		/// which best fitnesses are retrieved through an optimization process 
		/// </summary>
		public BasicInSampleFitnessDistributionEstimator()
		{
		}
			
		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			;
		}
		
		private void runGeneticOptimizerAndSetSampleFitnesses_setSampleFitnesses()
		{
			this.sampleFitnesses = new ArrayList();
			for(int i = 0; i < this.geneticOptimizer.CurrentGeneration.Count; i++)
				if ( this.geneticOptimizer.CurrentGeneration[i] is Genome &&
				     !double.IsInfinity( ((Genome)this.geneticOptimizer.CurrentGeneration[i]).Fitness ) &&
						 !double.IsNaN( ((Genome)this.geneticOptimizer.CurrentGeneration[i]).Fitness ) &&
						 double.MinValue != ((Genome)this.geneticOptimizer.CurrentGeneration[i]).Fitness )
							
							this.sampleFitnesses.Add( ((Genome)this.geneticOptimizer.CurrentGeneration[i]).Fitness);
		}
		
		private void runGeneticOptimizerAndSetSampleFitnesses(GeneticChooser geneticChooser ,
		                    		 				 EligibleTickers eligibleTickers ,
		                    		 				 ReturnsManager returnsManager ,
		                         				 int sampleLength)
		{
			if(	this.genomeManager == null ||
			    !ReferenceEquals(this.currentEligibleTickers, eligibleTickers) ||
			    !ReferenceEquals(this.currentReturnsManager, returnsManager) ||
			    this.sampleLength != sampleLength)
			{
				this.sampleLength = sampleLength;
				this.currentEligibleTickers = eligibleTickers;
				this.currentReturnsManager = returnsManager;
				this.genomeManager = 
					geneticChooser.GetGenomeManager(eligibleTickers, returnsManager);
				this.geneticOptimizer = new GeneticOptimizer(this.genomeManager,
				                                             sampleLength,
				                                             0, ConstantsProvider.SeedForRandomGenerator);
				this.geneticOptimizer.NewGeneration +=
					new NewGenerationEventHandler( this.newGenerationEventHandler );
				this.geneticOptimizer.Run( false );
				this.runGeneticOptimizerAndSetSampleFitnesses_setSampleFitnesses();
			}
		}
		
		/// <summary>
		/// Returns an estimation of the average fitness
		/// for the population of fitnesses targeted
		/// by the given geneticChooser (through IGenomeManager) 
		/// </summary>
		public double GetAverage(GeneticChooser geneticChooser,
		                    		 EligibleTickers eligibleTickers ,
		                    		 ReturnsManager returnsManager,
		                         int sampleLength)
		{
			this.runGeneticOptimizerAndSetSampleFitnesses(geneticChooser ,	eligibleTickers ,
		                    			 returnsManager , sampleLength);
			double average =
				BasicFunctions.GetSimpleAverage(this.sampleFitnesses);
			
			return average;
		}
		
		/// <summary>
		/// Returns an estimation of the variance 
		/// for the population of fitnesses targeted
		/// by the given geneticChooser (through IGenomeManager) 
		/// </summary>
		public double GetVariance(GeneticChooser geneticChooser,
		                    		 EligibleTickers eligibleTickers ,
		                    		 ReturnsManager returnsManager,
		                         int sampleLength)
		{
			this.runGeneticOptimizerAndSetSampleFitnesses(geneticChooser ,	eligibleTickers ,
		                    			 returnsManager , sampleLength);
			double variance = 
				BasicFunctions.GetVariance(this.sampleFitnesses);
			
			return variance;
		}
	}
}
