/*
QuantProject - Quantitative Finance Library

BruteForceChooser.cs
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

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing;
using QuantProject.Business.Strategies.Optimizing.BruteForce;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// Abstract brute force in sample chooser to be used for
	/// in sample optimization
	/// </summary>
	public abstract class BruteForceChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;

		protected int numberOfBestTestingPositionsToBeReturned;
		protected IDecoderForTestingPositions decoderForTestingPositions;
		protected IFitnessEvaluator fitnessEvaluator;
		protected IHistoricalQuoteProvider historicalQuoteProvider;

		public string Description
		{
			get
			{
				string description = "BruteForce" +
					"_FitnEval_" + this.fitnessEvaluator.Description +
					"_DecoderForTestingPositions_" + this.decoderForTestingPositions;
				return description;
			}
		}

		public BruteForceChooser(
			int numberOfBestTestingPositionsToBeReturned ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
			this.numberOfBestTestingPositionsToBeReturned =
				numberOfBestTestingPositionsToBeReturned;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator = fitnessEvaluator;
			this.historicalQuoteProvider = historicalQuoteProvider;
		}

		protected abstract IBruteForceOptimizableParametersManager
			getBruteForceOptimizableParametersManager( EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager );
		
		#region AnalyzeInSample
		private void analyzeInSample_checkParameters(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( eligibleTickers.Count <= 0	)
				throw new Exception( "Eligible tickers for driving positions is empty!" );
		}

		#region getBestTestingPositionsInSample

		#region newBruteForceOptimizerProgressEventHandler

		#region sendNewMessage
		private string getProgressMessage(
			NewProgressEventArgs eventArgs )
		{
//			string progressMessage =
//				this.ToString() + " / " +
//				generationNumber.ToString() +
//				" - " +
//				DateTime.Now.ToString();
			string progressMessage =
				eventArgs.CurrentProgress.ToString() + " / " +
				eventArgs.Goal.ToString() +
				" - " +
				DateTime.Now.ToString();
			return progressMessage;
		}
		private void sendNewMessage( NewProgressEventArgs eventArgs )
		{
			string message = this.getProgressMessage( eventArgs );
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		#endregion sendNewMessage

		private void newBruteForceOptimizerProgressEventHandler(
			Object sender , NewProgressEventArgs eventArgs )
		{
			if ( this.NewProgress != null )
				this.NewProgress( sender , eventArgs );
			this.sendNewMessage( eventArgs );
		}
		#endregion newBruteForceOptimizerProgressEventHandler

		private void
			getBestTestingPositionsInSample_getTestingPositionsActually_checkParameter(
			BruteForceOptimizer bruteForceOptimizer )
		{
			if ( bruteForceOptimizer.TopBestParameters.Length <
				this.numberOfBestTestingPositionsToBeReturned )
				throw new Exception( "The brute force optimizer has not examined enough " +
					"candidates!" );
		}
		private TestingPositions[] getBestTestingPositionsInSample_getTestingPositionsActually(
			BruteForceOptimizer bruteForceOptimizer )
		{
			this.getBestTestingPositionsInSample_getTestingPositionsActually_checkParameter(
				bruteForceOptimizer );
			TestingPositions[] bestTestingPositions =
				new TestingPositions[ this.numberOfBestTestingPositionsToBeReturned ];
			for ( int i = 0 ; i < bruteForceOptimizer.TopBestParameters.Length ; i++ )
			{
				BruteForceOptimizableParameters bruteForceOptimizableParameters =
					bruteForceOptimizer.TopBestParameters[ i ];
				bestTestingPositions[ i ] =
					(TestingPositions)bruteForceOptimizableParameters.Meaning;
			}
			return bestTestingPositions;
		}
	
		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			IBruteForceOptimizableParametersManager
				bruteForceOptimizableParametersManager =
				this.getBruteForceOptimizableParametersManager(
				eligibleTickers , returnsManager );
			BruteForceOptimizer bruteForceOptimizer =
				new BruteForceOptimizer( bruteForceOptimizableParametersManager ,
				this.numberOfBestTestingPositionsToBeReturned );
			bruteForceOptimizer.NewProgress +=
				new NewProgressEventHandler(
				this.newBruteForceOptimizerProgressEventHandler );
			bruteForceOptimizer.Run( 10000 ,
				bruteForceOptimizableParametersManager.TotalIterations );
			
			return this.getBestTestingPositionsInSample_getTestingPositionsActually(
				bruteForceOptimizer );
		}
		#endregion getBestTestingPositionsInSample

		public object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			this.analyzeInSample_checkParameters( eligibleTickers ,
			                                     returnsManager );
			TestingPositions[] bestTestingPositionsInSample =
				this.getBestTestingPositionsInSample( eligibleTickers ,
				                                     returnsManager );
			return bestTestingPositionsInSample;
		}
		#endregion AnalyzeInSample
	}
}
