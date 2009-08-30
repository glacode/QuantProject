/*
QuantProject - Quantitative Finance Library

BasicDecoderForPVOPositions.cs
Copyright (C) 2008
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

using QuantProject.ADT.Collections;
using QuantProject.ADT.Optimizing.Decoding;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding
{
	/// <summary>
	/// Decodes optimization candidates to a 
	/// PVOPositions
	/// In this implementation, only tickers and thresholds are decoded
	/// </summary>
	[Serializable]
	public class BasicDecoderForPVOPositions : BasicDecoderForTestingPositions
	{
		protected int numOfGenesDedicatedToThresholds;
		protected int numDaysForOscillatingPeriod;
		protected double oversoldThreshold;
		protected double overboughtThreshold;
		protected int divisorForThresholdComputation;
		
		public BasicDecoderForPVOPositions(bool symmetricalThresholds,
		                                   int divisorForThresholdComputation,
		                                   int numDaysForOscillatingPeriod) :
																			 base()
		{
			if(symmetricalThresholds)
        this.numOfGenesDedicatedToThresholds = 1;
			else
				this.numOfGenesDedicatedToThresholds = 2;
			this.divisorForThresholdComputation = divisorForThresholdComputation;
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
		}

		protected override TestingPositions getMeaningForUndecodable()
		{
			return new PVOPositions();
		}
		
		private void decodeDecodable_setThresholds()
		{
			if(this.numOfGenesDedicatedToThresholds == 1)
			//symmetrical	thresholds
			{
				this.oversoldThreshold =
					Convert.ToDouble(this.encoded[0])/Convert.ToDouble(this.divisorForThresholdComputation);
				this.overboughtThreshold = this.oversoldThreshold;
			}
			else//different thresholds, that is this.numOfGenesDedicatedToThresholds == 1
			{
				this.oversoldThreshold =
					Convert.ToDouble(this.encoded[0])/Convert.ToDouble(this.divisorForThresholdComputation);
				this.overboughtThreshold =
					Convert.ToDouble(this.encoded[1])/Convert.ToDouble(this.divisorForThresholdComputation);
			}
		}
		
		protected override TestingPositions decodeDecodable()
		{
			SignedTickers signedTickers =	this.decodeSignedTickers();
			this.decodeDecodable_setThresholds();
			PVOPositions pvoPositions =	new PVOPositions(
					new WeightedPositions( this.getUnsignedWeights(signedTickers), signedTickers),
				  this.oversoldThreshold, this.overboughtThreshold,
				  this.numDaysForOscillatingPeriod);
	
			return pvoPositions;
		}
		
		protected override string getDescription()
		{
			return "PVO_Dcdr_NoWghts";
		}

		protected override void setTickerRelatedGeneValues()
		{
			this.tickerRelatedGeneValues = new int[this.encoded.Length - this.numOfGenesDedicatedToThresholds];
			Array.Copy(this.encoded , this.numOfGenesDedicatedToThresholds ,
								 this.tickerRelatedGeneValues , 0, 
								 this.encoded.Length - this.numOfGenesDedicatedToThresholds);
		}
	}
}
