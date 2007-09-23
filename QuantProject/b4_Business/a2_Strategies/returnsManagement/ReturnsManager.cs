/*
QuantProject - Quantitative Finance Library

ReturnsManager.cs
Copyright (C) 2007
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

using QuantProject.ADT.Collections;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Strategies.ReturnsManagement
{
	/// <summary>
	/// This class is used to keep and provide, in an efficient
	/// way, array of returns on EndOfDayIntervals (to be used
	/// by in sample optimizations)
	/// </summary>
	public class ReturnsManager
	{
		private ReturnIntervals returnIntervals; // a return for each interval
		private IHistoricalQuoteProvider historicalQuoteProvider; 
		
		private Set tickersMissingQuotes;

		private Hashtable tickersReturns;
		private Hashtable tickersReturnsStandardDeviations;

		/// <summary>
		/// End of day intervals on which returns are computed
		/// </summary>
		public ReturnIntervals ReturnIntervals
		{
			get { return this.returnIntervals; }
		}

		/// <summary>
		/// Number of returns, that is number of intervals
		/// </summary>
		public int NumberOfReturns
		{
			get { return this.returnIntervals.Count; }
		}
//		protected DateTime firstDateTime
//		{
//			get { return this.timeLineForQuotes.GetDateTime( 0 ); }
//		}
//		protected DateTime lastDateTime
//		{
//			get
//			{
//				int lastIndex = this.timeLineForQuotes.Count - 1;
//				return this.timeLineForQuotes.GetDateTime( lastIndex );
//			}
//		}
//
		/// <summary>
		/// Abstract class used to store and efficiently provide arrays of
		/// returns for several tickers, on dates within a given interval
		/// when a given benchmark is quoted too.
		/// If the ticker lacks some quote, all ticker's quotes are
		/// discarded
		/// </summary>
		/// <param name="firstDate">first quote date</param>
		/// <param name="lastDate">last quote date</param>
		/// <param name="benchmark">returns are computed only for
		/// dates when the benchmark is quoted; if n quotes are given
		/// for the benchmark, each array of returns will have exactly
		/// n-1 elements</param>
//		public ReturnsManager( DateTime firstDate , DateTime lastDate ,
//			string benchmark )
//		{
//			// TO DO: let WFLagEligibleTickers use this class also!!!
//			this.timeLineForQuotes =
//				this.getMarketDaysForQuotes( firstDate , lastDate , benchmark );
//			this.commonInitialization();
//		}
		public ReturnsManager( ReturnIntervals returnIntervals ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
			// TO DO: let WFLagEligibleTickers use this class also!!!
			this.returnIntervals = returnIntervals;
			this.commonInitialization();
			this.historicalQuoteProvider = historicalQuoteProvider;
		}
		private void commonInitialization()
		{
			this.tickersMissingQuotes = new Set();

			this.tickersReturns = new Hashtable();
			this.tickersReturnsStandardDeviations = new Hashtable();
		}
		private History getMarketDaysForQuotes( DateTime firstDate ,
			DateTime lastDate , string benchmark )
		{
			return Quotes.GetMarketDays( benchmark ,
				firstDate , lastDate );
		}

		private bool isAValidIndexForAReturn( int index )
		{
			return ( ( index >= 0 ) &&
				( index <= this.ReturnIntervals.Count - 1 ) );
		}
		#region GetReturns
		private bool areReturnsAlreadySet( string ticker )
		{
			return this.tickersReturns.ContainsKey( ticker );
		}
//		protected abstract History getQuotes( string ticker );
		#region setReturns
		private float selectReturnWithRespectToTheGivenIterval(
			EndOfDayHistory endOfDayQuotes , int i )
		{
			ReturnInterval returnInterval =
				this.returnIntervals[ i ];
			double firstQuote = (double)endOfDayQuotes[ returnInterval.Begin ];
			double lastQuote = (double)endOfDayQuotes[ returnInterval.End ];
			float intervalReturn = Convert.ToSingle( lastQuote / firstQuote - 1 );
			return intervalReturn;
		}
		private float[] selectReturnsWithRespectToTheGivenIntervals(
			EndOfDayHistory endOfDayQuotes )
		{
			// TO DO: this method is n log n, it could be implemented to
			// be have a linear complexity!!!
			float[] returnsWithRespectToTheGivenIntervals =
				new float[ this.returnIntervals.Count ];
			for ( int i = 0 ; i < this.returnIntervals.Count ; i++ )
				returnsWithRespectToTheGivenIntervals[ i ] =
					this.selectReturnWithRespectToTheGivenIterval( endOfDayQuotes , i );
			return returnsWithRespectToTheGivenIntervals;
		}
		private void setReturnsActually( string ticker ,
			EndOfDayHistory endOfDayQuotes )
		{
			float[] arrayOfReturns =
				this.selectReturnsWithRespectToTheGivenIntervals( endOfDayQuotes );
			this.tickersReturns.Add( ticker , arrayOfReturns );
		}
		private void setReturns( string ticker ,
			EndOfDayHistory endOfDayQuotes )
		{
			if ( this.returnIntervals.AreIntervalBordersAllCoveredBy(
				endOfDayQuotes ) )
				this.setReturnsActually( ticker , endOfDayQuotes );
			else
				this.tickersMissingQuotes.Add( ticker );
		}
		private void setReturns( string ticker )
		{
//			EndOfDayDateTime firstEndOfDayDateTime =
//				this.returnIntervals[ 0 ].Begin;
//			EndOfDayDateTime lastEndOfDayDateTime =
//				this.returnIntervals.LastEndOfDayDateTime;
			EndOfDayHistory endOfDayQuotes =
				this.historicalQuoteProvider.GetEndOfDayQuotes( ticker ,
				this.returnIntervals.BordersHistory );
			this.setReturns( ticker , endOfDayQuotes );
		}
		#endregion setReturns
		private float[] getAlreadySetReturns( string ticker )
		{
			return (float[])this.tickersReturns[ ticker ];
		}
		public float[] GetReturns( string ticker )
		{
			if ( !this.areReturnsAlreadySet( ticker ) )
				// returns for this tickerhave not been set yet
				this.setReturns( ticker );
			return this.getAlreadySetReturns( ticker );
		}
		#endregion GetReturns
		#region GetReturn
		/// <summary>
		/// Gives out the returnIndex_th return for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="returnIndex"></param>
		/// <returns></returns>
		private void getReturnCheckParameters( string ticker , int returnIndex )
		{
			this.GetReturns( ticker );  //enforces returns calculation for the ticker
			if ( this.tickersMissingQuotes.Contains( ticker ) )
				throw new Exception( "The ticker '" + ticker + "' has missing " +
					"quotes with respect to the timeLine!" );
			if ( !this.isAValidIndexForAReturn( returnIndex ) )
				throw new Exception( "returnIndex is too large for the time line" );
		}
		private float getReturnActually( string ticker , int returnIndex )
		{
			float[] tickerReturns = this.GetReturns( ticker );
			return tickerReturns[ returnIndex ];
		}
		public float GetReturn( string ticker , int returnIndex )
		{
			this.getReturnCheckParameters( ticker , returnIndex );
			return this.getReturnActually( ticker , returnIndex );
		}
		#endregion GetReturn

		#region GetReturnsStandardDeviation
		private bool isReturnsStandardDeviationAlreadySet( string ticker )
		{
			return this.tickersReturns.ContainsKey( ticker );
		}
		private void setReturnsStandardDeviation(
			string ticker , float[] tickerReturns )
		{
			this.tickersReturnsStandardDeviations.Add( ticker ,
				BasicFunctions.GetStdDev( tickerReturns ) );
		}

		private void setReturnsStandardDeviation( string ticker )
		{
			float[] tickerReturns = this.GetReturns( ticker );
			this.setReturnsStandardDeviation( ticker , tickerReturns );
		}
		private float getReturnsStandardDeviation( string ticker )
		{
			return Convert.ToSingle( this.tickersReturnsStandardDeviations[ ticker ] );
		}
		/// <summary>
		/// Returns the standard deviation of ticker returns (and stores
		/// it for future fast response)
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public float GetReturnsStandardDeviation( string ticker )
		{
			if ( !this.isReturnsStandardDeviationAlreadySet( ticker ) )
				this.setReturnsStandardDeviation( ticker );
			return this.getReturnsStandardDeviation( ticker );
		}
		#endregion //GetReturnsStandardDeviation
	}
}
