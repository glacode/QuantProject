/*
QuantProject - Quantitative Finance Library

PairsTradingFitnessEvaluator.cs
Copyright (C) 2007
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

using QuantProject.ADT.Statistics;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Evaluates (in sample) the Pearson Correlation
	/// between two weighted positions
	/// </summary>
	public class PairsTradingFitnessEvaluator : IFitnessEvaluator
	{
		private double maxCorrelationAllowed;

		public string Description
		{
			get
			{
				return "pearsonCrrltn";
			}
		}

		public PairsTradingFitnessEvaluator(
			double maxCorrelationAllowed )
		{
			this.maxCorrelationAllowed = maxCorrelationAllowed;
		}
		#region GetFitnessValue
		private void getFitnessValue_checkParameters( object meaning )
		{
			if ( !( meaning is TestingPositions ) )
				throw new Exception(
					"The meaning should always be a TestingPositions!" );
		}
		private void getFitnessValue_checkWeightedPositions(
			WeightedPositions weightedPositions )
		{
			if ( weightedPositions.Count != 2 )
				throw new Exception( "This fitness evaluatore requires " +
					"two positions!" );
		}
		private double getFitnessValue(
			WeightedPosition firstPosition , WeightedPosition secondPosition ,
			ReturnsManager returnsManager )
		{
			float[] firstPositionReturns =
				returnsManager.GetReturns( firstPosition.Ticker );
			float[] secondPositionReturns =
				returnsManager.GetReturns( secondPosition.Ticker );
			double fitnessValue =	BasicFunctions.PearsonCorrelationCoefficient(
				firstPosition.Weight , firstPositionReturns ,
				secondPosition.Weight , secondPositionReturns );
			return fitnessValue;
		}

		private double getFitnessValue(
			WeightedPositions weightedPositions , ReturnsManager returnsManager )
		{
			this.getFitnessValue_checkWeightedPositions( weightedPositions );
			WeightedPosition firstPosition = weightedPositions[ 0 ];
			WeightedPosition secondPosition = weightedPositions[ 1 ];
			double fitnessValue = this.getFitnessValue(
				firstPosition , secondPosition , returnsManager );
			return fitnessValue;
		}
		private double getFitnessValue( TestingPositions testingPositions ,
			ReturnsManager returnsManager )
		{
			double fitnessValue;
			WeightedPositions weightedPositions = testingPositions.WeightedPositions;
			if ( weightedPositions == null )
				// the current optimization's candidate contains
				// two genes that decode to the same tickers
				fitnessValue = -0.4;
			else
				// for the current optimization's candidate,
				// all positions's tickers are distinct
			{
				fitnessValue = this.getFitnessValue( weightedPositions , returnsManager );
				if ( fitnessValue > this.maxCorrelationAllowed )
					// the two positions are too correlated. They may represent
					// the same instrument
					fitnessValue = -0.4;
			}
			return fitnessValue;
		}
		public double GetFitnessValue( object meaning , ReturnsManager returnsManager )
		{
			this.getFitnessValue_checkParameters( meaning );			
			double fitnessValue =
				this.getFitnessValue( (TestingPositions)meaning , returnsManager );
			return fitnessValue;
		}
		#endregion
	}
}
