/*
QuantProject - Quantitative Finance Library

AverageNumberOfTransactionsPerDay.cs
Copyright (C) 2007 
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

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary row containing the average number of transactions for each market day
	/// in the backtest's period for
	/// the account report equity line
	/// </summary>
	[Serializable]
	public class AverageNumberOfTransactionsPerDay : DoubleSummaryRow
	{
		public AverageNumberOfTransactionsPerDay( Summary summary ) : base( 2 )
		{
			this.rowDescription = "Average n. of transactions per market day";
			//int totalNumberOfTransactions = summary.AccountReport.Account.Transactions.Count;
			//there must be a mistake somewhere: why the previous line is not right?
			int totalNumberOfTransactions = summary.AccountReport.TransactionTable.DataTable.Rows.Count;
			History marketDays = Quotes.GetMarketDays(
				summary.AccountReport.Benchmark,
				summary.AccountReport.StartDateTime,
				summary.AccountReport.EndDateTime);
			this.rowValue = (double)totalNumberOfTransactions / (double)marketDays.Count;
		}
	}
}
