/*
QuantProject - Quantitative Finance Library

ByRelativeDifferenceBetweenPriceAndFairValue.cs
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
using QuantProject.ADT.Collections;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Statistics;
using QuantProject.Business.Financial.Fundamentals.FairValueProviders;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Implements IEligiblesSelector for selecting a given max number of tickers through
	/// the following step-by-step selecting process:
	/// -step 1: the first maxNumberOfEligibleTickersToBeChosen tickers
	/// by liquidity, with a market price >= minPriceForTickers, 
	/// belonging to a given group (the group can be "temporized":
	/// that is tickers are returned depending on the time the
	/// selection is requested:
	/// the group SP 500 should be like that)
	/// are selected;
	/// -step 2: from tickers selected by step 1, 
	/// maxNumberOfEligibleTickersToBeChosen / 2 tickers
	/// with the highest negative difference between
	/// their average current price (computed for a given
	/// number of days) and their fair value 
	/// (provided by a IFairValueProvider)
	/// are added to the first positions of the Eligible
	/// Tickers. Choice of these tickers may be done
	/// excluding the outliers within a given percentage
	/// -step 3: just like step 2, but in this case
	/// the focus is on tickers with the highest
	/// positive difference (that is, the
	/// most expensive with respect to the fair value)
	/// </summary>
	[Serializable]
	public class ByRelativeDifferenceBetweenPriceAndFairValue :  IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private ITickerSelectorByDate tickerSelectorByDate;
		private int maxNumberOfEligibleTickersToBeChosen;
		private IFairValueProvider fairValueProvider;
		private int firstPercentilesOfTheMostDiscountedOutliersToExclude;
		private int firstPercentilesOfTheMostExpensiveOutliersToExclude;
		private	int numOfDaysForAverageMarketPriceComputation;
		
		public string Description
		{
			get{
				return 
					"MaxNumOfEligibles_" + this.maxNumberOfEligibleTickersToBeChosen.ToString() + "\n" +
					"Most discounted price and the most expensive with respect to a given IFairValueProvider";
			}
		}
				
		public ByRelativeDifferenceBetweenPriceAndFairValue(IFairValueProvider fairValueProvider,
			ITickerSelectorByDate tickerSelectorByDate,
			int maxNumberOfEligibleTickersToBeChosen,
			int firstPercentilesOfTheMostDiscountedOutliersToExclude,
		  int firstPercentilesOfTheMostExpensiveOutliersToExclude,
			int numOfDaysForAverageMarketPriceComputation)
		{
			this.fairValueProvider = fairValueProvider;
			this.tickerSelectorByDate = tickerSelectorByDate;
			this.maxNumberOfEligibleTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen;
			this.firstPercentilesOfTheMostDiscountedOutliersToExclude = 
				firstPercentilesOfTheMostDiscountedOutliersToExclude;
			this.firstPercentilesOfTheMostExpensiveOutliersToExclude = 
				firstPercentilesOfTheMostExpensiveOutliersToExclude;
			this.numOfDaysForAverageMarketPriceComputation =
				numOfDaysForAverageMarketPriceComputation;
		}
		
		private double getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostExpensive_getValueFromPercentile(
			DataTable initialTableFromWhichToChooseTickers )
		{
			double returnValue;
			double[] relativeDifferencesBetweenFairValueAndPrice =
				ExtendedDataTable.GetArrayOfDoubleFromColumn(initialTableFromWhichToChooseTickers,
				                                             "RelativeDifferenceBetweenFairAndMarketPrice");
			double[] negativeRelativeDifferencesBetweenFairValueAndPrice =
				DoubleArrayManager.ExtractNegative(relativeDifferencesBetweenFairValueAndPrice);
			returnValue =
				BasicFunctions.Percentile(negativeRelativeDifferencesBetweenFairValueAndPrice,
				                          this.firstPercentilesOfTheMostExpensiveOutliersToExclude);
			return returnValue;
		}
		
		private DataTable getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostExpensive(
			DataTable initialTableFromWhichToChooseTickers )
		{
			DataTable returnValue;
			string filterString =
      	"AverageMarketPrice is not null AND FairPrice is not null AND " +
      	"RelativeDifferenceBetweenFairAndMarketPrice < 0 AND " +
      	"RelativeDifferenceBetweenFairAndMarketPrice >= " +
				this.getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostExpensive_getValueFromPercentile(initialTableFromWhichToChooseTickers);
      	
      returnValue =
      	ExtendedDataTable.CopyAndSort(initialTableFromWhichToChooseTickers,
      	           filterString,
      	           "RelativeDifferenceBetweenFairAndMarketPrice", true);
      
      ExtendedDataTable.DeleteRows(
				returnValue,
			  this.maxNumberOfEligibleTickersToBeChosen / 2 );
			return returnValue;
		}
		
		private double getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostDiscounted_getValueFromPercentile(
			DataTable initialTableFromWhichToChooseTickers )
		{
			double returnValue;
			double[] relativeDifferencesBetweenFairValueAndPrice =
				ExtendedDataTable.GetArrayOfDoubleFromColumn(initialTableFromWhichToChooseTickers,
				                                             "RelativeDifferenceBetweenFairAndMarketPrice");
			double[] positiveRelativeDifferencesBetweenFairValueAndPrice =
				DoubleArrayManager.ExtractPositive(relativeDifferencesBetweenFairValueAndPrice);
			returnValue =
				BasicFunctions.Percentile(positiveRelativeDifferencesBetweenFairValueAndPrice, 100 - this.firstPercentilesOfTheMostDiscountedOutliersToExclude);
			return returnValue;
		}
		
		private DataTable getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostDiscounted(
			DataTable initialTableFromWhichToChooseTickers )
		{
			DataTable returnValue;
			string filterString =
      	"AverageMarketPrice is not null AND FairPrice is not null AND " +
      	"RelativeDifferenceBetweenFairAndMarketPrice > 0 AND " +
      	"RelativeDifferenceBetweenFairAndMarketPrice <= " +
				this.getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostDiscounted_getValueFromPercentile(initialTableFromWhichToChooseTickers);
      	
      returnValue =
      	ExtendedDataTable.CopyAndSort(initialTableFromWhichToChooseTickers,
      	           filterString,
      	           "RelativeDifferenceBetweenFairAndMarketPrice", false);
      ExtendedDataTable.DeleteRows(
				returnValue,
			  this.maxNumberOfEligibleTickersToBeChosen / 2 );
			return returnValue;
		}
		
		private EligibleTickers getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers(
			DataTable initialTableFromWhichToChooseTickers, DateTime date )
		{
			double currentFairPrice;
			DataTable tableOfEligibleTickers;
			if(!initialTableFromWhichToChooseTickers.Columns.Contains("FairPrice"))
        initialTableFromWhichToChooseTickers.Columns.Add("FairPrice", System.Type.GetType("System.Double"));
			if(!initialTableFromWhichToChooseTickers.Columns.Contains("AverageMarketPrice"))
        initialTableFromWhichToChooseTickers.Columns.Add("AverageMarketPrice", System.Type.GetType("System.Double"));
			if(!initialTableFromWhichToChooseTickers.Columns.Contains("RelativeDifferenceBetweenFairAndMarketPrice"))
        initialTableFromWhichToChooseTickers.Columns.Add("RelativeDifferenceBetweenFairAndMarketPrice", System.Type.GetType("System.Double"));
			tableOfEligibleTickers = initialTableFromWhichToChooseTickers.Clone();
			this.fairValueProvider.Run(date);
			double averageMarketPrice = 0.0;
			foreach(DataRow row in initialTableFromWhichToChooseTickers.Rows)
      {
      	try{
	      	double[] quotesForAveragePriceComputation = 
	      		Quotes.GetDoubleArrayOfAdjustedCloseQuotes((string)row[0], date.AddDays(-this.numOfDaysForAverageMarketPriceComputation), date);
					averageMarketPrice = ADT.Statistics.BasicFunctions.SimpleAverage(quotesForAveragePriceComputation);
					if(averageMarketPrice > 0.0)
					{
						currentFairPrice = this.fairValueProvider.GetFairValue( (string)row[0],
	      	                                       //date.AddDays(-this.numOfDaysForFairValueAnalysis),
	      	                                       date );
		      	row["FairPrice"] = currentFairPrice;
		      	row["AverageMarketPrice"] = averageMarketPrice;
		      	row["RelativeDifferenceBetweenFairAndMarketPrice"] = 
		      		((double)row["FairPrice"]-(double)row["AverageMarketPrice"])/(double)row["AverageMarketPrice"];	
					}
      	}
      	catch(Exception ex)
      	{
					string s = ex.ToString();
				}	
      }
			DataTable tableOfMostDiscounted =
				this.getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostDiscounted(
				initialTableFromWhichToChooseTickers );
			DataTable tableOfMostExpensive = 
				this.getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers_getMostExpensive(
				initialTableFromWhichToChooseTickers );
			ExtendedDataTable.ImportRowsFromFirstRowOfSource(tableOfMostDiscounted, tableOfEligibleTickers);
			ExtendedDataTable.ImportRowsFromLastRowOfSource(tableOfMostExpensive, tableOfEligibleTickers);
			//the table has to be ordered in such a way that at the top you find
      //the most discounted, at the bottom the most expensive (the nearer you go
      //towards the middle from the top / bottom, the less discounted / expensive you find)
      DataColumn[] columnPrimaryKeys = new DataColumn[1];
			columnPrimaryKeys[0] = tableOfEligibleTickers.Columns[0];
			tableOfEligibleTickers.PrimaryKey = columnPrimaryKeys;
      
			string[] tableOfEligibleTickersForDebugging =
				ExtendedDataTable.GetArrayOfStringFromRows(tableOfEligibleTickers);
			
      return new EligibleTickers(tableOfEligibleTickers);
		}
		
		private EligibleTickers getEligibleTickers_actually(
			History history )
		{
			DateTime currentDate = history.LastDateTime; 
			DataTable initialTable = 
				this.tickerSelectorByDate.GetTableOfSelectedTickers(currentDate);
      return this.getEligibleTickers_actually_getTableOfMostDiscountedAndMostExpensiveTickers(
						initialTable, currentDate);
		}
		
		private void getEligibleTickers_sendNewMessage(
			EligibleTickers eligibleTickers )
		{
			string message = this.fairValueProvider.Description + "; " +
											 "Number of Eligible tickers: " +
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

