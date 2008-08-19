/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBiasedPVO.cs
Copyright (C) 2006
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO.WeightedBalancedPVO;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO
{
	/// <summary>
	/// Implements MarketOpenEventHandler and MarketCloseEventHandler
	/// These handlers contain the core strategy for the Biased Portfolio Value
	/// Oscillator
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerBiasedPVO : EndOfDayTimerHandlerPVO
	{
		protected int numOfDifferentGenomesToEvaluateOutOfSample;
		protected int currentGenomeIndex = 0;
		protected double currentWeightedPositionsGainOrLoss = 0.0;
		protected Hashtable genomesCollector;
		protected WeightedPositions[] weightedPositionsToEvaluateOutOfSample;
		new protected double[] currentOverboughtThreshold;
		new protected double[] currentOversoldThreshold;
		protected double maxCoefficientForDegreeComputationOfCrossingThreshold;
		protected int numDaysWithOpenPositions;
		protected double numOfStdDevForThresholdsComputation;
		protected int numDaysOfStayingOnTheMarket;
		protected bool resetThresholdsBeforeCheckingOutOfSample;
		protected int numDaysForThresholdsReComputation;
		protected bool buyOnlyPositionsThatAreMovingTogether;
		protected bool doNotOpenReversedPositionsThatHaveJustBeenClosed;
		protected double minPriceForTickersToBeChosen;
		protected double maxPriceForTickersToBeChosen;
		protected WeightedPositions lastWeightedPositionsClosed;
		protected string pathOfFileContainingGenomes;
		protected OptimizationOutput optimizationOutput;

		public EndOfDayTimerHandlerBiasedPVO(string tickerGroupID, int numberOfEligibleTickers,
		                                     double minPriceForTickersToBeChosen,
		                                     double maxPriceForTickersToBeChosen,
		                                     int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                     Account account,
		                                     string pathOfFileContainingGenomes,
		                                     int generationNumberForGeneticOptimizer,
		                                     int populationSizeForGeneticOptimizer,
		                                     string benchmark,
		                                     int numOfDifferentGenomesToEvaluateOutOfSample,
		                                     bool resetThresholdsBeforeCheckingOutOfSample,
		                                     int numDaysForThresholdsReComputation,
		                                     double numOfStdDevForThresholdsComputation,
		                                     double maxCoefficientForDegreeComputationOfCrossingThreshold,
		                                     bool buyOnlyPositionsThatAreMovingTogether,
		                                     bool doNotOpenReversedPositionsThatHaveJustBeenClosed,
		                                     int numDaysForOscillatingPeriod,
		                                     int numDaysOfStayingOnTheMarket,
		                                     int minLevelForOversoldThreshold,
		                                     int maxLevelForOversoldThreshold,
		                                     int minLevelForOverboughtThreshold,
		                                     int maxLevelForOverboughtThreshold,
		                                     int divisorForThresholdComputation,
		                                     bool symmetricalThresholds,
		                                     bool overboughtMoreThanOversoldForFixedPortfolio,
		                                     int numDaysBetweenEachOptimization,
		                                     PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
		                                     double minimumAcceptableGain):
			base(tickerGroupID,numberOfEligibleTickers,
			     numberOfTickersToBeChosen,numDaysForOptimizationPeriod,
			     account,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer,
			     benchmark,
			     numDaysForOscillatingPeriod,
			     minLevelForOversoldThreshold,
			     maxLevelForOversoldThreshold,
			     minLevelForOverboughtThreshold,
			     maxLevelForOverboughtThreshold,
			     divisorForThresholdComputation,
			     symmetricalThresholds,
			     overboughtMoreThanOversoldForFixedPortfolio,
			     numDaysBetweenEachOptimization,
			     portfolioType, maxAcceptableCloseToCloseDrawdown,
			     minimumAcceptableGain)
			
		{
			this.minPriceForTickersToBeChosen = minPriceForTickersToBeChosen;
			this.maxPriceForTickersToBeChosen = maxPriceForTickersToBeChosen;
			this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
			this.weightedPositionsToEvaluateOutOfSample = new WeightedPositions[numOfDifferentGenomesToEvaluateOutOfSample];
			this.currentOversoldThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
			this.currentOverboughtThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
			this.resetThresholdsBeforeCheckingOutOfSample = resetThresholdsBeforeCheckingOutOfSample;
			this.numDaysForThresholdsReComputation = numDaysForThresholdsReComputation;
			this.maxCoefficientForDegreeComputationOfCrossingThreshold = maxCoefficientForDegreeComputationOfCrossingThreshold;
			this.numOfStdDevForThresholdsComputation = numOfStdDevForThresholdsComputation;
			this.numDaysOfStayingOnTheMarket = numDaysOfStayingOnTheMarket;
			this.buyOnlyPositionsThatAreMovingTogether = buyOnlyPositionsThatAreMovingTogether;
			this.doNotOpenReversedPositionsThatHaveJustBeenClosed = doNotOpenReversedPositionsThatHaveJustBeenClosed;
			this.genomesCollector = new Hashtable();
			this.pathOfFileContainingGenomes = pathOfFileContainingGenomes;
		}
		
		//to avoid handlers in inherited classes
		public override void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			;
		}

		#region MarketCloseEventHandler
		
		protected virtual double getCurrentWeightedPositionsGainOrLoss(
			IndexBasedEndOfDayTimer timer,
			ReturnsManager returnsManager,
			int indexForChosenWeightedPositions )
		{
			double returnValue = double.MinValue;
			try
			{
				returnValue =
					this.weightedPositionsToEvaluateOutOfSample[indexForChosenWeightedPositions].GetReturn(
						0 , returnsManager);
			}
			catch(MissingQuotesException ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
			return returnValue;
		}
		
		protected bool areAllTickersMovingTogetherUpOrDown(
			IndexBasedEndOfDayTimer timer,
			ReturnsManager returnsManager,
			int indexForChosenWeightedPositions )
		{
			bool returnValue = true;
			SignedTickers signedTickers =
				this.weightedPositionsToEvaluateOutOfSample[indexForChosenWeightedPositions].SignedTickers;
			float returnOfCurrentTicker, returnOfNextTicker;
			for( int i = 0;
			    signedTickers.Count > 1 && i < signedTickers.Count - 1 && returnValue == true;
			    i++ )
			{
				returnOfCurrentTicker = returnsManager.GetReturn(signedTickers[ i ].Ticker, 0);
				returnOfNextTicker = returnsManager.GetReturn(signedTickers[ i+1 ].Ticker, 0);
				if( (returnOfCurrentTicker > 0 && returnOfNextTicker < 0) ||
				   (returnOfCurrentTicker < 0 && returnOfNextTicker > 0) )
					returnValue = false;
			}
			return returnValue;
		}

		protected virtual void openPositions_chooseGenome_resetThresholds(DateTime today)
		{
			for(int i = 0; i < this.weightedPositionsToEvaluateOutOfSample.Length; i++)
			{
				this.currentOversoldThreshold[i] = (double)this.maxLevelForOversoldThreshold/
					(double)this.divisorForThresholdComputation;
				this.currentOverboughtThreshold[i] = (double)this.maxLevelForOverboughtThreshold/
					(double)this.divisorForThresholdComputation;
			}
//
			//    	ReturnsManager returnsManager = new ReturnsManager(
			//    		new CloseToCloseIntervals(new EndOfDayDateTime(today.AddDays(-this.numDaysForThresholdsReComputation), EndOfDaySpecificTime.MarketClose),
//		  	                         new EndOfDayDateTime(today, EndOfDaySpecificTime.MarketClose),
//		  	                         this.benchmark,
//		  	                         this.numDaysForOscillatingPeriod),
//		  	new HistoricalAdjustedQuoteProvider() );
			//    	//double returnsAverage;
			//    	double returnsStdDev;
			//    	for(int i = 0; i < this.weightedPositionsToEvaluateOutOfSample.Length; i++)
			//    	{
			////				returnsAverage = BasicFunctions.GetSimpleAverage(
			////					this.weightedPositionsToEvaluateOutOfSample[i].GetReturns(returnsManager) );
//				returnsStdDev = BasicFunctions.GetStdDev(
			//    			this.weightedPositionsToEvaluateOutOfSample[i].GetReturns(returnsManager) );
			//     		this.currentOversoldThreshold[i] = -this.numOfStdDevForThresholdsComputation * returnsStdDev;
			//    		this.currentOverboughtThreshold[i] = this.numOfStdDevForThresholdsComputation * returnsStdDev;
			//     	}
		}
		//NEW IMPLEMENTATION of chooseBestGenome, now named chooseGenome
		//sets currentGenomeIndex with the first genome's index that crosses an overbought/oversold threshold with the
		//"acceptable" degree and sets currentWeightedPositionsGainOrLoss accordingly
		protected virtual void openPositions_chooseGenome(IndexBasedEndOfDayTimer timer)
		{
			DateTime today = timer.GetCurrentTime().DateTime;
			DateTime firstMarketDayForOscillatingPeriod =
				timer.GetPreviousDateTime(this.numDaysForOscillatingPeriod);
			ReturnsManager returnsManager = new ReturnsManager(
				new CloseToCloseIntervals(new EndOfDayDateTime(firstMarketDayForOscillatingPeriod, EndOfDaySpecificTime.MarketClose),
				                          new EndOfDayDateTime(today, EndOfDaySpecificTime.MarketClose),
				                          this.benchmark,
				                          this.numDaysForOscillatingPeriod),
				new HistoricalAdjustedQuoteProvider() );
			if(this.resetThresholdsBeforeCheckingOutOfSample)
				this.openPositions_chooseGenome_resetThresholds(today);
			this.currentWeightedPositionsGainOrLoss = double.MinValue;
			double currentWeightedPositionsGainOrLoss_temp = double.MinValue;
			for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
			{
				currentWeightedPositionsGainOrLoss_temp =
					this.getCurrentWeightedPositionsGainOrLoss(
						timer, returnsManager, i);
				if( currentWeightedPositionsGainOrLoss_temp != double.MinValue &&
				   ( this.buyOnlyPositionsThatAreMovingTogether == false ||
				    ( this.buyOnlyPositionsThatAreMovingTogether == true &&
				     this.areAllTickersMovingTogetherUpOrDown(timer, returnsManager, i) == true	 ) )		)
					//currentWeightedPositionsGainOrLoss_temp has been properly computed and
					//only positions that are moving together can be bought
				{
					if( (currentWeightedPositionsGainOrLoss_temp >= this.currentOverboughtThreshold[i] &&
					     currentWeightedPositionsGainOrLoss_temp <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
					     this.currentOverboughtThreshold[i] ) ||
					   (currentWeightedPositionsGainOrLoss_temp <= - this.currentOversoldThreshold[i] &&
					    Math.Abs(currentWeightedPositionsGainOrLoss_temp) <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
					    this.currentOversoldThreshold[i] ) )
						// if the current genome matches the requested criteria
					{
						this.currentGenomeIndex = i;
						this.currentWeightedPositionsGainOrLoss = currentWeightedPositionsGainOrLoss_temp;
						i = this.numOfDifferentGenomesToEvaluateOutOfSample; //exit from for
					}
				}
			}
		}
		
		/* OLD IMPLEMENTATION OF chooseBestGenome
    //sets currentGenomeIndex with the genome's index that crosses an overbought/oversold threshold with the
    //highest "acceptable" degree and sets currentWeightedPositionsGainOrLoss accordingly
    protected void openPositions_chooseBestGenome(IndexBasedEndOfDayTimer timer)
    {
      //default index is the first
      //genome (with the highest plain fitness), if no other genome
      //has crossed the threshold with a higher degree than the first genome
      double currentMaxDegreeOfCrossingThreshold = 0.0;
      double currentDegreeOfCrossingThreshold = 0.0;
      DateTime today = timer.GetCurrentTime().DateTime;
		  DateTime firstMarketDayForOscillatingPeriod =
		  	timer.GetPreviousDateTime(this.numDaysForOscillatingPeriod);
		  ReturnsManager returnsManager = new ReturnsManager(
		  	new CloseToCloseIntervals(new EndOfDayDateTime(firstMarketDayForOscillatingPeriod, EndOfDaySpecificTime.MarketClose),
		  	                         new EndOfDayDateTime(today, EndOfDaySpecificTime.MarketClose),
		  	                         this.benchmark,
		  	                         this.numDaysForOscillatingPeriod),
		  	new HistoricalAdjustedQuoteProvider() );
		  if(this.resetThresholdsBeforeCheckingOutOfSample)
		  	this.openPositions_chooseBestGenome_resetThresholds(today);
		  this.currentWeightedPositionsGainOrLoss = 999.0;
      for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
      {
        double currentChosenWeightedPositionsGainOrLoss =
                 this.getCurrentWeightedPositionsGainOrLoss(
        				 timer, returnsManager, i);
				if(currentChosenWeightedPositionsGainOrLoss != 999.0)
          //currentChosenWeightedPositionsGainOrLoss has been properly computed
        {
          if( ( this.buyOnlyPositionsThatAreMovingTogether == false ||
        	      ( this.buyOnlyPositionsThatAreMovingTogether == true &&
									this.areAllTickersMovingTogetherUpOrDown(timer, returnsManager, i) == true	 ) ) &&
						 currentChosenWeightedPositionsGainOrLoss >= this.currentOverboughtThreshold[i] &&
             currentChosenWeightedPositionsGainOrLoss <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
            																						 this.currentOverboughtThreshold[i] )
          // if it is requested that current tickers have to move together in the same direction
					// if current gain crosses overbought threshold but not
        	// as maxCoefficientForDegreeComputationOfCrossingThreshold times
        	// as overbought threshold (that is, it could be a reasonable
        	// market inefficiency)
          {
            currentDegreeOfCrossingThreshold =
              (currentChosenWeightedPositionsGainOrLoss - this.currentOverboughtThreshold[i])/
            	 this.currentOverboughtThreshold[i];
 
          }
					else if ( ( this.buyOnlyPositionsThatAreMovingTogether == false ||
											( this.buyOnlyPositionsThatAreMovingTogether == true &&
												this.areAllTickersMovingTogetherUpOrDown(timer, returnsManager, i) == true		)	)  &&
									 currentChosenWeightedPositionsGainOrLoss <= - this.currentOversoldThreshold[i] &&
        	         Math.Abs(currentChosenWeightedPositionsGainOrLoss) <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
            																						 								 this.currentOversoldThreshold[i] )
          // if it is requested that current tickers have to move together in the same direction
					// if current loss crosses oversold threshold but not
        	// as maxCoefficientForDegreeComputationOfCrossingThreshold times
        	// as oversold threshold (that is, it could be a reasonable
        	// market inefficiency)
          {
            currentDegreeOfCrossingThreshold =
            	( Math.Abs(currentChosenWeightedPositionsGainOrLoss) - this.currentOversoldThreshold[i])/
            	  this.currentOversoldThreshold[i];
          }
          
        	if(currentDegreeOfCrossingThreshold > currentMaxDegreeOfCrossingThreshold)
          // if a genome crosses a threshold without crossing
        	// as maxCoefficientForDegreeComputationOfCrossingThreshold times
        	// as oversold or overbought thresholds
        	{
          	currentMaxDegreeOfCrossingThreshold = currentDegreeOfCrossingThreshold;
          	this.currentGenomeIndex = i;
          	this.currentWeightedPositionsGainOrLoss = currentChosenWeightedPositionsGainOrLoss;
          }
        }
      }
    }
		 *///END OF OLD IMPLEMENTATION
		
		protected void openPositions_open(WeightedPositions weightedPositions, bool overboughtPortfolio)
		{
			if( this.lastWeightedPositionsClosed == null ||
			   !this.doNotOpenReversedPositionsThatHaveJustBeenClosed ||
			   !weightedPositions.HasTheOppositeSignedTickersAs(this.lastWeightedPositionsClosed) )
				//it is the first time positions are opening or
				//positions that have been just closed can be reversed or
				//positions to be opened are just the reversed
				//positions that have just been closed
			{
				AccountManager.OpenPositions(weightedPositions, this.account, 30000);
				this.portfolioHasBeenOverbought = overboughtPortfolio;
				this.portfolioHasBeenOversold = !overboughtPortfolio;
			}
		}

		protected void openPositions(IndexBasedEndOfDayTimer timer)
		{
			this.currentWeightedPositionsGainOrLoss = double.MinValue;
			try{
				this.openPositions_chooseGenome(timer);
				//if the genome has been chosen, then this.currentWeightedPositionsGainOrLoss
				//has been set to a value different from double.MinValue
			}
			catch(Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
			if(this.currentWeightedPositionsGainOrLoss != double.MinValue)
				//there is a genome that matches requested criteria
			{
				if(this.currentWeightedPositionsGainOrLoss >= currentOverboughtThreshold[this.currentGenomeIndex] &&
				   (this.portfolioType == PortfolioType.ShortAndLong || this.portfolioType == PortfolioType.OnlyMixed) )
				{
					this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					try{
						this.openPositions_open(weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],true);
					}
					catch(Exception ex)
					{
						string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
					}
					finally{
						this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					}
				}
				else if (this.currentWeightedPositionsGainOrLoss <= - currentOversoldThreshold[this.currentGenomeIndex])
				{
					this.openPositions_open(weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],false);
				}
				this.previousAccountValue = this.account.GetMarketValue();
			}
		}
		
		
		private bool marketCloseEventHandler_isTimerStateValidForStrategy(IndexBasedEndOfDayTimer timer)
		{
			bool returnValue;
			DateTime currentTime =  timer.GetCurrentTime().DateTime;
			DateTime firstDateOfOscillatingPeriod =
				timer.GetPreviousDateTime(this.numDaysForOscillatingPeriod);
			returnValue = currentTime.CompareTo(firstDateOfOscillatingPeriod) > 0;
			return returnValue;
			//currentTime is greater than the date at which previous close
			//has to be compared to the current close with respect to
			//the oscillating strategy's criteria
		}
		
		protected override void marketCloseEventHandler_closePositionsIfNeeded()
		{
			if( this.numDaysWithOpenPositions == this.numDaysOfStayingOnTheMarket ||
			   this.stopLossConditionReached || this.takeProfitConditionReached 		)
			{
				this.lastWeightedPositionsClosed = AccountManager.GetWeightedPositions(this.account);
				AccountManager.ClosePositions(this.account);
				this.portfolioHasBeenOverbought = false;
				this.portfolioHasBeenOversold = false;
				this.numDaysWithOpenPositions = 0;
			}
		}

		public override void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if(this.account.Portfolio.Count > 0)
			{
				this.numDaysWithOpenPositions++;
				this.marketCloseEventHandler_updateStopLossAndTakeProfitConditions();
				this.marketCloseEventHandler_closePositionsIfNeeded();
			}
			
			if ( this.account.Portfolio.Count == 0 &&
			    this.weightedPositionsToEvaluateOutOfSample[0] != null &&
			    this.marketCloseEventHandler_isTimerStateValidForStrategy( (IndexBasedEndOfDayTimer)sender) )
				//portfolio is empty and optimization has been already launched and
				//currentTime is greater than the date at which previous close
				//has to be compared to the current close with respect to
				//the oscillating strategy's criteria
				this.openPositions( (IndexBasedEndOfDayTimer)sender );
		}

		#endregion
		

		#region OneHourAfterMarketCloseEventHandler
		
		protected void setTickers_updateTickersWeightsAndThresholdsAndAddGenomesForLog(GeneticOptimizer GO,
		                                                                               int eligibleTickersForGO)
		{
			int addedGenomes = 0;
			int counter = 0;
			Genome currentGenome = null;
			this.genomesCollector.Clear();
			while(addedGenomes < this.numOfDifferentGenomesToEvaluateOutOfSample &&
			      counter < GO.PopulationSize)
			{
				currentGenome = (Genome)GO.CurrentGeneration[GO.PopulationSize - 1 - counter];
				if(   counter == 0 ||
				   !this.genomesCollector.ContainsKey(
				   	( (GenomeMeaning)currentGenome.Meaning ).HashCodeForTickerComposition   )   )
//								currentGenome.Fitness)   )
					//it is the first genome to be added or no genome with the current
					// fitness has been added to the hashtable yet
				{
					this.weightedPositionsToEvaluateOutOfSample[addedGenomes] = new WeightedPositions(
						((GenomeMeaningPVO)currentGenome.Meaning).TickersPortfolioWeights,
						new SignedTickers( ((GenomeMeaningPVO)currentGenome.Meaning).Tickers ) );
					this.currentOversoldThreshold[addedGenomes] =
						((GenomeMeaningPVO)currentGenome.Meaning).OversoldThreshold;
					this.currentOverboughtThreshold[addedGenomes] =
						((GenomeMeaningPVO)currentGenome.Meaning).OverboughtThreshold;
					this.genomesCollector.Add(
						( (GenomeMeaning)currentGenome.Meaning ).HashCodeForTickerComposition, null);
//						currentGenome.Fitness,null);
					this.addPVOGenomeToBestGenomes(currentGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
					                               ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, eligibleTickersForGO,
					                               this.numDaysForOscillatingPeriod, this.portfolioType, GO.GenerationCounter,
					                               this.currentOversoldThreshold[addedGenomes], this.currentOverboughtThreshold[addedGenomes]);
					addedGenomes ++ ;
				}
				counter ++ ;
			}
		}
		
		protected override DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
			DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
			int numOfTickersInGroupAtCurrentDate = tickersFromGroup.Rows.Count;
			
			SelectorByAverageRawOpenPrice byPrice =
				new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-15),
				                                  currentDate,
				                                  numOfTickersInGroupAtCurrentDate,
				                                  this.minPriceForTickersToBeChosen,this.maxPriceForTickersToBeChosen,
				                                  0.0001,100);
			
			SelectorByLiquidity mostLiquidSelector =
				new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
				                        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                        this.numberOfEligibleTickers);
			
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection =
				new SelectorByQuotationAtEachMarketDay(mostLiquidSelector.GetTableOfSelectedTickers(),
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                                       this.numberOfEligibleTickers, this.benchmark);

			return quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
			
		}
		
		
		protected override void setTickers(DateTime currentDate,
		                                   bool setGenomeCounter)
		{
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			this.iGenomeManager =
				new GenomeManagerPVO(setOfTickersToBeOptimized,
				                     currentDate.AddDays(-this.numDaysForOptimizationPeriod),
				                     currentDate, this.numberOfTickersToBeChosen,
				                     this.numDaysForOscillatingPeriod,
				                     this.minLevelForOversoldThreshold,
				                     this.maxLevelForOversoldThreshold,
				                     this.minLevelForOverboughtThreshold,
				                     this.maxLevelForOverboughtThreshold,
				                     this.divisorForThresholdComputation,
				                     this.symmetricalThresholds,
				                     this.overboughtMoreThanOversoldForFixedPortfolio,
				                     this.portfolioType, this.benchmark);
			GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
			                                           this.populationSizeForGeneticOptimizer,
			                                           this.generationNumberForGeneticOptimizer,
			                                           this.seedForRandomGenerator);
			//      if(setGenomeCounter)
			//        this.genomeCounter = new GenomeCounter(GO);
			GO.MutationRate = 0.1;
			GO.CrossoverRate = 0.85;
			GO.Run(false);
			
			this.setTickers_updateTickersWeightsAndThresholdsAndAddGenomesForLog(GO, setOfTickersToBeOptimized.Rows.Count);
			
		}
		
		private void setTickersFromFile_updateWeightedPositionsToEvaluateOutOfSample_addGenomeRepresentation(GenomeRepresentation genomeRepresentation)
		{
			WeightedPositions weightedPositions = new WeightedPositions(
				GenomeRepresentation.GetWeightsArray(genomeRepresentation.WeightsForSignedTickers),
				new SignedTickers(GenomeRepresentation.GetSignedTickers(genomeRepresentation.SignedTickers)) );
			for(int i = 0; i<this.weightedPositionsToEvaluateOutOfSample.Length; i++)
			{
				if(this.weightedPositionsToEvaluateOutOfSample[i] == null)
				{
					this.weightedPositionsToEvaluateOutOfSample[i] = weightedPositions;
					this.currentOversoldThreshold[i] = genomeRepresentation.OversoldThreshold;
					this.currentOverboughtThreshold[i] = genomeRepresentation.OverboughtThreshold;
					i = this.weightedPositionsToEvaluateOutOfSample.Length;//exit from for
				}
			}
		}
		
		private void setTickersFromFile_updateWeightedPositionsToEvaluateOutOfSample(DateTime currentDate)
		{
			GenomeRepresentation currentGenomeRepresentation;
			for(int i = 0; i<this.optimizationOutput.Count;i++)
			{
				currentGenomeRepresentation = (GenomeRepresentation)this.optimizationOutput[i];
				if(currentGenomeRepresentation.LastOptimizationDate == currentDate)
				{
					this.setTickersFromFile_updateWeightedPositionsToEvaluateOutOfSample_addGenomeRepresentation(
						currentGenomeRepresentation);
				}
			}
		}
		
		private void setTickersFromFile_loadGenomesFromFile()
		{
			if( this.optimizationOutput == null )
				this.optimizationOutput =
					(OptimizationOutput)ObjectArchiver.Extract(
						this.pathOfFileContainingGenomes);
		}
		
		private void setTickersFromFile_clearWeightedPositionsToEvaluateOutOfSample()
		{
			for(int i = 0; i< this.weightedPositionsToEvaluateOutOfSample.Length; i++)
				this.weightedPositionsToEvaluateOutOfSample[i] = null;
		}
		
		private void setTickersFromFile(DateTime currentDate)
		{
			this.setTickersFromFile_loadGenomesFromFile();
			this.setTickersFromFile_clearWeightedPositionsToEvaluateOutOfSample();
			this.setTickersFromFile_updateWeightedPositionsToEvaluateOutOfSample(currentDate);
		}
		
		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public override void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.lastCloseDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
			this.seedForRandomGenerator++;
			this.numDaysElapsedSinceLastOptimization++;
			if((this.numDaysElapsedSinceLastOptimization ==
			    this.numDaysBetweenEachOptimization))
				//num days without optimization has elapsed
			{
				if(this.pathOfFileContainingGenomes == null)
					//tickers have to be set by a new optimization process (in sample)
					this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime, false);
				//sets tickers to be chosen next Market Close event
				else//this.pathOfFileContainingGenomes != null
					this.setTickersFromFile(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
				//set tickers from file
				this.numDaysElapsedSinceLastOptimization = 0;
			}
		}
		
		#endregion

	}
}
