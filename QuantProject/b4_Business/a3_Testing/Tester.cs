/*
QuantProject - Quantitative Finance Library

Tester.cs
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
using System.Diagnostics;
using QuantProject.Business.Strategies;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Testing
{
	/// <summary>
	/// Used to test an account (with its account strategy), given a time window,
	/// a trading system and a starting cash amount
	/// </summary>
	public class Tester : BackTester
	{
    private TestWindow testWindow;
    private OrderManager orderManager = new OrderManager();
    private double initialCash = 0.0;
    //private TestResults testResults;

    public OrderManager OrderManager
    {
      get { return orderManager; }
      set { orderManager = value; }
    }

    public Tester(TestWindow testWindow , TradingSystems tradingSystems , double initialCash)
		{
			this.testWindow = testWindow;
      this.TradingSystems = tradingSystems;
      this.initialCash = initialCash;
      this.Account.AddCash( new ExtendedDateTime( testWindow.StartDateTime , BarComponent.Open ) ,
        initialCash );
		}

    public override double Objective()
    {
      this.Account.Clear();
      this.Account.AddCash( new ExtendedDateTime( testWindow.StartDateTime , BarComponent.Open ) ,
        initialCash );
      this.Test();
      return - this.Account.GetProfitNetLoss(
        new EndOfDayDateTime( testWindow.EndDateTime , EndOfDaySpecificTime.MarketClose ) );
    }

    #region "Test"
    private void initializeTradingSystems()
    {
      foreach (TradingSystem tradingSystem in this.TradingSystems)
      {
        tradingSystem.Parameters = this.Parameters;
        tradingSystem.TestStartDateTime = this.testWindow.StartDateTime;
        tradingSystem.InitializeData();
      }
    }
    private void handleCurrentSignal( Signal signal )
    {
      Orders orders = this.Account.AccountStrategy.GetOrders( signal );
      foreach (Order order in orders )
      {
        TimedTransaction transaction = this.OrderManager.GetTransaction( order );
        this.Account.Add( transaction );
        //Debug.WriteLine( account.ToString( dateTime ) );
      }
    }
    private void testCurrentDateForTradingSystem( TradingSystem tradingSystem ,
      ExtendedDateTime extendedDateTime )
    {
      Signals signals = tradingSystem.GetSignals( extendedDateTime );
      foreach (Signal signal in signals)
        handleCurrentSignal( signal );
    }
    private void testCurrentExtendedDateTime( ExtendedDateTime extendedDateTime )
    {
      foreach (TradingSystem tradingSystem in this.TradingSystems)
        testCurrentDateForTradingSystem( tradingSystem , extendedDateTime );
    }
    public override void Test()
    {
      DateTime dateTime = this.testWindow.StartDateTime;
      initializeTradingSystems();
      while (dateTime <= this.testWindow.EndDateTime)
      {
        testCurrentExtendedDateTime( new ExtendedDateTime( dateTime , BarComponent.Open ) );
        testCurrentExtendedDateTime( new ExtendedDateTime( dateTime , BarComponent.Close ) );
        dateTime = dateTime.AddDays( 1 );
      }
    }
    #endregion
	}
}
