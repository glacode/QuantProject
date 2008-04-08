/*
QuantProject - Quantitative Finance Library

PVOChooserFromSavedBackTestLog.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers
{
	/// <summary>
	/// IInSampleChooser for returning PVOPositions
	/// already saved in a BackTestLog saved to disk
	/// </summary>
	public class PVOChooserFromSavedBackTestLog : BasicChooserFromSavedBackTestLog 
	{
				
		public override string Description
		{
			get
			{
				string description = "ChooserFromSavedBackTestLog_PVO";
				return description;
			}
		}
				
		public PVOChooserFromSavedBackTestLog(
			string backTestLogFullPath)
			: base(backTestLogFullPath)
		{
		}
		
		protected override TestingPositions[] getTestingPositionsFromBackTestLog(EndOfDayDateTime lastInSampleDateOfOptimizedTestingPositions)
		{
			TestingPositions[] testingPositions = 
				new TestingPositions[((PVOLogItem)this.backTestLog[0]).BestPVOPositionsInSample.Length];
			for( int i = 0; i<this.backTestLog.Count ; i++ )
			{
				if( this.backTestLog[i].SimulatedCreationTime.DateTime ==
					  lastInSampleDateOfOptimizedTestingPositions.DateTime )
				{
						testingPositions = ((PVOLogItem)this.backTestLog[i]).BestPVOPositionsInSample;
						i = this.backTestLog.Count;
				}
			}
			return testingPositions;
		}
	}
}
