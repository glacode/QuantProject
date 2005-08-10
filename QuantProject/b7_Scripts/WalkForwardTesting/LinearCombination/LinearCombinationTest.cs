/*
QuantProject - Quantitative Finance Library

LinearCombinationTest.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Object to test a linear combination strategy on given positions,
	/// on a given period of time 
	/// </summary>
	public class LinearCombinationTest
	{
		private DateTime firstDate;
		private DateTime lastDate;
		private GenomeRepresentation genomeRepresentation;
		private bool openToCloseDaily;

		private IHistoricalQuoteProvider historicalQuoteProvider;
		private HistoricalEndOfDayTimer historicalEndOfDayTimer;
		private Account account;
		private IEndOfDayStrategy endOfDayStrategy;

		public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
			GenomeRepresentation genomeRepresentation , bool openToCloseDaily )
		{
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.genomeRepresentation = genomeRepresentation;
			this.openToCloseDaily = openToCloseDaily;
		}

		private void oneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >=
				this.lastDate )
				this.account.EndOfDayTimer.Stop();
		}

		private void run_setHistoricalQuoteProvider()
		{
			if ( this.openToCloseDaily )
				this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			else
				this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}
		private void run_setStrategy()
		{
			string[] signedTickers = GenomeRepresentation.GetSignedTickers(
				this.genomeRepresentation.SignedTickers );
			if ( this.openToCloseDaily )
				this.endOfDayStrategy = new OpenToCloseDailyStrategy(
					this.account , signedTickers );
			else
				this.endOfDayStrategy = new OpenToCloseWeeklyStrategy(
					this.account , signedTickers );
		}
		private string getDateString( DateTime dateTime )
		{
			string returnValue = dateTime.ToString( "yy-MM-dd" );
			return returnValue;
		}
		private string run_getReportTitle()
		{
			string returnValue = "Fitness:" +
				this.genomeRepresentation.Fitness.ToString() +
				" | Tickers:" +
				this.genomeRepresentation.SignedTickers  + " - " +
				"from " + this.getDateString( this.firstDate ) +
				" to " + this.getDateString( this.lastDate ) +
				" opt. in sample from " + this.getDateString(
				this.genomeRepresentation.FirstOptimizationDate ) +
				" to " + this.getDateString(
				this.genomeRepresentation.LastOptimizationDate );
			return returnValue;
		}
		public void Run()
		{
			this.historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.firstDate ,
				EndOfDaySpecificTime.MarketOpen ) , "DYN" );
			run_setHistoricalQuoteProvider();
			this.account = new Account( "LinearCombination" , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) );
			run_setStrategy();
			//			OneRank oneRank = new OneRank( account ,
			//				this.endDateTime );
			this.historicalEndOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.endOfDayStrategy.MarketOpenEventHandler );
			this.historicalEndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.account.EndOfDayTimer.Start();

			Report report = new Report( this.account , this.historicalQuoteProvider );
			report.Create( "Linear Combination" , 1 ,
				new EndOfDayDateTime( this.lastDate , EndOfDaySpecificTime.MarketClose ) ,
				"^SPX" );
			//			ObjectArchiver.Archive( report.AccountReport ,
			//				@"C:\Documents and Settings\Glauco\Desktop\reports\runOneRank.qPr" );
			report.Text = this.run_getReportTitle();
			report.Show();
		}
	}
}
