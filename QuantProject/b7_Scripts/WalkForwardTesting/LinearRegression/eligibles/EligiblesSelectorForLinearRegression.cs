/*
QuantProject - Quantitative Finance Library

EligiblesSelectorForLinearRegression.cs
Copyright (C) 2010
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
using System.Data;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Data.Selectors;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Eligibles selector to be used by the Linear Regression strategy
	/// </summary>
	[Serializable]
	public class EligiblesSelectorForLinearRegression : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;

		private string groupIdForTradingTickers;
		private	string groupIdForAdditionalSignalingTickers;
		private ITickerSelectorByGroup tickerSelectorByGroup;
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private double minPercentageOfDateTimesWithMarketValues;
		private double minPriceForTradingTicker;
		private double maxPriceForTradingTicker;
		private	int maxNumberOfEligiblesForTrading;

		private EligibleTickers eligibleTickersForTrading;
		private EligibleTickers eligibleTickersForSignaling;

		public EligibleTickers EligibleTickersForTrading {
			get { return eligibleTickersForTrading; }
		}
		public EligibleTickers EligibleTickersForSignaling {
			get { return this.eligibleTickersForSignaling; }
		}
		
		public string Description
		{
			get{
				string description =
					"TradingGroup_" + this.groupIdForTradingTickers + "\n" +
					"SignalingGroup_" + this.groupIdForAdditionalSignalingTickers + "\n" +
					"MaxNumberOfTrdingTickers_" + this.maxNumberOfEligiblesForTrading + "\n" +
					"MinPrercentageOfAvailableMarketValues_" +
					this.minPercentageOfDateTimesWithMarketValues + "\n" +
					"From_" + this.minPriceForTradingTicker +
					"_to_" + this.maxPriceForTradingTicker + "\n" +
					"Eligibles For Linear Regression";
				return description;
			}
		}
		
		public EligiblesSelectorForLinearRegression(
			string groupIdForTradingTickers ,
			string groupIdForAdditionalSignalingTickers ,
			ITickerSelectorByGroup tickerSelectorByGroup ,
			Benchmark benchmark ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double minPercentageOfDateTimesWithMarketValues ,
			double minPriceForTradingTicker ,
			double maxPriceForTradingTicker ,
			int maxNumberOfEligiblesForTrading
		)
		{
			this.eligiblesSelectorForLinearRegression_checkParameters
				( minPercentageOfDateTimesWithMarketValues );
			this.groupIdForTradingTickers = groupIdForTradingTickers;
			this.groupIdForAdditionalSignalingTickers = groupIdForAdditionalSignalingTickers;
			this.tickerSelectorByGroup = tickerSelectorByGroup;
			this.benchmark = benchmark;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.minPercentageOfDateTimesWithMarketValues =
				minPercentageOfDateTimesWithMarketValues;
			this.minPriceForTradingTicker = minPriceForTradingTicker;
			this.maxPriceForTradingTicker = maxPriceForTradingTicker;
			this.maxNumberOfEligiblesForTrading = maxNumberOfEligiblesForTrading;
		}
		private void eligiblesSelectorForLinearRegression_checkParameters(
			double minPercentageOfDateTimesWithMarketValues )
		{
			if ( minPercentageOfDateTimesWithMarketValues < 0 )
				throw new Exception(
					"minPercentageOfDateTimesWithMarketValues cannot be less than zero!" );
			if ( minPercentageOfDateTimesWithMarketValues > 1 )
				throw new Exception(
					"minPercentageOfDateTimesWithMarketValues cannot be more than 1!" );				                    
		}
		
		#region GetEligibleTickers
		
		#region setEligibleTickers
		private EligibleTickers getTickersWithEnoughTradingDays(
			string groupId , History history )
		{
			IEligiblesSelector oftenExchanged = new OftenExchanged(
				this.groupIdForTradingTickers ,
				this.tickerSelectorByGroup ,
				this.historicalMarketValueProvider ,
				this.minPercentageOfDateTimesWithMarketValues );
			EligibleTickers tickersFromTradingGroupWithEnoughTradingDays =
				oftenExchanged.GetEligibleTickers( history );
//			DateTime currentDateTime = history.LastDateTime;
//			SelectorByGroup tradingGroup = new SelectorByGroup(
//				this.groupIdForTradingTickers , currentDate);
//			DataTable tickersFromTheTradingGroup = group.GetTableOfSelectedTickers();
			return tickersFromTradingGroupWithEnoughTradingDays;
		}
		private EligibleTickers getEligibleTickersForTrading(
			EligibleTickers tickersFromTradingGroupWithEnoughTradingDays ,
			History history )
		{
			DataTable tickersWithEnoughTradingDays =
				TickerSelector.GetDataTableForTickerSelectors(
					tickersFromTradingGroupWithEnoughTradingDays.Tickers );
			
			DateTime dateTime = history.LastDateTime;
			SelectorByAverageRawOpenPrice byPrice =
				new SelectorByAverageRawOpenPrice(
					tickersWithEnoughTradingDays , false ,
					dateTime.AddDays( -15 ) , dateTime ,
					this.maxNumberOfEligiblesForTrading * 10 ,
					this.minPriceForTradingTicker ,
					this.maxPriceForTradingTicker ,
					0.000001 , 10000000 );
			DataTable tickersWithinPriceRange = byPrice.GetTableOfSelectedTickers();
			
			SelectorByLiquidity byLiquidity =
				new SelectorByLiquidity(
					tickersWithinPriceRange , true,
					dateTime.AddDays( -15 ) , dateTime ,
					this.maxNumberOfEligiblesForTrading );
			DataTable liquidTickers = byLiquidity.GetTableOfSelectedTickers();
			
			EligibleTickers eligibleTickersForTrading = new EligibleTickers( liquidTickers );
			return eligibleTickersForTrading;
		}
		private EligibleTickers getEligibleTickersForSignaling(
			EligibleTickers tickersFromTradingGroupWithEnoughTradingDays ,
			History history )
		{
			EligibleTickers eligibleTickers =
				this.getTickersWithEnoughTradingDays(
					this.groupIdForAdditionalSignalingTickers , history );
			eligibleTickers.AddAdditionalEligibles(
				tickersFromTradingGroupWithEnoughTradingDays );
			return eligibleTickers;
		}
		private void setEligibleTickers(
			History history )
		{
			EligibleTickers tickersFromTradingGroupWithEnoughTradingDays =
				this.getTickersWithEnoughTradingDays(
					this.groupIdForTradingTickers , history );
			this.eligibleTickersForTrading = this.getEligibleTickersForTrading(
				tickersFromTradingGroupWithEnoughTradingDays , history );
			this.eligibleTickersForSignaling = this.getEligibleTickersForSignaling(
				tickersFromTradingGroupWithEnoughTradingDays , history );
		}
		#endregion setEligibleTickers
		
		private void getEligibleTickers_sendNewMessage(
			EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if(this.NewMessage != null)
				this.NewMessage( this , newMessageEventArgs );
		}
		
		/// <summary>
		/// Returns the eligible tickers for trading, but it sets also
		/// the eligibles tickers for signaling
		/// </summary>
		/// <returns></returns>
		public EligibleTickers GetEligibleTickers(
			History history )
		{
			this.setEligibleTickers( history );
			this.getEligibleTickers_sendNewMessage( this.eligibleTickersForTrading );
			return this.eligibleTickersForTrading;
		}
		#endregion GetEligibleTickers
		


	}
}
