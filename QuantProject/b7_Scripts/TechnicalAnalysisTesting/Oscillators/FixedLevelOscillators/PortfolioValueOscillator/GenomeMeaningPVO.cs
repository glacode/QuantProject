/*
QuantProject - Quantitative Finance Library

GenomeMeaningPVO.cs
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
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
  /// <summary>
  /// This is the class representing the meaning for genome
  /// for the Portfolio Value Oscillator strategy
  /// </summary>
  [Serializable]
  public class GenomeMeaningPVO : GenomeMeaning
  {
  	private double oversoldThreshold;
  	private double overboughtThreshold;
  	private int numDaysForOscillatingPeriod;
    
    public double OversoldThreshold
    {
      get{return this.oversoldThreshold;}
    }
     
    public double OverboughtThreshold
    {
      get{return this.overboughtThreshold;}
    }
		
    public int NumDaysForOscillatingPeriod
    {
      get{return this.numDaysForOscillatingPeriod;}
    }
    
    public GenomeMeaningPVO(string[] tickers,
                            double[] tickersPortfolioWeights,
                            double oversoldThreshold,
                            double overboughtThreshold,
                           	int numDaysForOscillatingPeriod):
                            
                            base(tickers, tickersPortfolioWeights)
                            
    {
 			this.oversoldThreshold = oversoldThreshold;
 			this.overboughtThreshold = overboughtThreshold;
 			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
		}
    
    public GenomeMeaningPVO(string[] tickers,
                            double oversoldThreshold,
                            double overboughtThreshold,
                            int numDaysForOscillatingPeriod):
                            
                            base(tickers)
                            
    {
      this.oversoldThreshold = oversoldThreshold;
      this.overboughtThreshold = overboughtThreshold;
      this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
    }

  }
}
