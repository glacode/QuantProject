/*
QuantProject - Quantitative Finance Library

ExtremeCounterTrendStrategy.cs
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
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Strategy that implies an immediate reversal in gain
	/// for the next period
	/// </summary>
	[Serializable]
	public class ExtremeCounterTrendStrategy : IEndOfDayStrategy
	{
		private Account account;
		private string[] signedTickers;
		private double[] weightsForSignedTickers;
		private int numDaysForReturnCalculation;
    private int numOfClosesElapsed = 0;
    private int numOfDaysWithOpenPosition = 0;
    private PortfolioType portfolioType;

		public ExtremeCounterTrendStrategy( Account account ,
			                              string[] signedTickers,
			                              double[] weightsForSignedTickers,
                                    int numDaysForReturnCalculation,
                                    PortfolioType portfolioType)
		{
			this.account = account;
			this.signedTickers = signedTickers;
			this.weightsForSignedTickers = weightsForSignedTickers;
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.portfolioType = portfolioType;
		}
    
    public void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    }
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }

    private long marketCloseEventHandler_addOrder_getQuantity(
			int indexForSignedTicker)
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( SignedTicker.GetTicker(this.signedTickers[indexForSignedTicker]) );
			double maxPositionValueForThisTicker =
				accountValue*this.weightsForSignedTickers[indexForSignedTicker];
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void marketCloseEventHandler_addOrder( int indexForSignedTicker )
		{
			OrderType orderType = GenomeRepresentation.GetOrderType( this.signedTickers[indexForSignedTicker] );
			string ticker = GenomeRepresentation.GetTicker( this.signedTickers[indexForSignedTicker] );
			long quantity = marketCloseEventHandler_addOrder_getQuantity( indexForSignedTicker );
			Order order = new Order( orderType , new Instrument( ticker ) ,
				quantity );
			this.account.AddOrder( order );
		}
		private void marketCloseEventHandler_addOrders()
		{
			for(int i = 0; i<this.signedTickers.Length; i++)
				marketCloseEventHandler_addOrder( i );
		}
  
    private double marketCloseEventHandler_openPositions_getLastHalfPeriodGain(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      try
      {
		    DateTime initialDateForHalfPeriod = 
	          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForReturnCalculation + 1]["quDate"];
	      DateTime finalDateForHalfPeriod = 
	        (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
      	returnValue =
	      	 SignedTicker.GetCloseToClosePortfolioReturn(
	      	     this.signedTickers,initialDateForHalfPeriod,finalDateForHalfPeriod);
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

    private void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      double lastHalfPeriodGain = 
      		this.marketCloseEventHandler_openPositions_getLastHalfPeriodGain(timer);
    	if(lastHalfPeriodGain != 999.0)
    	//last half period gain has been properly computed
    	{
    		if(lastHalfPeriodGain < 0.0)
    			this.marketCloseEventHandler_addOrders();
    		else if (lastHalfPeriodGain > 0.0 &&
                 this.portfolioType == PortfolioType.ShortAndLong )
        //if gain of the last half period is positive and
        //original positions can be reversed        
    		{
    			SignedTicker.ChangeSignOfEachTicker(this.signedTickers);
    			//short the portfolio (short --> long; long --> short)
    			try{
            this.marketCloseEventHandler_addOrders();
          }
    			catch(Exception ex)
          {
            ex = ex;
          }
    			finally{
            SignedTicker.ChangeSignOfEachTicker(this.signedTickers);
          }
    		}
    	}
    }
    
    private void marketCloseEventHandler_closePositions()
    {
      ArrayList tickers = new ArrayList();
      foreach ( Position position in this.account.Portfolio.Positions )
        tickers.Add( position.Instrument.Key );
      foreach ( string ticker in tickers )
        this.account.ClosePosition( ticker );
      this.numOfDaysWithOpenPosition = 0;
    }
        
    public void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.numOfClosesElapsed++;

      if(this.account.Transactions.Count == 0)
        this.account.AddCash(30000);
      
      if(this.account.Portfolio.Count > 0)
        this.numOfDaysWithOpenPosition++;
      
      if(this.numOfDaysWithOpenPosition == this.numDaysForReturnCalculation)
          this.marketCloseEventHandler_closePositions();
     
      if(this.account.Portfolio.Count == 0 &&
          this.numOfClosesElapsed >= this.numDaysForReturnCalculation)
      //portfolio is empty and previous closes can be now checked
      		this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
    }
		
    public void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
