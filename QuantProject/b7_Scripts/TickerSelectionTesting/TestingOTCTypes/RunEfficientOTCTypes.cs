/*
QuantProject - Quantitative Finance Library

RunEfficientOTCTypes.cs
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
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
	/// <summary>
	/// Script to test OTC daily, OTC multiday and OTC - CTO
	/// all together with one optimization and 3 accounts (each for
	/// each type of strategy)
	/// </summary>
	[Serializable]
	public class RunEfficientOTCTypes : RunEfficientPortfolio
	{
		protected int numDaysBetweenEachOptimization;
		private Account[] accounts;
		
		public RunEfficientOTCTypes(string tickerGroupID, int numberOfEligibleTickers,
		                            int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                            int generationNumberForGeneticOptimizer,
		                            int populationSizeForGeneticOptimizer, string benchmark,
		                            DateTime startDate, DateTime endDate, double targetReturn,
		                            PortfolioType portfolioType, double maxRunningHours,
		                            int numDaysBetweenEachOptimization):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, targetReturn,
			     portfolioType, maxRunningHours)
		{
			this.ScriptName = "OTCTypes_SR_WithCoeffOnlyMutationPriceSel";
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.accounts = new Account[4];
		}
		
		#region auxiliary overriden methods for Run
		
		
		protected override void run_initializeAccount()
		{
			for(int i = 0; i<this.accounts.Length; i++)
			{
				this.accounts[i] = new Account( this.ScriptName, this.endOfDayTimer ,
				                               new HistoricalDataStreamer( this.endOfDayTimer ,
				                                                                  this.historicalMarketValueProvider ) ,
				                               new HistoricalOrderExecutor( this.endOfDayTimer ,
				                                                                   this.historicalMarketValueProvider ));
			}
			
		}
		
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerOTCTypes(this.tickerGroupID,
			                                                             this.numberOfEligibleTickers,
			                                                             this.numberOfTickersToBeChosen,
			                                                             this.numDaysForOptimizationPeriod,
			                                                             this.generationNumberForGeneticOptimizer,
			                                                             this.populationSizeForGeneticOptimizer,
			                                                             this.benchmark,
			                                                             this.targetReturn,
			                                                             this.portfolioType, this.numDaysBetweenEachOptimization, this.accounts);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			//this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		protected override void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

			
//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayTimerHandler.MarketOpenEventHandler);
//			
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//			
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
//			
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		#endregion
		
		//necessary far calling RunEfficientPortfolio.Run()
		//in classes that inherit from this class
		public override void Run()
		{
			base.Run();
		}
		public override void SaveScriptResults()
		{
			string fileName = DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
				"From"+ this.tickerGroupID+ "_" +
				this.numberOfEligibleTickers +
				"_OptDays" + this.numDaysForOptimizationPeriod + "_Portf" +
				this.numberOfTickersToBeChosen + "_GenNum" +
				this.generationNumberForGeneticOptimizer +
				"_PopSize" + this.populationSizeForGeneticOptimizer +
				Convert.ToString(this.portfolioType);
			string dirNameWhereToSaveAccounts =
				System.Configuration.ConfigurationManager.AppSettings["AccountsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveBestGenomes =
				System.Configuration.ConfigurationManager.AppSettings["GenomesArchive"] +
				"\\" + this.ScriptName + "\\";
			
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveAccounts);
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
			
			for(int i = 0; i<this.accounts.Length; i++)
			{
				ObjectArchiver.Archive(accounts[i],
				                       dirNameWhereToSaveAccounts +
				                       fileName + "#" + i.ToString() + ".qPa");
				ObjectArchiver.Archive(this.accounts[i].Transactions,
				                       dirNameWhereToSaveTransactions +
				                       fileName + "#" + i.ToString() + ".qPt");
			}
			OptimizationOutput optimizationOutput = new OptimizationOutput();
			foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
				optimizationOutput.Add(genomeRepresentation);
			ObjectArchiver.Archive(optimizationOutput,
			                       dirNameWhereToSaveBestGenomes +
			                       fileName + ".bgn");
			this.endOfDayTimer.Stop();
		}
	}
}
