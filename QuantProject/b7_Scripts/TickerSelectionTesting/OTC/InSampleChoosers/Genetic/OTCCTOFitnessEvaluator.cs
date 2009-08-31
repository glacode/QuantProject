/*
QuantProject - Quantitative Finance Library

OTCCTOFitnessEvaluator.cs
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
	/// It evaluates the strategy:
	/// buy at open, sell at close; sell at close, buy at open
	/// </summary>
	[Serializable]
	public class OTCCTOFitnessEvaluator : IFitnessEvaluator
	{
		private IEquityEvaluator strategyEquityEvaluator;
		
		public string Description
		{
			get
			{
				string description = "OTCCTOFitnessEvaluatedWith_" +
								this.strategyEquityEvaluator.GetType().ToString();
				return description;
			}
		}
		
		public OTCCTOFitnessEvaluator(IEquityEvaluator strategyEquityEvaluator)
		{
			this.strategyEquityEvaluator = strategyEquityEvaluator;
		}

		private float getFitnessValue_getFitnessValueActually(
			TestingPositions testingPositions, ReturnsManager returnsManager )
		{
			float fitnessValue;
			float[] strategyReturns = 
				testingPositions.WeightedPositions.GetReturns(returnsManager);
			for(int i = 0; i < strategyReturns.Length; i++)
				if(i%2 != 0)
				//strategyReturns[i] is a CloseToOpen return:
				//the OTCCTO strategy implies to reverse positions at night
				strategyReturns[i] = - strategyReturns[i];
			fitnessValue =
				this.strategyEquityEvaluator.GetReturnsEvaluation(strategyReturns);
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
