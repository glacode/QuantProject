/*
QuantProject - Quantitative Finance Library

Account.cs
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
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Summary description for Account.
	/// </summary>
	/// 

	[Serializable]
	public class Account : Keyed , IComparable
	{
		private double cashAmount;
		private AccountStrategy accountStrategy;
		private Timer timer;
		private IDataStreamer dataStreamer;
		private IOrderExecutor orderExecutor;
		private ICommissionManager commissionManager;
		private ArrayList activeOrders;
		private AccountReport accountReport;

		
		public Portfolio Portfolio = new Portfolio();
		//public AccountReport accountReport;

		public double CashAmount
		{
			get	{	return cashAmount; }
		}

		public Timer Timer
		{
			get	{	return this.timer;	}
			set	{	this.timer = value;	}
		}

		public IDataStreamer DataStreamer
		{
			get	{	return this.dataStreamer;	}
			set	{	this.dataStreamer = value;	}
		}

		public IOrderExecutor OrderExecutor
		{
			get	{	return this.orderExecutor;	}
			set	{	this.orderExecutor = value;	}
		}

		public AccountStrategy AccountStrategy
		{
			get { return accountStrategy; }
			set { accountStrategy = value; }
		}
		
		public TransactionHistory Transactions = new TransactionHistory();

		public Account( string accountName ) : base ( accountName )
		{
			this.initialize();
		}

		private void initialize()
		{
			cashAmount = 0;
			this.activeOrders = new ArrayList();
			accountStrategy = new AccountStrategy( this );
		}

		public Account() : base ( "account" )
		{
			this.initialize();
		}
		private void initialize( Timer timer ,	IDataStreamer dataStreamer ,
		                        IOrderExecutor orderExecutor )
		{
			this.timer = timer;
			this.dataStreamer = dataStreamer;
			this.orderExecutor = orderExecutor;
			this.orderExecutor.OrderFilled += new OrderFilledEventHandler(
				this.orderFilledEventHandler );
		}
		public Account( string accountName , Timer timer ,
		               IDataStreamer dataStreamer , IOrderExecutor orderExecutor ) : base( accountName )
		{
			this.initialize( timer , dataStreamer , orderExecutor );
			this.commissionManager = new ZeroCommissionManager();
			this.initialize();
		}

		public Account( string accountName , Timer timer ,
		               IDataStreamer dataStreamer , IOrderExecutor orderExecutor ,
		               ICommissionManager commissionManager ) : base( accountName )
		{
			this.initialize( timer , dataStreamer , orderExecutor );
			this.commissionManager = commissionManager;
			this.initialize();
		}

		private void orderFilledEventHandler( Object sender , OrderFilledEventArgs
		                                     eventArgs )
		{
			this.Add( eventArgs.TimedTransaction );
		}

		public virtual double GetFitnessValue()
		{
			if ( this.accountReport == null )
				// the account report has not been computed yet
			{
				AccountReport accountReport = new AccountReport(
					this ,
					new HistoricalAdjustedQuoteProvider() ,
					new SelectorForMaketClose(
						this.Transactions.FirstDateTime ) );
				this.accountReport = accountReport.Create( this.Key ,
				                                          1 , this.timer.GetCurrentDateTime() );
			}
			return (double)this.accountReport.Summary.ReturnOnAccount.Value;
		}
		public int CompareTo( Object account )
		{
			int returnValue = 0;
			if ( this.GetFitnessValue() < (( Account )account).GetFitnessValue() )
				returnValue = -1;
			if ( this.GetFitnessValue() > (( Account )account).GetFitnessValue() )
				returnValue = 1;
			return returnValue;
		}
		public void Clear()
		{
			this.cashAmount = 0;
			this.Transactions.Clear();
			this.Portfolio.Clear();
		}
		public void AddCash( DateTime dateTime , double moneyAmount )
		{
			try
			{
				TimedTransaction timedTransaction =
					new TimedTransaction(
						TransactionType.AddCash ,
						moneyAmount ,
						ExtendedDateTime.Copy( dateTime ) );
				this.Add( timedTransaction );
				//Transactions.MultiAdd( extendedDateTime.DateTime , timedTransaction );
				//cashAmount = cashAmount + moneyAmount;
			}
			catch (Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}

		public void AddCash( double moneyAmount )
		{
			this.AddCash( this.timer.GetCurrentDateTime() ,
			             moneyAmount );
		}

		private void addOrder_throwExceptions( Order order )
		{
			if  ( ( ( order.Type == OrderType.MarketSell ) ||
			       ( order.Type == OrderType.LimitSell ) ) &&
			     ( !this.Portfolio.IsLong( order.Instrument.Key ) ) )
				throw new Exception( "A sell order has been submitted, but this " +
				                    "account doesn't contain a long position for this ticker" );
			if  ( ( ( order.Type == OrderType.MarketCover ) ||
			       ( order.Type == OrderType.LimitCover ) ) &&
			     ( !this.Portfolio.IsShort( order.Instrument.Key ) ) )
				throw new Exception( "A cover order has been submitted, but this " +
				                    "account doesn't contain a short position for this ticker" );
		}
		public void AddOrder( Order order )
		{
			this.addOrder_throwExceptions( order );
			this.orderExecutor.Execute( order );
		}
		public bool Contains( Instrument instrument )
		{
			return Portfolio.Contains( instrument );
		}
		public bool Contains( string ticker )
		{
			return Portfolio.Contains( ticker );
		}


		private void updateCash( Transaction transaction )
		{
			cashAmount += transaction.CashFlow();
			if ( transaction.Commission != null )
				cashAmount -= transaction.Commission.Value;
		}

		protected virtual Commission getCommission( Transaction transaction )
		{
			return new Commission( transaction );
		}
		public void Add( TimedTransaction transaction )
		{
			if ( this.commissionManager != null )
				// an ICommissionManager has been passed to the constructor
				transaction.Commission = this.commissionManager.GetCommission( transaction );
			this.Transactions.Add( transaction );
			this.updateCash( transaction );
			this.Portfolio.Update( transaction );
			//this.accountReport.AddRecord( this );
		}
		//		public void Add( Transaction transaction )
		//		{
		//			this.Transactions.Add( transaction );
		//			this.updateCash( transaction );
		//			this.Portfolio.Update( transaction );
		//			//this.accountReport.AddRecord( this );
		//		}
		//

		//		public double GetMarketValue( EndOfDayDateTime endOfDayDateTime )
		//		{
		//			return this.CashAmount + this.Portfolio.GetMarketValue( endOfDayDateTime );
		//		}
		public double GetMarketValue( string ticker )
		{
			return this.dataStreamer.GetCurrentBid( ticker );
		}
		/// <summary>
		/// Returns the total account value ( cash + position value )
		/// </summary>
		/// <returns></returns>
		public double GetMarketValue()
		{
			return this.cashAmount +
				this.Portfolio.GetMarketValue( this.dataStreamer );
		}
		//		public double GetProfitNetLoss( EndOfDayDateTime endOfDayDateTime )
		//    {
		//      return GetMarketValue( endOfDayDateTime ) +
		//        this.Transactions.TotalWithdrawn -
		//        this.Transactions.TotalAddedCash;
		//    }
		//    public History GetProfitNetLossHistory( EndOfDayDateTime finalDateTime )
		//    {
		//      History history = new History();
		//      Account account = new Account( "ToGetProfitNetLossHistory" );
		//      foreach ( ArrayList arrayList in this.Transactions.Values )
		//        foreach ( EndOfDayTransaction transaction in arrayList )
		//        {
		//          account.Add( transaction );
		//          history.MultiAdd( transaction.EndOfDayDateTime.DateTime ,
		//            account.GetProfitNetLoss( transaction.EndOfDayDateTime ) );
		//        }
		//      history.MultiAdd( finalDateTime.DateTime ,
		//        account.GetProfitNetLoss( finalDateTime ) );
		//      return history;
		//    }
		//
		public string ToString( DateTime dateTime )
		{
			return
				"\nCashAmount : " + this.CashAmount +
				"\nPortfolioContent : " + this.Portfolio.ToString() +
				"\nPortfolioMarketValue : " + this.Portfolio.GetMarketValue(
					this.dataStreamer );
		}

		//		public AccountReport CreateReport( string reportName ,
		//			int numDaysForInterval , EndOfDayDateTime endDateTime )
		//		{
		//			AccountReport accountReport = new AccountReport( this );
		//			return accountReport.Create( reportName , numDaysForInterval , endDateTime );
		//		}
		public AccountReport CreateReport( string reportName ,
		                                  int numDaysForInterval , DateTime dateTime , string buyAndHoldTicker ,
		                                  HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			AccountReport accountReport = new AccountReport(
				this , historicalMarketValueProvider ,
				new SelectorForMaketClose( this.Transactions.FirstDateTime ) );
			return accountReport.Create( reportName , numDaysForInterval ,
			                            dateTime , buyAndHoldTicker );
		}
		public void Serialize( string filePathAndName )
		{
			//      //Dim FS As New IO.FileStream("c:\Rect.xml", IO.FileMode.Create, IO.FileAccess.Write)
			//   Dim XMLFormatter As New SoapFormatter()
			//   Dim R As New Rectangle(8, 8, 299, 499)
			//   XMLFormatter.Serialize(FS, R)
			FileStream fileStream = new FileStream( filePathAndName , FileMode.Create , FileAccess.Write );
			SoapFormatter soapFormatter = new SoapFormatter();
			soapFormatter.Serialize( fileStream , this );
		}
		#region ClosePosition_position
		private OrderType closePosition_getOrderType( Position position )
		{
			OrderType returnValue;
			if ( position.Quantity >= 0 )
				// long position
				returnValue = OrderType.MarketSell;
			else
				// short position
				returnValue = OrderType.MarketCover;
			return returnValue;
		}
		public void ClosePosition( Position position )
		{
			OrderType orderType = closePosition_getOrderType( position );
			Order order = new Order( orderType , position.Instrument , Math.Abs( position.Quantity ) );
			this.orderExecutor.Execute( order );
		}
		#endregion ClosePosition_position
		public void ClosePosition( string ticker )
		{
			this.ClosePosition( (Position)this.Portfolio[ ticker ] );
		}
	}
}
