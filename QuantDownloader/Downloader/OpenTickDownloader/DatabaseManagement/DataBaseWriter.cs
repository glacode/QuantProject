/*
QuantProject - Quantitative Finance Library

DataBaseWriter.cs
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
using System.Threading;

using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void DatabaseUpdatedEventHandler(
		object sender , DatabaseUpdatedEventArgs e );

	/// <summary>
	/// Writes downloaded bars into the database
	/// </summary>
	public class DataBaseWriter
	{
		public event DatabaseUpdatedEventHandler DatabaseUpdated;
		private BarQueue barQueue;
		private int maxNumberOfBarsToBeWrittenWithASingleSqlCommand;
		
		private Thread writeToDataBaseThread;
		private bool areAllBarsWrittenToDatabase;
		
		public DataBaseWriter(
			BarQueue barQueue ,
			int maxNumberOfBarsToBeWrittenWithASingleSqlCommand )
		{
			this.barQueue = barQueue;
			this.maxNumberOfBarsToBeWrittenWithASingleSqlCommand =
				maxNumberOfBarsToBeWrittenWithASingleSqlCommand;
			
			this.areAllBarsWrittenToDatabase = false;
		}
		
		#region writeToDataBase
		
		#region writeToDataBaseIfEnoughBars
		private bool isThereEnoughBarsInTheQueue()
		{
			bool isThereEnoughBars;
			lock( this.barQueue )
			{
				isThereEnoughBars =
					( this.barQueue.Queue.Count >=
					 this.maxNumberOfBarsToBeWrittenWithASingleSqlCommand );
			}
			return isThereEnoughBars;
		}
		
		#region writeToDataBaseActually
		
		private Bar dequeue()
		{
			Bar bar;
			lock( this.barQueue )
			{
				bar = this.barQueue.Queue.Dequeue();
			}
			return bar;
		}
		
//		#region getSqlCommand
//		
//		#region getSqlCommand_getValues
//		private string formatDoubleForSql( double value )
//		{
//			string formattedValue =
//				value.ToString().Replace( ',' , '.' );
//			return formattedValue;
//		}
//		private string getSqlCommand_getValues( Bar bar )
//		{
//			DateTime utcDateTimeForOpen =
//				TimeZoneManager.ConvertToEST( bar.DateTimeForOpenInUTCTime );
//			string values =
//				"'" + bar.Ticker + "' , " +
//				"'" + bar.Exchange + "' , " +
//				DataBaseWriter.GetDateConstant( utcDateTimeForOpen ) + " , " +
//				bar.Interval + " , " +
//				formatDoubleForSql( bar.Open ) + " , " +
//				formatDoubleForSql( bar.High ) + " , " +
//				formatDoubleForSql( bar.Low ) + " , " +
//				formatDoubleForSql( bar.Close ) + " , " +
//				bar.Volume;
//			return values;
//		}
//		#endregion getSqlCommand_getValues
//		
//		private string getSqlCommand( Bar bar )
//		{
//			string sqlCommand =
//				"INSERT INTO bars " +
//				"( baTicker, baExchange, baDateTimeForOpen, baInterval, baOpen, baHigh, baLow, baClose, baVolume ) " +
//				"SELECT " + this.getSqlCommand_getValues( bar ) + ";";
////				"SELECT 'MSFT' , 'Q' , #12/13/2004 15:16:17# , 60 , 30.2 , 30.5 , 29.9 , 30.3 , 100000 ;";
//			return sqlCommand;
//		}
//		#endregion getSqlCommand
		
		private void writeToDataBaseActually( Bar bar )
		{
			DateTime dateTimeForOpenInESTTime =
				TimeZoneManager.ConvertToEST( bar.DateTimeForOpenInUTCTime );
			Bars.AddBar(
				bar.Ticker , bar.Exchange , dateTimeForOpenInESTTime , bar.Interval ,
				bar.Open , bar.High , bar.Low , bar.Close , bar.Volume );
//			string sqlCommand =
//				this.getSqlCommand( bar );
//			SqlExecutor.ExecuteNonQuery( sqlCommand );
		}
		private void riseDatabaseUpdatedEvent( Bar bar )
		{
			if ( this.DatabaseUpdated != null )
			{
				DatabaseUpdatedEventArgs eventArgs =
					new DatabaseUpdatedEventArgs(
						bar.Ticker ,
						TimeZoneManager.ConvertToEST( bar.DateTimeForOpenInUTCTime ) );
				this.DatabaseUpdated( this , eventArgs );
			}
		}
		private void writeToDataBaseActually()
		{
			Bar bar = this.dequeue();
			this.writeToDataBaseActually( bar );
			this.riseDatabaseUpdatedEvent( bar );
		}
		#endregion writeToDataBaseActually
		
		private void writeToDataBaseIfEnoughBars()
		{
			if ( this.isThereEnoughBarsInTheQueue() )
				this.writeToDataBaseActually();
		}
		#endregion writeToDataBaseIfEnoughBars		
			
		private void writeToDataBase()
		{
			while ( !this.areAllBarsWrittenToDatabase )
			{
				this.writeToDataBaseIfEnoughBars();
				Thread.Sleep( 50 );
			}			
		}
		#endregion writeToDataBase
		
		public void StartWritingBuffersToDatabase()
		{
			this.writeToDataBaseThread = new Thread(
				new ThreadStart( this.writeToDataBase ) );
			this.writeToDataBaseThread.Start();
		}
		
//		/// <summary>
//		/// Builds a date to be used in a Sql query
//		/// </summary>
//		/// <param name="dateTime"></param>
//		/// <returns></returns>
//		public static string GetDateConstant( DateTime dateTime )
//		{
//			string dateConstant =
//				"#" +
//				dateTime.Month + "/" +
//				dateTime.Day + "/" +
//				dateTime.Year + " " +
//				dateTime.Hour + ":" +
//				dateTime.Minute + ":" +
//				dateTime.Second +
//				"#";
//			return dateConstant;
//		}
	}
}
