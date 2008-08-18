/*
QuantProject - Quantitative Finance Library

AverageReturnOnDayOfWeekWithOpenPositions.cs
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
using QuantProject.ADT.Histories;
using QuantProject.ADT.Statistics;
using QuantProject.Data.DataTables;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;

namespace QuantProject.Business.Financial.Accounting.Reporting.StatisticsSummaryRows
{
	/// <summary>
	/// Summary row containing the average return of the strategy on a specific day of the week
	/// </summary>
	[Serializable]
	public abstract class AverageReturnOnDayOfWeekWithOpenPositions : PercentageSummaryRow
	{
		private StatisticsSummary statisticsSummary;

		private double getReturnForDate(DateTime date, DateTime previousDate)
		{
			double accountValueOnCloseForDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(date);
			double accountValueOnCloseForPreviousDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(previousDate);
			
			return (accountValueOnCloseForDate / accountValueOnCloseForPreviousDate) - 1.0;
		}
		
		private bool hasPositionsOnDate( DateTime date, DateTime previousDate )
		{
			double accountValueOnCloseForDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(date);
			double accountValueOnCloseForPreviousDate = 
				(double)this.statisticsSummary.AccountReport.EquityLine.GetValue(previousDate);
			
			return accountValueOnCloseForDate != accountValueOnCloseForPreviousDate;
		}

		protected abstract string getRowDescription();
		protected abstract DayOfWeek getSpecificDayOfWeek();

		private object getRowValue( int totalNumberOfSpecificDayOfWeek , 
																double sumOfReturnsOnSpecificDayOfWeek)
		{
			double averagePercentageReturn;
			averagePercentageReturn = 100.0 *
				sumOfReturnsOnSpecificDayOfWeek / (double)totalNumberOfSpecificDayOfWeek;
			string numOfDays = totalNumberOfSpecificDayOfWeek.ToString();
			return averagePercentageReturn.ToString().Substring(0,4) +
				"#" + 
				numOfDays.Substring(0,Math.Min(numOfDays.Length,4));
		}

		public AverageReturnOnDayOfWeekWithOpenPositions( StatisticsSummary statisticsSummary )
							: base( )
		{
			this.statisticsSummary = statisticsSummary;
			this.rowDescription = this.getRowDescription();
			DayOfWeek specificDayOfWeek = this.getSpecificDayOfWeek();
			History marketDays = Quotes.GetMarketDays(
				statisticsSummary.AccountReport.Benchmark,
				statisticsSummary.AccountReport.StartDateTime,
				statisticsSummary.AccountReport.EndDateTime.DateTime);
			DateTime date;
			DateTime previousDate;
			int totalNumberOfSpecificDayOfWeek = 0;
			double sumOfReturnsOnSpecificDayOfWeek = 0.0;
			for(int i = 1; i < marketDays.Count - 1; i++)
			{
				date = marketDays.GetDateTime( i );
				previousDate = marketDays.GetDateTime( i - 1 );
				if( date.DayOfWeek == specificDayOfWeek &&
					  this.hasPositionsOnDate( date, previousDate ) )
				{
					totalNumberOfSpecificDayOfWeek++;
					sumOfReturnsOnSpecificDayOfWeek += 
						this.getReturnForDate( date, previousDate );
				}
			}
			try
			{
				this.rowValue = this.getRowValue(totalNumberOfSpecificDayOfWeek, sumOfReturnsOnSpecificDayOfWeek);
			}
			catch (Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
	}
}
