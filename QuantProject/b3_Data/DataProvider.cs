/*
QuantProject - Quantitative Finance Library

DataProvider.cs
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
using QuantProject.DataAccess;

namespace QuantProject.Data
{
	/// <summary>
	/// Summary description for DataProvider.
	/// </summary>
	public class DataProvider
	{
    private static Hashtable cachedHistories = new Hashtable();

		public DataProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    // to be deleted
    public static void Add( string instrumentKey )
    {
      Hashtable barComponentHistories = new Hashtable();
      cachedHistories.Add( instrumentKey , barComponentHistories );
    }

    /// <summary>
    /// Adds a new instrument quote history to be cached in memory
    /// </summary>
    /// <param name="instrument">Instrument to be monitored</param>
    /// <param name="barComponent">Bar component to be monitored (Open, High, Low, Close or Volume)</param>
    public static void Add( string instrumentKey , BarComponent barComponent )
    {
      if ( !cachedHistories.ContainsKey( instrumentKey ) )
        cachedHistories.Add( instrumentKey , new Hashtable() );
      ((Hashtable) cachedHistories[ instrumentKey ]).Add(
        barComponent , barComponent );
    }

    //public static void

    public static void SetCachedHistories(
      DateTime startDateTime , DateTime endDateTime )
    {
      ArrayList keyArray = new ArrayList();
      foreach (string instrumentKey in cachedHistories.Keys)
        keyArray.Add( instrumentKey );
      foreach (string instrumentKey in keyArray )
      {
        Hashtable barComponents = new Hashtable();
        foreach (BarComponent barComponent in
          (( Hashtable )cachedHistories[ instrumentKey ]).Keys )
          barComponents.Add( barComponent , barComponent );
        Hashtable histories = DataBase.GetHistories(
          instrumentKey , barComponents , startDateTime , endDateTime );
        cachedHistories[ instrumentKey ] = histories;
      }
    }

    public static History GetOpenHistory( string instrumentKey )
    {
      return (History)((Hashtable)cachedHistories[ instrumentKey ])[
        BarComponent.Open ];
    }

		private static History getHistory( string instrumentKey , BarComponent barComponent )
		{
			if ( ( !cachedHistories.Contains( instrumentKey ) ) ||
				( !((Hashtable)cachedHistories[ instrumentKey ]).Contains( barComponent ) ) )
			{
				Add( instrumentKey , barComponent );
				((Hashtable)cachedHistories[ instrumentKey ])[ barComponent ] =
					DataBase.GetHistory( instrumentKey , barComponent );
			}
			return (History)((Hashtable)cachedHistories[ instrumentKey ])[ barComponent ];
		}

		public static History GetCloseHistory( string instrumentKey )
		{
			if ( ( !cachedHistories.Contains( instrumentKey ) ) ||
				( !((Hashtable)cachedHistories[ instrumentKey ]).Contains( BarComponent.Close ) ) )
			{
				Add( instrumentKey , BarComponent.Close );
				((Hashtable)cachedHistories[ instrumentKey ])[ BarComponent.Close ] =
					DataBase.GetHistory( instrumentKey , BarComponent.Close );
			}
			return (History)((Hashtable)cachedHistories[ instrumentKey ])[
				BarComponent.Close ];
		}

		public static History GetHighHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , BarComponent.High );
		}

		public static History GetLowHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , BarComponent.Low );
		}

		public static double GetMarketValue( string instrumentKey , ExtendedDateTime extendedDateTime )
    {
      //DateTime dateTime = 
      return Convert.ToDouble(
        ( (History) ((Hashtable)
          cachedHistories[ instrumentKey ])[ extendedDateTime.BarComponent ] ).GetByIndex(
          ( (History) ((Hashtable) cachedHistories[ instrumentKey ])[ extendedDateTime.BarComponent ]
            ).IndexOfKeyOrPrevious( extendedDateTime.DateTime ) ) );
    }
	}
}
