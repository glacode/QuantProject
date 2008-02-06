/*
QuantProject - Quantitative Finance Library

ByPriceMostLiquidAlwaysQuoted.cs
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
using System.Data;

using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Implements IEligiblesSelector for selecting a given max number of tickers through
	/// the following step-by-step selecting process:
	/// -step 1: all tickers belonging to a given group
	/// are selected (the group can be "temporized": that is tickers
	/// are returned depending on the time the selection is requested:
	/// the group SP 500 should be like that);
	/// -step 2: from tickers selected by step 1, the most liquid
	///  are selected (not more than a given max number);
	/// -step 3: from tickers selected by step 2, the ones that are
	/// always quoted at all market days are selected (not more than a given max number);
	/// </summary>
	public class ByPriceMostLiquidAlwaysQuoted : IEligiblesSelector
	{
		public event NewMessageHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		private int maxNumberOfEligibleTickersToBeChosen;
		private int numOfDaysForAverageOpenRawPriceComputation;
		private double minPrice;
		private double maxPrice;
		
		private string description;
		public string Description
		{
			get{
				return "Change me!";
			}
		}
				
		public ByPriceMostLiquidAlwaysQuoted(
			string tickersGroupID , bool temporizedGroup, 
			int maxNumberOfEligibleTickersToBeChosen,
		 	int numOfDaysForAverageOpenRawPriceComputation, double minPrice, double maxPrice)
		{
			this.temporizedGroup = temporizedGroup;
			this.tickersGroupID = tickersGroupID;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
			this.numOfDaysForAverageOpenRawPriceComputation =
				numOfDaysForAverageOpenRawPriceComputation;
			this.minPrice = minPrice;
			this.maxPrice = maxPrice;
		}

		private EligibleTickers getEligibleTickers_actually(
			EndOfDayHistory endOfDayHistory )
		{
			DateTime currentDate = endOfDayHistory.LastEndOfDayDateTime.DateTime; 
			SelectorByGroup group;
			if(this.temporizedGroup)
			//the group is "temporized": returned set of tickers
			// depend on time
				group = new SelectorByGroup(this.tickersGroupID,
				                            currentDate);
      else//the group is not temporized
      	group = new SelectorByGroup(this.tickersGroupID);
      DataTable tickersFromGroup = group.GetTableOfSelectedTickers();
      int numOfTickersInGroupAtCurrentDate = tickersFromGroup.Rows.Count;
      SelectorByAverageRawOpenPrice byPrice =
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,
      	                                  currentDate.AddDays(-this.numOfDaysForAverageOpenRawPriceComputation),
      	                                  currentDate,
      	                                  numOfTickersInGroupAtCurrentDate,
      	                                  this.minPrice,this.maxPrice, 0.0001,100);
      SelectorByLiquidity mostLiquidSelector =
      	new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
        false, endOfDayHistory.FirstEndOfDayDateTime.DateTime, currentDate,
        this.maxNumberOfEligibleTickersToBeChosen);
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
        new SelectorByQuotationAtEachMarketDay(mostLiquidSelector.GetTableOfSelectedTickers(),
        false, endOfDayHistory.History,
        this.maxNumberOfEligibleTickersToBeChosen);
			
      return
				new EligibleTickers( quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers() );
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
	}
}
