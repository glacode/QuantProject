/*
QuantProject - Quantitative Finance Library

AverageReturnOnMonday.cs
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
	/// Summary row containing the average return of the strategy on Monday
	/// </summary>
	[Serializable]
	public class AverageReturnOnMonday : DoubleSummaryRow
	{
		public AverageReturnOnMonday( StatisticsSummary statisticsSummary ) : base( 2 )
		{
			this.rowDescription = "Average return on Monday";
//			int totalNumberOfTransactions = statisticsSummary.AccountReport.TransactionTable.DataTable.Rows.Count;
			//it has to be implemented yet
//			History marketDays = Quotes.GetMarketDays(
//				statisticsSummary.AccountReport.Benchmark,
//				statisticsSummary.AccountReport.StartDateTime,
//				statisticsSummary.AccountReport.EndDateTime.DateTime);
//			DateTime date;
//			for(int i = 0; i<marketDays.Count; i++)
//			{
//				date = marketDays.GetDateTime(i);
//			}

//			this.rowValue = (double)totalNumberOfTransactions / (double)marketDays.Count;
		}
	}
}
