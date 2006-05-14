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
	/// Thrown when needed if an object DataTables.Quotes has no quotes
	/// for a given TimeFrame
	/// </summary>
	public class MissingQuotesException : Exception
	{
		private string ticker;
		private DateTime initialDateTime;
		private DateTime finalDateTime;
		public override string Message
		{
			get
			{
				return "No quotes available for ticker " +
					this.ticker + ", between " + this.initialDateTime.ToShortDateString() + 
					" and " + this.finalDateTime.ToShortDateString();
			}
		}
		public MissingQuotesException(
		         string ticker,DateTime initialDateTime, DateTime finalDateTime )
		{
			this.ticker = ticker;
			this.initialDateTime = initialDateTime;
			this.finalDateTime = finalDateTime;
		}
	}
}
