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
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Soap;
using Excel;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;


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
		private IEndOfDayTimer endOfDayTimer;
		private IDataStreamer dataStreamer;
		private IOrderExecutor orderExecutor;
		private ArrayList activeOrders;
		private AccountReport accountReport;

    public Portfolio Portfolio = new Portfolio();
    //public AccountReport accountReport;

		public double CashAmount
		{
			get	{	return cashAmount; }
		}

		public IEndOfDayTimer EndOfDayTimer
		{
			get	{	return this.endOfDayTimer;	}
			set	{	this.endOfDayTimer = value;	}
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
		public Account( string accountName , IEndOfDayTimer endOfDayTimer ,
			IDataStreamer dataStreamer , IOrderExecutor orderExecutor ) : base( accountName )
		{
			this.endOfDayTimer = endOfDayTimer;
			this.dataStreamer = dataStreamer;
			this.orderExecutor = orderExecutor;
			this.orderExecutor.OrderFilled += new OrderFilledEventHandler(
				this.orderFilledEventHandler );
			this.initialize();
		}

		private void orderFilledEventHandler( Object sender , OrderFilledEventArgs
			eventArgs )
		{
			this.Add( eventArgs.EndOfDayTransaction );
		}

		public virtual double GetFitnessValue()
		{
			if ( this.accountReport == null )
				// the account report has not been computed yet
				this.accountReport = this.CreateReport( this.Key ,
					1 , this.endOfDayTimer.GetCurrentTime() );
			return this.accountReport.Summary.ReturnOnAccount;
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
    public void AddCash( ExtendedDateTime extendedDateTime , double moneyAmount )
    {
      try
      {
        TimedTransaction timedTransaction =
          new TimedTransaction( TransactionType.AddCash , moneyAmount , extendedDateTime );
        this.Add( timedTransaction );
        //Transactions.MultiAdd( extendedDateTime.DateTime , timedTransaction );
        //cashAmount = cashAmount + moneyAmount;
      }
      catch (Exception exception)
      {
				exception = exception;  // to avoid warning message
        /// TO DO!!!
      }
    }

		public void AddCash( double moneyAmount )
		{
			this.AddCash( this.endOfDayTimer.GetCurrentTime().GetNearestExtendedDateTime() ,
				moneyAmount );
		}

		public void AddOrder( Order order )
		{
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
    }

    public void Add( TimedTransaction transaction )
    {
      this.Transactions.Add( transaction );
      this.updateCash( transaction );
      this.Portfolio.Update( transaction );
      //this.accountReport.AddRecord( this );
    }
		public void Add( EndOfDayTransaction transaction )
		{
			this.Transactions.Add( transaction );
			this.updateCash( transaction );
			this.Portfolio.Update( transaction );
			//this.accountReport.AddRecord( this );
		}


		public double GetMarketValue( EndOfDayDateTime endOfDayDateTime )
		{
			return this.CashAmount + this.Portfolio.GetMarketValue( endOfDayDateTime );
		}
		public double GetMarketValue( string ticker )
		{
			return HistoricalDataProvider.GetMarketValue( ticker ,
				this.endOfDayTimer.GetCurrentTime().GetNearestExtendedDateTime() );
		}
		public double GetProfitNetLoss( EndOfDayDateTime endOfDayDateTime )
    {
      return GetMarketValue( endOfDayDateTime ) +
        this.Transactions.TotalWithdrawn -
        this.Transactions.TotalAddedCash;
    }

    public History GetProfitNetLossHistory( EndOfDayDateTime finalDateTime )
    {
      History history = new History();
      Account account = new Account( "ToGetProfitNetLossHistory" );
      foreach ( ArrayList arrayList in this.Transactions.Values )
        foreach ( EndOfDayTransaction transaction in arrayList )
        {
          account.Add( transaction );
          history.MultiAdd( transaction.EndOfDayDateTime.DateTime ,
            account.GetProfitNetLoss( transaction.EndOfDayDateTime ) );
        }
      history.MultiAdd( finalDateTime.DateTime ,
        account.GetProfitNetLoss( finalDateTime ) );
      return history;
    }

    public string ToString( DateTime dateTime )
    {
      return
        "\nCashAmount : " + this.CashAmount +
        "\nPortfolioContent : " + this.Portfolio.ToString() +
        "\nPortfolioMarketValue : " + this.Portfolio.GetMarketValue(
          new EndOfDayDateTime( dateTime , EndOfDaySpecificTime.MarketClose ) ) +
        "\nAccountProfitNetLoss : " + this.GetProfitNetLoss(
          new EndOfDayDateTime( dateTime , EndOfDaySpecificTime.MarketClose ) );
    }

		public AccountReport CreateReport( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime )
		{
			AccountReport accountReport = new AccountReport( this );
			return accountReport.Create( reportName , numDaysForInterval , endDateTime );
		}
		public AccountReport CreateReport( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime , string buyAndHoldTicker )
		{
			AccountReport accountReport = new AccountReport( this );
			return accountReport.Create( reportName , numDaysForInterval ,
				endDateTime , buyAndHoldTicker );
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
			Order order = new Order( orderType , position.Instrument , position.Quantity );
			this.orderExecutor.Execute( order );
		}
		#endregion ClosePosition_position
		public void ClosePosition( string ticker )
		{
			this.ClosePosition( (Position)this.Portfolio[ ticker ] );
		}
	}
}
