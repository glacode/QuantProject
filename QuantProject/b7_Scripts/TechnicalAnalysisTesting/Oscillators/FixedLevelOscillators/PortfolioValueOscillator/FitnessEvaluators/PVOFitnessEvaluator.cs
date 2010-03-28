/*
QuantProject - Quantitative Finance Library

PVOFitnessEvaluator.cs
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

using QuantProject.ADT;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators
{
	/// <summary>
	/// Evaluates (in sample) the fitness for a given TestingPositions
	/// </summary>
	[Serializable]
	public class PVOFitnessEvaluator : IFitnessEvaluator
	{
		private IEquityEvaluator strategyEquityEvaluator;
		private float takeProfit;
		private float stopLoss;
		
		public string Description
		{
			get
			{
				string description = "FitnessEvaluatedWith_" +
								this.strategyEquityEvaluator.GetType().ToString();
				return description;
			}
		}
		
		public PVOFitnessEvaluator(IEquityEvaluator strategyEquityEvaluator,
		                           double takeProfit, double stopLoss)
		{
			this.strategyEquityEvaluator = strategyEquityEvaluator;
			this.takeProfit = Convert.ToSingle(takeProfit);
			this.stopLoss = Convert.ToSingle(stopLoss);
		}
		
		#region GetFitnessValue
		//returns true if the strategy gets effective positions on
		//the market for at least a quarter of the market days
		private bool getFitnessValue_getFitnessValueActually_strategyGetsSufficientPositions(float[] strategyReturns)
		{
			int daysOnTheMarket = 0;
			foreach(float strategyReturn in strategyReturns)
				if(strategyReturn != 0)
					//the applied strategy gets positions on the market
					daysOnTheMarket++;

			return daysOnTheMarket >= strategyReturns.Length / 2;
		}

		private bool getFitnessValue_getFitnessValueActually_itIsTimeToExit(
			float[] strategyReturns, int currentIndex, int indexOfLastSignal )
		{
			bool returnValue = false;
			float gainOrLoss;
			float equityValue = 1;
			if(indexOfLastSignal >= 1 )
				for(int i = 0; i + indexOfLastSignal < currentIndex; i++)
					equityValue = equityValue + equityValue * strategyReturns[indexOfLastSignal + 1 + i];
			gainOrLoss = equityValue - 1;
			if(gainOrLoss >= this.takeProfit || gainOrLoss <= -this.stopLoss)
				returnValue = true;
			return returnValue;
		}
		
		private float getFitnessValue_getFitnessValueActually(
			TestingPositions testingPositions, IReturnsManager returnsManager )
		{
			float fitnessValue = -0.5f;
			PVOPositions pvoPositions = (PVOPositions)testingPositions;
			float oversoldThreshold = Convert.ToSingle(pvoPositions.OversoldThreshold);
			float overboughtThreshold = Convert.ToSingle(pvoPositions.OverboughtThreshold);
			float[] plainWeightedPositionsReturns = 
				testingPositions.WeightedPositions.GetReturns(returnsManager);
			float[] strategyReturns = new float[ plainWeightedPositionsReturns.Length ];
			strategyReturns[0] = 0;
			strategyReturns[1] = 0;//at the two first days the
			//strategy returns is equal to 0 because no position
			//has been entered
			float coefficient = 0;
			int indexOfLastSignal = 0;// the last position where a threshold has been reached
			for(int i = 1; i < strategyReturns.Length - 1; i++)
			{
				if( plainWeightedPositionsReturns[i] >= overboughtThreshold &&
				    (plainWeightedPositionsReturns[i - 1] > - oversoldThreshold && 
				    plainWeightedPositionsReturns[i - 1] < overboughtThreshold ) &&
				    indexOfLastSignal == 0 )
					//portfolio has been overbought and in the previous period 
					//it was efficient
				{
					coefficient = -1;
					indexOfLastSignal = i;
				}	
				if( plainWeightedPositionsReturns[i] <= - oversoldThreshold && 
				         (plainWeightedPositionsReturns[i - 1] > - oversoldThreshold && 
				    			plainWeightedPositionsReturns[i - 1] < overboughtThreshold ) &&
				    indexOfLastSignal == 0 )
					//portfolio has been oversold and in the previous period 
					//it was efficient 			
				{
					coefficient = 1;
					indexOfLastSignal = i;
				}	
				if ( getFitnessValue_getFitnessValueActually_itIsTimeToExit(strategyReturns, i, indexOfLastSignal) )
				{	
					coefficient = 0;
					indexOfLastSignal = 0;
				}
				strategyReturns[i + 1] = coefficient * plainWeightedPositionsReturns[i + 1];
			}
			if(this.getFitnessValue_getFitnessValueActually_strategyGetsSufficientPositions(strategyReturns))
				fitnessValue = this.strategyEquityEvaluator.GetReturnsEvaluation(strategyReturns);
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
		#endregion
	}
}
