/*
QuantProject - Quantitative Finance Library

BuyAndHoldFitnessEvaluator.cs
Copyright (C) 2010
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

namespace QuantProject.Business.Strategies.Optimizing.FitnessEvaluation
{
	/// <summary>
	/// Implements IFitnessEvaluator interface for the 
	/// basic strategy of buying and holding a portofolio
	/// (represented, normally, by a TestingPositions
	/// passed to the GetFitnessValue method)
	/// </summary>
	[Serializable]
	public class BuyAndHoldFitnessEvaluator : IFitnessEvaluator
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
		
		public BuyAndHoldFitnessEvaluator(IEquityEvaluator strategyEquityEvaluator)
		{
			this.strategyEquityEvaluator = strategyEquityEvaluator;
		}

		private float getFitnessValue_getFitnessValueActually(
			TestingPositions testingPositions, IReturnsManager returnsManager )
		{
			float fitnessValue;
			float[] returns = 
				testingPositions.WeightedPositions.GetReturns(returnsManager);
			fitnessValue = 
				this.strategyEquityEvaluator.GetReturnsEvaluation(returns);
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
