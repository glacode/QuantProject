/*
QuantProject - Quantitative Finance Library

ChosenTickers.cs
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
using QuantProject.Business.Timing;
using QuantProject.Data;
using QuantProject.Data.DataProviders;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// The collection of the chosen tickers, among the best performing ones
	/// </summary>
	public class ChosenTickers : Hashtable
	{
		private int numberTickersToBeChosen;

		public ChosenTickers( int numberTickersToBeChosen )
		{
			this.numberTickersToBeChosen = numberTickersToBeChosen;
		}
		#region SetTickers
		private void setTickers_build_handleTicker( string ticker , Account account )
		{
			double todayMarketValueAtClose = account.DataStreamer.GetCurrentBid(
				ticker );
			EndOfDayDateTime yesterdayAtClose = new
				EndOfDayDateTime( account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
				EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose = HistoricalDataProvider.GetMarketValue(
				ticker ,
				yesterdayAtClose.GetNearestExtendedDateTime() );
			if ( todayMarketValueAtClose > yesterdayMarketValueAtClose )
				// today close is higher than yesterday close
				this.Add( ticker , ticker );
		}
		private void setTickers_build( BestPerformingTickers bestPerformingTickers ,
			Account account )
		{
			int index = bestPerformingTickers.Count - 1;
			while ( ( this.Count < this.numberTickersToBeChosen ) &&
				( index >= 0) )
			{
				string ticker = ((Account)bestPerformingTickers[ index ]).Key;
				if ( account.DataStreamer.IsExchanged( ticker ) )
					setTickers_build_handleTicker( ticker ,
						account );
				index--;
			}
		}
		/// <summary>
		/// Populates the collection of eligible tickers
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetTickers( BestPerformingTickers bestPerformingTickers ,
			Account account )
		{
			this.Clear();
			this.setTickers_build( bestPerformingTickers , account );
		}
		#endregion
	}
}
