/*
QuantProject - Quantitative Finance Library

PairsTradingChooserFromSavedBackTestLog.cs
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

using QuantProject.Business.Timing;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// IInSampleChooser for returning PairsTradingPositions
	/// already saved in a BackTestLog saved to disk
	/// </summary>
	public class PairsTradingChooserFromSavedBackTestLog :
		BasicChooserFromSavedBackTestLog
	{
		public PairsTradingChooserFromSavedBackTestLog(
			string backTestLogFullPath ,
			int numberOfBestTestingPositionsToBeReturned )
			: base( backTestLogFullPath ,
			       numberOfBestTestingPositionsToBeReturned )
		{
			int maxNumberOfTestingPositionsFromBackTestLogItems =
				((PairsTradingLogItem)this.backTestLog[0]).GetTestingPositions().Length;
		 	if(numberOfBestTestingPositionsToBeReturned > maxNumberOfTestingPositionsFromBackTestLogItems)
				throw new Exception("Number of TestingPositions to be returned " +
		 		                    "is too high for the given BackTestLog");
		}
		
		#region getTestingPositionsFromBackTestLog
		private void getTestingPositionsFromBackTestLog_checkParameters(
			DateTime currentOutOfSampleDateTime )
		{
			if ( currentOutOfSampleDateTime <
				this.backTestLog[ 0 ].SimulatedCreationDateTime )
				throw new Exception(
					"The backTestLog doesn't contain any log item produced before the " +
					"requested DateTime!" );
		}
		private int getIndexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime(
			DateTime lastReturnsManagerDate )
		{
			int currentIndexForLogItem = 1;
			while ( ( currentIndexForLogItem < this.backTestLog.Count ) &&
				( this.backTestLog[ currentIndexForLogItem ].SimulatedCreationDateTime <=
			       	lastReturnsManagerDate ) )
				currentIndexForLogItem++;
			int indexForLastLogItemProducedBeforeLastReturnsManagerDate;
			if ( currentIndexForLogItem >= this.backTestLog.Count )
				// all the items in the log have a SimulatedCreationTime that's
				// less than or equal to lastReturnsManagerDate, thus the last
				// log item produced before lastReturnsManagerDate is the last in the log
				indexForLastLogItemProducedBeforeLastReturnsManagerDate =
					this.backTestLog.Count - 1;
			else
				// currentIndexForLogItem points to the first log item with a
				// SimulatedCreationTime that's greater than lastReturnsManagerDate
				// thus the last log item produced before lastReturnsManagerDate is
				// the previous one
				indexForLastLogItemProducedBeforeLastReturnsManagerDate =
					currentIndexForLogItem - 1;
			return indexForLastLogItemProducedBeforeLastReturnsManagerDate;			
		}
		private TestingPositions[] getTestingPositions(
			int indexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime )
		{
			TestingPositions[] testingPositionsToBeReturned =
				new TestingPositions[ this.numberOfBestTestingPositionsToBeReturned ];
			PairsTradingTestingPositions[] bestTestingPositionsInSample =
				((PairsTradingLogItem)this.backTestLog[
				indexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime ]
				).GetTestingPositions();
			Array.Copy( bestTestingPositionsInSample , 0 ,
				testingPositionsToBeReturned , 0 ,
				testingPositionsToBeReturned.Length ); 
			return testingPositionsToBeReturned;
		}
		private TestingPositions[]
			getTestingPositionsFromBackTestLog_withCurrentOutOfSampleEODDateTime(
			DateTime currentOutOfSampleDateTime )
		{
			this.getTestingPositionsFromBackTestLog_checkParameters(
				currentOutOfSampleDateTime );
			int indexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime =
				this.getIndexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime(
				currentOutOfSampleDateTime );
			TestingPositions[] testingPositions =
				this.getTestingPositions(
				indexForLastLogItemProducedBeforeCurrentOutOfSampleEndOfDayDateTime );
			return testingPositions;
		}

		protected override TestingPositions[]
			getTestingPositionsFromBackTestLog(
				DateTime lastReturnsManagerDate )
		{
			DateTime currentOutOfSampleEndOfDayDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
					lastReturnsManagerDate );
//				new EndOfDayDateTime( lastReturnsManagerDate.DateTime ,
//				EndOfDaySpecificTime.OneHourAfterMarketClose );
			TestingPositions[] testingPositions =
				this.getTestingPositionsFromBackTestLog_withCurrentOutOfSampleEODDateTime(
				currentOutOfSampleEndOfDayDateTime );


//			TestingPositions[] testingPositions =
//				new TestingPositions[ this.numberOfBestTestingPositionsToBeReturned ];
//			for( int i = 0;
//			    i < this.backTestLog.Count;
//			    i++ )
//			{
//				if( this.backTestLog[i].SimulatedCreationTime.DateTime ==
//				   lastInSampleDateOfOptimizedTestingPositions.DateTime )
//				{
//					Array.Copy( ((PVOLogItem)this.backTestLog[i]).BestPVOPositionsInSample ,
//					           0, testingPositions, 0, numberOfBestTestingPositionsToBeReturned );
//					i = this.backTestLog.Count;
//				}
//			}
			return testingPositions;
		}
		#endregion getTestingPositionsFromBackTestLog
	}
}
