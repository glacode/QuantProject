/*
QuantProject - Quantitative Finance Library

GenomeManagerForLinearRegressionWithRatios.cs
Copyright (C) 2010
Glauco Siliprandi

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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Decodes genes when two groups of eligibles are given
	/// </summary>
	[Serializable]
	public class GenomeManagerForLinearRegression : GenomeManagerWithDuplicateGenes
	{
		private EligibleTickers eligibleTickersForTrading;
		private EligibleTickers eligibleTickersForSignaling;
		private IReturnsManager returnsManagerForTradingTickers;
		private IReturnsManager returnsManagerForSignalingTickers;
		private IGenomeManager genomeManagerForTradingTickers;
		private IGenomeManager genomeManagerForSignalingTickers;
		private DecoderForLinearRegressionTestingPositions
			decoderForLinearRegressionTestingPositions;
		
		public GenomeManagerForLinearRegression(
			EligibleTickers eligibleTickersForTrading ,
			EligibleTickers eligibleTickersForSignaling ,
//			ReturnsManager returnsManager ,
			IReturnsManager returnsManagerForTradingTickers ,
			IReturnsManager returnsManagerForSignalingTickers ,
			DecoderForLinearRegressionTestingPositions
			decoderForLinearRegressionTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			GenomeManagerType genomeManagerType ,
			int seedForRandomGeneratorForTradingTickers ,
			int seedForRandomGeneratorForSignalingTickers ) :
			base(
				decoderForLinearRegressionTestingPositions.NumberOfTickersForTrading ,
				eligibleTickersForTrading ,
				returnsManagerForTradingTickers ,	// this one should not be used
				new BasicDecoderForTestingPositions() ,
				fitnessEvaluator ,
				genomeManagerType ,
				seedForRandomGeneratorForTradingTickers )
			
		{
			this.eligibleTickersForTrading = eligibleTickersForTrading;
			this.eligibleTickersForSignaling = eligibleTickersForSignaling;
			this.returnsManagerForTradingTickers = returnsManagerForTradingTickers;
			this.returnsManagerForSignalingTickers = returnsManagerForSignalingTickers;
			((LinearRegressionFitnessEvaluator)this.fitnessEvaluator).ReturnsManagerForTradingTickers =
				this.returnsManagerForTradingTickers;
			((LinearRegressionFitnessEvaluator)this.fitnessEvaluator).ReturnsManagerForSignalingTickers =
				this.returnsManagerForSignalingTickers;
			this.genomeManagerForTradingTickers =
				new GenomeManagerWithDuplicateGenes(
					decoderForLinearRegressionTestingPositions.NumberOfTickersForTrading ,
					this.eligibleTickersForTrading ,
					this.returnsManagerForTradingTickers ,
					new BasicDecoderForTestingPositions() ,
					fitnessEvaluator ,
					GenomeManagerType.ShortAndLong ,
					seedForRandomGeneratorForTradingTickers );
			this.genomeManagerForSignalingTickers =
				new GenomeManagerWithDuplicateGenes(
					decoderForLinearRegressionTestingPositions.NumberOfSignalingPortfolios ,
					this.eligibleTickersForSignaling ,
					this.returnsManagerForSignalingTickers ,
					new BasicDecoderForTestingPositions() ,
					fitnessEvaluator ,
					GenomeManagerType.ShortAndLong ,
					seedForRandomGeneratorForSignalingTickers );
			this.decoderForLinearRegressionTestingPositions =
				decoderForLinearRegressionTestingPositions;
//				new DecoderForLinearRegressionTestingPositions(
////					decoderForLinearRegressionTestingPositions.NumberOfTickersForTrading ,
//					decoderForLinearRegressionTestingPositions.NumberOfSignalingPortfolios );
			this.genomeSize =
				decoderForLinearRegressionTestingPositions.NumberOfTickersForTrading +
				decoderForLinearRegressionTestingPositions.NumberOfSignalingPortfolios;
		}
		
		public override int GetMaxValueForGenes( int genePosition )
		{
			int maxValueForGenes = 0;
			if ( genePosition < this.genomeManagerForTradingTickers.GenomeSize )
				// genePosition referst to a tradable ticker
				maxValueForGenes =
					this.genomeManagerForTradingTickers.GetMaxValueForGenes(
						genePosition );
			else
			{
				// genePosition referst to a signaling ticker
				int genePositionInTheSecondGroup = genePosition -
					this.genomeManagerForTradingTickers.GenomeSize;
				maxValueForGenes = this.genomeManagerForSignalingTickers.GetMaxValueForGenes(
					genePositionInTheSecondGroup );
			}
			return maxValueForGenes;
		}

		public override int GetMinValueForGenes( int genePosition )
		{
			int minValueForGenes = 0;
			if ( genePosition < this.genomeManagerForTradingTickers.GenomeSize )
				// genePosition referst to a tradable ticker
				minValueForGenes = this.genomeManagerForTradingTickers.GetMinValueForGenes(
					genePosition );
			else
			{
				// genePosition referst to a signaling ticker
				int genePositionInTheSecondGroup = genePosition -
					genomeManagerForTradingTickers.GenomeSize;
				minValueForGenes = this.genomeManagerForSignalingTickers.GetMinValueForGenes(
					genePositionInTheSecondGroup );				
			}
			return minValueForGenes;
		}
		
//		public override int GetNewGeneValue( Genome genome, int genePosition )
//		{
//			double newGeneValue = 0;
//			if ( genePosition < genomeManagerForTradingTickers.GenomeSize )
//				// genePosition referst to the first group of eligibles
//				minValueForGenes = genomeManagerForTradingTickers.GetNewGeneValue(
//					genePosition );
//			else
//			{
//				// genePosition referst to the second group of eligibles
//				int genePositionInTheSecondGroup = genePosition -
//					genomeManagerForTradingTickers.GenomeSize;
//				minValueForGenes = genomeManagerForSignalingTickers.GetNewGeneValue(
//					genePositionInTheSecondGroup );
//
//			}
//			return minValueForGenes;
//		}
		
		/// <summary>
		/// Returns an array with two TestingPositions. The first element contains
		/// the decoding for the first group of eligibles, the second element contains
		/// the decoding for the second group of eligibles
		/// </summary>
		/// <param name="genome"></param>
		/// <returns></returns>
		public override object Decode(Genome genome)
		{
			object decoded =
				this.decoderForLinearRegressionTestingPositions.Decode(
					genome.Genes() ,
					this.eligibleTickersForTrading ,
					this.eligibleTickersForSignaling ,
					this.returnsManagerForTradingTickers ,
					this.returnsManagerForSignalingTickers );
//					this.returnsManager );
			return decoded;
		}
//		public virtual double GetFitnessValue(Genome genome)
//		{
//			double fitnessValue =
//					this.fitnessEvaluator.GetFitnessValue(genome.Meaning, this.returnsManager);
//
//			return fitnessValue;
//		}
	}
}
