/*
QuantProject - Quantitative Finance Library

AccountReport.cs
Copyright (C) 2003
Glauco Siliprandi

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
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.Financial.Accounting.Reporting
{
	/// <summary>
	/// Summary description for AccountReport.
	/// </summary>
	[Serializable]
	public class AccountReport : ISerializable
	{
		private Account account;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private IDateTimeSelectorForEquityLine dateTimeSelectorForEquityLine;
		
		private Account accountCopy = new Account( "AccountCopy" );
		private string reportName;
		private DateTime endDateTime;
		private string benchmark;
		private History benchmarkEquityLine;
		//private long numDaysForInterval;
		private DataTable detailedDataTable;
		private Tables.Transactions transactionTable;
		private ReportTable roundTrades;
		private ReportTable equity;
		private EquityLine equityLine;
		private Tables.Summary summary;
		private Tables.StatisticsSummary statisticsSummary;
		
		private HistoricalMarketValueProvider historicalAdjustedQuoteProvider;

		public string Name
		{
			get { return this.reportName; }
			set { this.reportName = value; }
		}
		public DateTime EndDateTime
		{
			get { return this.endDateTime; }
		}
		public string Benchmark
		{
			get { return this.benchmark; }
		}
		public History BenchmarkEquityLine
		{
			get { return this.benchmarkEquityLine; }
		}
		public Account Account
		{
			get { return this.account; }
		}
		//    public long NumDaysForInterval
		//    {
		//      get { return numDaysForInterval; }
		//    }
		public DataTable DetailedDataTable
		{
			get { return detailedDataTable; }
		}
		public ReportTable TransactionTable
		{
			get { return transactionTable; }
		}
		public ReportTable RoundTrades
		{
			get { return roundTrades; }
		}
		public ReportTable Equity
		{
			get { return equity; }
		}
		public EquityLine EquityLine
		{
			get
			{
				if ( this.equityLine == null )
					// this.equityHistory has not been imported yet
				{
					this.equityLine = new EquityLine();
					this.equityLine.Import( this.Equity.DataTable ,
					                       QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.Date ,
					                       QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.AccountValue );
				}
				return this.equityLine;
			}
		}
		public Tables.Summary Summary
		{
			get { return summary; }
		}
		public Tables.StatisticsSummary StatisticsSummary
		{
			get { return this.statisticsSummary; }
		}
		public DateTime StartDateTime
		{
			get
			{
				return (DateTime) account.Transactions.GetKey( 0 );
			}
		}

		/// <summary>
		/// A text description for the report, suitable for file names also
		/// </summary>
		public string Description
		{
			get { return this.reportName; }
		}

		/// <summary>
		/// Creates report data for the given account
		/// </summary>
		/// <param name="account"></param>
		/// <param name="historicalMarketValueProvider">used to evaluate
		/// the market value for the given instruments at the given
		/// time</param>
		/// <param name="timerForEquityLine">used to decide the points
		/// in time when the equity line will be computed</param>
		public AccountReport(
			Account account ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			IDateTimeSelectorForEquityLine dateTimeSelectorForEquityLine )
		{
			this.account = account;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.dateTimeSelectorForEquityLine = dateTimeSelectorForEquityLine;
			
			this.historicalAdjustedQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}

		#region Create
		
		#region setDetailedDataTable
		
		#region getDetailedDataTable
		private void setColumns()
		{
			this.detailedDataTable.Columns.Add( "DateTime" , Type.GetType( "System.DateTime" ) );
//			transactions.Columns.Add( "BarComponent" , Type.GetType( "System.String" ) );
			this.detailedDataTable.Columns.Add( "TransactionType"  , Type.GetType( "System.String" ) );
			this.detailedDataTable.Columns.Add( "InstrumentKey"  , Type.GetType( "System.String" ) );
			this.detailedDataTable.Columns.Add( "Quantity"  , Type.GetType( "System.Int32" ) );
			this.detailedDataTable.Columns.Add( "Price"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "TransactionAmount"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "Commission"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "AccountCash"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "PortfolioValue"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "AccountValue"  , Type.GetType( "System.Double" ) );
			this.detailedDataTable.Columns.Add( "PnL"  , Type.GetType( "System.Double" ) );
		}
		
		#region setRows
		
		#region addBalanceItems
		
		private double getMarketValue( DateTime dateTime )
		{
			double marketValue = double.MinValue;
			if ( HistoricalEndOfDayTimer.IsMarketTime( dateTime ) )
				// market is open
				marketValue = this.accountCopy.Portfolio.GetMarketValue(
					dateTime , this.historicalMarketValueProvider );
			else
				// instruments are not exchanged at dateTime
				marketValue = this.accountCopy.Portfolio.GetMarketValue(
					dateTime , this.historicalAdjustedQuoteProvider );
			return marketValue;
		}
		
		private void addBalanceItems ( DateTime dateTime ,  DataRow dataRow )
		{
			dataRow[ "AccountCash" ] = this.accountCopy.CashAmount;

			dataRow[ "PortfolioValue" ] = this.getMarketValue( dateTime );
//			this.accountCopy.Portfolio.GetMarketValue(
//				dateTime , this.historicalAdjustedQuoteProvider );
//				dateTime , this.historicalMarketValueProvider );

			dataRow[ "AccountValue" ] = (double)dataRow[ "AccountCash" ] +
				(double)dataRow[ "PortfolioValue" ];
			dataRow[ "PnL" ] = (double)dataRow[ "AccountValue" ] +
				this.accountCopy.Transactions.TotalWithdrawn -
				this.accountCopy.Transactions.TotalAddedCash;
		}
		#endregion addBalanceItems
		
		#region setRows_addRowsForTransactionsBefore_nextDateTimeForEquityLine
		private bool isNextTransactionToBeAdded(
			int indexForTheNextTransactionToBeAdded ,
			DateTime nextDateTimeForEquityLine )
		{
			bool isToBeAdded = false;
			if ( indexForTheNextTransactionToBeAdded <
			    this.account.Transactions.Count )
				// not all transactions have been added, yet
			{
				DateTime dateTimeForTheNextTransactionToBeAdded =
					(DateTime)this.account.Transactions.GetKey(
						indexForTheNextTransactionToBeAdded );
				isToBeAdded = ( dateTimeForTheNextTransactionToBeAdded <=
				               nextDateTimeForEquityLine );
			}
			return isToBeAdded;
		}
		
		#region addRowsForTransactionsAtTheGivenDateTime
		private void addTransactionRow( TimedTransaction transaction )
		{
			DataRow dataRow = detailedDataTable.NewRow();
			dataRow[ "DateTime" ] = transaction.DateTime;
//			dataRow[ "BarComponent" ] = transaction.EndOfDayDateTime.EndOfDaySpecificTime.ToString();
			dataRow[ "TransactionType" ] = transaction.Type.ToString();
			if ( transaction.Instrument != null )
				dataRow[ "InstrumentKey" ] = transaction.Instrument.Key;
			else
				dataRow[ "InstrumentKey" ] = "";
			dataRow[ "Quantity" ] = transaction.Quantity;
			dataRow[ "Price" ] = transaction.InstrumentPrice;
			dataRow[ "TransactionAmount" ] = transaction.InstrumentPrice * transaction.Quantity;
			if ( transaction.Commission != null )
				dataRow[ "Commission" ] = transaction.Commission.Value;
			this.addBalanceItems( transaction.DateTime , dataRow );
			this.detailedDataTable.Rows.Add( dataRow );
		}
		private void addRowsForTransactionsAtTheGivenDateTime(
			DateTime dateTimeForTheNextTransactionToBeAdded )
		{
			foreach ( TimedTransaction timedTransaction in
			         (ArrayList)this.account.Transactions[
			         	dateTimeForTheNextTransactionToBeAdded ] )
			{
				this.accountCopy.Add( timedTransaction );
				addTransactionRow( timedTransaction );
			}
		}
		#endregion addRowsForTransactionsAtTheGivenDateTime
		
		private int setRows_addRowsForTransactionsTill_nextDateTimeForEquityLine(
			int indexForTheNextTransactionToBeAdded ,
			DateTime nextDateTimeForEquityLine )
		{
//			DateTime dateTimeForTheNextTransactionToBeAdded =
//				(DateTime)this.account.Transactions.GetKey(
//					indexForTheNextTransactionToBeAdded );
			while ( this.isNextTransactionToBeAdded(
				indexForTheNextTransactionToBeAdded , nextDateTimeForEquityLine ) )
			{
				DateTime dateTimeForTheNextTransactionToBeAdded =
					(DateTime)this.account.Transactions.GetKey(
						indexForTheNextTransactionToBeAdded );
				this.addRowsForTransactionsAtTheGivenDateTime(
					dateTimeForTheNextTransactionToBeAdded );
				indexForTheNextTransactionToBeAdded++;
			}
			return indexForTheNextTransactionToBeAdded;
		}
		#endregion setRows_addRowsForTransactionsBefore_nextDateTimeForEquityLine
		
		private void addRowForPnl_actually( DateTime nextDateTimeForEquityLine )
		{
			DataRow dataRow = detailedDataTable.NewRow();
			dataRow[ "DateTime" ] = nextDateTimeForEquityLine;
			this.addBalanceItems( nextDateTimeForEquityLine , dataRow );
//			addBalanceItems( new EndOfDayDateTime( currentDate , EndOfDaySpecificTime.MarketClose ) , dataRow );
			this.detailedDataTable.Rows.Add( dataRow );
		}
		private void addRowForPnl( DateTime nextDateTimeForEquityLine )
		{
//			if ( ( Convert.ToInt32(((TimeSpan)(currentDate.Date -
//			                                   (DateTime) account.Transactions.GetKey( 0 ))).TotalDays )
//			      % numDaysForInterval ) == 0 )
			addRowForPnl_actually( nextDateTimeForEquityLine );
		}
		private void setRows()
		{
			int indexForTheNextTransactionToBeAdded = 0;
			DateTime nextDateTimeForEquityLine =
				this.dateTimeSelectorForEquityLine.GetNextDateTime();
			while ( nextDateTimeForEquityLine <= this.endDateTime )
			{
				indexForTheNextTransactionToBeAdded =
					this.setRows_addRowsForTransactionsTill_nextDateTimeForEquityLine(
						indexForTheNextTransactionToBeAdded ,
						nextDateTimeForEquityLine );
				this.addRowForPnl( nextDateTimeForEquityLine );
				nextDateTimeForEquityLine =
					this.dateTimeSelectorForEquityLine.GetNextDateTime();
			}
			
//			DateTime currentDateTime = (DateTime) account.Transactions.GetKey( 0 );
//			try
//			{
//				while ( currentDateTime < this.endDateTime )
//				{
//					//addTransactionsToAccountCopy( currentDate );
//					addRowsForTransactions( currentDateTime , detailedDataTable );
//					addRowForPnl( numDaysForInterval , currentDateTime , detailedDataTable );
//					currentDateTime = this.getNextDateTimeForDetailedDataTable(
//						currentDateTime );
			////					currentDateTime = currentDate.AddDays( 1 );
//				}
//			}
//			catch (Exception ex)
//			{
//				MessageBox.Show( ex.ToString() );
//			}
		}
		#endregion setRows
		
		private void setDetailedDataTable_actually()
		{
			this.detailedDataTable = new System.Data.DataTable();
			this.setColumns();
			this.setRows();
//			return detailedDataTable;
		}
		#endregion getDetailedDataTable

		private void setDetailedDataTable( long numDaysForInterval )
		{
			if ( this.detailedDataTable == null )
				// the detailedDataTable has not been computed yet
				this.setDetailedDataTable_actually();
		}
		#endregion setDetailedDataTable
		
		
		private void setBenchmarkEquityLine()
		{
			History benchmarkQuotes = HistoricalQuotesProvider.GetAdjustedCloseHistory(
				this.benchmark , (DateTime)this.EquityLine.GetKey( 0 ) ,
				(DateTime)this.EquityLine.GetKey( this.EquityLine.Count - 1 ) );
			this.benchmarkEquityLine = benchmarkQuotes.Select( this.EquityLine );
			this.benchmarkEquityLine.Interpolate( this.EquityLine.Keys , new PreviousInterpolator() );
		}
		
		public AccountReport Create(
			string reportName , long numDaysForInterval ,
			DateTime endDateTime , string benchmark )
		{
			this.reportName = reportName;
			this.endDateTime = endDateTime;
			this.benchmark = benchmark;
			this.setDetailedDataTable( numDaysForInterval );
			this.transactionTable = new Tables.Transactions( reportName , detailedDataTable );
			//      this.transactionTable = getTransactionTable( reportName , detailedDataTable );
			this.roundTrades = new Tables.RoundTrades( reportName , this.transactionTable );
			this.equity = new Tables.Equity( reportName , detailedDataTable );
			if ( benchmark != "" )
				this.setBenchmarkEquityLine();
			//this.equity = getEquity( reportName , detailedDataTable );
			//this.summary = getSummary( reportName );
			this.summary = new Tables.Summary( this , historicalMarketValueProvider );
			this.statisticsSummary = new Tables.StatisticsSummary( this, historicalMarketValueProvider );
			return this;
		}
		#endregion Create

		/// <summary>
		/// Creates the equity line in a faster way than if it is created
		/// by the Create method
		/// </summary>
		/// <param name="numDaysForInterval"></param>
		/// <param name="endDateTime"></param>
		/// <returns></returns>
		public void SetEquityLine(
			long numDaysForInterval , DateTime endDateTime )
		{
			this.endDateTime = endDateTime;
			this.setDetailedDataTable( numDaysForInterval );
			this.equity = new Tables.Equity( reportName , detailedDataTable );
		}
		
		#region Serialization

		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AccountReport( SerializationInfo info , StreamingContext context ) :
			base( )
		{
			// get the set of serializable members for this class and its base classes
			Type thisType = this.GetType();
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(
				thisType , context);

			// deserialize the fields from the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo) mi[i];
				// set the field to the deserialized value
				try
				{
					fieldInfo.SetValue(
						this ,
						info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
				}
				catch (Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
			}
		}
		
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context)
		{
			// get the set of serializable members for this class and base classes
			Type thisType = this.GetType();
			MemberInfo[] mi =
				FormatterServices.GetSerializableMembers( thisType , context);

			// serialize the fields to the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				info.AddValue(mi[i].Name, ((FieldInfo) mi[i]).GetValue(this));
			}
		}
		#endregion

		public AccountReport Create(
			string reportName , long numDaysForInterval , DateTime endDateTime )
		{
			return this.Create( reportName , numDaysForInterval , endDateTime , "" );
		}
	}
}
