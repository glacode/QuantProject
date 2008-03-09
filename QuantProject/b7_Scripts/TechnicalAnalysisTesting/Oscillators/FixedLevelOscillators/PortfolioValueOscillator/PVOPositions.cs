/*
QuantProject - Quantitative Finance Library

PVOPositions.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Business.DataProviders;



namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
  /// <summary>
  /// This is the class representing a TestingPositions for the
  /// portfolio value oscillator strategy
  /// </summary>
  [Serializable]
  public class PVOPositions : TestingPositions, IGeneticallyOptimizable
  {
  	private double oversoldThreshold;
  	private double overboughtThreshold;
  	private int numDaysForOscillatingPeriod;
  	private int generation;
				
		private static ReturnsManager returnsManager;

  	  	  	
  	public double OversoldThreshold
    {
      get{return this.oversoldThreshold;}
			set{this.oversoldThreshold = value;}
    }
    public double OverboughtThreshold
    {
      get{return this.overboughtThreshold;}
			set{this.overboughtThreshold = value;}
    }
		public int NumDaysForOscillatingPeriod
    {
      get{return this.numDaysForOscillatingPeriod;}
    }
    
		//explicit interface implementation
		//the property can be used only by a interface
		//instance or through a cast to the interface
		int IGeneticallyOptimizable.Generation
		{
			get{return this.generation;}
			set{this.generation = value;}
		}
		
		public PVOPositions Copy()
		{
			return new PVOPositions(this.WeightedPositions,this.OversoldThreshold,
					this.overboughtThreshold, this.numDaysForOscillatingPeriod);
		}

		public PVOPositions(WeightedPositions weightedPositions,
												double oversoldThreshold,
												double overboughtThreshold,
												int numDaysForOscillatingPeriod) :
												base(weightedPositions)
                            
    {
 			this.oversoldThreshold = oversoldThreshold;
 			this.overboughtThreshold = overboughtThreshold;
 			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
 			this.generation = -1;
		}
		
		private void setReturnsManager(EndOfDayDateTime beginOfPeriod,
																	 EndOfDayDateTime endOfPeriod,
																	 string benchmark,
																	 HistoricalQuoteProvider quoteProvider)
		{
			if(PVOPositions.returnsManager == null ||
			   PVOPositions.returnsManager.ReturnIntervals[0].Begin != beginOfPeriod ||
				 PVOPositions.returnsManager.ReturnIntervals[0].End != endOfPeriod)
			//if a returnsManager has not been set yet or a different one has to be set
			//for a different returnInterval
					PVOPositions.returnsManager = new ReturnsManager(new ReturnIntervals(
						new ReturnInterval( beginOfPeriod, endOfPeriod ) ) ,
						quoteProvider );
		}

		private double getOscillatingPeriodReturn(EndOfDayDateTime beginOfPeriod,
																					EndOfDayDateTime endOfPeriod,
		                                      string benchmark,
		                                      HistoricalQuoteProvider quoteProvider)
    {
      this.setReturnsManager(beginOfPeriod, endOfPeriod, benchmark, quoteProvider);
			return this.WeightedPositions.GetReturn(0, PVOPositions.returnsManager);
  	}
		
		public PVOPositionsStatus GetStatus(EndOfDayDateTime beginOfPeriod,
																				EndOfDayDateTime endOfPeriod,
		                                    string benchmark,
		                                    HistoricalQuoteProvider quoteProvider,
		                                    double maxCoefficientOfCrossingThresholds)
    {
      PVOPositionsStatus returnValue;
			double oscillatingPeriodReturn = double.NaN;
      oscillatingPeriodReturn = 
      	this.getOscillatingPeriodReturn(beginOfPeriod, endOfPeriod, benchmark,
      	                                quoteProvider);
      if(oscillatingPeriodReturn >= this.overboughtThreshold && 
         oscillatingPeriodReturn <= maxCoefficientOfCrossingThresholds*this.overboughtThreshold)
      		returnValue = PVOPositionsStatus.Overbought;
      else if(oscillatingPeriodReturn <= -this.oversoldThreshold &&
              Math.Abs(oscillatingPeriodReturn)<=maxCoefficientOfCrossingThresholds*this.oversoldThreshold)
      		returnValue = PVOPositionsStatus.Oversold;
      else if ( Math.Abs(oscillatingPeriodReturn) > maxCoefficientOfCrossingThresholds*this.oversoldThreshold ||
                oscillatingPeriodReturn > maxCoefficientOfCrossingThresholds*this.overboughtThreshold  )
      		returnValue = PVOPositionsStatus.TooDistantFromThresholds;
      else
      	returnValue = PVOPositionsStatus.InTheMiddle;
         	
			return returnValue;
  	}
		
		public PVOPositionsStatus GetStatus(EndOfDayDateTime beginOfPeriod,
																				EndOfDayDateTime endOfPeriod,
		                                    string benchmark,
		                                    HistoricalQuoteProvider quoteProvider)
    {
			return this.GetStatus(beginOfPeriod, endOfPeriod ,
														benchmark, quoteProvider, 10000.0);
			//a very high maxCoefficientOfCrossingThresholds is given:
			//so the status TooDistantFromThresholds should never been reached
  	}

		public bool AreAllTickersMovingTogetherUpOrDown(EndOfDayDateTime beginOfPeriod,
																										EndOfDayDateTime endOfPeriod,
																										string benchmark,
																										HistoricalQuoteProvider quoteProvider)
		{
			bool returnValue = true;
			SignedTickers signedTickers = this.WeightedPositions.SignedTickers;
			float returnOfCurrentTicker, returnOfNextTicker;
			this.setReturnsManager(beginOfPeriod, endOfPeriod, benchmark, quoteProvider);
			for( int i = 0;
				signedTickers.Count > 1 && i < signedTickers.Count - 1 && returnValue == true;
				i++ )
			{
				returnOfCurrentTicker = PVOPositions.returnsManager.GetReturn(signedTickers[ i ].Ticker, 0);
				returnOfNextTicker = PVOPositions.returnsManager.GetReturn(signedTickers[ i+1 ].Ticker, 0);
				if( (returnOfCurrentTicker > 0 && returnOfNextTicker < 0) ||
					(returnOfCurrentTicker < 0 && returnOfNextTicker > 0) )
					returnValue = false;
			}	
			return returnValue;
		}
		
  }
}
