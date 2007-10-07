/*
QuantProject - Quantitative Finance Library

WFLagFitnessEvaluator.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Computes the fitness for a given WFLagWeightedPositions object
	/// </summary>
	public class WFLagFitnessEvaluator
	{
		private ReturnsManager returnsManager;
		private IEquityEvaluator equityEvaluator;


		public WFLagFitnessEvaluator(
			IEquityEvaluator equityEvaluator ,
			ReturnsManager returnsManager )
		{
			this.equityEvaluator = equityEvaluator;
			this.returnsManager = returnsManager;
		}
		#region GetFitnessValue
		private string[] getTickers( WeightedPositions weightedPositions )
		{
			string[] tickers = new string[ weightedPositions.Count ];
			for ( int i = 0 ; i < weightedPositions.Count ; i++ )
			{
				WeightedPosition weightedPosition = weightedPositions.GetWeightedPosition( i );
				tickers[ i ] = weightedPosition.Ticker;
			}
			return tickers;
		}
		private float[] getMultipliers( WeightedPositions weightedPositions )
		{
			float[] multipliers = new float[ weightedPositions.Count ];
			for ( int i = 0 ; i < weightedPositions.Count ; i++ )
			{
				WeightedPosition weightedPosition = weightedPositions.GetWeightedPosition( i );
				multipliers[ i ] = Convert.ToSingle( weightedPosition.Weight );
			}
			return multipliers;
		}

		private float[] getFitnessValue_getLinearCombinationReturns(
			WeightedPositions weightedPositions )
		{
			float[] linearCombinatioReturns =
				weightedPositions.GetReturns(
				this.returnsManager );
			return linearCombinatioReturns;
		}
		private float[] getFitnessValue_getStrategyReturn(
			float[] drivingPositionsReturns , float[] portfolioPositionsReturns )
		{
			// strategyReturns contains one element less than drivingPositionsReturns,
			// because there is no strategy for the very first period (at least
			// one day signal is needed)
			float[] strategyReturns = new float[ portfolioPositionsReturns.Length - 1 ];
			for ( int i = 0 ; i < portfolioPositionsReturns.Length - 1 ; i++ )
				if ( drivingPositionsReturns[ i ] < 0 )
					// the current linear combination of tickers, at period i
					// has a negative return
					
					// go short tomorrow
					strategyReturns[ i ] = -portfolioPositionsReturns[ i + 1 ];
				else
					// the current linear combination of tickers, at period i
					// has a positive return
					
					// go long tomorrow
					strategyReturns[ i ] = portfolioPositionsReturns[ i + 1 ];
			return strategyReturns;

		}
		private float getFitnessValue( float[] strategyReturns )
		{
			float fitnessValue =
				this.equityEvaluator.GetReturnsEvaluation(
				strategyReturns );
			return fitnessValue;
		}

		private float getFitnessValue(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			float[] drivingPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.DrivingWeightedPositions );
			float[] portfolioPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.PortfolioWeightedPositions );
			float[] strategyReturns =
				this.getFitnessValue_getStrategyReturn(
				drivingPositionsReturns , portfolioPositionsReturns );
			float fitnessValue = this.getFitnessValue( strategyReturns );
			return fitnessValue;
		}
		public double GetFitnessValue( object meaning )
		{
			double fitnessValue;
			if ( meaning is WFLagWeightedPositions )
			{
				// for the current optimization's candidate,
				// all driving positions are distinct and
				// all portfolio positions are distinct
				WFLagWeightedPositions wFLagWeightedPositions =
					(WFLagWeightedPositions)meaning;
				fitnessValue = this.getFitnessValue( wFLagWeightedPositions );
			}
			else
			{
				// the current optimization's candidate contains
				// a duplicate gene for either
				// driving positions or portfolio positions
				fitnessValue = -0.4;
			}
			return fitnessValue;
		}
		#endregion GetFitnessValue
	}
}
