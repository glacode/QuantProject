/*
QuantProject - Quantitative Finance Library

BuyAndHoldStrategy.cs
Copyright (C) 2011
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
using System.Data;
using System.Collections;
using System.Collections.Generic;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.InSample.InSampleFitnessDistributionEstimation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.DataProviders;
using QuantProject.Data;
using QuantProject.Data.DataProviders;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Implements a simple buy and hold strategy:
	/// given testing positions are bought on the first day
	/// fired by the timer (at 16 or after)
	/// and held through all the period of testing
	/// </summary>
	[Serializable]
	public class BuyAndHoldStrategy : IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;
		
		private TestingPositions testingPositions;
//		private Benchmark benchmark;
//		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		public string Description
		{
			get
			{
				string description =
					"BuyAndHoldStrategy";
				return description;
			}
		}
		
		public bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return true;
			}
		}
				
//		public BuyAndHoldStrategy(TestingPositions testingPositions,  
//				         Benchmark benchmark,
//				         HistoricalMarketValueProvider historicalMarketValueProvider)
//		{
//			this.testingPositions = testingPositions;
//			this.benchmark = benchmark;
//			this.historicalMarketValueProvider = historicalMarketValueProvider;
//		}
		
		public BuyAndHoldStrategy(TestingPositions testingPositions)
		{
			this.testingPositions = testingPositions;
		}
		
		#region newDateTimeEventHandler_closePositions
				
		private void newDateTimeEventHandler_closePositions()
		{
			DateTime currentDateTime = this.now();
	  	AccountManager.ClosePositions( this.account );
		}
		#endregion newDateTimeEventHandler_closePositions
		
		#region newDateTimeEventHandler_openPositions
	
		private void newDateTimeEventHandler_openPositions()
		{
			try
			{
				AccountManager.OpenPositions( this.testingPositions.WeightedPositions,
				                             	this.account );
			}
			catch(Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
		#endregion newDateTimeEventHandler_openPositions
						
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if( this.account.Portfolio.Count == 0 &&
			    dateTime.Hour >= 16)
				this.newDateTimeEventHandler_openPositions();
		}
		
		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}
		
		private void raiseNewLogItem(  )
		{
			DummyLogItem logItem = new DummyLogItem( this.now() );
			NewLogItemEventArgs newLogItemEventArgs =
				new NewLogItemEventArgs( logItem );
			this.NewLogItem( this , newLogItemEventArgs );
		}
		private void notifyMessage(  )
		{
			string message = "BuyAndHold";
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		private void logOptimizationInfo(  )
		{
			this.raiseNewLogItem(  );
			this.notifyMessage(  );
		}
	}
}
