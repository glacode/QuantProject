/*
QuantProject - Quantitative Finance Library

WalkForwardOpenToCloseDailyStrategy.cs
Copyright (C) 2003
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
using System.Collections;
using System.Data;

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Open to close daily strategy, for walk forward testing
	/// </summary>
	[Serializable]
	public class WalkForwardOpenToCloseDailyStrategy : EndOfDayStrategy
	{
//		private Account account;
		private string tickerGroupID;
		private int numDaysForInSampleOptimization;
		private int numberOfEligibleTickers;
		private int numberOfTickersToBeChosen;
		private string benchmark;
		private double targetReturn;
		private PortfolioType portfolioType;
		private int populationSizeForGeneticOptimizer;
		private int generationNumberForGeneticOptimizer;

		private string[] signedTickersFromLastOptimization;
		private GeneticOptimizer geneticOptimizer;

		private OptimizationOutput optimizationOutput;
		
//		public Account Account
//		{
//			get { return this.account; }
//			set { this.account = value; }
//		}
		
		/// best genomes, one for each optimization process
		public OptimizationOutput OptimizationOutput
		{
			get { return this.optimizationOutput; }
		}

		public WalkForwardOpenToCloseDailyStrategy( Account account ,
		                                           string tickerGroupID , int numDaysForInSampleOptimization ,
		                                           int numberOfEligibleTickers ,
		                                           int numberOfTickersToBeChosen , string benchmark ,
		                                           double targetReturn , PortfolioType portfolioType ,
		                                           int populationSizeForGeneticOptimizer ,
		                                           int generationNumberForGeneticOptimizer )
		{
			this.account = account;
			this.tickerGroupID = tickerGroupID;
			this.numDaysForInSampleOptimization = numDaysForInSampleOptimization;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.benchmark = benchmark;
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
		}
		private long marketOpenEventHandler_addOrder_getQuantity(
			string ticker )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( ticker );
			double maxPositionValueForThisTicker =
				accountValue/this.signedTickersFromLastOptimization.Length;
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void marketOpenEventHandler_addOrder( string signedTicker )
		{
			OrderType orderType = GenomeRepresentation.GetOrderType( signedTicker );
			string ticker = GenomeRepresentation.GetTicker( signedTicker );
			long quantity = marketOpenEventHandler_addOrder_getQuantity( ticker );
			Order order = new Order( orderType , new Instrument( ticker ) ,
			                        quantity );
			this.account.AddOrder( order );
		}
		private void marketOpenEventHandler_addOrders()
		{
			if ( this.signedTickersFromLastOptimization != null )
				foreach ( string signedTicker in this.signedTickersFromLastOptimization )
				marketOpenEventHandler_addOrder( signedTicker );
		}
		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( ( this.account.CashAmount == 0 ) &&
			    ( this.account.Transactions.Count == 0 ) )
				// cash has not been added yet
				this.account.AddCash( 30000 );
			marketOpenEventHandler_addOrders();
		}
		private void marketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( Position position in this.account.Portfolio.Positions )
				tickers.Add( position.Instrument.Key );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		protected override void marketCloseEventHandler( Object sender ,
		                                                     DateTime dateTime)
		{
			this.marketCloseEventHandler_closePositions();
		}

		protected DataTable getSetOfTickersToBeOptimized(
			DateTime optimizationFirstDate , DateTime optimizationLastDate )
		{
			SelectorByLiquidity mostLiquid = new SelectorByLiquidity(
				this.tickerGroupID , false ,
				optimizationFirstDate , optimizationLastDate ,
				this.numberOfEligibleTickers );
			
			DataTable eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
//			SelectorByGroup selectorByGroup =
//				new SelectorByGroup( "SP500" , optimizationLastDate );
//			DataTable eligibleTickers = selectorByGroup.GetTableOfSelectedTickers();
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible =
				new SelectorByQuotationAtEachMarketDay( eligibleTickers,
				                                       false , optimizationFirstDate ,
				                                       optimizationLastDate ,
				                                       this.numberOfEligibleTickers, this.benchmark);
			return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
		}
		private void newGenerationEventHandler( object sender ,
		                                       NewGenerationEventArgs newGenerationEventArgs )
		{
			Console.WriteLine(
				newGenerationEventArgs.GenerationCounter.ToString() + " / " +
				newGenerationEventArgs.GenerationNumber.ToString() +
				" - " + DateTime.Now.ToString() );
		}
		private void addGenomeToBestGenomes( Genome genome,
		                                    DateTime firstOptimizationDate ,
		                                    DateTime lastOptimizationDate )
		{
			if( this.optimizationOutput == null)
				this.optimizationOutput = new OptimizationOutput();
			
			this.optimizationOutput.Add( new GenomeRepresentation( genome ,
			                                                      firstOptimizationDate ,
			                                                      lastOptimizationDate ) );
		}
		private void oneHourAfterMarketCloseEventHandler_set_signedTickersFromLastOptimization(
			DateTime currentDate )
		{
//			this.bestGenomes = new ArrayList();
			//			DataTable setOfTickersToBeOptimized =
			//				this.getSetOfTickersToBeOptimized_quickly();
			DateTime optimizationFirstDate =
				currentDate.AddDays( -this.numDaysForInSampleOptimization + 1 );
			DateTime optimizationLastDate =	currentDate;
			DataTable setOfTickersToBeOptimized =
				this.getSetOfTickersToBeOptimized( optimizationFirstDate ,
				                                  optimizationLastDate );
			Console.WriteLine( "Number of tickers to be optimized: " +
			                  setOfTickersToBeOptimized.Rows.Count.ToString() );
			GenomeManagerForEfficientOTCPortfolio genManEfficientOTCPortfolio =
				new GenomeManagerForEfficientOTCPortfolio(
					setOfTickersToBeOptimized ,
					currentDate.AddDays( -this.numDaysForInSampleOptimization + 1 ) ,
					currentDate ,
					this.numberOfTickersToBeChosen ,
					this.targetReturn ,
					this.portfolioType,
					this.benchmark);
			
			this.geneticOptimizer = new GeneticOptimizer(genManEfficientOTCPortfolio,
			                                             this.populationSizeForGeneticOptimizer,
			                                             this.generationNumberForGeneticOptimizer);
			this.geneticOptimizer.NewGeneration += new NewGenerationEventHandler(
				this.newGenerationEventHandler );

			//			Thread thread = new Thread(new ThreadStart(this.createOptimizedGenomes_go));
			//			thread.IsBackground = true;
			//			thread.Start();

			this.geneticOptimizer.Run(false);

			this.signedTickersFromLastOptimization =
				((GenomeMeaning)this.geneticOptimizer.BestGenome.Meaning).Tickers;
			this.addGenomeToBestGenomes( geneticOptimizer.BestGenome ,
			                            optimizationFirstDate , optimizationLastDate );
		}
		protected override void oneHourAfterMarketCloseEventHandler( Object sender ,
		                                                DateTime dateTime)
		{
			if ( dateTime.DayOfWeek ==
			    DayOfWeek.Friday )
				// current day is Friday
			{
				Console.WriteLine(
					dateTime.ToString() +
					" - " + DateTime.Now.ToString() );
				oneHourAfterMarketCloseEventHandler_set_signedTickersFromLastOptimization(
					dateTime );
			}
		}
		
//		public virtual void NewDateTimeEventHandler(
//			Object sender , DateTime dateTime )
//		{
//			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
//				this.marketOpenEventHandler( sender , dateTime );
//			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
//				this.marketCloseEventHandler( sender , dateTime );
//			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
//				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
//		}
	}
}
