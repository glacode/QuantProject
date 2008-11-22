/*
QuantProject - Quantitative Finance Library

BestPerformingTickers.cs
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

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.DataProviders.Caching;
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// The collection of best performing tickers, among the eligible ones
	/// </summary>
	public class BestPerformingTickers : ArrayList,IProgressNotifier
	{
		private int numberBestPerformingTickers;
		private int numberDaysForPerformanceCalculation;

		private HistoricalMarketValueProvider historicalMarketValueProvider =
			new HistoricalAdjustedQuoteProvider();

		private double calculatedTickers = 0;

		private ArrayList eligibleAccounts;
		private ArrayList tickersWithGoodness;

		private DateTime lastUpdate;

		public DateTime LastUpdate
		{
			get { return this.lastUpdate; }
		}

		public BestPerformingTickers( int numberBestPerformingTickers ,
			int numberDaysForPerformanceCalculation )
		{
			this.numberBestPerformingTickers = numberBestPerformingTickers;
			this.numberDaysForPerformanceCalculation = numberDaysForPerformanceCalculation;
			this.eligibleAccounts = new ArrayList();
			this.tickersWithGoodness = new ArrayList();
		}

		public event NewProgressEventHandler NewProgress;

		#region SetTickers
		#region setTickers_build_forDebug
		private void setTickers_build_forDebug_addAccount( string ticker , DateTime dateTime )
		{
			HistoricalEndOfDayTimer historicalEndOfDayTimer =
				new HistoricalEndOfDayTimer(
					HistoricalEndOfDayTimer.GetMarketOpen(
						dateTime.AddDays( -this.numberDaysForPerformanceCalculation ) ) );
//				new EndOfDayDateTime( dateTime.AddDays( -this.numberDaysForPerformanceCalculation ) ,
//				EndOfDaySpecificTime.MarketOpen ) );
			ComparableAccount account = new ComparableAccount( ticker , historicalEndOfDayTimer ,
				new HistoricalDataStreamer( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) ,
				new HistoricalOrderExecutor( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) );
			OneRank oneRank = new OneRank( account , dateTime );
			double goodness = account.Goodness;  // forces Goodness computation here (for a better ProgressBar effect)
			this.eligibleAccounts.Add( account );
		}
		/// <summary>
		/// used to debug insample results
		/// </summary>
		/// <param name="eligibleTickers"></param>
		/// <param name="dateTime"></param>
		private void setTickers_build_forDebug( EligibleTickers eligibleTickers , DateTime dateTime )
		{
			this.eligibleAccounts.Clear();
			foreach ( string ticker in eligibleTickers.Keys )
			{
				setTickers_build_forDebug_addAccount( ticker , dateTime );
				this.calculatedTickers++;
				if ( Math.Floor( this.calculatedTickers / eligibleAccounts.Count * 100 ) >
					Math.Floor( ( this.calculatedTickers - 1 ) / eligibleAccounts.Count * 100 ) )
					// a new time percentage point has been elapsed
					this.NewProgress( this , new NewProgressEventArgs(
						Convert.ToInt32( Math.Floor( this.calculatedTickers / eligibleTickers.Count * 100 ) ) ,
						100 ) );
			}
			this.eligibleAccounts.Sort();
			for ( int index=this.eligibleAccounts.Count - 1 ;
				index >= this.eligibleAccounts.Count - this.numberBestPerformingTickers ;
				index-- )
			{
				ComparableAccount account = (ComparableAccount)this.eligibleAccounts[ index ];
				this.Add( account );
			}
		}
		#endregion
		private void setTickers_build_addTickerWithGoodness( string ticker , DateTime dateTime )
		{
			HistoricalEndOfDayTimer historicalEndOfDayTimer =
				new HistoricalEndOfDayTimer(
					HistoricalEndOfDayTimer.GetMarketOpen(
						dateTime.AddDays( -this.numberDaysForPerformanceCalculation ) ) );
//				new EndOfDayDateTime( dateTime.AddDays( -this.numberDaysForPerformanceCalculation ) ,
//				EndOfDaySpecificTime.MarketOpen ) );
			ComparableAccount account = new ComparableAccount( ticker , historicalEndOfDayTimer ,
				new HistoricalDataStreamer( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) ,
				new HistoricalOrderExecutor( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) );
			OneRank oneRank = new OneRank( account , dateTime );
			this.tickersWithGoodness.Add( new EstimatedObject( account.Key , account.Goodness ) );
		}
		private void setTickers_build( EligibleTickers eligibleTickers , DateTime dateTime )
		{
			this.tickersWithGoodness.Clear();
			foreach ( string ticker in eligibleTickers.Keys )
			{
				try
				{
					setTickers_build_addTickerWithGoodness( ticker , dateTime );
					this.calculatedTickers++;
					if ( Math.Floor( this.calculatedTickers / this.tickersWithGoodness.Count * 100 ) >
						Math.Floor( ( this.calculatedTickers - 1 ) / this.tickersWithGoodness.Count * 100 ) )
						// a new time percentage point has been elapsed
						this.NewProgress( this , new NewProgressEventArgs(
							Convert.ToInt32( Math.Floor( this.calculatedTickers / eligibleTickers.Count * 100 ) ) ,
							100 ) );
				} 
				catch ( MissingQuoteException exception )
				{
					// the given ticker has not quotes for alle the inSample period
					string doNothing = exception.Message;
				}
			}
			this.tickersWithGoodness.Sort();
			for ( int index=this.tickersWithGoodness.Count - 1 ;
				index >= this.tickersWithGoodness.Count - this.numberBestPerformingTickers ;
				index-- )
			{
				this.Add( ((EstimatedObject)this.tickersWithGoodness[ index ]).ObjectToBeEstimated );
			}
		}
		/// <summary>
		/// Populates the collection of the best performing tickers
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetTickers( EligibleTickers eligibleTickers , DateTime dateTime )
		{
			this.calculatedTickers = 0;
			this.NewProgress( this , new NewProgressEventArgs( 0 , 100 ) );
			this.Clear();
//			this.setTickers_build_forDebug( eligibleTickers , dateTime );
			this.setTickers_build( eligibleTickers , dateTime );
			this.lastUpdate = dateTime;
		}
		#endregion
	}
}
