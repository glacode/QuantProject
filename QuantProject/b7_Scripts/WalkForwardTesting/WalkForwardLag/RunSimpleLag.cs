/*
QuantProject - Quantitative Finance Library

RunWalkForwardLag.cs
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
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Tests a simple (non walk forward) lag strategy
	/// </summary>
	public class RunSimpleLag
	{
		public RunSimpleLag()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region Run
		#region getWFLagWeightedPositions
		#region getDrivingWeightedPositions
		private WeightedPositions getWeightedPositions(
			double[] drivingWeights , string[] drivingTickers )
		{
			double[] normalizedWeightValues =
				WeightedPositions.GetNormalizedWeights( drivingWeights );
			WeightedPositions weightedPositions =
				new WeightedPositions( normalizedWeightValues , drivingTickers );
			return weightedPositions;
		}
		#endregion getDrivingWeightedPositions
		private WFLagWeightedPositions getWFLagWeightedPositions(
			double[] drivingWeights , string[] drivingTickers ,
			double[] portfolioWeights , string[] portfolioTickers )
		{
			WeightedPositions drivingWeightedPositions =
				this.getWeightedPositions( drivingWeights , drivingTickers );
			WeightedPositions portfolioWeightedPositions =
				this.getWeightedPositions( portfolioWeights , portfolioTickers );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions( drivingWeightedPositions ,
				portfolioWeightedPositions);
			return wFLagWeightedPositions;
		}
		#endregion getWFLagWeightedPositions
		/// <summary>
		/// Executes the backtest
		/// </summary>
		public void Run()
		{
			// 4 optimal driving positions
//			double[] drivingWeights = { 0.550560760218599 , -0.172289820708961 , -0.16220520521007 , 0.11494421386237 };
//			string[] drivingTickers = { "^OEX" , "BDH" , "HHH" , "IIH" };

			// bad driving positions, despite having two optimal tickers
//			double[] drivingWeights = { -0.272826060576681 , 0.304736475342285 , -0.0953627582891244 , 0.32707470579191 };
//			string[] drivingTickers = { "^DJT" , "^OEX" , "BDH" , "EWJ" };
//			Description	"^DJT;-0.272826060576681--^OEX;0.304736475342285--BDH;-0.0953627582891244--EWJ;0.32707470579191--"	string

			// subset of 2 optimal driving positions
//			double[] drivingWeights = { 0.550560760218599 , -0.172289820708961 };
//			string[] drivingTickers = { "^OEX" , "BDH" };

			// subset of three optimal driving positions
//			double[] drivingWeights = { 0.550560760218599 , -0.172289820708961 , -0.16220520521007 };
//			string[] drivingTickers = { "^OEX" , "BDH" , "HHH" };

			// subseto of two optimal driving with similar weights
//			double[] drivingWeights = { 0.304736475342285 , -0.0953627582891244 };
//			string[] drivingTickers = { "^OEX" , "BDH" };
			
			// subset of single optimal driving from the 4 optimal driving
//			double[] drivingWeights = { 1 };
//			string[] drivingTickers = { "IIH" };



			// 2 optimal driving positions
//			double[] drivingWeights = { 0.74152744108066 , -0.25847255891934 };
//			string[] drivingTickers = { "^NYA" , "^PSE" };

			// subset of single driving positions out of 2 optimal driving
						double[] drivingWeights = { 1 };
						string[] drivingTickers = { "^NYA" };
//						double[] drivingWeights = { -1 };
//						string[] drivingTickers = { "^PSE" };

			// 2 optimal driving positions, but with weights slightly changed
//			double[] drivingWeights = { 0.7 , -0.3 };
//			string[] drivingTickers = { "^NYA" , "^PSE" };


			double[] portfolioWeights = { 0.442536856335084 , -0.557463143664916 };
			string[] portfolioTickers = { "IWM" , "SPY" };
			DateTime timeWhenOptimizationWasRequested = new DateTime( 2001 , 1 , 4 );
			int numberDaysForInSampleOptimization = 100;
			string benchmark = "EWQ";

			WFLagWeightedPositions wFLagWeightedPositions =
				this.getWFLagWeightedPositions( drivingWeights , drivingTickers ,
				portfolioWeights , portfolioTickers );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( wFLagWeightedPositions ,
				timeWhenOptimizationWasRequested ,
				0 ,
				numberDaysForInSampleOptimization ,
				0 ,
				benchmark );
			wFLagDebugPositions.Run();
		}
		#endregion Run
	}
}
