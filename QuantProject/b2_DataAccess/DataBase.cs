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

    /// <summary>
    /// Returns the field name corresponding to the quote field
    /// </summary>
    /// <param name="quoteField">Discriminates among Open, High, Low and Closure</param>
    /// <returns>Field name corresponding to the quote field</returns>
    private static string getFieldName( QuoteField quoteField )
    {
      string fieldName = "";
      switch ( quoteField )
      {
        case QuoteField.Open:
          fieldName = "quOpen";
          break;
        case QuoteField.High:
          fieldName = "quHigh";
          break;
        case QuoteField.Low:
          fieldName = "quLow";
          break;
				case QuoteField.Close:
					fieldName = "quClose";
					break;
				case QuoteField.AdjustedClose:
					fieldName = "quAdjustedClose";
					break;
				case QuoteField.AdjustedCloseToCloseRatio:
					fieldName = "quAdjustedCloseToCloseRatio";
					break;
				case QuoteField.Volume:
					fieldName = "quVolume";
					break;
				default:
          break;
      }
      return fieldName;
    }

    #region "GetHistory"
    private static History getHistory_try( string instrumentKey , QuoteField quoteField )
    {
      History history = new History();
      string commandString =
        "select * from quotes where quTicker='" + instrumentKey + "'";
      OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( commandString , oleDbConnection );
      DataTable dataTable = new DataTable();
      oleDbDataAdapter.Fill( dataTable );
      history.Import( dataTable , "quDate" , getFieldName( quoteField ) );
      return history;
    }
    /// <summary>
    /// Returns the full history for the instrument and the specified quote field
    /// </summary>
    /// <param name="instrumentKey">Identifier (ticker) for the instrument whose story
    /// has to be returned</param>
    /// <param name="quoteField">Discriminates among Open, High, Low and Closure</param>
    /// <returns>The history for the given instrument and quote field</returns>
    public static History GetHistory( string instrumentKey , QuoteField quoteField )
    {
      History history;
      try
      {
        history = getHistory_try( instrumentKey , quoteField );
      }
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
        history = null;
      }
      return history;
    }
    #endregion

//    #region "GetHistories"
//    private static Single getHistories_try_getValue( DataRow dataRow , DateTime dateTime ,
//      QuoteField quoteField )
//    {
//      Single returnValue;
//      if ( quoteField == QuoteField.cl )
//        returnValue = (Single)dataRow[ getFieldName( quoteField ) ];
//      else
//        returnValue = (Single)dataRow[ getFieldName( quoteField ) ] *
//          (Single)dataRow[ "quAdjustedClose" ] / (Single)dataRow[ "quClose" ];
//      return returnValue;
//    }
//    private static Hashtable getHistories_try( string instrumentKey , Hashtable barComponents , DateTime startDateTime , DateTime endDateTime )
//    {
//      Hashtable histories = new Hashtable();
//      foreach (BarComponent barComponent in barComponents.Keys)
//        histories.Add( barComponent , new History() );
//      string commandString =
//        "select * from quotes where quTicker='" + instrumentKey + "' and " +
//        "quDate>=" + SQLBuilder.GetDateConstant( startDateTime ) + " and " +
//        "quDate<=" + SQLBuilder.GetDateConstant( endDateTime );
//      OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( commandString , oleDbConnection );
//      DataSet dataSet = new DataSet();
//      oleDbDataAdapter.Fill( dataSet , "history" );
//      foreach ( DataRow dataRow in dataSet.Tables[ "history" ].Rows )
//        foreach ( BarComponent barComponent in barComponents.Keys )
//          ((History) histories[ barComponent ]).Add( (DateTime) dataRow[ "quDate" ] ,
//            getHistories_try_getValue( dataRow , (DateTime) dataRow[ "quDate" ] , barComponent ) );
////          ((History) histories[ barComponent ]).Add( (DateTime) dataRow[ "quDate" ] ,
////            dataRow[ getFieldName( barComponent ) ] );
//      return histories;
//    }
//    public static Hashtable GetHistories( string instrumentKey , Hashtable barComponents , DateTime startDateTime , DateTime endDateTime )
//    {
//      Hashtable histories;
//      try
//      {
//        histories = getHistories_try( instrumentKey , barComponents , startDateTime , endDateTime );
//      }
//      catch (Exception ex)
//      {
//        MessageBox.Show( ex.ToString() );
//        histories = null;
//      }
//      return histories;
//    }
//    #endregion
  }
}
