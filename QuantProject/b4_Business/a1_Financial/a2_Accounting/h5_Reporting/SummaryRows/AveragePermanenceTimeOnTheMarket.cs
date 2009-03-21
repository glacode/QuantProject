/*
QuantProject - Quantitative Finance Library

AveragePermanenceTimeOnTheMarket.cs
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
using System.Collections;
using System.Data;

using QuantProject.Data.DataTables;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary row containing the average permanence time in minutes
	/// on the market in the backtest's period for
	/// the account report equity line
	/// </summary>
	[Serializable]
	public class AveragePermanenceTimeOnTheMarket : DoubleSummaryRow
	{
		private DataTable transactions;
		private string fieldNameForTicker;
		private string fieldNameForDateTime;
		private string fieldNameForTransactionType;
		private double getMinutesForCurrentRoundTrade( int rowIndex )
		{
			DateTime startDateTimeOfTrade = 
				(DateTime)this.transactions.Rows[ rowIndex ][fieldNameForDateTime];
			DateTime endDateTimeOfTrade = 
				new DateTime(1900,1,1);
			string currentTicker =
				(string)this.transactions.Rows[ rowIndex ][fieldNameForTicker];
			string currentTradeType = 
				(string)this.transactions.Rows[ rowIndex ][fieldNameForTransactionType];
			for (int j = rowIndex + 1; j < this.transactions.Rows.Count ; j++)
			{
				if( currentTicker == (string)this.transactions.Rows[ j ][fieldNameForTicker] &&
				    ((currentTradeType == "BuyLong" && 
				     (string)this.transactions.Rows[ j ][fieldNameForTransactionType] == "Sell")
				     ||
				     (currentTradeType == "SellShort" && 
				     (string)this.transactions.Rows[ j ][fieldNameForTransactionType] == "Cover")
				    ) )
				{
					endDateTimeOfTrade = 
						(DateTime)this.transactions.Rows[ j ][fieldNameForDateTime];
					j = this.transactions.Rows.Count;
				}
			}
			TimeSpan timeSpanBetweenEnterAndExit = 
				endDateTimeOfTrade.Subtract(startDateTimeOfTrade);
			
			return timeSpanBetweenEnterAndExit.TotalMinutes;
		}
		private bool doesCurrentRowIndexPointToABuyLongOrASellShort( int rowIndex )
		{
			bool returnValue;
			string currentTransactionType =
				(string)this.transactions.Rows[ rowIndex ][fieldNameForTransactionType];
			returnValue =
				( currentTransactionType == "BuyLong" || currentTransactionType == "SellShort" );
			return returnValue;
		}
		private void setFieldNames()
		{
			this.fieldNameForTicker = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForTicker;
			this.fieldNameForDateTime = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForDateTime;
			this.fieldNameForTransactionType = 
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Transactions.FieldNameForTransactionType;
		}
		public AveragePermanenceTimeOnTheMarket( Summary summary ) : base(2)
		{
			this.rowDescription = "Average permanence time on the market (minutes)";
			double totalNumberOfMinutesOnTheMarket = 0.0;
			int totalNumberOfRoundTrades = 0;
			this.transactions = summary.AccountReport.TransactionTable.DataTable;
			this.setFieldNames();
			for(int rowIndex = 0; rowIndex<this.transactions.Rows.Count; rowIndex++)
			{
				if( this.doesCurrentRowIndexPointToABuyLongOrASellShort( rowIndex ) )
				{
					totalNumberOfRoundTrades++;
					totalNumberOfMinutesOnTheMarket += this.getMinutesForCurrentRoundTrade(rowIndex);
				}
			}
			this.rowValue = totalNumberOfMinutesOnTheMarket / (double)totalNumberOfRoundTrades;
		}
	}
}


