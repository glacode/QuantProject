/*
QuantProject - Quantitative Finance Library

GenomeMeaning.cs
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

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the class representing the meaning for genome
  /// </summary>
  [Serializable]
  public class GenomeMeaning
  {
  	  	
  	private string[] tickers;
  	private double[] tickersPortfolioWeights;
  	private double returnAtLastDayInSample;
  	private double averageReturnInSample;
  	private double varianceReturnInSample;
    private string hashCodeForTickerComposition;
    
  	public string[] Tickers
    {
      get{return this.tickers;}
    }
    
    public double ReturnAtLastDayInSample
    {
      get{return this.returnAtLastDayInSample;}
    }
    
    public double AverageReturnInSample
    {
      get{return this.averageReturnInSample;}
    }
    
    public double VarianceReturnInSample
    {
      get{return this.varianceReturnInSample;}
    }
    
    public double[] TickersPortfolioWeights
    {
      get{return this.tickersPortfolioWeights;}
    }
    
    public string HashCodeForTickerComposition
    {
      get{return this.hashCodeForTickerComposition;}
    }

    private void setDefaultTickersPortfolioWeights()
    {
    	this.tickersPortfolioWeights = new double[this.tickers.Length];
    	for(int i = 0;i<this.tickers.Length;i++)
    		this.tickersPortfolioWeights[i]=1.0/this.tickers.Length;
		}
    
    private void genomeMeaning_setHashCodeForTickerComposition()
    {
      ArrayList listOfTickers = new ArrayList(this.tickers);
      listOfTickers.Sort();
      foreach(string tickerCode in listOfTickers)
        this.hashCodeForTickerComposition += tickerCode;
    }

    private void genomeMeaning(string[] tickers)
    {
      this.tickers = tickers;
      this.genomeMeaning_setHashCodeForTickerComposition();
    }

    public GenomeMeaning(string[] tickers)
    {
 			this.genomeMeaning(tickers);
 			this.setDefaultTickersPortfolioWeights();
		}
    
    public GenomeMeaning(string[] tickers, double[] tickersPortfolioWeights)
    {
 			this.genomeMeaning(tickers);
 			this.tickersPortfolioWeights = tickersPortfolioWeights;
		}
    
    public GenomeMeaning(string[] tickers,
                            double returnAtLastDayInSample, 
  													 double averageReturnInSample,
  													 double varianceReturnInSample)
    {
 			this.genomeMeaning(tickers);
 			this.returnAtLastDayInSample = returnAtLastDayInSample;
 			this.averageReturnInSample = averageReturnInSample;
 			this.varianceReturnInSample = varianceReturnInSample;
 			this.setDefaultTickersPortfolioWeights();
		}
    
    public GenomeMeaning(string[] tickers,
                         double[] tickersPortfolioWeights,
                            double returnAtLastDayInSample, 
  													 double averageReturnInSample,
  													 double varianceReturnInSample)
    {
 			this.genomeMeaning(tickers);
 			this.tickersPortfolioWeights = tickersPortfolioWeights;
 			this.returnAtLastDayInSample = returnAtLastDayInSample;
 			this.averageReturnInSample = averageReturnInSample;
 			this.varianceReturnInSample = varianceReturnInSample;
		}
    
  }

}
