/*
QuantProject - Quantitative Finance Library

RunLastChosenPortfolioOutOfSample.cs
Copyright (C) 2003
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
using System.IO;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Class for running a test based on last chosen tickers of a given
	/// PortfolioType
	/// </summary>
	[Serializable]
	public class RunLastChosenPortfolioOutOfSample : RunEfficientPortfolio
	{
		private string[] tickers;
		private PortfolioType typeOfPortfolio;
		public RunLastChosenPortfolioOutOfSample(string[] chosenTickers,
		                                         PortfolioType typeOfPortfolio,
		                                         string benchmark,
		                                         DateTime startDate,
		                                         DateTime endDate,
		                                         double maxRunningHours):
			base(benchmark,
			     startDate,
			     endDate,
			     typeOfPortfolio,
			     maxRunningHours)
		{
			this.tickers = chosenTickers;
			this.typeOfPortfolio = typeOfPortfolio;
			this.startDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( startDate );
//			new EndOfDayDateTime(
//				startDate, EndOfDaySpecificTime.MarketOpen );
			this.endDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( endDate );
//			new EndOfDayDateTime(
//				endDate, EndOfDaySpecificTime.MarketClose );
			this.ScriptName = "LastChosenPortfolioOutOfSample";
			
		}
		#region Run
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new EndOfDayTimerHandlerLastChosenPortfolio(this.tickers,
				                                            this.typeOfPortfolio,
				                                            this.account,
				                                            this.benchmark,
				                                            this.startDateTime,
				                                            this.endDateTime);
		}
		
		protected override void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new HistoricalEndOfDayTimer(this.startDateTime);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
			
		}
		protected override void run_initializeAccount()
		{
			//default account with no commissions
			this.account =  new Account( this.scriptName , this.endOfDayTimer ,
			                            new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider ) ,
			                            new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
			                                                                this.historicalMarketValueProvider ));
			
		}
		
		private void newDateTimeEventHandler( object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.checkDateForReport( sender , dateTime );
		}

		protected override void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );
			
//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayTimerHandler.MarketOpenEventHandler);
//
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
			//
			//this.endOfDayTimer.OneHourAfterMarketClose +=
			//  new OneHourAfterMarketCloseEventHandler(
			//  this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
			
		}
		
		
		public override void Run()
		{
			base.Run();
			Report report = new Report( this.account , this.historicalMarketValueProvider );
			report.Create(
				"Run last chosen tickers out of sample", 1 ,
				HistoricalEndOfDayTimer.GetMarketClose( this.endDateTime ) ,
//				new EndOfDayDateTime( this.endDateTime.DateTime ,
//				                     EndOfDaySpecificTime.MarketClose ) ,
				"^SPX" );
			report.Show();
		}
		
		//     protected override void checkDateForReport(
		//    	Object sender , DateTime dateTime)
		//    {
		//      if(dateTime.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime ||
		//        DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
		//        //last date is reached by the timer or maxRunning hours
		//        //are elapsed from the time script started
		//      {
		//        this.endOfDayTimer.Stop();
		//      }
//
		//    }

		protected override void checkDateForReport(
			Object sender , DateTime dateTime)
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
			{
				if( dateTime >= this.endDateTime ||
				   DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
					//last date is reached by the timer or maxRunning hours
					//are elapsed from the time script started
					this.endOfDayTimer.Stop();
			}
		}
		#endregion
		
	}
}
