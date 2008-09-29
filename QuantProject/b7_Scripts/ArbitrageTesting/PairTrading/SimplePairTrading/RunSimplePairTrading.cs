/*
QuantProject - Quantitative Finance Library

RunSimplePairTrading.cs
Copyright (C) 2003
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

using QuantProject.ADT.FileManaging;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Data.DataProviders;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample;


namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
	
	/// <summary>
	/// Script to test the pair trading strategy on two single tickers
	/// </summary>
	[Serializable]
	public class RunSimplePairTrading : RunPairTrading
	{
		private EndOfDayTimerHandlerSimplePT endOfDayTimerHandler;
		public RunSimplePairTrading(string tickerGroupID, int numberOfEligibleTickers,
		                            int numDaysForOptimizationPeriod,
		                            int generationNumberForGeneticOptimizer,
		                            int populationSizeForGeneticOptimizer, string benchmark,
		                            DateTime startDate, DateTime endDate,
		                            double maxNumOfStdDevForNormalGap, int minNumOfDaysForGapComputation,
		                            int maxNumOfDaysForGapComputation, int numDaysBetweenEachOptimization,
		                            double maxRunningHours):
			base(tickerGroupID, numberOfEligibleTickers,
			     numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate,
			     minNumOfDaysForGapComputation, maxNumOfDaysForGapComputation,
			     maxNumOfStdDevForNormalGap, numDaysBetweenEachOptimization,
			     maxRunningHours)
		{

		}
		
		#region auxiliary overriden methods for Run
		
		
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerSimplePT(this.tickerGroupID,
			                                                             this.numberOfEligibleTickers,
			                                                             this.numDaysForOptimizationPeriod,
			                                                             this.generationNumberForGeneticOptimizer,
			                                                             this.populationSizeForGeneticOptimizer,
			                                                             this.benchmark, this.startDateTime, this.endDateTime,
			                                                             this.maxNumOfStdDevForNormalGap, this.minNumOfDaysForGapComputation,
			                                                             this.maxNumOfDaysForGapComputation,
			                                                             this.numDaysBetweenEachOptimization, this.maxRunningHours, this.account);
		}
		
		
		protected override void run_addEventHandlers()
		{
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

//			this.timer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayTimerHandler.MarketOpenEventHandler);
//			
//			this.timer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//			
//			this.timer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
//			
//			this.timer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		#endregion
		
		//necessary far calling RunPairTrading.Run()
		//in classes that inherit from this class
		public override void Run()
		{
			base.Run();
		}
		public override void SaveScriptResults()
		{
			string fileName = "From"+this.numberOfEligibleTickers +
				"OptDays" + this.numDaysForOptimizationPeriod + "GenNum" +
				this.generationNumberForGeneticOptimizer +
				"PopSize" + this.populationSizeForGeneticOptimizer;
			string dirNameWhereToSaveAccounts =
				System.Configuration.ConfigurationManager.AppSettings["AccountsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";
			//      string dirNameWhereToSaveBestGenomes = System.Configuration.ConfigurationSettings.AppSettings["GenomesArchive"] +
			//        "\\" + this.ScriptName + "\\";
			
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveAccounts);
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
			//      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
			
			ObjectArchiver.Archive(this.account,
			                       dirNameWhereToSaveAccounts +
			                       fileName + ".qPa");
			ObjectArchiver.Archive(this.account.Transactions,
			                       dirNameWhereToSaveTransactions +
			                       fileName + ".qPt");
			
			//      OptimizationOutput optimizationOutput = new OptimizationOutput();
			//      foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
			//        optimizationOutput.Add(genomeRepresentation);
			//      ObjectArchiver.Archive(optimizationOutput,
			//                              dirNameWhereToSaveBestGenomes +
			//                              fileName + ".bgn");
			this.timer.Stop();
			new OutputDisplayer(this.startDateTime, this.endDateTime,
			                    this.endOfDayTimerHandler.OptimizationOutput).Show();
			
		}
	}
}
