/*
QuantProject - Quantitative Finance Library

WFLagEligibleTickers.cs
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
using System.Data;

using QuantProject.Business.Timing;
using QuantProject.Data.Selectors;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Tickers eligible for the lag strategy
	/// </summary>
	public class WFLagEligibleTickers
	{
		private string tickerGroupID;
		private string benchmark;
		private int numberEligibleTickersToBeChosen;
		private int numberDaysForPerformanceCalculation;
		private IEndOfDayTimer endOfDayTimer;

		private DataTable eligibleTickers;

		public DataTable EligibleTickers
		{
			get
			{
				if ( this.eligibleTickers == null )
					this.SetTickers();
				return this.eligibleTickers;
			}
		}
		public WFLagEligibleTickers(
			string tickerGroupID ,
			string benchmark ,
			int numberEligibleTickersToBeChosen ,
			int numberDaysForPerformanceCalculation ,
			IEndOfDayTimer endOfDayTimer )
		{
			this.tickerGroupID = tickerGroupID;
			this.benchmark = benchmark;
			this.numberEligibleTickersToBeChosen = numberEligibleTickersToBeChosen;
			this.numberDaysForPerformanceCalculation = numberDaysForPerformanceCalculation;
			this.endOfDayTimer = endOfDayTimer;
			this.eligibleTickers = new DataTable();
		}
		#region setTickers
		private DataTable setTickers_build_getSelectedTickers()
		{
			DateTime dateTime = this.endOfDayTimer.GetCurrentTime().DateTime;
//			SelectorByGroup selectorByGroup =
//				new SelectorByGroup( this.tickerGroupID , dateTime );
//			DataTable groupTickers = selectorByGroup.GetTableOfSelectedTickers();
			SelectorByLiquidity mostLiquid =
				new SelectorByLiquidity( this.tickerGroupID , true ,
				dateTime.AddDays( - ( this.numberDaysForPerformanceCalculation - 1 ) ) ,
				dateTime , 200000 , 99999 );
			DataTable groupTickers = mostLiquid.GetTableOfSelectedTickers();

		
			//			SelectorByLiquidity mostLiquid =
			//				new SelectorByLiquidity("Test", false ,	dateTime.AddDays( - this.numDaysToComputeLiquidity ) , dateTime ,
			//				this.numberEligibleTickersToBeChosen );
			//			DataTable mostLiquidTickers =
			//				mostLiquid.GetTableOfSelectedTickers();
			SelectorByQuotationAtEachMarketDay quotedInEachMarketDay = 
				new SelectorByQuotationAtEachMarketDay( groupTickers ,
				false ,
				dateTime.AddDays( - ( this.numberDaysForPerformanceCalculation - 1 ) ) ,
				dateTime , this.numberEligibleTickersToBeChosen , this.benchmark );
			return quotedInEachMarketDay.GetTableOfSelectedTickers();
		}
		private DataTable setTickers_buildQuickly_getSelectedTickers()
		{
			DataTable returnValue =
				new QuantProject.Data.DataTables.Tickers_tickerGroups( "millo" );
			returnValue.Columns[ 0 ].ColumnName = "tiTicker";
			return returnValue;
		}
		private void debug_displayTickersToConsole()
		{
			foreach ( DataRow dataRow in this.eligibleTickers.Rows )
				Console.WriteLine( (string)dataRow[ 0 ] );
		}
		private void setTickers_build()
		{
			// for fast debug, comment the following line
			this.eligibleTickers = setTickers_build_getSelectedTickers();
			// for fast debug, uncomment the following line 
			//			DataTable selectedTickers = setTickers_buildQuickly_getSelectedTickers();
//			this.debug_displayTickersToConsole();
//			this.debug_displayTickersToConsole();
		}
		/// <summary>
		/// Populates the collection of eligible tickers
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetTickers()
		{
			this.eligibleTickers.Clear();
			setTickers_build();
		}
		#endregion
	}
}
