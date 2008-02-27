/*
QuantProject - Quantitative Finance Library

TickerNotExchangedException.cs
Copyright (C) 2008 
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Thrown when a quote is requested, but it is not present in
	/// the database
	/// </summary>
	public class TickerNotExchangedException : Exception
	{
		private string ticker;
		private EndOfDayDateTime endOfDayDateTime;
		public override string Message
		{
			get
			{
				return "Missing quote for ticker " +
					this.ticker + " for end of day date time " +
					this.endOfDayDateTime.Description;
			}
		}
		public TickerNotExchangedException( string ticker ,
			EndOfDayDateTime endOfDayDateTime )
		{
			this.ticker = ticker;
			this.endOfDayDateTime = endOfDayDateTime;
		}
	}
}
