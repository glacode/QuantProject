/*
QuantProject - Quantitative Finance Library

EligibleTickers.cs
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
using System.Data;

using QuantProject.Data.Selectors;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Selects the tickers to among which best performers will be searched
	/// </summary>
	public class EligibleTickers : Hashtable
	{
		private int numberEligibleTickersToBeChosen;
		private int numberDaysForPerformanceCalculation;
		private int numDaysToComputeLiquidity = 10;

		public EligibleTickers( int numberEligibleTickersToBeChosen ,
			int numberDaysForPerformanceCalculation )
		{
			this.numberEligibleTickersToBeChosen = numberEligibleTickersToBeChosen;
			this.numberDaysForPerformanceCalculation = numberDaysForPerformanceCalculation;
		}

		#region SetTickers
		private void setTickers_build( DateTime dateTime )
		{
			TickerSelector mostLiquid =
				new TickerSelector( SelectionType.Liquidity , false , "Test" ,
				dateTime.AddDays( - this.numDaysToComputeLiquidity ) , dateTime ,
				this.numberEligibleTickersToBeChosen );
			DataTable mostLiquidTickers =
				mostLiquid.GetTableOfSelectedTickers();
			TickerSelector quotedInEachMarketDayFromMostLiquid = 
				new TickerSelector( mostLiquidTickers,
				SelectionType.QuotedInEachMarketDay, false, "",
				dateTime.AddDays( - this.numberDaysForPerformanceCalculation ) ,
				dateTime, this.numberEligibleTickersToBeChosen);
			quotedInEachMarketDayFromMostLiquid.MarketIndex = "^SPX";
			DataTable selectedTickers =
				quotedInEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
			foreach ( DataRow dataRow in selectedTickers.Rows )
				this.Add( dataRow[ "tiTicker" ].ToString() ,
					dataRow[ "tiTicker" ].ToString() );
		}
		/// <summary>
		/// Populates the collection of eligible tickers
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetTickers( DateTime dateTime )
		{
			this.Clear();
			setTickers_build( dateTime );
		}
		#endregion
	}
}
