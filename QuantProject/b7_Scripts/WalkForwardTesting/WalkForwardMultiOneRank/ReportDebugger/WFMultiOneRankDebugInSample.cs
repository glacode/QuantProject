/*
QuantProject - Quantitative Finance Library

WFMultiOneRankDebugInSample.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Simulation to test the in sample optimization
	/// </summary>
	public class WFMultiOneRankDebugInSample
	{
		private string[] signedTickers;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private string benchmark;

		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private IndexBasedEndOfDayTimer endOfDayTimer;
		private Account account;
		private WFMultiOneRankDebugEndOfDayTimerHandler
			endOfDayTimerHandler;

		public WFMultiOneRankDebugInSample( string[] signedTickers ,
		                                   DateTime firstDateTime , DateTime lastDateTime ,
		                                   string benchmark )
		{
			this.signedTickers = signedTickers;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;

			this.benchmark = benchmark;

//			this.startDateTime = this.dateTime.AddDays(
//				-this.numberDaysForInSampleOptimization - 1 );
			this.historicalMarketValueProvider =
				new HistoricalAdjustedQuoteProvider();
		}
		#region run
		private void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer(
					HistoricalEndOfDayTimer.GetMarketOpen( this.firstDateTime ) ,
//				new EndOfDayDateTime( this.firstDateTime ,
//				EndOfDaySpecificTime.MarketOpen ),
					this.benchmark );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WFMultiOneRankDebugInSample" ,
			                           this.endOfDayTimer ,
			                           new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new WFMultiOneRankDebugEndOfDayTimerHandler(
					this.account , this.signedTickers );
		}
		public void marketOpenEventHandler(	Object sender ,
		                                   DateTime dateTime )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( ( ( Timer )sender ).GetCurrentDateTime() >
			    this.lastDateTime )
			{
				// the simulation has reached the ending date
				this.account.Timer.Stop();
				Report report = new Report( this.account , this.historicalMarketValueProvider );
				report.Create(
					"WFT One Rank" , 1 ,
					HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
						this.lastDateTime ) ,
//						new EndOfDayDateTime( this.lastDateTime , EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
					"^SPX" );
				report.Show();
			}
		}
		
		private void newDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler( sender , dateTime );
//			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
//				this.marketCloseEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
		}

		public void Run()
		{
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
			
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );
			
//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler( this.marketOpenEventHandler );
//			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//					this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.oneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.Start();
		}
		#endregion
	}
}
