/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 3/6/2010
 * Time: 3:34 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using NUnit.Mocks;

using QuantProject.ADT.Optimizing.Decoding;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	[TestFixture]
	/// <summary>
	/// Test for the LinearRegressionSetUpManager class
	/// </summary>
	public class TestLinearRegressionSetUpManager
	{
		private ReturnIntervals getReturnIntervals()
		{
			ReturnIntervals returnIntervals = new ReturnIntervals();
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 1 ) ,new DateTime( 2000 , 1 , 2 ) ) );
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 3 ) ,new DateTime( 2000 , 1 , 4 ) ) );
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 5 ) ,new DateTime( 2000 , 1 , 6 ) ) );
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 7 ) ,new DateTime( 2000 , 1 , 8 ) ) );
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 9 ) ,new DateTime( 2000 , 1 , 10 ) ) );
			returnIntervals.Add(
				new ReturnInterval( new DateTime( 2000 , 1 , 11 ) ,new DateTime( 2000 , 1 , 12 ) ) );
			return returnIntervals;				
		}
		[Test]
		public void TestMethod()
		{
			// TODO: Add your test.
//			DynamicMock mock = new DynamicMock(typeof(ReturnsManager));
//			ReturnsManager mockReturnsManager = (ReturnsManager)mock.MockInstance;
			DynamicMock dynamicMockReturnsCalculatorForTheRegressand =
				new DynamicMock( typeof(IReturnsCalculator) );
			dynamicMockReturnsCalculatorForTheRegressand.SetReturnValue(
				"GetReturns" , new float[]
				{ 0.013F , -0.02F , 0.014F , 0.017F , 0.025F , -0.02F} );
			IReturnsCalculator mockReturnsCalculatorForTheRegressand =
				(IReturnsCalculator)dynamicMockReturnsCalculatorForTheRegressand.MockInstance;
			IReturnsManager mockReturnsManager =
				new ReturnsManager( this.getReturnIntervals() , new HistoricalRawQuoteProvider() );
			float[] floatTest =
				mockReturnsCalculatorForTheRegressand.GetReturns(
					mockReturnsManager , 0 , 1 );
			
//			DynamicMock dynamicMockReturnsManager = new DynamicMock( typeof( IReturnsManager ) );
//			dynamicMockReturnsManager.SetReturnValue( "get_NumberOfReturns" , 6 );
//			mockReturnsManager =
//				(IReturnsManager)dynamicMockReturnsManager.MockInstance;

			DynamicMock dynamicMockReturnsCalculatorForTheFirstRegressor =
				new DynamicMock( typeof(IReturnsCalculator) );
//			dynamicMockReturnsCalculatorForTheFirstRegressor.SetReturnValue(
//				"GetReturns" , new float[]
//				{ 0.015F , -0.07F , 0.034F , -0.002F , 0.011F } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , 0.015F , new object[] { 0 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , -0.07F , new object[] { 1 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , 0.034F , new object[] { 2 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , -0.002F , new object[] { 3 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , 0.015F , new object[] { 4 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheFirstRegressor.ExpectAndReturn(
				"GetReturn" , 0.018F , new object[] { 5 , mockReturnsManager } );
			IReturnsCalculator mockReturnsCalculatorForTheFirstRegressor=
				(IReturnsCalculator)dynamicMockReturnsCalculatorForTheFirstRegressor.MockInstance;
			
			DynamicMock dynamicMockReturnsCalculatorForTheSecondRegressor =
				new DynamicMock( typeof(IReturnsCalculator) );
//			dynamicMockReturnsCalculatorForTheSecondRegressor.SetReturnValue(
//				"GetReturns" , new float[]
//				{ 0.025F , -0.02F , 0.13F , 0.004F , -0.024F , -0.013F } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , 0.025F , new object[] { 0 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , -0.02F , new object[] { 1 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , 0.13F , new object[] { 2 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , 0.004F , new object[] { 3 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , -0.024F , new object[] { 4 , mockReturnsManager } );
			dynamicMockReturnsCalculatorForTheSecondRegressor.ExpectAndReturn(
				"GetReturn" , -0.013F , new object[] { 5 , mockReturnsManager } );
			IReturnsCalculator mockReturnsCalculatorForTheSecondRegressor=
				(IReturnsCalculator)dynamicMockReturnsCalculatorForTheSecondRegressor.MockInstance;
			IReturnsCalculator[] mockReturnsCalculatorForTheRegressors =
				(IReturnsCalculator[])new IReturnsCalculator[ 2 ] {
				mockReturnsCalculatorForTheFirstRegressor ,
				mockReturnsCalculatorForTheSecondRegressor };
			LinearRegressionSetupManager linearRegressionSetupManager =
				new LinearRegressionSetupManager();
			
			ILinearRegressionValues linearRegressionValues =
				linearRegressionSetupManager.SetUpTheLinearRegressionValues(
					mockReturnsCalculatorForTheRegressand ,
					mockReturnsCalculatorForTheRegressors ,
					mockReturnsManager , mockReturnsManager );

			Assert.AreEqual( 6 , linearRegressionValues.Regressand.Length );
			Assert.AreEqual( -0.02F , linearRegressionValues.Regressand[ 1 ] , "-0.02F" );
			
			Assert.AreEqual( 6 , linearRegressionValues.Regressors.GetLength( 0 ) ,
			               "GetLength( 0 )" );
			Assert.AreEqual( 3 , linearRegressionValues.Regressors.GetLength( 1 ),
			               "GetLength( 1 )" );

			Assert.AreEqual( 1 , linearRegressionValues.Regressors[ 2 , 0 ] , "1=...[ 2 , 0 ]" );
			Assert.AreEqual( -0.002F , linearRegressionValues.Regressors[ 3 , 1 ] , "-0.002F" );
			Assert.AreEqual( 0.025F , linearRegressionValues.Regressors[ 0 , 2 ] , "0.025F" );
			
			
			
			
			
			
			
//			mock.ExpectAndReturn( "Decode" , 6 , new int[] { 3 , 5 } );
//			mock.ExpectAndReturn( "Decode" , 17 , new int[] { 11 , 12 } );
//			IDecoder mockDecoder = (IDecoder)mock.MockInstance;
//
//			object decoded;
//			decoded = mockDecoder.Decode( new int[] { 3 , 5 } );
//			decoded = mockDecoder.Decode( new int[] { 11 , 12 } );
			////			decoded = mockDecoder.Decode( new int[] { 11 , 12 } );
			
		}
	}
}
