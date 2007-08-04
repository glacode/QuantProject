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
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Strategies.ReturnsManagement
{
	/// <summary>
	/// This abstract class is used to keep and provide, in an efficient
	/// way, array of returns (to be used by in sample optimizations)
	/// </summary>
	public abstract class ReturnsManager
	{
		protected History timeLineForQuotes; // if we have n market days for quotes,
		// we will then have n-1 returns
		
		private Set tickersMissingQuotes;

		private Hashtable tickersReturns;
		private Hashtable tickersReturnsStandardDeviations;

		/// <summary>
		/// Dates when quotes are computed. If TimeLine contains
		/// n elements, then returns are n-1 elements
		/// </summary>
		public History TimeLine
		{
			get { return this.timeLineForQuotes; }
		}

		/// <summary>
		/// Number of returns, that is TimeLine's elements minus 1
		/// </summary>
		public int NumberOfReturns
		{
			get { return this.TimeLine.Count - 1; }
		}
		protected DateTime firstDateTime
		{
			get { return this.timeLineForQuotes.GetDateTime( 0 ); }
		}
		protected DateTime lastDateTime
		{
			get
			{
				int lastIndex = this.timeLineForQuotes.Count - 1;
				return this.timeLineForQuotes.GetDateTime( lastIndex );
			}
		}

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
		public ReturnsManager( DateTime firstDate , DateTime lastDate ,
			string benchmark )
		{
			// TO DO: let WFLagEligibleTickers use this class also!!!
			this.timeLineForQuotes =
				this.getMarketDaysForQuotes( firstDate , lastDate , benchmark );
			this.commonInitialization();
		}
		public ReturnsManager( History timeLine )
		{
			// TO DO: let WFLagEligibleTickers use this class also!!!
			this.timeLineForQuotes = timeLine;
			this.commonInitialization();
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
			return ( ( index >= 0 ) && ( index <= this.TimeLine.Count - 2 ) );
		}
		#region GetReturns
		private bool areReturnsAlreadySet( string ticker )
		{
			return this.tickersReturns.ContainsKey( ticker );
		}
		protected abstract History getQuotes( string ticker );
		#region setReturns
		private bool areMarketDaysForQuotesAllCovered( History returns )
		{
			bool areAllCovered = true;
			foreach ( DateTime dateTime in this.timeLineForQuotes.TimeLine  )
				if ( !returns.ContainsKey( dateTime ) )
					areAllCovered = false;
			return areAllCovered;
		}
		private float selectReturnWithRespectToTheTimeLine( History quotes ,
			int i )
		{
			float currentQuote = (float)quotes.GetByIndex( i );
			float nextQuote = (float)quotes.GetByIndex( i + 1 );
			float currentReturn = nextQuote / currentQuote - 1;
			return currentReturn;
		}
		private float[] selectReturnsWithRespectToTheTimeLine( History quotes )
		{
			// TO DO: this method is n log n, it could be implemented to
			// be have a linear complexity!!!
			float[] returnsWithRespectToTheTimeLine =
				new float[ this.timeLineForQuotes.Count - 1 ];
			for ( int i = 0 ; i < this.timeLineForQuotes.Count - 1 ; i++ )
				returnsWithRespectToTheTimeLine[ i ] =
					this.selectReturnWithRespectToTheTimeLine( quotes , i );
			return returnsWithRespectToTheTimeLine;
		}
		private void setReturnsActually( string ticker , History quotes )
		{
			float[] arrayOfReturns =
				this.selectReturnsWithRespectToTheTimeLine( quotes );
			this.tickersReturns.Add( ticker , arrayOfReturns );
		}
		private void setReturns( string ticker , History quotes )
		{
			if ( this.areMarketDaysForQuotesAllCovered( quotes ) )
				this.setReturnsActually( ticker , quotes );
			else
				this.tickersMissingQuotes.Add( ticker );
		}
		private void setReturns( string ticker )
		{
			History quotes = this.getQuotes( ticker );
			this.setReturns( ticker , quotes );
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
