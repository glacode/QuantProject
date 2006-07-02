/*
QuantProject - Quantitative Finance Library

FixedPeriodOscillatorStrategy.cs
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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Close To Close Oscillator strategy
	/// </summary>
	[Serializable]
	public class FixedPeriodOscillatorStrategy : IEndOfDayStrategy
	{
		private Account account;
		private string[] signedTickers;
		private double[] weightsForSignedTickers;
    private int daysForRightPeriod;
    private int daysForReversalPeriod;
    //length for movement upwards or downwards of the given tickers
    private int daysCounterWithRightPositions;
    private int daysCounterWithReversalPositions;
    private bool isReversalPeriodOn = false;
    private int numOfClosesElapsed = 0;
     

		public FixedPeriodOscillatorStrategy( Account account ,
			                                     string[] signedTickers,
			                                     double[] weightsForSignedTickers,
                                           int daysForRightPeriod,
                                           int daysForReversalPeriod)
		{
			this.account = account;
			this.signedTickers = signedTickers;
			this.weightsForSignedTickers = weightsForSignedTickers;
      this.daysForRightPeriod = daysForRightPeriod;
      this.daysForReversalPeriod = daysForReversalPeriod;
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
		
    public void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
		}
		
    private void marketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( Position position in this.account.Portfolio.Positions )
				tickers.Add( position.Instrument.Key );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
		}

    private void marketCloseEventHandler_reverseSignOfTickers()
    {
      for(int i = 0; i<this.signedTickers.Length; i++)
      {
        if(this.signedTickers[i] != null)
        {
          if(this.signedTickers[i].StartsWith("-"))
            this.signedTickers[i] =
              GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.signedTickers[i]);
          else
            this.signedTickers[i] = "-" + this.signedTickers[i];
        }
      }
    }

    private void marketCloseEventHandler_updateCounters(bool isTheFirstClose)
    {
      if(this.account.Portfolio.Count > 0 && isTheFirstClose == false)
      {
        if(this.isReversalPeriodOn)
          this.daysCounterWithReversalPositions++ ;
        else
          this.daysCounterWithRightPositions++ ;
      }
    }

		public void MarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      bool firstClose = false;
      if( (this.numOfClosesElapsed + 1) >= 
          (this.daysForRightPeriod + this.daysForReversalPeriod) )
      //strategy can now be applied because it is tuned with the optimization's results
      {
        if (this.account.Transactions.Count == 0)
          // it is the first close
        {
          firstClose = true;
          this.account.AddCash( 30000 );
          this.marketCloseEventHandler_addOrders();
        }

        this.marketCloseEventHandler_updateCounters(firstClose);

        if(firstClose == false && this.isReversalPeriodOn == false &&
           this.daysCounterWithRightPositions == this.daysForRightPeriod)
      
        {
          this.marketCloseEventHandler_closePositions();
          this.daysCounterWithRightPositions = 0;
          this.marketCloseEventHandler_reverseSignOfTickers();
          this.marketCloseEventHandler_addOrders();
          this.isReversalPeriodOn = true;
        }
      
        if(this.isReversalPeriodOn == true &&
           this.daysCounterWithReversalPositions == this.daysForReversalPeriod)
      
        {
          this.marketCloseEventHandler_closePositions();
          this.daysCounterWithReversalPositions = 0;
          this.marketCloseEventHandler_reverseSignOfTickers();
          this.marketCloseEventHandler_addOrders();
          this.isReversalPeriodOn = false;
        }
      }
      
      this.numOfClosesElapsed++;

		}
		
    public void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
