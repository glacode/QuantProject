/*
QuantProject - Quantitative Finance Library

FixedLevelOscBiasedOTC_PVONoThresholdsStrategy.cs
Copyright (C) 2007 
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
using QuantProject.Data;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedPVONoThresholds;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Fixed Level Oscillators 
	/// Open To Close Portfolio Value Oscillator Biased strategy:
	/// from a group of non-moving OTC - CTO genomes, the strategy indicates
	/// to buy the portfolio that presents at open the highest gain or loss
	/// closing all positions at close
	/// </summary>
	[Serializable]
	public class FixedLevelOscBiasedOTC_PVONoThresholdsStrategy : EndOfDayTimerHandlerBiasedOTC_PVONoThresholds, IEndOfDayStrategy
	{
    
		public FixedLevelOscBiasedOTC_PVONoThresholdsStrategy( Account accountPVO ,
			                                     string[,] tickers,
			                                     double[,] tickersPortfolioWeights,
                                           int numOfDifferentGenomesToEvaluateOutOfSample):
                                        base("", 0, 
                                        tickers.GetUpperBound(1)+1, 0, accountPVO, 0, 0,
                                        "", numOfDifferentGenomesToEvaluateOutOfSample,0,
                                        PortfolioType.ShortAndLong)
                                       
		{
			this.chosenTickers = tickers;
			this.chosenTickersPortfolioWeights = tickersPortfolioWeights;
    }
		
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.account.Portfolio.Count == 0)
        this.marketOpenEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
    }

    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }
    		
    protected override double getCurrentChosenTickersGainOrLoss(IndexBasedEndOfDayTimer timer,
		                                                       int indexForChosenTickers)
    {
      double returnValue = 999.0;
      if(timer.CurrentDateArrayPosition >= 1)
      //if there are sufficient data for computing currentChosenTickersValue
      //that's why the method has been overriden
      {
	      try
	      {
          DateTime today = 
            (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
          DateTime lastMarketDay = 
            (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - 1]["quDate"];
	      
          string[] tickers = new string[this.numberOfTickersToBeChosen];
          double[] tickerWeights = new double[this.numberOfTickersToBeChosen];
          for(int i = 0; i < this.numberOfTickersToBeChosen; i++)
          {
            tickers[i] = this.chosenTickers[indexForChosenTickers,i];
            tickerWeights[i] = this.chosenTickersPortfolioWeights[indexForChosenTickers,i];
          }
          returnValue =
            SignedTicker.GetLastNightPortfolioReturn(
            tickers, tickerWeights,
            lastMarketDay, today);
	      }
	    	catch(MissingQuotesException ex)
	    	{
	    		ex = ex;
	    	}
      }
      return returnValue;
    }   

    public override void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
