/*
QuantProject - Quantitative Finance Library

AnalyzerForLinearRegressionTestingPositions.cs
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
using System.Drawing;
using System.Windows.Forms;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Presentation;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Analyzes a LinearRegressionTestingPositions that has been
	/// logged in sample
	/// </summary>
	[Serializable]
	public class AnalyzerForLinearRegressionTestingPositions : IExecutable
	{
		private LinearRegressionTestingPositions linearRegressionTestingPositions;
		private DateTime firstDateTimeInSample;
		private IIntervalsSelector intervalsSelector;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		private DateTime accountTimerDateTimeWhenThisObjectWasLogged;
		
		/// <summary>
		/// Generation when the TestingPositions object has been created
		/// (if genetically optimized)
		/// </summary>
		public int Generation
		{
			get
			{
				return this.linearRegressionTestingPositions.Generation;
			}
		}
		public double FitnessInSample
		{
			get { return this.linearRegressionTestingPositions.FitnessInSample; }
		}
		
		#region ShortDescription
		private string getShortDescription()
		{
			string shortDescription =
				this.linearRegressionTestingPositions.TradingPortfolio.Description;
			foreach( WeightedPositions weightedPositions in
			        this.linearRegressionTestingPositions.SignalingPortfolios )
				shortDescription += " -- " + weightedPositions.Description;
			return shortDescription;
		}
		public string ShortDescription
		{
			get { return this.getShortDescription(); }
		}
		#endregion ShortDescription

		public AnalyzerForLinearRegressionTestingPositions(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
			DateTime accountTimerDateTimeWhenThisObjectWasLogged
		)
		{
			this.linearRegressionTestingPositions = linearRegressionTestingPositions;
//			this.firstDateTimeInSample = firstDateTimeInSample;
//			this.intervalsSelector = intervalsSelector;
//			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.accountTimerDateTimeWhenThisObjectWasLogged =
				accountTimerDateTimeWhenThisObjectWasLogged;
		}
		
		public void SetOtherMembers(
			DateTime firstDateTimeInSample ,
			IIntervalsSelector intervalsSelector ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling )
		{
			this.firstDateTimeInSample = firstDateTimeInSample;
			this.intervalsSelector = intervalsSelector;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
		}
		
		#region Run
		
		#region getReturnIntervals
		private ReturnIntervals initializeReturnIntervalsWithTheFirstInterval(
			DateTime firstDateTime )
		{
			ReturnInterval firstReturnInterval =
				this.intervalsSelector.GetFirstInterval( firstDateTime );
			ReturnIntervals returnIntervals = new ReturnIntervals( firstReturnInterval );
			return returnIntervals;
		}
		private DateTime getFirstDateTime()
		{
			DateTime firstDateTime = this.accountTimerDateTimeWhenThisObjectWasLogged.AddDays( - 180 );
			return firstDateTime;
		}
		private DateTime getLastDateTime()
		{
			DateTime lastDateTime = this.accountTimerDateTimeWhenThisObjectWasLogged;
			return lastDateTime;
		}
		private ReturnInterval getNextInterval( ReturnIntervals returnIntervals )
		{
			ReturnInterval returnIntervalForTrading = this.intervalsSelector.GetNextInterval(
				returnIntervals );
//			ReturnInterval returnIntervalForSignaling =
//				this.returnIntervalSelectorForSignaling.GetReturnIntervalUsedForSignaling(
//					returnIntervalForTrading );
//			return returnIntervalForSignaling;
			return returnIntervalForTrading;
		}
		private ReturnIntervals getReturnIntervals(
			DateTime firstDateTime , DateTime lastDateTime )
		{
			ReturnIntervals returnIntervals =
				this.initializeReturnIntervalsWithTheFirstInterval( firstDateTime );
			while ( returnIntervals.LastInterval.End < lastDateTime )
			{
				ReturnInterval nextInterval = this.getNextInterval( returnIntervals );
				returnIntervals.Add( nextInterval );
			}
			return returnIntervals;
		}
		private ReturnIntervals getReturnIntervals()
		{
//			DateTime firstDateTime = this.getFirstDateTime();
			DateTime firstDateTime = this.firstDateTimeInSample;
			DateTime lastDateTime = this.getLastDateTime();
			ReturnIntervals returnIntervals = this.getReturnIntervals(
				firstDateTime , lastDateTime );
			return returnIntervals;
		}
		#endregion getReturnIntervals
		
		private History getVirtualValues(
			ReturnIntervals returnIntervals , IVirtualReturnComputer virtualReturnComputer )
		{
			History virtualValues = new History();
			int currentIntervalIndex = 0;
			double currentValue = 30000;
			while ( currentIntervalIndex < returnIntervals.Count )
			{
				ReturnInterval currentInterval = returnIntervals[ currentIntervalIndex ];
				if ( !virtualValues.ContainsKey( currentInterval.Begin ) )
					virtualValues.Add( currentInterval.Begin , currentValue );
				try
				{
					double currentReturn = virtualReturnComputer.ComputeReturn( currentInterval );
					currentValue = currentValue + currentValue * currentReturn;
					virtualValues.Add( currentInterval.End , currentValue );
				}
				catch( TickerNotExchangedException )
				{
				}
				currentIntervalIndex++;
			}
			return virtualValues;
		}
		
		#region getVirtualValues
		private void getVirtualValues(
			ReturnIntervals returnIntervals ,
			out History predictedVirtualValues , out History tradingPortfolioVirtualValues )
		{
			IHistoricalMarketValueProvider historicalMarketValueProvider =
				new HistoricalAdjustedQuoteProvider();
			
			ReturnPredictor returnPredictor = new ReturnPredictor(
				this.linearRegressionTestingPositions , this.returnIntervalSelectorForSignaling ,
				historicalMarketValueProvider );
			predictedVirtualValues = this.getVirtualValues(
				returnIntervals , returnPredictor );
			
			PortfolioReturnComputer tradingPortfolioReturnComputer =
				new PortfolioReturnComputer(
					this.linearRegressionTestingPositions.TradingPortfolio ,
					historicalMarketValueProvider );
			tradingPortfolioVirtualValues = this.getVirtualValues(
				returnIntervals , tradingPortfolioReturnComputer );
		}
		#endregion getVirtualValues
		
		#region showHistoriesPlots
		private HistoriesViewer showHistoriesPlots(
			History predictedVirtualValues , History tradingPortfolioVirtualValues )
		{
			HistoriesViewer historiesViewer =
				new HistoriesViewer( "Linear Regression Log Analyzer" );
			historiesViewer.StartPosition = FormStartPosition.Manual;
			historiesViewer.Location = new Point( 200 , 200 );
			historiesViewer.Add( predictedVirtualValues , Color.Red );
			historiesViewer.Add( tradingPortfolioVirtualValues , Color.Green );
			historiesViewer.Show();
			return historiesViewer;
		}
		#endregion showHistoriesPlots
		
		#region showCorrelation
		private double[] getArrayFromHistory( History history )
		{
			double[] arrayValues = new double[ history.Count ];
			for( int i = 0 ; i < arrayValues.Length ; i++ )
				arrayValues[ i ] = (double)history.GetValueList()[ i ];
			return arrayValues;
		}

		private void showCorrelation(
			History predictedVirtualValues , History tradingPortfolioVirtualValues )
		{
			double correlationValue = BasicFunctions.PearsonCorrelationCoefficient(
				this.getArrayFromHistory( predictedVirtualValues ) ,
				this.getArrayFromHistory( tradingPortfolioVirtualValues ) );
			MessageBox.Show( "Correlation value: " + correlationValue );
		}
		#endregion showCorrelation
		
		
		public void Run()
		{
			// an idea would be to show some t-statistics for the
			// coefficients and then show which are those who seem to
			// be more significant
			ReturnIntervals returnIntervals = this.getReturnIntervals();
			
			History predictedVirtualValues;
			History tradingPortfolioVirtualValues;
			this.getVirtualValues(
				returnIntervals , out predictedVirtualValues , out tradingPortfolioVirtualValues );
			
			this.showHistoriesPlots( predictedVirtualValues , tradingPortfolioVirtualValues );
			
			this.showCorrelation( predictedVirtualValues , tradingPortfolioVirtualValues );
		}
		#endregion Run
	}
}
