/*
QuantProject - Quantitative Finance Library

Quote.cs
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
using QuantProject.ADT;

namespace QuantProject.Data.DataProviders
{
	/// <summary>
	/// Single ticker quote
	/// </summary>
	public class Quote
	{
		private string ticker;
		private double value;
		private ExtendedDateTime extendedDateTime;

		public string Ticker
		{
			get	{	return this.ticker;	}
			set	{	this.ticker = value;	}
		}

		public double Value
		{
			get	{	return this.value;	}
			set	{	this.value = value;	}
		}

		public ExtendedDateTime ExtendedDataTable
		{
			get	{	return this.extendedDateTime;	}
			set	{	this.extendedDateTime = value;	}
		}

		public Quote()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
