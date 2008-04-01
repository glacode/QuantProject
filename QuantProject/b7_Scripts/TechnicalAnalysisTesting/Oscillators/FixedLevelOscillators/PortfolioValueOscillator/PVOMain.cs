/*
QuantProject - Quantitative Finance Library

PVOMain.cs
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
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Entry point for the PVO strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PVOMain
	{
		private string strategyIdentifier;
		private string fileNameWithoutExt;
		private string dirNameWhereToSaveResults;
				
		public PVOMain()
		{
			this.strategyIdentifier = "PVO";
		}
		
		private void setFileNamesAndDirectory(EndOfDayStrategyBackTester endOfDayStrategyBackTester)
		{
			if(this.fileNameWithoutExt == null)
			{
				this.fileNameWithoutExt = this.strategyIdentifier + "_" +
												DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" +
    										DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
												DateTime.Now.Second.ToString().PadLeft(2,'0');
			  this.dirNameWhereToSaveResults = System.Configuration.ConfigurationSettings.AppSettings["LogArchive"] +
                         								 "\\" + fileNameWithoutExt + "\\";
      
      	if( !Directory.Exists(dirNameWhereToSaveResults) )
    			Directory.CreateDirectory(dirNameWhereToSaveResults);
			}
		}
		
		#region Run
		private MessageManager setMessageManager(
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IEndOfDayStrategy endOfDayStrategy ,
			EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{
			this.setFileNamesAndDirectory(endOfDayStrategyBackTester);
			
      string fullPathFileNameForMessagesLog = dirNameWhereToSaveResults + 
      																				this.fileNameWithoutExt +
      																				"LogMessages.txt";
			MessageManager messageManager =
				new MessageManager( fullPathFileNameForMessagesLog );
			messageManager.Monitor( eligiblesSelector );
			messageManager.Monitor( inSampleChooser );
			//			messageManager.Monitor( endOfDayStrategy );
			messageManager.Monitor( endOfDayStrategyBackTester );
			return messageManager;
		}
			
		//Saves (in silent mode):
		//- a log file where the In Sample Analysis are
		//  stored;
		//- a report;
		//- a txt file with a full description of the
		//  strategy's features
		private void saveScriptResults( EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{
			this.setFileNamesAndDirectory(endOfDayStrategyBackTester);
      string fullPathFileNameForLog = dirNameWhereToSaveResults +
      																this.fileNameWithoutExt + ".qpL";
      string fullPathFileNameForReport = dirNameWhereToSaveResults + 
      																	 this.fileNameWithoutExt + ".qpR";
      string fullPathFileNameForParametersLog = dirNameWhereToSaveResults + 
      																					this.fileNameWithoutExt + "_Parameters.txt";
      ObjectArchiver.Archive(endOfDayStrategyBackTester.Log,
                             fullPathFileNameForLog);
      ObjectArchiver.Archive(endOfDayStrategyBackTester.AccountReport,
                             fullPathFileNameForReport);
      StreamWriter w = File.AppendText(fullPathFileNameForParametersLog);
      w.WriteLine ("\n---\r\n");
      w.WriteLine ( endOfDayStrategyBackTester.Description );
      w.WriteLine ("\n---\r\n");
			w.Flush();
			w.Close();
 		}

		public void Run()
		{
			//general
			DateTime firstDateTime = new DateTime( 2000 , 6 , 1 );
			DateTime lastDateTime = new DateTime( 2001 , 6 , 1 );
			double maxRunningHours = 5;
			Benchmark benchmark = new Benchmark( "^GSPC" );
			// definition for the Fitness Evaluator (for
			// objects that use it)
      IEquityEvaluator equityEvaluator = new SharpeRatio();
     	//cash and portfolio type
			double cashToStart = 30000;
			int numberOfPortfolioPositions = 4;
			      
			// parameters for the in sample Chooser
//			double crossoverRate = 0.85;
//			double mutationRate = 0.02;
//			double elitismRate = 0.001;
//			int populationSizeForGeneticOptimizer = 500;
//			int generationNumberForGeneticOptimizer = 10;
			int numberOfBestTestingPositionsToBeReturnedInSample = 5;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
      int numDaysBetweenEachOptimization = 15;
//			int minLevelForOversoldThreshold = 50;
//      int maxLevelForOversoldThreshold = 100;
//      int minLevelForOverboughtThreshold = 50;
//      int maxLevelForOverboughtThreshold = 100;
      int divisorForThresholdComputation = 10000;
      int numDaysForOscillatingPeriodForChooser = 1; //for genetic optimization
      bool symmetricalThresholds = true;
//      bool overboughtMoreThanOversoldForFixedPortfolio = false;
      double maxAcceptableCloseToCloseDrawdown = 0.03;
      double minimumAcceptableGain = 0.008;
      IDecoderForTestingPositions decoderForTestingPositions
      	= new BasicDecoderForPVOPositions(symmetricalThresholds, divisorForThresholdComputation ,
																				  numDaysForOscillatingPeriodForChooser);
      IFitnessEvaluator	fitnessEvaluator =
				new PVOFitnessEvaluator( equityEvaluator );
      HistoricalQuoteProvider historicalQuoteProviderForBackTester,
															historicalQuoteProviderForInSampleChooser,
															historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester =
				new HistoricalAdjustedQuoteProvider();
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			
//			IInSampleChooser inSampleChooser =
//				new PVOGeneticChooser(numDaysForOscillatingPeriodForChooser ,
//				numberOfPortfolioPositions , numberOfBestTestingPositionsToBeReturnedInSample, 
//				benchmark, decoderForTestingPositions , fitnessEvaluator ,
//				historicalQuoteProviderForInSampleChooser , 			 
//				crossoverRate , mutationRate , elitismRate ,
//				populationSizeForGeneticOptimizer , generationNumberForGeneticOptimizer ,
//				seedForRandomGenerator , minLevelForOversoldThreshold ,
//				maxLevelForOversoldThreshold , minLevelForOverboughtThreshold ,
//				maxLevelForOverboughtThreshold , divisorForThresholdComputation , 
//				symmetricalThresholds , overboughtMoreThanOversoldForFixedPortfolio );
			
			IInSampleChooser inSampleChooser =
				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturnedInSample,
				                              numDaysForOscillatingPeriodForChooser);
			//parameters for eligiblesSelector
			bool temporizedGroup = true;
			double minRawOpenPrice = 25;
			double maxRawOpenPrice = 500;
			int numDaysForAverageOpenRawPrice = 20;
			string tickersGroupId = "SP500";
      int maxNumberOfEligiblesToBeChosen = 100;
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidAlwaysQuoted(
				tickersGroupId , temporizedGroup, maxNumberOfEligiblesToBeChosen,
				numDaysForAverageOpenRawPrice ,
				minRawOpenPrice , maxRawOpenPrice );
			//strategyParameters
			int inSampleDays = 120;
			int numDaysForOscillatingPeriodForOutOfSampleChoosing =
				numDaysForOscillatingPeriodForChooser;
			PVOStrategy strategy =
				new PVOStrategy(eligiblesSelector,
					inSampleChooser, inSampleDays,
					numDaysForOscillatingPeriodForOutOfSampleChoosing,
					numberOfPortfolioPositions , benchmark ,
					numDaysBetweenEachOptimization ,
					historicalQuoteProviderForStrategy ,
					maxAcceptableCloseToCloseDrawdown , minimumAcceptableGain );
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					this.strategyIdentifier , strategy ,
					historicalQuoteProviderForBackTester ,
				  new SimpleAccountProvider(), firstDateTime ,
					lastDateTime , benchmark , cashToStart , maxRunningHours );

			MessageManager messageManager = this.setMessageManager(
				eligiblesSelector , inSampleChooser ,
				strategy , endOfDayStrategyBackTester );
			endOfDayStrategyBackTester.Run();
			this.saveScriptResults(endOfDayStrategyBackTester);
		}

		#endregion Run
		
	}
}
