/*
QuantProject - Quantitative Finance Library

CachePage.cs
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

using QuantProject.ADT.Histories;
using QuantProject.DataAccess;

namespace QuantProject.Data.DataProviders.Caching
{
	/// <summary>
	/// A page containing quotes in main memory (fetched from the disk)
	/// </summary>
	public class CachePage : Hashtable
	{
    private string ticker;
		private int year;
		private QuoteField quoteField;

		private long rank;
		private History quotes;

		public long Rank
		{
			get { return this.rank; }
			set { this.rank = value; }
		}
		public History Quotes
		{
			get { return this.quotes; }
		}
		/// <summary>
		/// Contains (in main memory) data for the given ticker, for the given year,
		/// for the given quote field (open, high, low, close, volume, adjustedClose )
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="year"></param>
		public CachePage( string ticker , int year , QuoteField quoteField )
		{
			this.ticker = ticker;
			this.year = year;
			this.quoteField = quoteField;
			this.rank = long.MinValue;
		}
		/// <summary>
		/// Loads data from disk to main memory
		/// </summary>
		public void LoadData()
		{
			this.quotes = DataBase.GetHistory( ticker , quoteField ,
				new DateTime( year , 1 , 1 ) , new DateTime( year , 12 , 31 ) );
		}
	}
}
