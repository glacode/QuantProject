/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesFitnessEvaluator.cs
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
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Evaluates (in sample) the fitness for a given WeightedPositions
	/// </summary>
	[Serializable]
	public class FixedLengthTwoPhasesFitnessEvaluator : IFitnessEvaluator
	{
		private const double fitnessForInvalidCandidate = -1000d;

		private IEquityEvaluator equityEvaluator;

		public string Description
		{
			get
			{
				return "FtnssEvltr_fltp_1phLong_2phShort_" +
					this.equityEvaluator.Description;
			}
		}

		public FixedLengthTwoPhasesFitnessEvaluator(
			IEquityEvaluator equityEvaluator )
		{
			this.equityEvaluator = equityEvaluator;
		}
		
		#region GetFitnessValue
		private void getFitnessValue_checkParameters( object meaning )
		{
			if ( !( meaning is TestingPositions ) )
				throw new Exception(
					"The meaning should always be a TestingPositions!" );
		}
		/// <summary>
		/// This private method is written in compact mode, in order
		/// to gain efficiency (it would have been more readable if
		/// we used a nested method call in it, but it would have
		/// been much slower)
		/// </summary>
		/// <param name="weightedPositionsReturns"></param>
		/// <returns></returns>
		private float[] getFitnessValue_getStrategyReturns(
			float[] weightedPositionsReturns )
		{
			float[] strategyReturns = new float[ weightedPositionsReturns.Length ];
			for ( int i = 0 ; i < weightedPositionsReturns.Length ; i++ )
			{
				// this for content would be more readable if a method call was used,
				// but it would have dramatically slowed down the script
				if ( i % 2 == 0 )
					// the i-th return refers to the first phase, thus it is
					// a long phase
					strategyReturns[ i ] = weightedPositionsReturns[ i ];
				else
					// the i-th return refers to the first phase, thus it is
					// a long phase
					strategyReturns[ i ] = -weightedPositionsReturns[ i ];
			}
			return strategyReturns;
		}
		private double getFitnessValue(
			WeightedPositions weightedPositions , ReturnsManager returnsManager )
		{
			float[] weightedPositionsReturns =
				weightedPositions.GetReturns( returnsManager );
			float[] strategyReturns =
				this.getFitnessValue_getStrategyReturns(	weightedPositionsReturns );
			float fitnessValue =
				this.equityEvaluator.GetReturnsEvaluation( strategyReturns );
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
				fitnessValue = fitnessForInvalidCandidate;
			else
				// for the current optimization's candidate,
				// all positions's tickers are distinct
				fitnessValue = this.getFitnessValue( weightedPositions , returnsManager );
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
