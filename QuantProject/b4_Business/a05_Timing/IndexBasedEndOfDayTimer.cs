/*
QuantProject - Quantitative Finance Library

IndexBasedEndOfDayTimer.cs
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
using System.Collections;
using QuantProject.Data.DataTables;

using QuantProject.ADT;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// IEndOfDayTimer implementation using a market index as the base
	/// for time walking simulation
	/// </summary>
	public class IndexBasedEndOfDayTimer : HistoricalEndOfDayTimer
	{
		private string marketIndex;
    public string MarketIndex
    {
      get	{	return this.marketIndex;	}
    }
    private Quotes indexQuotes;

    public override event MarketOpenEventHandler MarketOpen;
    public override event FiveMinutesBeforeMarketCloseEventHandler FiveMinutesBeforeMarketClose;
    public override event MarketCloseEventHandler MarketClose;
    public override event OneHourAfterMarketCloseEventHandler OneHourAfterMarketClose;

		public IndexBasedEndOfDayTimer( EndOfDayDateTime startDateTime,
                                    string marketIndex): base(startDateTime)
		{
			this.marketIndex = marketIndex;
      this.indexQuotes = new Quotes(marketIndex,startDateTime.DateTime,DateTime.Now);
      this.StartDateTime = 
              new EndOfDayDateTime(this.indexQuotes.GetQuoteDateOrFollowing(this.StartDateTime.DateTime),
                                                                    EndOfDaySpecificTime.MarketOpen);
			this.tickers = new Hashtable();
		}

		/// <summary>
		/// Starts the time walking simulation, based on index's dates
		/// </summary>
		public override void Start()
		{
			this.isActive = true;
			this.currentTime = this.startDateTime.Copy();
			while ( this.isActive )
			{
				if ( ( this.MarketOpen != null ) && ( this.currentTime.EndOfDaySpecificTime ==
					EndOfDaySpecificTime.MarketOpen ) )
					this.MarketOpen( this , new EndOfDayTimingEventArgs( this.currentTime ) );
				if ( ( this.FiveMinutesBeforeMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
					EndOfDaySpecificTime.FiveMinutesBeforeMarketClose ) )
					this.FiveMinutesBeforeMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
				if ( ( this.MarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
					EndOfDaySpecificTime.MarketClose ) )
					this.MarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
				if ( ( this.OneHourAfterMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
					EndOfDaySpecificTime.OneHourAfterMarketClose ) )
					this.OneHourAfterMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
				this.moveNext(this.currentTime);
			}
		}
    //move the current endOfDayDateTime to the next moment
    //at which the market is surely open
    private void moveNext( EndOfDayDateTime endOfDayDateTimeToMove)
    {
      EndOfDaySpecificTime nextSpecificTime = endOfDayDateTimeToMove.GetNextEndOfDaySpecificTime();
      if ( nextSpecificTime < endOfDayDateTimeToMove.EndOfDaySpecificTime )
      {
        // the current end of day specific time is the last end of day specific time in the day
        endOfDayDateTimeToMove.DateTime =
                        this.indexQuotes.GetFollowingDate(endOfDayDateTimeToMove.DateTime, 1);
      }
      endOfDayDateTimeToMove.EndOfDaySpecificTime = nextSpecificTime;
    }


	}
}
