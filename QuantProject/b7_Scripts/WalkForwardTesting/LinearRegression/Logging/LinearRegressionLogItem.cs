/*
QuantProject - Quantitative Finance Library

LinearRegressionLogItem.cs
Copyright (C) 2010
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

using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Presentation;
using QuantProject.Scripts.General.Logging;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Log item for the linear regression strategy
	/// </summary>
	[Serializable]
	public class LinearRegressionLogItem : AnalyzableLogItem
	{
//		private LinearRegressionTestingPositions[] bestTestingPositionsInSample;
		DateTime firstDateTimeInSample;
		private IIntervalsSelector intervalsSelector;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		
		public LinearRegressionLogItem(
			DateTime now ,
			TestingPositions[] bestTestingPositionsInSample ,
			DateTime firstDateTimeInSample ,
			IIntervalsSelector intervalsSelector ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ) :
			base( now , bestTestingPositionsInSample )
		{
			this.intervalsSelector = intervalsSelector;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.firstDateTimeInSample = firstDateTimeInSample;
			
			foreach( AnalyzerForLinearRegressionTestingPositions analizer in
			        this.analizersForTestingPositions )
				analizer.SetOtherMembers(
					firstDateTimeInSample ,
					intervalsSelector , returnIntervalSelectorForSignaling );
//			this.bestTestingPositionsInSample = bestTestingPositionsInSample;
		}
		
		#region getAnalyzerForTestingPositions
		private void getAnalyzerForTestingPositions_checkParameters(
			TestingPositions testingPositions )
		{
			if ( ! ( testingPositions is LinearRegressionTestingPositions ) )
				throw new Exception(
					"TestingPositions here is expected to be " +
					"LinearRegressionTestingPositions. But the current TestingPositions " +
					"is not a LinearRegressionTestingPositions!" );
		}
		protected override IExecutable getAnalyzerForTestingPositions(
			DateTime now , TestingPositions testingPositionsInSample )
		{
			this.getAnalyzerForTestingPositions_checkParameters( testingPositionsInSample );
			AnalyzerForLinearRegressionTestingPositions
				analyzerForLinearRegressionTestingPositions =
				new AnalyzerForLinearRegressionTestingPositions(
					(LinearRegressionTestingPositions)testingPositionsInSample , now );
			return analyzerForLinearRegressionTestingPositions;
		}
		#endregion getAnalyzerForTestingPositions

//		public override void Run()
//		{
//			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
//				new ExecutablesListViewer(
//					this.analyzersForBestTestingPositionsInSample );
//			executablesListViewer.Show();
//		}
	}
}
