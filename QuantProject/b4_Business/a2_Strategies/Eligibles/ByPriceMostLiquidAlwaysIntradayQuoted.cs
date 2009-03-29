/*
QuantProject - Quantitative Finance Library

ByPriceMostLiquidAlwaysIntradayQuoted.cs
Copyright (C) 2009
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
	/// always intraday quoted (the ones that have at least
	/// the given number of 60seconds-bars for each market day) are selected
	/// (not more than a given max number);
	/// </summary>
	[Serializable]
	public class ByPriceMostLiquidAlwaysIntradayQuoted : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		private Benchmark benchmark;
		private int maxNumberOfEligibleTickersToBeChosen;
		private int numberOfTopRowsToDeleteFromLiquidSelector;
		private int numOfDaysForAverageOpenRawPriceComputation;
		private double minPrice;
		private double maxPrice;
		private int minimumNumberOfBars;
		
		public string Description
		{
			get{
				return "From_" + this.tickersGroupID + " (temporized: " +
					this.temporizedGroup.ToString() + ")\n" +
					"MaxNumOfEligibles_" + this.maxNumberOfEligibleTickersToBeChosen.ToString() + "\n" +
					"AverageRawOpenPriceRange(computed for the last " + 
					this.numOfDaysForAverageOpenRawPriceComputation.ToString() + "):\n" +
					"From_" + this.minPrice + "_to_" + this.maxPrice + "\n" +
					"Most Liquid and Always intraday quoted at each market day (MSFT) for the in sample time frame";
			}
		}
				
		public ByPriceMostLiquidAlwaysIntradayQuoted(
			string tickersGroupID , Benchmark benchmark, 
			bool temporizedGroup,
			int maxNumberOfEligibleTickersToBeChosen,
			int numberOfTopRowsToDeleteFromLiquidSelector,
		 	int numOfDaysForAverageOpenRawPriceComputation,
		 	double minPrice, double maxPrice,
		  int minimumNumberOfBars)
		{
			this.temporizedGroup = temporizedGroup;
			this.tickersGroupID = tickersGroupID;
			this.benchmark = benchmark;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
			this.numberOfTopRowsToDeleteFromLiquidSelector = 
				numberOfTopRowsToDeleteFromLiquidSelector;
			this.numOfDaysForAverageOpenRawPriceComputation =
				numOfDaysForAverageOpenRawPriceComputation;
			this.minPrice = minPrice;
			this.maxPrice = maxPrice;
			this.minimumNumberOfBars = minimumNumberOfBars;
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
        this.maxNumberOfEligibleTickersToBeChosen,
        this.numberOfTopRowsToDeleteFromLiquidSelector);
      DataTable dataTableMostLiquid =
				mostLiquidSelector.GetTableOfSelectedTickers();

//			DataSet dataSet = new DataSet();
//			dataSet.Tables.Add( dataTableMostLiquid );
//			dataSet.WriteXml( "c:\\qpReports\\pairsTrading\\eligiblesCon_ByPriceMostLiquidAlwaysQuoted.xml" );

      SelectorByIntradayQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
        new SelectorByIntradayQuotationAtEachMarketDay( dataTableMostLiquid ,
        history.FirstDateTime, currentDate, minimumNumberOfBars,
        this.maxNumberOfEligibleTickersToBeChosen,
       	this.benchmark.Ticker);
      DataTable dataTableToBeReturned =
				quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
			
      return
				new EligibleTickers( dataTableToBeReturned );
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
