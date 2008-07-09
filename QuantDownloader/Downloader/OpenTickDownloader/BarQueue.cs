/*
QuantProject - Quantitative Finance Library

BarQueue.cs
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
using System.Collections.Generic;
using System.Threading;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void
		NewChunkOfBarsToBeWrittenWithASingleSqlCommandEventHandler();
		
	/// <summary>
	/// Keeps a queue of bars, rises events when needed.
	/// </summary>
	public class BarQueue
	{
		public event NewChunkOfBarsToBeWrittenWithASingleSqlCommandEventHandler
			NewChunkOfBarsToBeWrittenWithASingleSqlCommand;
			
//		private Queue<Bar> barQueue;
		private int numberOfBarsToBeWrittenWithASingleSqlCommand;
		private int numberOfBarsEnqueuedSinceLast_NewChunkOfBarsEvent;
		
		private Queue<Bar> queue;
		
		public Queue<Bar> Queue
		{
			get { return this.queue; }
		}
		
		public BarQueue(
			int numberOfBarsToBeWrittenWithASingleSqlCommand ) :
			base()
		{
			this.numberOfBarsToBeWrittenWithASingleSqlCommand =
				numberOfBarsToBeWrittenWithASingleSqlCommand;
			this.numberOfBarsEnqueuedSinceLast_NewChunkOfBarsEvent = 0;
			
			this.queue = new Queue<Bar>();
		}
		
		#region Enqueue
		private void riseNewChunkOfBarsToBeWrittenWithASingleSqlCommand()
		{
			if ( this.NewChunkOfBarsToBeWrittenWithASingleSqlCommand != null )
				this.NewChunkOfBarsToBeWrittenWithASingleSqlCommand();
			this.numberOfBarsEnqueuedSinceLast_NewChunkOfBarsEvent = 0;
		}
		private void enqueue_handleChunkOfBarsToBeWrittenToDatabase()
		{
			this.numberOfBarsEnqueuedSinceLast_NewChunkOfBarsEvent++;
			if ( this.numberOfBarsEnqueuedSinceLast_NewChunkOfBarsEvent
			    == this.numberOfBarsToBeWrittenWithASingleSqlCommand )
				this.riseNewChunkOfBarsToBeWrittenWithASingleSqlCommand();
		}
		public void Enqueue( Bar bar )
		{
			this.Queue.Enqueue( bar );
			this.enqueue_handleChunkOfBarsToBeWrittenToDatabase();
		}
		#endregion Enqueue
	}
}
