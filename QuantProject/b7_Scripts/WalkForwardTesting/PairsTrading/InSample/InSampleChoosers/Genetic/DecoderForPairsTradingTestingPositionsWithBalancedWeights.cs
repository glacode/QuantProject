/*
QuantProject - Quantitative Finance Library

DecoderForPairsTradingTestingPositionsWithBalancedWeights.cs
Copyright (C) 2008
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
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Decoder for the Pairs Trading strategy
	/// </summary>
	public class DecoderForPairsTradingTestingPositionsWithBalancedWeights :
		DecoderForTestingPositionsWithBalancedWeights
	{
		public DecoderForPairsTradingTestingPositionsWithBalancedWeights()
		{
		}
		
		protected override TestingPositions getTestingPositions(double[] weights, string[] tickers)
		{
			WeightedPositions weightedPositions =
				new WeightedPositions( weights , tickers );
			PairsTradingTestingPositions pairsTradingTestingPositions =
				new PairsTradingTestingPositions( weightedPositions );
			return pairsTradingTestingPositions;
		}
	}
}
