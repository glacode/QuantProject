/*
QuantProject - Quantitative Finance Library

OneRank.cs
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
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// Script to test the One Rank strategy on a single ticker
	/// </summary>
	public class RunOneRank : Script
	{
		private DateTime startDateTime = new DateTime( 2000 , 1 , 1 );
		private DateTime endDateTime = new DateTime( 2000 , 12 , 31 );
		private Account account;
		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();
		/// <summary>
		/// Script to test the One Rank strategy on a single ticker
		/// </summary>
		public RunOneRank()
		{
			HistoricalEndOfDayTimer historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.startDateTime ,
				EndOfDaySpecificTime.MarketOpen ) , "MSFT" );
			this.account = new Account( "MSFT" , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new IBCommissionManager() );
//			this.account = new Account( "MSFT" , historicalEndOfDayTimer ,
//				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
//				this.historicalQuoteProvider ) ,
//				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
//				this.historicalQuoteProvider ) );
			OneRank oneRank = new OneRank( account ,
				this.endDateTime );
			Report report = new Report( this.account , this.historicalQuoteProvider );
			report.Show( "WFT One Rank" , 1 ,
			new EndOfDayDateTime( this.endDateTime , EndOfDaySpecificTime.MarketClose ) ,
			"MSFT" );
		}
	}
}
