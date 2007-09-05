/*
QuantProject - Quantitative Finance Library

MissingQuotesException.cs
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

namespace QuantProject.Data
{
	/// <summary>
	/// This kind of exception is intended to be thrown
	/// when a ticker or an array of ticker 
	/// doesn't have the expected number of available
	/// quotes for a given time frame
	/// </summary>
	public class MissingQuotesException : Exception
	{
		private string ticker;
		private string[] tickers = null;
		private DateTime initialDateTime;
		private DateTime finalDateTime;
		private string message_getMessageForMissingQuotesForTickers()
		{
			string tickersSeparatedBySemicolon = null;
			foreach (string ticker in this.tickers)
				tickersSeparatedBySemicolon += ticker + ";" ;
			return "Not all these tickers: " + 
						tickersSeparatedBySemicolon + 
				    " have the same number of " +
				    "available quotes, " +
						"between " + this.initialDateTime.ToShortDateString() + 
						" and " + this.finalDateTime.ToShortDateString();
		}
		public override string Message
		{
			get
			{
				if( this.tickers == null )
				//this.tickers has not been set, and so the exception
				//refers to only one single ticker
					return "No quotes available for ticker " +
						this.ticker + ", between " + this.initialDateTime.ToShortDateString() + 
						" and " + this.finalDateTime.ToShortDateString();
				else // this.tickers has been set, so the
						// exception refers to two ore more tickers.
						// In other words, not all the tickers have the same
					  // number of quotes for the given period
					return message_getMessageForMissingQuotesForTickers();
			}
		}
		public MissingQuotesException(
		         string ticker,DateTime initialDateTime, DateTime finalDateTime )
		{
			this.ticker = ticker;
			this.initialDateTime = initialDateTime;
			this.finalDateTime = finalDateTime;
		}
		public MissingQuotesException(
		      string[] tickers,DateTime initialDateTime, DateTime finalDateTime )
		{
			this.tickers = tickers;
			this.initialDateTime = initialDateTime;
			this.finalDateTime = finalDateTime;
		}
	}
}
