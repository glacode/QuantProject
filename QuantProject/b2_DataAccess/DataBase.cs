/*
QuantProject - Quantitative Finance Library

DataBase.cs
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
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Summary description for DataBase.
	/// </summary>
	/// 
	public class DataBase
	{
    private static OleDbConnection oleDbConnection = ConnectionProvider.OleDbConnection;

		public DataBase()
		{
			//
			
			//
		}

    #region "GetHistories"
    private static string getFieldName( BarComponent barComponent )
    {
      string fieldName = "";
      switch ( barComponent )
      {
        case BarComponent.Open:
          fieldName = "quAdjustedOpen";
          break;
        case BarComponent.High:
          fieldName = "quHigh";
          break;
        case BarComponent.Low:
          fieldName = "quLow";
          break;
        case BarComponent.Close:
          fieldName = "quAdjustedClose";
          break;
        default:
          break;
      }
      return fieldName;
    }
    private static Hashtable getHistories_try( string instrumentKey , Hashtable barComponents , DateTime startDateTime , DateTime endDateTime )
    {
      Hashtable histories = new Hashtable();
      foreach (BarComponent barComponent in barComponents.Keys)
        histories.Add( barComponent , new History() );
      string commandString =
        "select * from quotes where quTicker='" + instrumentKey + "' and " +
        "quDate>=" + SQLBuilder.GetDateConstant( startDateTime ) + " and " +
        "quDate<=" + SQLBuilder.GetDateConstant( endDateTime );
      OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( commandString , oleDbConnection );
      DataSet dataSet = new DataSet();
      oleDbDataAdapter.Fill( dataSet , "history" );
      foreach ( DataRow dataRow in dataSet.Tables[ "history" ].Rows )
        foreach ( BarComponent barComponent in barComponents.Keys )
          ((History) histories[ barComponent ]).Add( (DateTime) dataRow[ "quDate" ] ,
            dataRow[ getFieldName( barComponent ) ] );
      return histories;
    }
    public static Hashtable GetHistories( string instrumentKey , Hashtable barComponents , DateTime startDateTime , DateTime endDateTime )
    {
      Hashtable histories;
      try
      {
        histories = getHistories_try( instrumentKey , barComponents , startDateTime , endDateTime );
      }
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
        histories = null;
      }
      return histories;
    }
    #endregion
  }
}
