/*
QuantProject - Quantitative Finance Library

FixedLevelOscillatorBiasedPVOStrategy.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Fixed Level Oscillators 
	/// Biased Portfolio Value Oscillator strategy:
	/// from a group of optimized PVO portfolios, the strategy indicates
	/// to buy the portfolio that presents the highest degree
	/// of moving from a threshold 
	/// </summary>
	[Serializable]
	public class FixedLevelOscillatorBiasedPVOStrategy : EndOfDayTimerHandlerBiasedPVO, IEndOfDayStrategy
	{
    
		public FixedLevelOscillatorBiasedPVOStrategy( Account accountPVO ,
			                                     string[,] tickers,
			                                     double[,] tickersPortfolioWeights,
                                           double[] oversoldThreshold,
                                           double[] overboughtThreshold,
                                           int numOfDifferentGenomesToEvaluateOutOfSample,
                                           int numDaysForOscillatingPeriod,
                                           double maxAcceptableCloseToCloseDrawdown,
                                           double minimumAcceptableGain):
                                        base("", 0, 
                                        tickers.GetUpperBound(1)+1, 0, accountPVO, 0, 0,
                                        "", numOfDifferentGenomesToEvaluateOutOfSample,
                                        numDaysForOscillatingPeriod,1,1,1,1,1,false,false,0,
                                        PortfolioType.ShortAndLong,maxAcceptableCloseToCloseDrawdown,
                                        minimumAcceptableGain)
                                       
		{
			this.bestGenomesChosenTickers = tickers;
			this.bestGenomesChosenTickersPortfolioWeights = tickersPortfolioWeights;
      this.currentOversoldThreshold = oversoldThreshold;
      this.currentOverboughtThreshold = overboughtThreshold;
      this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
      this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
		}
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }
    		
    protected override double getCurrentChosenTickersGainOrLoss(IndexBasedEndOfDayTimer timer,
		                                                       int indexForChosenTickers)
    {
      double returnValue = 999.0;
      if(timer.CurrentDateArrayPosition + 2 >= this.numDaysForOscillatingPeriod)
      //if there are sufficient data for computing currentChosenTickersValue
      //that's why the method has been overriden
      {
	      try
	      {
			    DateTime initialDate = 
		          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForOscillatingPeriod + 2]["quDate"];
		      //so to replicate exactly in sample scheme, where only numOscillatingDay - 1 returns
	        //are computed
	        DateTime finalDate = 
		        (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
	      	string[] tickers = new string[this.numberOfTickersToBeChosen];
	        double[] tickerWeights = new double[this.numberOfTickersToBeChosen];
	        for(int i = 0; i < this.numberOfTickersToBeChosen; i++)
	        {
	          tickers[i] = this.bestGenomesChosenTickers[indexForChosenTickers,i];
	          tickerWeights[i] = this.bestGenomesChosenTickersPortfolioWeights[indexForChosenTickers,i];
	        }
	        returnValue =
		      	 SignedTicker.GetCloseToClosePortfolioReturn(
		      	     tickers, tickerWeights,
		      	     initialDate,finalDate) + 1.0;
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
