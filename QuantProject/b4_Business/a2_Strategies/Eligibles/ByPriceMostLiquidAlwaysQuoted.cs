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

using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
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
	[Serializable]
	public class ByPriceMostLiquidAlwaysQuoted : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		private int maxNumberOfEligibleTickersToBeChosen;
		private int numOfDaysForAverageOpenRawPriceComputation;
		private double minPrice;
		private double maxPrice;
		
		public string Description
		{
			get{
				return "From_" + this.tickersGroupID + " (temporized: " +
					this.temporizedGroup.ToString() + ")\n" +
					"MaxNumOfEligibles_" + this.maxNumberOfEligibleTickersToBeChosen.ToString() + "\n" +
					"AverageRawOpenPriceRange(computed for the last " + 
					this.numOfDaysForAverageOpenRawPriceComputation.ToString() + "):\n" +
					"From_" + this.minPrice + "_to_" + this.maxPrice + "\n" +
					"Most Liquid and Always Quoted at each market day (^GSPC) for the in sample time frame";
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
			History history )
		{
			DateTime currentDate = history.LastDateTime; 

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
      	                                  this.minPrice,this.maxPrice, 0.00001, double.MaxValue);
     	DataTable dataTableByPrice =
				byPrice.GetTableOfSelectedTickers();

			SelectorByLiquidity mostLiquidSelector =
				new SelectorByLiquidity( dataTableByPrice ,
        false, history.FirstDateTime, currentDate,
        this.maxNumberOfEligibleTickersToBeChosen);
      DataTable dataTableMostLiquid =
				mostLiquidSelector.GetTableOfSelectedTickers();

//			DataSet dataSet = new DataSet();
//			dataSet.Tables.Add( dataTableMostLiquid );
//			dataSet.WriteXml( "c:\\qpReports\\pairsTrading\\eligiblesCon_ByPriceMostLiquidAlwaysQuoted.xml" );

//      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
//        new SelectorByQuotationAtEachMarketDay( dataTableMostLiquid ,
//        false, history,
//        this.maxNumberOfEligibleTickersToBeChosen);
      
      SelectorByQuotationAtAGivenPercentageOfDateTimes quotedAtEachMarketDayFromLastSelection = 
        new SelectorByQuotationAtAGivenPercentageOfDateTimes( dataTableMostLiquid ,
        false, history, 0,
        this.maxNumberOfEligibleTickersToBeChosen, 0.9);
      
      DataTable dataTableToBeReturned =
				quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
			
      return
				new EligibleTickers( dataTableToBeReturned, currentDate );
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
			History history )
		{
			EligibleTickers eligibleTickers =
				this.getEligibleTickers_actually( history );
			this.getEligibleTickers_sendNewMessage( eligibleTickers );
			return eligibleTickers;
		}
	}
}
