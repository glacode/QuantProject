/*
QuantProject - Quantitative Finance Library

AnalyzableLogItem.cs
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
using System.Collections.Generic;

using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Presentation;

namespace QuantProject.Scripts.General.Logging
{
	/// <summary>
	/// A log item that can run a script to analyze the best TestingPositions
	/// logged in
	/// </summary>
	[Serializable]
	public abstract class AnalyzableLogItem : LogItem
	{
		private List<IExecutable> analizersForTestingPositions;
		protected TestingPositions[] bestTestingPositionsInSample;
		
		public AnalyzableLogItem(
			DateTime now ,
			TestingPositions[] bestTestingPositionsInSample ) :
			base( now )
		{
			this.bestTestingPositionsInSample = bestTestingPositionsInSample;
			this.setAnalyzersForTestingPositions(
				now , bestTestingPositionsInSample );
		}
		
		#region setAnalyzersForTestingPositions
//		private void setTestersForPairstTradingTestingPositions_checkParameters(
//			TestingPositions testingPositions )
//		{
//			if ( ! ( testingPositions is PairsTradingTestingPositions ) )
//				throw new Exception(
//					"TestingPositions are all expected to be " +
//					"PairsTradingTestingPositions. The current TestingPositions " +
//					"is not a PairsTradingTestingPositions instead!" );
//		}
		
		protected abstract IExecutable getAnalyzerForTestingPositions(
			DateTime now , TestingPositions testingPositionsInSample );
		
		private void setAnalyzerForTestingPositions(
			DateTime now ,
			TestingPositions testingPositionsInSample )
		{
//			this.setTestersForPairstTradingTestingPositions_checkParameters(
//				testingPositions );
			IExecutable analyzerForTestingPositions = this.getAnalyzerForTestingPositions(
				now , testingPositionsInSample );
			this.analizersForTestingPositions.Add( analyzerForTestingPositions );
//			this.testersForBestTestingPositionsInSample[ currentIndex ] =
//				new TesterForPairsTradingTestingPositions(
//					testingPositions ,
//					this.numberOfInSampleDays ,
//					now );
		}
		private void setAnalyzersForTestingPositions(
			DateTime now ,
			TestingPositions[] bestTestingPositionsInSample )
		{
			this.analizersForTestingPositions = new List<IExecutable>();
			foreach( TestingPositions testingPositions in bestTestingPositionsInSample )
				this.setAnalyzerForTestingPositions( now , testingPositions );
//			for ( int i = 0 ; i < bestTestingPositionsInSample.Length ; i++ )
//				this.setAnalizersForTestingPositions(
//					i ,
//					bestTestingPositionsInSample[ i ] ,
//					now );
		}
		#endregion setAnalyzersForTestingPositions

		public override void Run()
		{
			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
				new ExecutablesListViewer(
					this.analizersForTestingPositions );
			executablesListViewer.Show();
		}
	}
}
