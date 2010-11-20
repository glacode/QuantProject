/*
QuantProject - Quantitative Finance Library

ByMostDiscountedPrices.cs
Copyright (C) 2010
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
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;
using QuantProject.Business.Financial.Fundamentals;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Implements IEligiblesSelector for selecting a given max number of tickers through
	/// the following step-by-step selecting process:
	/// -step 1: all tickers belonging to a given group
	/// are selected (the group can be "temporized": that is tickers
	/// are returned depending on the time the selection is requested:
	/// the group SP 500 should be like that);
	/// -step 2: from tickers selected by step 1, the ones
	/// (not more than a given max number) that have
	/// the greatest (negative) relative difference between the current price
	/// and the fair price;
	/// </summary>
	[Serializable]
	public class ByMostDiscountedPrices : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		private int maxNumberOfEligibleTickersToBeChosen;
		private int numOfDaysForFundamentalAnalysis;
		private IFairValueProvider fairValueProvider;
		
		public string Description
		{
			get{
				return "From_" + this.tickersGroupID + " (temporized: " +
					this.temporizedGroup.ToString() + ")\n" +
					"MaxNumOfEligibles_" + this.maxNumberOfEligibleTickersToBeChosen.ToString() + "\n" +
					"Most discounted price with respect to a given IFairPriceEvaluator";
			}
		}
				
		public ByMostDiscountedPrices(IFairValueProvider fairValueProvider,
			string tickersGroupID , bool temporizedGroup, 
			int maxNumberOfEligibleTickersToBeChosen, int numOfDaysForFundamentalAnalysis)
		{
			this.fairValueProvider = fairValueProvider;
			this.temporizedGroup = temporizedGroup;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
			this.tickersGroupID = tickersGroupID;
			this.numOfDaysForFundamentalAnalysis = numOfDaysForFundamentalAnalysis;
		}
		
		private EligibleTickers getEligibleTickers_actually_getTableOfMostDiscountedTickers(
			DataTable initialTableFromWhichToChooseTheMostDiscountedTickers, DateTime date )
		{
			DataTable tableOfEligibleTickers;
			if(!initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Contains("FairPrice"))
        initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Add("FairPrice", System.Type.GetType("System.Double"));
			if(!initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Contains("MarketPrice"))
        initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Add("LastMonthAverageMarketPrice", System.Type.GetType("System.Double"));
			if(!initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Contains("RelativeDifferenceBetweenFairAndMarketPrice"))
        initialTableFromWhichToChooseTheMostDiscountedTickers.Columns.Add("RelativeDifferenceBetweenFairAndMarketPrice", System.Type.GetType("System.Double"));
      foreach(DataRow row in initialTableFromWhichToChooseTheMostDiscountedTickers.Rows)
      {
      	row["FairPrice"] = 
      		this.fairValueProvider.GetFairValue( (string)row[0],
      	                                       date.AddDays(-this.numOfDaysForFundamentalAnalysis),
      	                                       date );
      	float[] lastMonthQuotes = 
      		Quotes.GetArrayOfAdjustedCloseQuotes((string)row[0], date.AddDays(-30), date);
      	row["LastMonthAverageMarketPrice"] = 
      		ADT.Statistics.BasicFunctions.SimpleAverage(lastMonthQuotes);
      	row["RelativeDifferenceBetweenFairAndMarketPrice"] = 
      		((double)row["FairPrice"]-(double)row["LastMonthAverageMarketPrice"])/(double)row["FairPrice"];
      }
      tableOfEligibleTickers = 
      	ExtendedDataTable.CopyAndSort(initialTableFromWhichToChooseTheMostDiscountedTickers,
      	                              "RelativeDifferenceBetweenFairAndMarketPrice", true);
      ExtendedDataTable.DeleteRows(tableOfEligibleTickers, this.maxNumberOfEligibleTickersToBeChosen);
      
      DataColumn[] columnPrimaryKeys = new DataColumn[1];
			columnPrimaryKeys[0] = tableOfEligibleTickers.Columns[0];
			tableOfEligibleTickers.PrimaryKey = columnPrimaryKeys;
      
      return new EligibleTickers(tableOfEligibleTickers);
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
      
      return this.getEligibleTickers_actually_getTableOfMostDiscountedTickers(tickersFromGroup, currentDate);
      
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

