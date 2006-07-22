/*
QuantProject - Quantitative Finance Library

WFLagSharpeRatioComputer.cs
Copyright (C) 2003 
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
using System.Collections;

using QuantProject.ADT.Statistics;
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders.Caching;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Computes Sharpe Ratios for the WFLag strategy, for debug purposes
	/// </summary>
	public class WFLagSharpeRatioComputer
	{
		public WFLagSharpeRatioComputer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

//		private double[] getLinearCombinationReturns(
//			ICollection signedTickers )
//		{
////			ArrayList enumeratedSignedTicker =
////				this.getEnumeratedSignedTickers( signedTickers );
//			int numberOfSignedTickers = signedTickers.Count;
//			ArrayList tickers = this.getTickers( signedTickers );
//			float[] multipliers = this.getMultipliers( enumeratedSignedTicker );
//			// arrays of close to close returns, one for each signed ticker
//			float[][] tickersReturns =
//				this.wFLagCandidates.GetTickersReturns( tickers );
//			double[] linearCombinationReturns =
//				new double[ tickersReturns[ 0 ].Length ];
//			for( int i = 0; i < linearCombinationReturns.Length ; i++ )
//				// computes linearCombinationReturns[ i ]
//			{
//				linearCombinationReturns[ i ] = 0;
//				for ( int j=0 ; j < numberOfSignedTickers ; j++ )
//				{
//					double signedTickerReturn =
//						tickersReturns[ j ][ i ] * multipliers[ j ];
//					// the investment is assumed to be equally divided for each
//					// signed ticker
//					linearCombinationReturns[ i ] += signedTickerReturn /
//						numberOfSignedTickers;
//				}
//			}
//			return linearCombinationReturns;
		//		}

		private static double[] getStrategyReturns( double[] drivingPositionsReturns ,
			double[] portfolioPositionsReturns )
		{
			// strategyReturns contains one element less than drivingPositionsReturns,
			// because there is no strategy for the very first period (at least
			// one day signal is needed)
			double[] strategyReturns =
				new double[ portfolioPositionsReturns.Length - 1 ];
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
		private static SortedList getCommonMarketDays(
			WFLagChosenPositions wFLagChosenPositions ,
			DateTime firstDate , DateTime lastDate )
		{
			string[] tickers =
				WFLagChosenPositionsDebugInfo.GetDrivingAndPortfolioTickers(
				wFLagChosenPositions );
			SortedList commonMarketDays =
				QuantProject.Data.DataTables.Quotes.GetCommonMarketDays(
				tickers , firstDate , lastDate );
			return commonMarketDays;
		}
		private static double[] getStrategyReturns(
			WFLagChosenPositions wFLagChosenPositions ,
			DateTime firstDate , DateTime lastDate )
		{
			SortedList commonMarketDays =
				getCommonMarketDays( wFLagChosenPositions , firstDate , lastDate );
			double[] drivingPositionsReturns =
				SignedTicker.GetCloseToClosePortfolioReturns(
				wFLagChosenPositions.DrivingPositions.Keys ,
				commonMarketDays );
			double[] portfolioPositionsReturns =
				SignedTicker.GetCloseToClosePortfolioReturns(
				wFLagChosenPositions.PortfolioPositions.Keys ,
				commonMarketDays );
			double[] strategyReturns = getStrategyReturns(
				drivingPositionsReturns , portfolioPositionsReturns );
			return strategyReturns;
		}


		public static double GetSharpeRatio(
			WFLagChosenPositions wFLagChosenPositions ,
			DateTime firstDate , DateTime lastDate )
		{
			double sharpeRatio = double.MinValue;
			try
			{
				double[] strategyReturns = getStrategyReturns(
					wFLagChosenPositions , firstDate , lastDate );
				sharpeRatio = AdvancedFunctions.GetSharpeRatio( strategyReturns );
			}
			catch( MissingQuoteException ex )
			{
				sharpeRatio = double.MinValue;
				string dummy = ex.Message;
			}
			return sharpeRatio;
		}
		public static double GetExpectancyScore(
			WFLagChosenPositions wFLagChosenPositions ,
			DateTime firstDate , DateTime lastDate )
		{
			double expectancyScore = double.MinValue;
			try
			{
				double[] strategyReturns = getStrategyReturns(
					wFLagChosenPositions , firstDate , lastDate );
				expectancyScore =
					AdvancedFunctions.GetExpectancyScore( strategyReturns );
			}
			catch( MissingQuoteException ex )
			{
				expectancyScore = double.MinValue;
				string dummy = ex.Message;
			}
			return expectancyScore;
		}
	}
}
