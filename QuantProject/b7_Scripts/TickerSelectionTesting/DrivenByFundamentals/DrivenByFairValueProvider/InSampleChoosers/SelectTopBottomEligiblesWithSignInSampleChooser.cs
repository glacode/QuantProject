/*
QuantProject - Quantitative Finance Library

SelectTopBottomEligiblesWithSignInSampleChooser.cs
Copyright (C) 2011
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
using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies; 
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;


namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers
{
	/// <summary>
	/// Select TestingPositions half from top eligible tickers, 
	/// and half from bottom eligibles, without doing anything but
	/// distinguishing between tickers which have to
	/// be sell-shorted (from bottom) or bought (from top) according to a
	/// strategy based on a fundamental fair value
	/// provider
	/// </summary>
	[Serializable]
	public class SelectTopBottomEligiblesWithSignInSampleChooser : IInSampleChooser
	{
		public event NewMessageEventHandler NewMessage;
		public event NewProgressEventHandler NewProgress;
		
		protected int maxNumOfTestingPositionsToBeReturned;
		protected int numOfTickersInEachTestingPosition;
		protected HistoricalMarketValueProvider historicalMarketValueProvider;
		protected	Timer timer;
		
		
		public string Description
		{
			get{ return "TopBottomEligiblesInSampleChooser"; }
		}
		
		public SelectTopBottomEligiblesWithSignInSampleChooser(int numOfTickersInEachTestingPosition,
		                                                 			 int maxNumOfTestingPositionsToBeReturned,
		                                                 			 HistoricalMarketValueProvider historicalMarketValueProvider,
		                                                 			 Timer timer)
		{
			if(numOfTickersInEachTestingPosition %2 != 0 )
				throw new Exception("numOfTickersInEachTestingPositions must be even!");
			this.numOfTickersInEachTestingPosition =
				numOfTickersInEachTestingPosition;
			this.maxNumOfTestingPositionsToBeReturned =
				maxNumOfTestingPositionsToBeReturned;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.timer = timer;
		}
		
		protected string[] analyzeInSample_getTestingPositionsArray_getAlternatedTickers_getTop(EligibleTickers eligibleTickers)
		{
			string[] returnValue = new string[eligibleTickers.Count];
			int idx = 0;
			DataTable tableOfEligibleTickers = eligibleTickers.SourceDataTable;
			double relativeDiffBetweenFairAndMarket;
			for( int i = 0; i < returnValue.Length; i++ )
			{
				relativeDiffBetweenFairAndMarket =
					(double)tableOfEligibleTickers.Rows[i]["RelativeDifferenceBetweenFairAndMarketPrice"];
				if(relativeDiffBetweenFairAndMarket >= 0)
				{
					returnValue[idx] = eligibleTickers.Tickers[i];
					idx++;
				}
			}
			return returnValue;
		}
		
		protected string[] analyzeInSample_getTestingPositionsArray_getAlternatedTickers_getBottom(EligibleTickers eligibleTickers)
		{
			string[] returnValue = new string[eligibleTickers.Count];
			int idxBottom = 0;
			DataTable tableOfEligibleTickers = eligibleTickers.SourceDataTable;
			int lastIdxOfEligibles = eligibleTickers.Count - 1;
			double relativeDiffBetweenFairAndMarket;
			for( int i = 0; i < returnValue.Length; i++ )
			{
				relativeDiffBetweenFairAndMarket =
					(double)tableOfEligibleTickers.Rows[lastIdxOfEligibles - i]["RelativeDifferenceBetweenFairAndMarketPrice"];
				if(relativeDiffBetweenFairAndMarket < 0)
				{
					returnValue[idxBottom] = "-" + eligibleTickers.Tickers[lastIdxOfEligibles - i];
					idxBottom++;
				}
			}
			return returnValue;
		}
		
		protected string[] analyzeInSample_getTestingPositionsArray_getAlternatedTickers(EligibleTickers eligibleTickers)
		{
			string[] returnValue = new string[eligibleTickers.Count];
			string[] topTickers = new string[eligibleTickers.Count];
			string[] bottomTickers = new string[eligibleTickers.Count];
			//creare due liste: top e bottom da cui recuperare i ticker
			topTickers = 
				analyzeInSample_getTestingPositionsArray_getAlternatedTickers_getTop(eligibleTickers);
			bottomTickers = 
				analyzeInSample_getTestingPositionsArray_getAlternatedTickers_getBottom(eligibleTickers);
			int idxTop = 0;
			int idxBottom = 0;
			for(int i = 0; i < returnValue.Length; i++)
			{
				if(i%2 == 0)
				//at even position, a top ticker ( a bottom if top is null )
				{
					if(topTickers[idxTop] != null)
					{
						returnValue[i] = topTickers[idxTop];
						idxTop++;
					}
					else if(bottomTickers[idxBottom] != null)
					{
						returnValue[i] = bottomTickers[idxBottom];
						idxBottom++;
					}
				}
				else
				//at odd, a bottom ( a top, if bottom is null )
				{
					if(bottomTickers[idxBottom] != null)
					{
						returnValue[i] = bottomTickers[idxBottom];
						idxBottom++;
					}
					else if (topTickers[idxTop] != null)
					{
						returnValue[i] = topTickers[idxTop];
						idxTop++;
					}
				}	
			}
			return returnValue;
		}
		
		#region analyzeInSample_getTestingPositionsArray_getTheoreticalProfit
		private double analyzeInSample_getTestingPositionsArray_getTheoreticalProfit_getFairPrice(WeightedPosition position,
		                                                                                          EligibleTickers eligibleTickers)
		{
			double returnValue;
			object[] keys = new object[1];
			keys[0] = position.Ticker;
			DataRow foundRow = 
				eligibleTickers.SourceDataTable.Rows.Find(keys);
			returnValue = (double)foundRow["FairPrice"];
			
			return returnValue;
		}
		
		private double analyzeInSample_getTestingPositionsArray_getTheoreticalProfit_getBuyPrice(WeightedPosition position,
		                                                                                        	EligibleTickers eligibleTickers)
		{
			double returnValue;
			returnValue = 
				this.historicalMarketValueProvider.GetMarketValue(position.Ticker,
				                                                  this.timer.GetCurrentDateTime() );
			return returnValue;
		}
				
		protected double analyzeInSample_getTestingPositionsArray_getTheoreticalProfit(WeightedPositions weightedPositions,
		                                                                               EligibleTickers eligibleTickers)
		{
			double returnValue = 0.0;
			double marketPriceOfCurrentPosition;
			double fairPriceOfCurrentPosition;
			double weightOfCurrentPosition;
			try{
				foreach( WeightedPosition position in weightedPositions )
				{
					marketPriceOfCurrentPosition = this.analyzeInSample_getTestingPositionsArray_getTheoreticalProfit_getBuyPrice(position, eligibleTickers);
					fairPriceOfCurrentPosition = this.analyzeInSample_getTestingPositionsArray_getTheoreticalProfit_getFairPrice(position, eligibleTickers);
					weightOfCurrentPosition = position.Weight;
					returnValue +=
						weightOfCurrentPosition*(fairPriceOfCurrentPosition-marketPriceOfCurrentPosition)/
						marketPriceOfCurrentPosition;
				}
			}
			catch(Exception ex)
    	{
				string s = ex.ToString();
			}
			return returnValue;
		}
		#endregion analyzeInSample_getTestingPositionsArray_getTheoreticalProfit
		
		protected GeneticallyOptimizableTestingPositions[] analyzeInSample_getTestingPositionsArray(EligibleTickers eligibleTickers)
		{
			if( eligibleTickers.Count < 
			    (this.maxNumOfTestingPositionsToBeReturned *
			     this.numOfTickersInEachTestingPosition) )
				throw new Exception("Not enough eligibles !");
			
			GeneticallyOptimizableTestingPositions[] returnValue =
				new GeneticallyOptimizableTestingPositions[ this.maxNumOfTestingPositionsToBeReturned ];
			int idxEligible = 0;
			string[] signedTickers = new string[numOfTickersInEachTestingPosition];
			string[] eligibleTickersAlternatedTopAndBottom = 
				analyzeInSample_getTestingPositionsArray_getAlternatedTickers(eligibleTickers);
			WeightedPositions chosenWeightedPositions;
			for ( int i = 0; i < returnValue.Length ; i++)
			{
				for ( int j = 0;
				      idxEligible < eligibleTickersAlternatedTopAndBottom.Length && j < numOfTickersInEachTestingPosition;
				      j++ )
				{
					signedTickers[j] = eligibleTickersAlternatedTopAndBottom[idxEligible + j];
				}
				chosenWeightedPositions = new WeightedPositions(new SignedTickers(signedTickers)); 
				returnValue[i] =
						new GeneticallyOptimizableTestingPositions(chosenWeightedPositions);
				returnValue[i].FitnessInSample = 
					this.analyzeInSample_getTestingPositionsArray_getTheoreticalProfit(chosenWeightedPositions, eligibleTickers);
				idxEligible = idxEligible + numOfTickersInEachTestingPosition;
			}
			return returnValue;
		}
		
		public object AnalyzeInSample( EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , new NewMessageEventArgs( "New" ) );
			if ( this.NewProgress != null )
				this.NewProgress( this , new NewProgressEventArgs( 1 , 1 ) );
			GeneticallyOptimizableTestingPositions[] returnValue =
				this.analyzeInSample_getTestingPositionsArray(eligibleTickers);
			
			return returnValue;
		}
	}
}
