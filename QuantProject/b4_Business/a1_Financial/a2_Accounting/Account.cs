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
//using QuantProject.Data.MicrosoftExcel;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Strategies;

namespace QuantProject.Business.Financial.Accounting
{
  /// <summary>
  /// Summary description for Account.
  /// </summary>
  /// 

  [Serializable]
  public class Account : Keyed
  {
    private double cashAmount;
    private AccountStrategy accountStrategy;

    public Portfolio Portfolio = new Portfolio();
    //public AccountReport accountReport;

    public double CashAmount
    {
      get
      {
        return cashAmount;
      }
    }

    public AccountStrategy AccountStrategy
    {
      get { return accountStrategy; }
      set { accountStrategy = value; }
    }

    public Transactions Transactions = new Transactions();

    public Account( string accountName ) : base ( accountName )
    {
      this.initialize();
    }

    private void initialize()
    {
      cashAmount = 0;
      accountStrategy = new AccountStrategy( this );
    }

    public Account() : base ( "account" )
    {
      this.initialize();
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

    public bool Contains( Instrument instrument )
    {
      return Portfolio.Contains( instrument );
    }

    #region "Add( TimedTransaction transaction )"

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
    #endregion

    public double GetMarketValue( ExtendedDateTime extendedDateTime )
    {
      return this.CashAmount + this.Portfolio.GetMarketValue( extendedDateTime );
    }
    public double GetProfitNetLoss( ExtendedDateTime extendedDateTime )
    {
      return GetMarketValue( extendedDateTime ) +
        this.Transactions.TotalWithdrawn -
        this.Transactions.TotalAddedCash;
    }

    public History GetProfitNetLossHistory( ExtendedDateTime finalDateTime )
    {
      History history = new History();
      Account account = new Account( "ToGetProfitNetLossHistory" );
      foreach ( ArrayList arrayList in this.Transactions.Values )
        foreach ( TimedTransaction transaction in arrayList )
        {
          account.Add( transaction );
          history.MultiAdd( transaction.ExtendedDateTime.DateTime ,
            account.GetProfitNetLoss( transaction.ExtendedDateTime ) );
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
          new ExtendedDateTime( dateTime , BarComponent.Close ) ) +
        "\nAccountProfitNetLoss : " + this.GetProfitNetLoss(
          new ExtendedDateTime( dateTime , BarComponent.Close ) );
    }

    public AccountReport CreateReport( string reportName ,
      int numDaysForInterval , ExtendedDateTime endDateTime )
    {
      AccountReport accountReport = new AccountReport( this );
      return accountReport.Create( reportName , numDaysForInterval , endDateTime );
    }
    public AccountReport CreateReport( string reportName ,
      int numDaysForInterval , ExtendedDateTime endDateTime , string buyAndHoldTicker )
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
  }
}
