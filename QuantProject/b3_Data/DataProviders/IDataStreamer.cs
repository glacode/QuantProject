/*
QuantProject - Quantitative Finance Library

IDataStreamer.cs
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
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.DataProviders
{
  public delegate void NewQuoteEventHandler(
		Object sender , NewQuoteEventArgs eventArgs );

	/// <summary>
	/// Interface to be implemented by real time data providers
	/// </summary>
	public interface IDataStreamer
	{
    event NewQuoteEventHandler NewQuote;
		double GetCurrentBid( string ticker );
		double GetCurrentAsk( string ticker );
	}
}
