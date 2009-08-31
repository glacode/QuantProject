/*
QuantProject - Quantitative Finance Library

OTCFitnessEvaluator.cs
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
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic
{
	/// <summary>
	/// Evaluates (in sample) the fitness for a given TestingPositions
	/// with respect to the "open to close" (OTC) strategy
	/// </summary>
	[Serializable]
	public class OTCFitnessEvaluator : IFitnessEvaluator
	{
		private IEquityEvaluator strategyEquityEvaluator;
		
		public string Description
		{
			get
			{
				string description = "FitnessEvaluatedWith_" +
								this.strategyEquityEvaluator.GetType().ToString();
				return description;
			}
		}
		
		public OTCFitnessEvaluator(IEquityEvaluator strategyEquityEvaluator)
		{
			this.strategyEquityEvaluator = strategyEquityEvaluator;
		}

		private float getFitnessValue_getFitnessValueActually(
			TestingPositions testingPositions, ReturnsManager returnsManager )
		{
			float fitnessValue;
			//the returnsManager is based on OTCCTO End Of Day intervals -
			//always an even number, by definition!
			float[] returns = 
				testingPositions.WeightedPositions.GetReturns(returnsManager);
			float[] strategyReturns = new float[ returns.Length / 2 ];
			for( int i = 0; i < returns.Length; i++ )
				if( i%2 == 0 )
				//returns[i] is a OpenToClose return
				strategyReturns[ i / 2 ] = returns[ i ];
			fitnessValue = 
				this.strategyEquityEvaluator.GetReturnsEvaluation(strategyReturns);
			if( double.IsNaN(fitnessValue) )
				fitnessValue = 0;
			return fitnessValue;
		}
		
		public double GetFitnessValue(object meaning , ReturnsManager returnsManager )
		{
			float fitnessValue = -0.5f;
			TestingPositions testingPositions = (TestingPositions)meaning;
			if(testingPositions.WeightedPositions != null)
				fitnessValue = this.getFitnessValue_getFitnessValueActually( testingPositions, returnsManager );
			return fitnessValue;
		}
	}
}
