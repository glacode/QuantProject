/*
QuantProject - Quantitative Finance Library

DecoderForTestingPositionsWithWeights.cs
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

using QuantProject.ADT;
using QuantProject.Business.Strategies;

namespace QuantProject.Business.Strategies.Optimizing.Decoding
{
	/// <summary>
	/// Decodes optimization candidates to a
	/// TestingPositions
	/// In this implementation, weights are computed through encoded values,
	/// that should be optimized by a given optimizer
	/// </summary>
	[Serializable]
	public abstract class DecoderForTestingPositionsWithWeights :
		BasicDecoderForTestingPositions
	{
		protected int[] weightsRelatedGeneValues;
		
		public DecoderForTestingPositionsWithWeights() : base()
		{
		}
		
		//this method sets weightsRelatedGeneValues too
		protected override void setTickerRelatedGeneValues()
		{
			this.tickerRelatedGeneValues = new int[this.encoded.Length/2];
			this.weightsRelatedGeneValues = new int[this.encoded.Length/2];
			int idxForTickerRelatedGeneValues = 0;
			int idxForWeightsRelatedGeneValues = 0;
			for(int i = 0; i< this.encoded.Length; i++)
			{
				if(i%2 == 0) // position of current encoded is even
				{	
					this.weightsRelatedGeneValues[idxForWeightsRelatedGeneValues] =
						this.encoded[i];
					idxForWeightsRelatedGeneValues++;
				}
				else // position of current encoded is odd
				{	
					this.tickerRelatedGeneValues[idxForTickerRelatedGeneValues] =
						this.encoded[i];
					idxForTickerRelatedGeneValues++;
				}	
			}
		}
		
		protected double getWeight( int idxInGenes )
		{
			double weight = 0.0;
			double totVariableWeight =
				ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers;
			double minWeight = ( 1.0 - totVariableWeight ) /
												  weightsRelatedGeneValues.Length;
			double totalOfValuesForWeights = 0.0;
			for(int j = 0; j < weightsRelatedGeneValues.Length; j++)
			// ticker's weight is contained in weightsRelatedGeneValues
			// totalOfValuesForWeights can't be equal to 0 !	
					totalOfValuesForWeights += 
						( Math.Abs(weightsRelatedGeneValues[j]) + 1.0 );
			double freeWeight = 
				( Math.Abs(weightsRelatedGeneValues[idxInGenes]) + 1.0 ) /
				 totalOfValuesForWeights;
			weight = minWeight + freeWeight * totVariableWeight;
			
			return weight;
		}
		
		protected override double[] getWeights(SignedTickers signedTickers)
		{
			double[] weights = new double[this.weightsRelatedGeneValues.Length];
			double coefficient;
			for(int i = 0; i<weights.Length; i++)
			{	
				if(signedTickers[i].IsLong)
					coefficient = 1.0;
				else//is Short
					coefficient = -1.0;
				weights[i] = coefficient * this.getWeight( i );
			}
			return weights;
		}
		
		protected override double[] getUnsignedWeights(SignedTickers signedTickers)
		{
			double[] weights = this.getWeights( signedTickers );
			for(int i = 0; i<weights.Length; i++)
			{	
				weights[i] = Math.Abs(weights[i]);
			}
			return weights;
		}
		
		protected override string getDescription()
		{
			return "bscTstngPstnsWthWgths";
		}
	}
}


