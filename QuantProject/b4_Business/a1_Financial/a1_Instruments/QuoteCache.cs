/*
QuantProject - Quantitative Finance Library

QuoteCache.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders;

namespace QuantProject.Business.Financial.Instruments
{
	/// <summary>
	/// Summary description for QuoteIdentifiers.
	/// </summary>
	public class QuoteCache
	{
    private static ArrayList quoteIdentifierList = new ArrayList();

		public QuoteCache()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public static void Add( Instrument instrument , BarComponent barComponent )
    {
      Object[] array = new Object[] { instrument , barComponent };
      quoteIdentifierList.Add( array );
    }

//    public static void SetCache( DateTime startDateTime , DateTime endDateTime )
//    {
//      foreach ( Object[] array in quoteIdentifierList )
//        HistoricalDataProvider.Add( ((Instrument) array[ 0 ]).Key , (BarComponent) array[ 1 ] );
//      HistoricalDataProvider.SetCachedHistories(
//        startDateTime , endDateTime );
//    }

    public static History GetOpenHistory( string instrumentKey )
    {
      return HistoricalDataProvider.GetOpenHistory( instrumentKey );
    }
  
    public static History GetCloseHistory( string instrumentKey )
    {
      return HistoricalDataProvider.GetCloseHistory( instrumentKey );
    }
  }
}
