/*
QuantProject - Quantitative Finance Library

BestTickersScreener.cs
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
using System.Data;
using System.Collections;
using QuantProject.ADT.Statistics;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
  /// <summary>
  /// Base class for tickers' selection without combination 
  /// </summary>
  [Serializable]
  public class BestTickersScreener
  {
    protected DataTable setOfTickers;//used only for keeping
                                     //the same signature for 
                                     //protected retrieveData() method
    protected CandidatePropertiesForSimpleSelection[] setOfCandidates;
    protected DateTime firstQuoteDate;
    protected DateTime lastQuoteDate;
    protected double targetPerformance;
    protected PortfolioType portfolioType;

    //setOfInitialTickers has to contain the
    //ticker's symbol in the first column !

    public BestTickersScreener(DataTable setOfInitialTickers,
																DateTime firstQuoteDate,
																DateTime lastQuoteDate,
																double targetPerformance,
                                PortfolioType portfolioType)
                          
    {
 			this.setOfTickers = setOfInitialTickers;
			if ( setOfInitialTickers.Rows.Count == 0 )
				throw new Exception( "setOfInitialTickers cannot be empty!" );
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.targetPerformance = targetPerformance;
      this.portfolioType = portfolioType;
    }
        
    //this protected method has to be called by inherited classes
    //(open to close or close to close) 
    //only after all initializations provided
    //by their respective constructors
    protected void retrieveData()
    {
      this.setOfCandidates = new CandidatePropertiesForSimpleSelection[setOfTickers.Rows.Count];
      for(int i = 0; i<setOfTickers.Rows.Count; i++)
      {
        string ticker = (string)setOfTickers.Rows[i][0];
        this.setOfCandidates[i] = new CandidatePropertiesForSimpleSelection(ticker,
                                      this.getArrayOfRatesOfReturn(ticker), this.targetPerformance);
      }
    }

    //this protected method must be overriden by inherited classes
    //specifing the type of rates of return that have to 
    //be analyzed
    protected virtual float[] getArrayOfRatesOfReturn(string ticker)
    {
    	float[] returnValue = null;
    	return returnValue;
    }
    
    private void setSetOfCandidates(bool longRatesOfReturn)
    {
      for(int i = 0; i<this.setOfCandidates.Length;i++)
      {
      	this.setOfCandidates[i].LongRatesOfReturn = longRatesOfReturn;
      	this.setOfCandidates[i].setFitness();
      }
      Array.Sort(this.setOfCandidates);
    }

    private void getBestTickers_setReturnValueForOnlyLong(string[] returnValue)
    {
      this.setSetOfCandidates(true);
      for(int i = 0; i<returnValue.Length;i++)
      	//the last items of the array are the best tickers for long trades
         returnValue[i] = this.setOfCandidates[this.setOfCandidates.Length - i - 1].Ticker;
    }
    private void getBestTickers_setReturnValueForOnlyShort(string[] returnValue)
    {
      this.setSetOfCandidates(true);
      for(int i = 0; i<returnValue.Length;i++)
      	//the first items of the array are surely the best tickers for short trades
        returnValue[i] = "-" + this.setOfCandidates[i].Ticker;
    }
    /*OLD IMPLEMENTATION: HALF SHORT HALF LONG
    private void getBestTickers_setReturnValueForShortAndLong(string[] returnValue)
    {
      this.getBestTickers_setReturnValueForOnlyLong(returnValue);
      //the first half is for long tickers
      int numberOfTickersForShort = returnValue.Length / 2;
     	for(int i = 0; i<numberOfTickersForShort;i++)
        returnValue[i] = "-" + this.setOfCandidates[i].Ticker;
      //the second half is for short tickers (if returnValue.Length is 
      //odd, n° of short tickers = n° of long tickers - 1
    }
    */
    //new implementation tickers with highest fitness,
    //short or long
    private void getBestTickers_setReturnValueForShortAndLong(string[] returnValue)
    {
      CandidateProperties[] allTickersShortOrLong = 
      		new CandidateProperties[this.setOfCandidates.Length * 2];
      int numOfTickers = this.setOfCandidates.Length;
      this.setSetOfCandidates(true);
      for(int i = 0; i<numOfTickers; i++)
       	allTickersShortOrLong[i] = 
      		new CandidateProperties(this.setOfCandidates[i].Ticker,
      		                        this.setOfCandidates[i].ArrayOfRatesOfReturn);
      
 			this.setSetOfCandidates(false);
      for(int i = numOfTickers; i<numOfTickers*2; i++)
       	allTickersShortOrLong[i] = 
      		new CandidateProperties("-" + this.setOfCandidates[i].Ticker,
      		                        this.setOfCandidates[i].ArrayOfRatesOfReturn); 
      Array.Sort(allTickersShortOrLong);
      for(int i = 0; i<returnValue.Length; i++)
        returnValue[i] = allTickersShortOrLong[i].Ticker;
      
    }
    /// <summary>
    /// It gets best tickers for trading
    /// If PortfolioType is ShortAndLong, half of numberOfTickers is for short and
    /// the remaining half for long. Tickers for short trading are signed!
    /// </summary>
    public string[] GetBestTickers(int numberOfTickers)
    {
      string[] returnValue = new string[numberOfTickers];
      if(this.portfolioType == PortfolioType.OnlyLong)
        this.getBestTickers_setReturnValueForOnlyLong(returnValue);  
      else if(this.portfolioType == PortfolioType.OnlyShort)
        this.getBestTickers_setReturnValueForOnlyShort(returnValue);
      else if(this.portfolioType == PortfolioType.ShortAndLong)
        this.getBestTickers_setReturnValueForShortAndLong(returnValue);
      
      return returnValue;
    }
    
    
    
  }

}
