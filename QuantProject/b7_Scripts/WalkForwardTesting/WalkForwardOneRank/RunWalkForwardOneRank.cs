/*
QuantProject - Quantitative Finance Library

RunWalkForwardOneRank.cs
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
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Data.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Script to test the One Rank strategy on many tickers, chosing the best group
	/// when a fixed time span has elapsed.
	/// </summary>
	public class RunWalkForwardOneRank : Script
	{
		private string startTicker;
    private ReportTable reportTable;
    private ExtendedDateTime startDateTime;
    private ExtendedDateTime endDateTime;
    private int numIntervalDays;
		private HistoricalDataStreamer historicalDataStreamer;
		private DataStreamerHandler dataStreamerHandler =
			new DataStreamerHandler();

		public RunWalkForwardOneRank()
		{
			this.reportTable = new ReportTable( "Summary_Reports" );
			this.startDateTime = new ExtendedDateTime(
				new DateTime( 2002 , 1 , 1 ) , BarComponent.Open );
			this.endDateTime = new ExtendedDateTime(
				new DateTime( 2002 , 12 , 31 ) , BarComponent.Open );
      this.numIntervalDays = 7;
			this.startTicker = "MSFT";
		}
    #region Run
		private void run_initializeHistoricalDataStreamer()
		{
			this.historicalDataStreamer = new HistoricalDataStreamer();
			this.historicalDataStreamer.StartDateTime = this.startDateTime;
			this.historicalDataStreamer.EndDateTime = this.endDateTime;
			this.historicalDataStreamer.Add( this.startTicker );
		}
    public override void Run()
    {
			Report report;
			run_initializeHistoricalDataStreamer();
			this.historicalDataStreamer.NewQuote +=
				new NewQuoteEventHandler( this.dataStreamerHandler.NewQuoteEventHandler );
			this.historicalDataStreamer.GoSimulate();
			report = new Report( this.dataStreamerHandler.Account );
			report.Show( "WFT One Rank" , this.numIntervalDays , this.startDateTime , "MSFT" );
		}
    #endregion
	}
}
