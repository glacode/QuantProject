/*
QuantProject - Quantitative Finance Library

ByPriceLiquidityLowestPEQuotedAtAGivenPercentage.cs
Copyright (C) 2011
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

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Implements IEligiblesSelector for selecting a given max number of 
	/// tickers through the following step-by-step selecting process:
	/// -step 0: all tickers belonging to a given group
	/// are selected (the group can be "temporized": that is tickers
	/// are returned depending on the time the selection is requested:
	/// the group SP 500 should be like that);
	/// -step 1: from tickers selected by step 0, the most liquid
	///  are selected (not more than maxNumberOfMostLiquidTickersToBeChosen);
	/// -step 2: from tickers selected by step 1, the ones that have
	/// the lowest Price/Earnings ratio are selected;
	/// (not more than maxNumberOfEligibleTickersToBeChosen);
	/// -step 3: from tickers selected by step 3, the ones that are
	/// quoted at least at the given percentage of times
	/// with respect to the given benchmark are selected
	/// (not more than maxNumberOfEligibleTickersToBeChosen);
	/// </summary>
	[Serializable]
	public class ByPriceLiquidityLowestPEQuotedAtAGivenPercentage : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		private int maxNumberOfEligibleTickersToBeChosen;
		private int maxNumberOfMostLiquidTickersToBeChosen;
		private int numOfDaysForAverageOpenRawPriceComputation;
		private double minPrice;
		private double maxPrice;
		private double minPE;
		private double maxPE;
		private double minPercentageOfDaysWithQuotation;
		
		public string Description
		{
			get{
				return "ByPriceLiquidityLowestPEQuotedAtAGivenPercentage";
			}
		}
		
		public ByPriceLiquidityLowestPEQuotedAtAGivenPercentage(
			string tickersGroupID , bool temporizedGroup,
			int maxNumberOfEligibleTickersToBeChosen,
			int maxNumberOfMostLiquidTickersToBeChosen,
			int numOfDaysForAverageOpenRawPriceComputation,
			double minPrice, double maxPrice,
		  double minPE, double maxPE,
		  double minPercentageOfDaysWithQuotation)
		{
			this.temporizedGroup = temporizedGroup;
			this.tickersGroupID = tickersGroupID;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
			this.maxNumberOfMostLiquidTickersToBeChosen =
				maxNumberOfMostLiquidTickersToBeChosen;
			this.numOfDaysForAverageOpenRawPriceComputation =
				numOfDaysForAverageOpenRawPriceComputation;
			this.minPrice = minPrice;
			this.maxPrice = maxPrice;
			this.minPE = minPE;
			this.maxPE = maxPE;
			this.minPercentageOfDaysWithQuotation =
				minPercentageOfDaysWithQuotation;
		}

		private EligibleTickers getEligibleTickers_actually(
			History history )
		{
			DateTime currentDate = history.LastDateTime;

			SelectorByGroup group;
			if(this.temporizedGroup)
				//the group is "temporized": returned set of tickers
				// depends on time
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
				                        this.maxNumberOfMostLiquidTickersToBeChosen);
			DataTable dataTableMostLiquid =
				mostLiquidSelector.GetTableOfSelectedTickers();
			
			SelectorByPE tickersWithLowestPE = 
				new SelectorByPE(dataTableMostLiquid, 
				                 currentDate.AddDays(-this.numOfDaysForAverageOpenRawPriceComputation),
				                 currentDate,
				                 this.minPE, this.maxPE,
				                 this.maxNumberOfEligibleTickersToBeChosen,
				                 true);
			
//			DataSet dataSet = new DataSet();
//			dataSet.Tables.Add( dataTableLessVolatile );
//			dataSet.WriteXml( "c:\\qpReports\\pairsTrading\\eligiblesCon_ByPriceMostLiquidLessVolatileOTCAlwaysQuoted.xml" );
			DataTable dtTickersWithLowestPE = tickersWithLowestPE.GetTableOfSelectedTickers();
			SelectorByQuotationAtAGivenPercentageOfDateTimes quotedAtAGivenPercentage =
				new SelectorByQuotationAtAGivenPercentageOfDateTimes( dtTickersWithLowestPE ,
				                                       false, history, 0, 
				                                       this.maxNumberOfEligibleTickersToBeChosen,
				                                       this.minPercentageOfDaysWithQuotation);
			
			DataTable dataTableToBeReturned =
				quotedAtAGivenPercentage.GetTableOfSelectedTickers();
			
			string[] dataTableToBeReturnedForDebugging =
				ExtendedDataTable.GetArrayOfStringFromRows(dataTableToBeReturned);
			
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

