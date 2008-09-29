/*
QuantProject - Quantitative Finance Library

AverageReturnOnDayWithOpenPositions.cs
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
using System.Collections;
using System.Data;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Statistics;
using QuantProject.Data.DataTables;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Financial.Accounting.Reporting.StatisticsSummaryRows
{
	/// <summary>
	/// Summary row containing the average return of the strategy on each day 
	/// with open positions
	/// </summary>
	[Serializable]
	public class AverageReturnOnDayWithOpenPositions : PercentageSummaryRow
	{
		private StatisticsSummary statisticsSummary;
		
		private object getRowValue( int totalNumberOfDaysWithOpenPositions , 
																double sumOfReturnsOnDaysWithOpenPositions)
		{
			double averagePercentageReturn;
			averagePercentageReturn = 100.0 *
				sumOfReturnsOnDaysWithOpenPositions / (double)totalNumberOfDaysWithOpenPositions;
			string numOfDays = totalNumberOfDaysWithOpenPositions.ToString();
			return averagePercentageReturn.ToString().Substring(0,4) +
				"#" + 
				numOfDays.Substring(0,Math.Min(numOfDays.Length,4));
		}
		
		private double getReturnForDate(DateTime date, DateTime previousDate)
		{
			// TODO: this method works only if the equity line is computed
			// at market close; an exception is thrown otherwise; change the
			// logic to be more general; furthermore, this code is duplicated
			// and common to AverageReturnOnDayOfWeekWithOpenPositions.cs
			double accountValueOnCloseForDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(
					HistoricalEndOfDayTimer.GetMarketClose( date ) );
			double accountValueOnCloseForPreviousDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(
					HistoricalEndOfDayTimer.GetMarketClose( previousDate ) );
			
			return (accountValueOnCloseForDate / accountValueOnCloseForPreviousDate) - 1.0;
		}
		
		private bool hasPositionsOnDate( DateTime date, DateTime previousDate )
		{
			// TODO: this method works only if the equity line is computed
			// at market close; an exception is thrown otherwise; change the
			// logic to be more general; furthermore, this code is duplicated
			// and common to AverageReturnOnDayOfWeekWithOpenPositions.cs
			double accountValueOnCloseForDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(
					HistoricalEndOfDayTimer.GetMarketClose( date ) );
			double accountValueOnCloseForPreviousDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(
					HistoricalEndOfDayTimer.GetMarketClose(
						previousDate ) );
			
			return accountValueOnCloseForDate != accountValueOnCloseForPreviousDate;
		}

		public AverageReturnOnDayWithOpenPositions( StatisticsSummary statisticsSummary )
							: base( )
		{
			this.statisticsSummary = statisticsSummary;
			this.rowDescription = "Average % return on days with open positions ";
//			History marketDays = Quotes.GetMarketDays(
//				statisticsSummary.AccountReport.Benchmark,
//				statisticsSummary.AccountReport.StartDateTime,
//				statisticsSummary.AccountReport.EndDateTime);
			History marketDays =
				this.statisticsSummary.AccountReport.EquityLine;
			DateTime date;
			DateTime previousDate;
			int totalNumberOfDaysWithOpenPositions = 0;
			double sumOfReturnsOnDaysWithOpenPositions = 0.0;
			for(int i = 1; i < marketDays.Count - 1; i++)
			{
				date = marketDays.GetDateTime( i );
				previousDate = marketDays.GetDateTime( i - 1 );
				if( this.hasPositionsOnDate( date, previousDate ) )
				{
					totalNumberOfDaysWithOpenPositions++;
					sumOfReturnsOnDaysWithOpenPositions += this.getReturnForDate(date, previousDate);
				}
			}
			this.rowValue = this.getRowValue(	totalNumberOfDaysWithOpenPositions ,
			                                  sumOfReturnsOnDaysWithOpenPositions);
		}
	}
}
