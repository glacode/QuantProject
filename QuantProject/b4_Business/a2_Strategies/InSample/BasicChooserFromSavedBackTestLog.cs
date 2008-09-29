/*
QuantProject - Quantitative Finance Library

BasicChooserFromSavedBackTestLog.cs
Copyright (C) 2008
Marco Milletti

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
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// Abstract basic IInSampleChooser for returning
	/// log items already saved in a BackTestLog saved to disk
	/// </summary>
	public abstract class BasicChooserFromSavedBackTestLog : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		protected int numberOfBestTestingPositionsToBeReturned;
		protected TestingPositions[] bestTestingPositionsInSample;
		protected string backTestLogFullPath;
		protected BackTestLog backTestLog;
		
		public virtual string Description
		{
			get
			{
				string description = "ChooserFromSavedBackTestLog";
				return description;
			}
		}
		
		private void setBackTestLog()
		{
			if( this.backTestLog == null )
			{
				object savedObject = ObjectArchiver.Extract(
					this.backTestLogFullPath);
				if( savedObject is BackTestLog )
					this.backTestLog = (BackTestLog)savedObject;
				else // savedObject is not a BackTestLog 
					throw new Exception("The loaded object is not " +
					                    " a BackTestLog!");
			}
		}
		
		/// <summary>
		/// Abstract BasicChooserFromSavedBackTestLog to be used for
		/// retrieving TestingPositions from a BackTestLog
		/// already saved to disk
		/// </summary>
		public BasicChooserFromSavedBackTestLog(
			string backTestLogFullPath, int numberOfBestTestingPositionsToBeReturned)
		{
			this.backTestLogFullPath = backTestLogFullPath;
			this.numberOfBestTestingPositionsToBeReturned = 
				numberOfBestTestingPositionsToBeReturned;
			this.setBackTestLog();
		}
				
		protected abstract TestingPositions[] getTestingPositionsFromBackTestLog(
			DateTime lastInSampleDateTimeOfOptimizedTestingPositions	);
		
		private void analyzeInSample_fireEvents()
		{
			if(this.NewProgress != null)
				this.NewProgress( this ,
					new NewProgressEventArgs( 1 , 1 ) );
			if(this.NewMessage != null)
				this.NewMessage( this ,
					new NewMessageEventArgs( "AnalyzeInSample is complete" ) );
		}
		
		/// <summary>
		/// Returns the best TestingPositions 
		/// stored in the BackTestLog
		/// </summary>
		/// <param name="eligibleTickers">Also a dummy eligibleTickers (not used)</param>
		/// <param name="returnsManager">Also a dummy returnsManager (not used)</param>
		public object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			this.bestTestingPositionsInSample =
				this.getTestingPositionsFromBackTestLog( 
				     returnsManager.ReturnIntervals.LastDateTime );
			this.analyzeInSample_fireEvents();
			return bestTestingPositionsInSample;
		}
	}
}
