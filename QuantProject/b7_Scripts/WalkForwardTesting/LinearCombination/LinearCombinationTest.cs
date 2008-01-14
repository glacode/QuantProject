/*
QuantProject - Quantitative Finance Library

LinearCombinationTest.cs
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
using System.Drawing;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag;


namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Object to test a linear combination strategy on given positions,
	/// on a given period of time 
	/// </summary>
	public class LinearCombinationTest
	{
		private DateTime firstDate;
		private DateTime lastDate;
		private GenomeRepresentation[] genomeRepresentations;
//		private bool openToCloseDaily;
    private StrategyType strategyType;
    private int numDaysForOscillatorStrategy;
    private double stopLoss;
    private double takeProfit;
		private double oversoldThreshold;
		private double overboughtThreshold;
		private bool setDirectlyThresholdLevels;

		private IHistoricalQuoteProvider historicalQuoteProvider;
		private HistoricalEndOfDayTimer historicalEndOfDayTimer;
		private Account account;
		private IEndOfDayStrategy endOfDayStrategy;
    private PortfolioType portfolioType;
		
		private void linearCombinationTest_commonInitialization(DateTime firstDate , DateTime lastDate ,
			GenomeRepresentation[] genomeRepresentations , StrategyType strategyType,
			PortfolioType portfolioType, int numDaysForOscillatorStrategy,
		  double stopLoss, double takeProfit,
      double oversoldThreshold, double overboughtThreshold, bool setDirectlyThresholdLevels)
		{
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.genomeRepresentations = genomeRepresentations;
			this.strategyType = strategyType;
			this.portfolioType = portfolioType;
			this.numDaysForOscillatorStrategy = numDaysForOscillatorStrategy;
			this.stopLoss = stopLoss;
			this.takeProfit = takeProfit;
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.setDirectlyThresholdLevels = setDirectlyThresholdLevels;
		}

		public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
      GenomeRepresentation[] genomeRepresentations , StrategyType strategyType,
      PortfolioType portfolioType)
		{
			this.linearCombinationTest_commonInitialization( firstDate, lastDate,
				genomeRepresentations, strategyType, portfolioType, 0,0,0,0,0,false);
    }
    
    public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
      GenomeRepresentation[] genomeRepresentations , StrategyType strategyType,
      PortfolioType portfolioType,
      int numDaysForOscillatorStrategy)
    {
			this.linearCombinationTest_commonInitialization( firstDate, lastDate,
				genomeRepresentations, strategyType, portfolioType, numDaysForOscillatorStrategy,
				0,0,0,0,false);
    }

    public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
      GenomeRepresentation[] genomeRepresentations , StrategyType strategyType,
      PortfolioType portfolioType,
      int numDaysForOscillatorStrategy, double stopLoss, double takeProfit)
    {
			this.linearCombinationTest_commonInitialization( firstDate, lastDate,
				genomeRepresentations, strategyType, portfolioType, numDaysForOscillatorStrategy,
				stopLoss, takeProfit, 0,0,false);
    }
		public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
			GenomeRepresentation[] genomeRepresentations , StrategyType strategyType,
			PortfolioType portfolioType,
			int numDaysForOscillatorStrategy, double stopLoss, double takeProfit,
			double oversoldThreshold, double overboughtThreshold, bool setDirectlyThresholdLevels)
		{
			this.linearCombinationTest_commonInitialization( firstDate, lastDate,
				genomeRepresentations, strategyType, portfolioType, numDaysForOscillatorStrategy,
				stopLoss, takeProfit, oversoldThreshold, overboughtThreshold,
				setDirectlyThresholdLevels);
		}
    private void oneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >=
				this.lastDate )
				this.account.EndOfDayTimer.Stop();
		}

		private void run_setHistoricalQuoteProvider()
		{
			if ( this.strategyType == StrategyType.OpenToCloseDaily )
				this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			else
				this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}
		
//    private void run_setStrategy_setBiasedOTC_PVONoThresholdsStrategy()
//    {
//    	WeightedPositions[] weightedPositions = new WeightedPositions[this.genomeRepresentations.Length];
//    	for(int i = 0; i<this.genomeRepresentations.Length;i++)
//      {
//      	weightedPositions[i] = 
//      		new WeightedPositions(GenomeRepresentation.GetWeightsArray(this.genomeRepresentations[i].WeightsForSignedTickers),
//      		                      new SignedTickers(this.genomeRepresentations[i].SignedTickers));
//      		                      
//      }
//     
//      this.endOfDayStrategy = new FixedLevelOscBiasedOTC_PVONoThresholdsStrategy(
//        this.account , weightedPositions, 
//        this.genomeRepresentations.Length);
//    }

//    private void run_setStrategy_setBiasedPVONoThresholdsStrategy()
//    {
//			WeightedPositions[] weightedPositions = new WeightedPositions[this.genomeRepresentations.Length];
//			for(int i = 0; i<this.genomeRepresentations.Length;i++)
//			{
//				weightedPositions[i] = 
//					new WeightedPositions(GenomeRepresentation.GetWeightsArray(this.genomeRepresentations[i].WeightsForSignedTickers),
//					new SignedTickers(this.genomeRepresentations[i].SignedTickers));
//      		                      
//			}
//      this.endOfDayStrategy = new FixedLevelOscillatorBiasedPVONoThresholdsStrategy(
//        this.account , weightedPositions, 
//        this.genomeRepresentations.Length,
//        this.stopLoss,
//        this.takeProfit);
//    }

		private double run_setStrategy_setBiasedPVOStrategy_getOversoldThreshold(int i)
		{
			double returnValue;
			if(this.setDirectlyThresholdLevels)
				returnValue = this.oversoldThreshold;
			else
				returnValue = this.genomeRepresentations[i].OversoldThreshold;

			return returnValue;
		}
		private double run_setStrategy_setBiasedPVOStrategy_getOverboughtThreshold(int i)
		{
			double returnValue;
			if(this.setDirectlyThresholdLevels)
				returnValue = this.overboughtThreshold;
			else
				returnValue = this.genomeRepresentations[i].OverboughtThreshold;

			return returnValue;
		}
		
		private void run_setStrategy_setBiasedPVOStrategy()
		{
			WeightedPositions[] weightedPositions = new WeightedPositions[this.genomeRepresentations.Length];
			double[] oversoldThresholds = new double[this.genomeRepresentations.Length];
      double[] overboughtThresholds = new double[this.genomeRepresentations.Length];
			for(int i = 0; i<this.genomeRepresentations.Length;i++)
			{
				weightedPositions[i] = 
					new WeightedPositions(GenomeRepresentation.GetWeightsArray(this.genomeRepresentations[i].WeightsForSignedTickers),
					new SignedTickers(this.genomeRepresentations[i].SignedTickers));
				oversoldThresholds[i] = this.run_setStrategy_setBiasedPVOStrategy_getOversoldThreshold(i);
				overboughtThresholds[i] = this.run_setStrategy_setBiasedPVOStrategy_getOverboughtThreshold(i);
			}
			
		 	this.endOfDayStrategy = new FixedLevelOscillatorBiasedPVOStrategy(
            this.account , weightedPositions, 
            oversoldThresholds, overboughtThresholds,
            overboughtThresholds.Length,
            this.numDaysForOscillatorStrategy,this.stopLoss,
            this.takeProfit);
		}
		
		private void run_setStrategy()
		{
			WeightedPositions weightedPositions = 
				this.run_getWeightedPositions(this.genomeRepresentations[0]);
			switch (this.strategyType)
      {
        case StrategyType.OpenToCloseDaily:
          this.endOfDayStrategy = new OpenToCloseDailyStrategy(
					                            this.account , weightedPositions );
          break;
        case StrategyType.OpenToCloseWeekly:
          this.endOfDayStrategy = new OpenToCloseWeeklyStrategy(
                                      this.account , weightedPositions );
          break;
        case StrategyType.CloseToOpenDaily:
          this.endOfDayStrategy = new CloseToOpenDailyStrategy(
                                      this.account , weightedPositions );
          break;
        
        case StrategyType.OpenToCloseCloseToOpenDaily:
          this.endOfDayStrategy = new OTC_CTODailyStrategy(
            this.account , weightedPositions);
          break;
        
        case StrategyType.FixedPeriodOscillator:
          this.endOfDayStrategy = new FixedPeriodOscillatorStrategy(
			            this.account , weightedPositions,
			            this.numDaysForOscillatorStrategy , 
			            this.numDaysForOscillatorStrategy );
          break;
        
        case StrategyType.ExtremeCounterTrend:
          this.endOfDayStrategy = new ExtremeCounterTrendStrategy(
            this.account , weightedPositions, this.numDaysForOscillatorStrategy,
            this.portfolioType);
          break;
        
        case StrategyType.ImmediateTrendFollower:
          this.endOfDayStrategy = new ImmediateTrendFollowerStrategy(
            this.account , weightedPositions, this.numDaysForOscillatorStrategy );
          break;
        
        case StrategyType.PortfolioValueOscillator:
          this.endOfDayStrategy = new FixedLevelOscillatorPVOStrategy(
            this.account , weightedPositions, 
            this.genomeRepresentations[0].OversoldThreshold, this.genomeRepresentations[0].OverboughtThreshold,
            this.numDaysForOscillatorStrategy );
          break;
          
        case StrategyType.PortfolioValueOscillatorBiased:
          this.run_setStrategy_setBiasedPVOStrategy();
          break;
        
//        case StrategyType.PortfolioValueOscillatorBiasedNoThresholds:
//          this.run_setStrategy_setBiasedPVONoThresholdsStrategy();
//          break;
//
//        case StrategyType.OTC_PVOBiasedNoThresholds:
//          this.run_setStrategy_setBiasedOTC_PVONoThresholdsStrategy();
//          break;
      }
		}
		private string getDateString( DateTime dateTime )
		{
			string returnValue = dateTime.ToString( "yy-MM-dd" );
			return returnValue;
		}
		private string run_getReportTitle()
		{
			string returnValue = "Fitness:" +
				this.genomeRepresentations[0].Fitness.ToString() +
				" | Tickers:" +
				this.genomeRepresentations[0].SignedTickers  + " - " +
				"from " + this.getDateString( this.firstDate ) +
				" to " + this.getDateString( this.lastDate ) +
				" opt. in sample from " + this.getDateString(
				this.genomeRepresentations[0].FirstOptimizationDate ) +
				" to " + this.getDateString(
				this.genomeRepresentations[0].LastOptimizationDate );
			return returnValue;
		}
		
    private void run_addEquityLineForWeightedPositions(
      WeightedPositions weightedPositions , Color color , Report report )
    {
      EquityLine equityLineForWeightedPositions =
        weightedPositions.GetVirtualEquityLine(
        15000 , report.AccountReport.EquityLine );
      report.AddEquityLine( equityLineForWeightedPositions ,
        color );
    }
    
		private void run_addEquityLineForEachPositionInWeightedPositions(
      WeightedPositions weightedPositions , Color color , Report report )
    {
			foreach(WeightedPosition position in weightedPositions.Values)
			{
				WeightedPositions wp = new WeightedPositions(new SignedTickers(
																										 position.Ticker));
				EquityLine equityLineForWeightedPositions =
        wp.GetVirtualEquityLine(
        15000 , report.AccountReport.EquityLine );
      	report.AddEquityLine( equityLineForWeightedPositions ,
        color );
			}
	  }
		
    private WeightedPositions run_getWeightedPositions(GenomeRepresentation genomeRepresentation)
      
    {
      double[] normalizedWeights = 
        GenomeRepresentation.GetWeightsArray(genomeRepresentation.WeightsForSignedTickers);
      string[] tickers = 
        GenomeRepresentation.GetSignedTickers(genomeRepresentation.SignedTickers);
      for(int i = 0; i<tickers.Length; i++)
      {
        if(tickers[i].StartsWith("-"))
        {
          tickers[i] = SignedTicker.GetTicker(tickers[i]);
          normalizedWeights[i] = -1.0 * normalizedWeights[i];
        }
      }
      return new WeightedPositions(normalizedWeights, tickers);
    }

    public void Run()
		{
			this.historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.firstDate ,
				EndOfDaySpecificTime.MarketOpen ) , "^GSPC" );
			run_setHistoricalQuoteProvider();
			this.account = new Account( "LinearCombination" , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) );
			run_setStrategy();
			//			OneRank oneRank = new OneRank( account ,
			//				this.endDateTime );
			this.historicalEndOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.endOfDayStrategy.MarketOpenEventHandler );
			this.historicalEndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
      this.historicalEndOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.endOfDayStrategy.MarketCloseEventHandler );
			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.account.EndOfDayTimer.Start();

			Report report = new Report( this.account , this.historicalQuoteProvider );
      report.Create( "Linear Combination" , 1 ,
				new EndOfDayDateTime( this.lastDate , EndOfDaySpecificTime.MarketClose ) ,
			"^GSPC");
      foreach(GenomeRepresentation genomeRepresentation in this.genomeRepresentations)
      { 	
      	WeightedPositions weightedPositions = this.run_getWeightedPositions( genomeRepresentation );
      	this.run_addEquityLineForWeightedPositions(
      		weightedPositions,
      		Color.Brown,
      		report);
      	this.run_addEquityLineForEachPositionInWeightedPositions(
      		weightedPositions,
      		Color.DimGray,
      		report);
      		
      }
      report.Text = this.run_getReportTitle();
			report.Show();
		}
	}
}
