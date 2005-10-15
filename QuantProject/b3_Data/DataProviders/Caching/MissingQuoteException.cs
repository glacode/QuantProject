/*
QuantProject - Quantitative Finance Library

MissingQuoteException.cs
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

namespace QuantProject.Data.DataProviders.Caching
{
	/// <summary>
	/// Thrown when a quote is requested, but it is not present in
	/// the database
	/// </summary>
	public class MissingQuoteException : Exception
	{
		private string ticker;
		private DateTime dateTime;
		public override string Message
		{
			get
			{
				return "Missing quote for ticker " +
					this.ticker + " for date " + this.dateTime.ToString();
			}
		}
		public MissingQuoteException( string ticker , DateTime dateTime )
		{
			this.ticker = ticker;
			this.dateTime = dateTime;
		}
	}
}
