/*
QuantProject - Quantitative Finance Library

MostLiquidAndLessVolatile.cs
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
using System.Data;

using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Selects the most liquid and less volatile instruments,
	/// within the given group
	/// </summary>
	public class MostLiquidAndLessVolatile : IEligiblesSelector
	{
		public event NewMessageHandler NewMessage;

		private string tickersGroupID;
		private int maxNumberOfEligibleTickersToBeChosen;

		/// <summary>
		/// Selects the most liquid and less volatile within the given
		/// group
		/// </summary>
		/// <param name="groupID"></param>
		public MostLiquidAndLessVolatile(
			string tickersGroupID , int maxNumberOfEligibleTickersToBeChosen )
		{
			this.tickersGroupID = tickersGroupID;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
//			this.firstDateTime = DateTime.MinValue;
//			this.lastDateTime = DateTime.MinValue;
//			this.maxNumberOfEligibleTickersToBeChosen = int.MinValue;
//			this.benchmark = "";
		}
//		#region SetParameters
//		private void setParameters_checkParameters( string tickerGroupID ,
//			DateTime firstDateTime , DateTime lastDateTime ,
//			int maxNumberOfEligibleTickersToBeChosen , string benchmark )
//		{
//			if ( firstDateTime >= lastDateTime )
//				throw new Exception( "lastDateTime needs to be larger than " +
//					"firstDateTime" );
//			if ( maxNumberOfEligibleTickersToBeChosen <= 0 )
//				throw new Exception( "maxNumberOfEligibleTickersToBeChosen needs " +
//					"to be larger than zero" );
//		}
//		/// <summary>
//		/// Sets the parameters to be used by the selection
//		/// </summary>
//		/// <param name="tickerGroupID">groups in which tickers are to be searched</param>
//		/// <param name="firstDateTime">begin of the interval</param>
//		/// <param name="lastDateTime">end of the interval</param>
//		/// <param name="maxNumberOfEligibleTickersToBeChosen">if more tickers are found
//		/// the first maxNumberOfEligibleTickersToBeChosen are returned</param>
//		/// <param name="benchmark">returned tickers must be quoted in every
//		/// day when the benchmark is quoted</param>
//		public void SetParameters( string tickerGroupID ,
//			DateTime firstDateTime , DateTime lastDateTime ,
//			int maxNumberOfEligibleTickersToBeChosen , string benchmark )
//		{
//			this.setParameters_checkParameters( tickerGroupID ,
//				firstDateTime , lastDateTime ,
//				maxNumberOfEligibleTickersToBeChosen , benchmark );
//			this.tickerGroupID = tickerGroupID;
//			this.firstDateTime = firstDateTime;
//			this.lastDateTime = lastDateTime;
//			this.maxNumberOfEligibleTickersToBeChosen =
//				maxNumberOfEligibleTickersToBeChosen;
//			this.benchmark = benchmark;
//		}
//		#endregion SetParameters
		#region GetEligibleTickers
//		private void checkIfAllParametersHaveBeenSet()
//		{
//			if ( ( this.tickerGroupID == null ) ||
//				( this.firstDateTime == DateTime.MinValue ) ||
//				( this.lastDateTime == DateTime.MinValue ) ||
//				( this.maxNumberOfEligibleTickersToBeChosen == int.MinValue ) ||
//				( this.benchmark == "" ) )
//				throw new Exception( "GetEligbileTickers cannot be called before " +
//					"having called SetParameters" );
//		}

		private EligibleTickers getEligibleTickers_actually(
			EndOfDayHistory endOfDayHistory )
		{
			//			DateTime dateTime = this.endOfDayTimer.GetCurrentTime().DateTime;
			//			SelectorByGroup selectorByGroup =
			//				new SelectorByGroup( this.tickerGroupID , dateTime );
			//			DataTable groupTickers = selectorByGroup.GetTableOfSelectedTickers();
			SelectorByLiquidity mostLiquid =
				new SelectorByLiquidity( this.tickersGroupID , true ,
				endOfDayHistory.FirstEndOfDayDateTime.DateTime ,
				endOfDayHistory.LastEndOfDayDateTime.DateTime , 0 ,
				this.maxNumberOfEligibleTickersToBeChosen );
			DataTable groupTickers = mostLiquid.GetTableOfSelectedTickers();

			
			//			SelectorByLiquidity mostLiquid =
			//				new SelectorByLiquidity("Test", false ,	dateTime.AddDays( - this.numDaysToComputeLiquidity ) , dateTime ,
			//				this.numberEligibleTickersToBeChosen );
			//			DataTable mostLiquidTickers =
			//				mostLiquid.GetTableOfSelectedTickers();
			
			SelectorByQuotationAtEachMarketDay quotedInEachMarketDay =
				new SelectorByQuotationAtEachMarketDay(
				groupTickers , false , endOfDayHistory.History ,
				maxNumberOfEligibleTickersToBeChosen );
			DataTable dtEligibleTickers =
				quotedInEachMarketDay.GetTableOfSelectedTickers();
			
			EligibleTickers eligibleTickers =
				new EligibleTickers( dtEligibleTickers );
			return eligibleTickers;
		}
		private void getEligibleTickers_sendNewMessage(
			EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if(this.NewMessage != null)
				this.NewMessage( this , newMessageEventArgs );
		}
		/// <summary>
		/// Returns the eligible tickers
		/// </summary>
		/// <returns></returns>
		public EligibleTickers GetEligibleTickers(
			EndOfDayHistory endOfDayHistory )
		{
			EligibleTickers eligibleTickers =
				this.getEligibleTickers_actually( endOfDayHistory );
			this.getEligibleTickers_sendNewMessage( eligibleTickers );
			return eligibleTickers;
		}
		#endregion GetEligibleTickers
	}
}
