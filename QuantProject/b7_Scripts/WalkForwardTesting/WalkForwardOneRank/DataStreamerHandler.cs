/*
QuantProject - Quantitative Finance Library

DataStreamerHandler.cs
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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Data.DataProviders;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Implements NewQuoteEventHandler. This is the core strategy!
	/// </summary>
	public class DataStreamerHandler
	{
    private EligibleTickers eligibleTickers;
		private BestPerformingTickers bestPerformingTickers;
		private ChosenTickers chosenTickers;

    private int numberEligibleTickers = 100;
		private int numberBestPeformingTickers = 20;
		private int numberOfTickersToBeChosen = 5;
		private int windowDays;

		private Account account;

		public int NumberEligibleTickers
		{
			get { return this.numberEligibleTickers; }
		}
		public int NumberBestPeformingTickers
		{
			get { return this.numberBestPeformingTickers; }
		}
		public Account Account
		{
			get { return this.account; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberEligibleTickers">number of tickers to be chosen with the first selection:
		/// the best performers will be chosen among these first selected instruments</param>
		/// <param name="numberBestPeformingTickers">number of instruments to be chosen, as the best
		/// performers, among the eligible tickers</param>
		/// <param name="numberOfTickersToBeChosen">number of instruments to be chosen,
		/// among the best performers</param>
		/// <param name="windowDays">number of days between two consecutive
		/// best performing ticker calculation</param>
		public DataStreamerHandler( int numberEligibleTickers , int numberBestPeformingTickers ,
			int numberOfTickersToBeChosen , int windowDays )
		{
			this.numberEligibleTickers = numberEligibleTickers;
			this.numberBestPeformingTickers = numberBestPeformingTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.windowDays = windowDays;

			this.account = new Account( "Main" );

			this.eligibleTickers = new EligibleTickers( numberEligibleTickers );
			this.bestPerformingTickers = new BestPerformingTickers( numberBestPeformingTickers );
			this.chosenTickers = new ChosenTickers( this.numberOfTickersToBeChosen );
		}
		#region NewQuoteEventHandler
		private void newQuoteEventHandler_orderChosenTickers_closePositions()
		{
			foreach ( Position position in this.account.Portfolio )
				if ( this.chosenTickers.Contains( position.Instrument.Key ) )
				{
					this.account.ClosePosition( position , this.orderManager );
				}
		}
		private void newQuoteEventHandler_orderChosenTickers()
		{
			this.newQuoteEventHandler_orderChosenTickers_closePositions();
			this.newQuoteEventHandler_orderChosenTickers_openPositions();
		}
		/// <summary>
		/// Handles a NewQuote event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void NewQuoteEventHandler(
			Object sender , NewQuoteEventArgs eventArgs )
		{
			if ( ( this.eligibleTickers.Count == 0 ) ||
				( eventArgs.Quote.ExtendedDateTime.DateTime.CompareTo(
				this.bestPerformingTickers.LastUpdate.AddDays( this.windowDays ) ) >= 0 ) )
				// either eligible tickers have never been defined yet
				// or this.windowDays days elapsed since last best performing tickers calculation
			{
				this.eligibleTickers.SetTickers( eventArgs.Quote.ExtendedDateTime.DateTime );
				this.bestPerformingTickers.SetTickers( this.eligibleTickers ,
					eventArgs.Quote.ExtendedDateTime.DateTime );
			}
			this.chosenTickers.SetTickers( this.bestPerformingTickers );
			newQuoteEventHandler_orderChosenTickers();
		}
		#endregion
	}
}
