/*
QuantProject - Quantitative Finance Library

DrivenByFVProviderFitnessEvaluator.cs
Copyright (C) 2011
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

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers.Genetic
{
	/// <summary>
	/// Evaluates (in sample) the fitness for a given TestingPositions
	/// combining fundamental analysis with the sharpe ratio
	/// of a given series of returns
	/// </summary>
	[Serializable]
	public class DrivenByFVProviderFitnessEvaluator : IFitnessEvaluator
	{
		private IEquityEvaluator pastReturnsEvaluator;
		
		public string Description
		{
			get
			{
				string description = "FitnessEvaluatedWith_" +
								this.pastReturnsEvaluator.GetType().ToString();
				return description;
			}
		}
		
		public DrivenByFVProviderFitnessEvaluator(IEquityEvaluator strategyEquityEvaluator)
		{
			this.pastReturnsEvaluator = strategyEquityEvaluator;
		}

		private float getFitnessValue_getFitnessValueActually(
			TestingPositions testingPositions, IReturnsManager returnsManager )
		{
			float fitnessValue;
			//the returnsManager is based on OTCCTO End Of Day intervals -
			//always an even number, by definition!
			float[] returns = 
				testingPositions.WeightedPositions.GetReturns(returnsManager);
			float fitnessForPastReturns = this.pastReturnsEvaluator.GetReturnsEvaluation(returns);
			
			
			fitnessValue = fitnessForPastReturns * fitnessGivenByFundamentals;
				
			if( double.IsNaN(fitnessValue) )
				fitnessValue = 0;
			return fitnessValue;
		}
		
		public double GetFitnessValue(object meaning , IReturnsManager returnsManager )
		{
			float fitnessValue = -0.5f;
			TestingPositions testingPositions = (TestingPositions)meaning;
			if(testingPositions.WeightedPositions != null)
				fitnessValue = this.getFitnessValue_getFitnessValueActually( testingPositions, returnsManager );
			return fitnessValue;
		}
	}
}
