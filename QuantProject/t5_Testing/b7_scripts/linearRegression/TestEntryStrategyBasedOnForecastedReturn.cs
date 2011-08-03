/*
QuantProject - Quantitative Finance Library

TestEntryStrategyBasedOnForecastedReturn.cs
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
using NUnit.Framework;
using NUnit.Mocks;

using QuantProject.ADT.Econometrics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

using QuantTesting.Business.DataProviders;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	internal class FakeLnrRgrssnTstngPstns : LinearRegressionTestingPositions
	{
		private ILinearRegression linearRegression;
		
		public override ILinearRegression LinearRegression
		{
			get { return this.linearRegression; }
		}
		internal FakeLnrRgrssnTstngPstns(
			ILinearRegression linearRegression ,
			WeightedPositions tradingPortfolio ) :
			base( new WeightedPositions[ 0 ] , tradingPortfolio )
		{
			this.linearRegression = linearRegression;
		}
	}
	
	[TestFixture]
	public class TestEntryStrategyBasedOnForecastedReturn
	{
		#region Test_GetPositionsToBeOpened
		private LinearRegressionTestingPositions getMockLinearRegressionTestingPositions(
			double[] linearRegressionCoefficients , WeightedPositions tradingPortfolio )
		{
			DynamicMock dynamicMockLinearRegression =
				new DynamicMock( typeof( ILinearRegression ) );
			dynamicMockLinearRegression.SetReturnValue(
				"get_EstimatedCoefficients" , linearRegressionCoefficients );
			ILinearRegression mockLinearRegression =
				(ILinearRegression)dynamicMockLinearRegression.MockInstance;
			FakeLnrRgrssnTstngPstns mockLinearRegressionTestingPositions =
				new FakeLnrRgrssnTstngPstns(
					mockLinearRegression , tradingPortfolio );
			return mockLinearRegressionTestingPositions;
		}
		[Test]
		public void Test_GetPositionsToBeOpened()
		{
			ReturnInterval outOfSampleReturnIntervalForTrading =
				new ReturnInterval( new DateTime( 2000 , 1  , 5 ) , new DateTime( 2000 , 1 , 6 ) );
			HistoricalMarketValueProvider historicalMarketValueProvider =
				new FakeHistoricalMarketValueProvider();

			DynamicMock dynamicMockReturnIntervalSelectorForSignaling =
				new DynamicMock( typeof( IReturnIntervalSelectorForSignaling ) );
			ReturnInterval outOfSampleReturnIntervalForSignaling =
				new ReturnInterval( new DateTime( 2000 , 1  , 3 ) , new DateTime( 2000 , 1 , 4 ) );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" ,
				outOfSampleReturnIntervalForSignaling ,
				new object[] { outOfSampleReturnIntervalForTrading } );
			IReturnIntervalSelectorForSignaling fakeReturnIntervalSelectorForSignaling =
				(IReturnIntervalSelectorForSignaling)
				dynamicMockReturnIntervalSelectorForSignaling.MockInstance;
			

			LinearRegressionTestingPositions mockLinearRegressionTestingPositions1 =
				this.getMockLinearRegressionTestingPositions(
					new double[] { 1 , 2 , -3 } ,
					new WeightedPositions(
						new double[] { 0.3 , -0.7 } , new string[] { "TA1" , "TB1" } ) );

			LinearRegressionTestingPositions mockLinearRegressionTestingPositions2 =
				this.getMockLinearRegressionTestingPositions(
					new double[] { 0.3 , 0 , -2 } ,
					new WeightedPositions(
						new double[] { 0.4 , -0.6 } , new string[] { "TA2" , "TB2" } ) );

			LinearRegressionTestingPositions mockLinearRegressionTestingPositions3 =
				this.getMockLinearRegressionTestingPositions(
					new double[] { 0.5 , -1 , 3 } ,
					new WeightedPositions(
						new double[] { 0.5 , 0.5 } , new string[] { "TA3" , "TB3" } ) );

			TestingPositions[] mockBestTestingPositionsInSample =
				new LinearRegressionTestingPositions[ 3 ];
			mockBestTestingPositionsInSample[ 0 ] = mockLinearRegressionTestingPositions1;
			mockBestTestingPositionsInSample[ 1 ] = mockLinearRegressionTestingPositions2;
			mockBestTestingPositionsInSample[ 2 ] = mockLinearRegressionTestingPositions3;
			
			DynamicMock dynamicMockLinearRegressionFitnessEvaluator =
				new DynamicMock( typeof( ILinearRegressionFitnessEvaluator ) );
			
			dynamicMockLinearRegressionFitnessEvaluator.ExpectAndReturn(
				"GetIndependentVariablesValues" , new double[] { 1 , 2 } , // forecasted
				// return should be 1+2*1-3*2=-3 (not selected)
				new object[] {
					mockLinearRegressionTestingPositions1 ,
					outOfSampleReturnIntervalForSignaling ,
					historicalMarketValueProvider } );
			dynamicMockLinearRegressionFitnessEvaluator.ExpectAndReturn(
				"GetIndependentVariablesValues" , new double[] { 3 , -0.1 } , // forecasted
				// return should be 0.3+0*3-2*(-0.1)=0.5 (selected!!)
				new object[] {
					mockLinearRegressionTestingPositions2 ,
					outOfSampleReturnIntervalForSignaling ,
					historicalMarketValueProvider } );
			dynamicMockLinearRegressionFitnessEvaluator.ExpectAndReturn(
				"GetIndependentVariablesValues" , new double[] { 2 , 1 } ,	// forecasted
				// return should be 0.5-1*2+3*1=1.5 (not selected, because the previous one is selected)
				new object[] {
					mockLinearRegressionTestingPositions3 ,
					outOfSampleReturnIntervalForSignaling ,
					historicalMarketValueProvider } );
			ILinearRegressionFitnessEvaluator mockLinearRegressionFitnessEvaluator=
				(ILinearRegressionFitnessEvaluator)dynamicMockLinearRegressionFitnessEvaluator.MockInstance;
			
			EntryStrategyBasedOnForecastedReturn longAndShortBasedOnForecastedReturn =
				new EntryStrategyBasedOnForecastedReturn(
					0.3 ,
					mockLinearRegressionFitnessEvaluator ,
					fakeReturnIntervalSelectorForSignaling ,
					historicalMarketValueProvider );
			
			ReturnIntervals fakeOutOfSampleReturnIntervals =
				new ReturnIntervals( outOfSampleReturnIntervalForTrading );
			
			WeightedPositions positionsToBeOpened =
				longAndShortBasedOnForecastedReturn.GetPositionsToBeOpened(
					mockBestTestingPositionsInSample , fakeOutOfSampleReturnIntervals );
			
			Assert.AreEqual( "TA2" , positionsToBeOpened[ 0 ].Ticker );
			Assert.AreEqual( 0.4 , positionsToBeOpened[ 0 ].Weight );
			Assert.AreEqual( "TB2" , positionsToBeOpened[ 1 ].Ticker );
			Assert.AreEqual( -0.6 , positionsToBeOpened[ 1 ].Weight );
		}
		#endregion Test_GetPositionsToBeOpened
	}
}
