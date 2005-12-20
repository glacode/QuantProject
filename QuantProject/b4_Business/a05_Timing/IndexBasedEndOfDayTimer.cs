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
	[Serializable]
	public class IndexBasedEndOfDayTimer : HistoricalEndOfDayTimer
	{
		private string marketIndex;
    public string MarketIndex
    {
      get	{	return this.marketIndex;	}
    }
    private Quotes indexQuotes;

		public IndexBasedEndOfDayTimer( EndOfDayDateTime startDateTime,
                                    string marketIndex): base(startDateTime)
		{
			this.marketIndex = marketIndex;
      this.indexQuotes = new Quotes(marketIndex,startDateTime.DateTime,DateTime.Now);
			if ( this.indexQuotes.Rows.Count == 0 )
			{
				string errorMessage = "IndexBasedEndOfDayTimer error: the given " +
					"index (" + marketIndex + ") has no quotes in the interval.";
				throw new Exception( errorMessage );
			}
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
			base.activeTimer();
			while ( this.isActive )
			{
        base.callEvents();
				this.moveNext();
			}
		}
    //move the current endOfDayDateTime to the next moment
    //at which the market is surely open
    protected override void moveNext()
    {
      EndOfDaySpecificTime nextSpecificTime = this.currentTime.GetNextEndOfDaySpecificTime();
      if ( nextSpecificTime < this.currentTime.EndOfDaySpecificTime )
      {
        // the current end of day specific time is the last end of day specific time in the day
        this.currentTime.DateTime =
                        this.indexQuotes.GetFollowingDate(this.currentTime.DateTime, 1);
      }
      this.currentTime.EndOfDaySpecificTime = nextSpecificTime;
    }
    /// <summary>
    /// Gets the previous index based date time
    /// </summary>
    public DateTime GetPreviousDateTime()
    {
      return this.indexQuotes.GetPrecedingDate(this.currentTime.DateTime,1);
    }

	}
}
