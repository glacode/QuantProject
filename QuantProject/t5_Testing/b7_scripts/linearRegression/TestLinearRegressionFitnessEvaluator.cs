/*
QuantProject - Quantitative Finance Library

TestLinearRegressionFitnessEvaluator.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

using QuantTesting.Business.DataProviders;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	[TestFixture]
	/// <summary>
	/// Test for the LinearRegressionFitnessEvaluator class
	/// </summary>
	public class TestLinearRegressionFitnessEvaluator
	{
		#region Test_GetIndependentVariablesValues
		
		private LinearRegressionTestingPositions getLinearRegressionTestingPositions()
		{
			WeightedPositions[] signalingPortfolios = new WeightedPositions[ 2 ];
			signalingPortfolios[ 0 ] = new WeightedPositions(
				new double[] { 0.5 , 0.5 } , new string[] { "SA1" , "SB1" } );
			signalingPortfolios[ 1 ] = new WeightedPositions(
				new double[] { 0.75 , 0.25 } , new string[] { "SA2" , "SB2" } );
			LinearRegressionTestingPositions linearRegressionTestingPositions =
				new LinearRegressionTestingPositions(
					signalingPortfolios , new WeightedPositions(
						new double[] { 0.5 , 0.5 } , new string[] { "NOTUSEDA" , "NOTUSEDB" } ) );
			return linearRegressionTestingPositions;
		}
		
		private LinearRegressionTestingPositions getLinearRegressionTestingPositionsToTestNull()
		{
			WeightedPositions[] signalingPortfolios = new WeightedPositions[ 2 ];
			signalingPortfolios[ 0 ] = new WeightedPositions(
				new double[] { 0.5 , 0.5 } , new string[] { "SA1" , "SB1" } );
			signalingPortfolios[ 1 ] = new WeightedPositions(
				new double[] { 0.75 , 0.25 } , new string[] { "SA3" , "SB2" } );  // SA3 lacks a quote
			LinearRegressionTestingPositions linearRegressionTestingPositions =
				new LinearRegressionTestingPositions(
					signalingPortfolios , new WeightedPositions(
						new double[] { 0.5 , 0.5 } , new string[] { "NOTUSEDA" , "NOTUSEDB" } ) );
			return linearRegressionTestingPositions;
		}
		
		private FakeHistoricalMarketValueProvider getFakeHistoricalMarketValueProvider(
			ReturnInterval returnInterval )
		{
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProviderValuesBased =
				new FakeHistoricalMarketValueProvider();
			
			// "SA1" return is 2%
			fakeHistoricalMarketValueProviderValuesBased.Add( "SA1" , returnInterval.Begin , 1000 );
			fakeHistoricalMarketValueProviderValuesBased.Add( "SA1" , returnInterval.End , 1020 );
			
			// "SB1" return is -1%
			fakeHistoricalMarketValueProviderValuesBased.Add( "SB1" , returnInterval.Begin , 100 );
			fakeHistoricalMarketValueProviderValuesBased.Add( "SB1" , returnInterval.End , 99 );
			
			// "SA2" return is -2.4%
			fakeHistoricalMarketValueProviderValuesBased.Add( "SA2" , returnInterval.Begin , 1000 );
			fakeHistoricalMarketValueProviderValuesBased.Add( "SA2" , returnInterval.End , 976 );

			// "SA3" lacks a quote
//			fakeHistoricalMarketValueProviderValuesBased.Add( "SA3" , returnInterval.Begin , 1000 );
			fakeHistoricalMarketValueProviderValuesBased.Add( "SA3" , returnInterval.End , 976 );

			// "SB2" return is .4%
			fakeHistoricalMarketValueProviderValuesBased.Add( "SB2" , returnInterval.Begin , 50 );
			fakeHistoricalMarketValueProviderValuesBased.Add( "SB2" , returnInterval.End , 50.2 );
			
			return fakeHistoricalMarketValueProviderValuesBased;
		}
		
		[Test]
		public void Test_GetIndependentVariablesValues()
		{
			LinearRegressionTestingPositions linearRegressionTestingPositions =
				this.getLinearRegressionTestingPositions();
			ReturnInterval returnInterval =
				new ReturnInterval( new DateTime( 2000 , 1 , 1 ) , new DateTime( 2000 , 1 , 2 ) );
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider =
				this.getFakeHistoricalMarketValueProvider( returnInterval );
			
			DynamicMock dynamicMockLinearRegressionSetupManager =
				new DynamicMock( typeof( ILinearRegressionSetupManager ) );
			ILinearRegressionSetupManager fakeLinearRegressionSetupManager =
				(ILinearRegressionSetupManager)dynamicMockLinearRegressionSetupManager.MockInstance;
			
			LinearRegressionFitnessEvaluator linearRegressionFitnessEvaluator =
				new LinearRegressionFitnessEvaluator( fakeLinearRegressionSetupManager );
			
			double[] independentVariableValues =
				linearRegressionFitnessEvaluator.GetIndependentVariablesValues(
					linearRegressionTestingPositions , returnInterval ,
					fakeHistoricalMarketValueProvider );
			Assert.AreEqual( 0 , independentVariableValues[ 0 ] );
			Assert.AreEqual( 0.005 , independentVariableValues[ 1 ] , 0.0000001 );
			Assert.AreEqual( -0.017 , independentVariableValues[ 2 ] , 0.0000001 );
			
			LinearRegressionTestingPositions linearRegressionTestingPositionsToTestNull =
				this.getLinearRegressionTestingPositionsToTestNull();
			
			double[] independentVariableValuesToTestNull =
				linearRegressionFitnessEvaluator.GetIndependentVariablesValues(
					linearRegressionTestingPositionsToTestNull , returnInterval ,
					fakeHistoricalMarketValueProvider );
			Assert.AreEqual( null , independentVariableValuesToTestNull );
		}
		#endregion Test_GetIndependentVariablesValues
	}
}
