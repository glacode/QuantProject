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
		
		#region writeToDataBaseActually
		private void throwExceptionIfOtherThanBarAlreadyInTheDatabase(
			string ticker , string exchange , DateTime dateTimeForOpenInESTTime , long interval ,
			Exception exception )
		{
			if ( !Bars.ContainsBar(
				ticker , exchange , dateTimeForOpenInESTTime , interval ) )
				// exception was not due to a duplicate key
				throw exception;
		}
		private void writeToDataBaseActually( Bar bar )
		{
			DateTime dateTimeForOpenInESTTime =
				TimeZoneManager.ConvertToEST( bar.DateTimeForOpenInUTCTime );
			try
			{
				Bars.AddBar(
					bar.Ticker , bar.Exchange , dateTimeForOpenInESTTime , bar.Interval ,
					bar.Open , bar.High , bar.Low , bar.Close , bar.Volume );
			}
			catch ( Exception exception )
			{
				this.throwExceptionIfOtherThanBarAlreadyInTheDatabase(
					bar.Ticker , bar.Exchange , dateTimeForOpenInESTTime , bar.Interval ,
					exception );
			}
		}
		#endregion writeToDataBaseActually
		
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
				Thread.Sleep( 15 );
			}
		}
		#endregion writeToDataBase
		
		public void StartWritingBuffersToDatabase()
		{
			this.writeToDataBaseThread = new Thread(
				new ThreadStart( this.writeToDataBase ) );
			this.writeToDataBaseThread.Start();
		}
		
		// uncomment the two following methods to test if writeToDataBaseActually
		// properly handles duplicate keys attempt
//		public static void TestAddBar()
//		{
//			DataBaseWriter dataBaseWriter = new DataBaseWriter(
//				new BarQueue( 1 ) , 1 );
//			dataBaseWriter.TestAddBarForInstance();
//		}
//		public void TestAddBarForInstance()
//		{
//			Bar bar = new Bar(
//				"AAPL" , "Q" , new DateTime( 2009 , 1 , 2 , 15 , 52 , 0 ) , 60 ,
//				1 , 4 , 3 , 2 , 10000 );
//			// the following statement should actually add a bar, if no bar with the same
//			// key is in the database
//			this.writeToDataBaseActually( bar );
//			bar = new Bar(
//				"AAPL" , "Q" , new DateTime( 2009 , 1 , 2 , 15 , 52 , 0 ) , 60 ,
//				1 , 4 , 3 , 2 , 20000 );
//			// the following statement will not add the bar, because a bar with the
//			// same key has just been added above, but NO EXCEPTION will be risen
//			this.writeToDataBaseActually( bar );
//		}
	}
}
