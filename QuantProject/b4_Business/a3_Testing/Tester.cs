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
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;


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
		private IDataStreamer dataStreamer;
		//private TestResults testResults;

		public OrderManager OrderManager
		{
			get { return orderManager; }
			set { orderManager = value; }
		}

		public Tester(TestWindow testWindow , TradingSystems tradingSystems , double initialCash ,
		              IDataStreamer dataStreamer )
		{
			this.testWindow = testWindow;
			this.dataStreamer = dataStreamer;
			this.TradingSystems = tradingSystems;
			this.initialCash = initialCash;
			this.Account.AddCash(
				HistoricalEndOfDayTimer.GetMarketOpen( this.testWindow.StartDateTime ) ,
				this.initialCash );
			//      this.Account.AddCash( new EndOfDayDateTime( testWindow.StartDateTime , EndOfDaySpecificTime.MarketOpen ) ,
			//        initialCash );
		}

		public override double Objective()
		{
			this.Account.Clear();
			this.Account.AddCash(
				HistoricalEndOfDayTimer.GetMarketOpen( this.testWindow.StartDateTime ) ,
				this.initialCash );
//				new EndOfDayDateTime( testWindow.StartDateTime , EndOfDaySpecificTime.MarketOpen ) ,
//			                     initialCash );
//			this.Account.AddCash( new EndOfDayDateTime( testWindow.StartDateTime , EndOfDaySpecificTime.MarketOpen ) ,
//			                     initialCash );
			this.Test();
			return - this.Account.GetFitnessValue();
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
		private void handleCurrentSignal( Signal signal , IDataStreamer dataStreamer )
		{
			Orders orders = this.Account.AccountStrategy.GetOrders( signal , dataStreamer );
			foreach (Order order in orders )
			{
				TimedTransaction timedTransaction =
					this.OrderManager.GetTimedTransaction(
						order , dataStreamer );
				this.Account.Add( timedTransaction );
				//Debug.WriteLine( account.ToString( dateTime ) );
			}
		}
		private void testCurrentDateForTradingSystem( TradingSystem tradingSystem ,
		                                             DateTime dateTime , IDataStreamer dataStreamer )
		{
			Signals signals = tradingSystem.GetSignals( dateTime );
			foreach (Signal signal in signals)
				handleCurrentSignal( signal , dataStreamer );
		}
		private void testCurrentDateTime( DateTime dateTime ,
		                                         IDataStreamer dataStreamer )
		{
			foreach (TradingSystem tradingSystem in this.TradingSystems)
				testCurrentDateForTradingSystem( tradingSystem , dateTime ,dataStreamer );
		}
		public override void Test()
		{
			DateTime dateTime = this.testWindow.StartDateTime;
			initializeTradingSystems();
			while ( dateTime <= this.testWindow.EndDateTime )
			{
				this.testCurrentDateTime(
					HistoricalEndOfDayTimer.GetMarketOpen( dateTime ) , this.dataStreamer );
				this.testCurrentDateTime(
					HistoricalEndOfDayTimer.GetMarketClose( dateTime ) , this.dataStreamer );
//				testCurrentDateTime( new ExtendedDateTime( dateTime , BarComponent.Open ) ,
//				                            dataStreamer );
//				testCurrentDateTime( new ExtendedDateTime( dateTime , BarComponent.Close ) ,
//				                            dataStreamer );
				dateTime = dateTime.AddDays( 1 );
			}
		}
		#endregion
	}
}
