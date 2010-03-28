/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 17/02/2010
 * Time: 18.28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Data;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Selects from a given group of tickers, those tickers which
	/// are traded at least at a given percentage of DateTime(s)
	/// </summary>
	public class OftenExchanged : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;

		private string tickersGroupID;
		private ITickerSelectorByGroup tickerSelectorByGroup;
		private IHistoricalMarketValueProvider historicalMarketValueProvider;
		private double minPercentageOfDateTimesWithMarketValues;

		public string Description
		{
			get{
				return "From_" + this.tickersGroupID +
					this.historicalMarketValueProvider.Description +
					"MinPercentageOfDateTimesWithMarketValues_" +
					this.minPercentageOfDateTimesWithMarketValues;
			}
		}

		/// <summary>
		/// Selects from a given group of tickers, those tickers which
		/// are traded at least at a given percentage of DateTime(s)
		/// </summary>
		/// <param name="tickersGroupID"></param>
		/// <param name="historicalMarketValueProvider"></param>
		/// <param name="minPercentageOfDateTimesWithMarketValues">values between
		/// 0 and 1</param>
		public OftenExchanged(
			string tickersGroupID ,
			ITickerSelectorByGroup tickerSelectorByGroup ,
			IHistoricalMarketValueProvider historicalMarketValueProvider ,
			double minPercentageOfDateTimesWithMarketValues )
		{
			this.oftenExchanged_checkParameter( minPercentageOfDateTimesWithMarketValues );
			this.tickersGroupID = tickersGroupID;
			this.tickerSelectorByGroup = tickerSelectorByGroup;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.minPercentageOfDateTimesWithMarketValues =
				minPercentageOfDateTimesWithMarketValues;
		}
		private void oftenExchanged_checkParameter(
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
		
		#region getEligibleTickers_actually
		
		#region getOftenExchanged
		
		#region addCurrentTickerIfItIsOftenExchanged
		private void addCurrentTickerIfItIsOftenExchanged_checkParameter( DataRow dataRow )
		{
			if ( !( dataRow[ 0 ] is string ) )
				throw new Exception( "The datatable of eligible tickers is " +
				                    "expected to have a single element in each " +
				                    "DataRow and that element should be a string" );
		}
		private void addCurrentTickerIfItIsOftenExchangedWithValidDataRow(
			string candidate , History history , List<string> tickerList )
		{
//			string candidate = (string)tickerFromGroup[ 0 ];
			History dateTimesWithMarketValues =
				this.historicalMarketValueProvider.GetDateTimesWithMarketValues(
					candidate , history );
			bool isOftenExchanged =	dateTimesWithMarketValues.ContainsAtAGivenPercentageDateTimesIn(
				history ,
				this.minPercentageOfDateTimesWithMarketValues * 100 );
			if ( isOftenExchanged )
				tickerList.Add( candidate );
		}
		private void addCurrentTickerIfItIsOftenExchanged(
			string tickerFromGroup , History history , List<string> tickerList )
		{
//			this.addCurrentTickerIfItIsOftenExchanged_checkParameter( dataRow );
			this.addCurrentTickerIfItIsOftenExchangedWithValidDataRow(
				tickerFromGroup , history , tickerList );
		}
		#endregion addCurrentTickerIfItIsOftenExchanged
		
		private List<string> getOftenExchanged( List<string> tickersFromGroup , History history )
		{
			List<string> oftenExchanged = new List<string>();
			foreach ( string tickerFromGroup in tickersFromGroup )
				this.addCurrentTickerIfItIsOftenExchanged(
					tickerFromGroup , history , oftenExchanged );
//			string[] oftenExchanged = oftenExchanged.ToArray();
			return oftenExchanged;
		}
		#endregion getOftenExchanged
		
		private EligibleTickers getEligibleTickers_actually( History history )
		{
			DateTime currentDate = history.LastDateTime;

//			SelectorByGroup group;
//			if(this.temporizedGroup)
//				//the group is "temporized": returned set of tickers
//				// depend on time
//				group = new SelectorByGroup(this.tickersGroupID,
//				                            currentDate);
//			else//the group is not temporized
//				group = new SelectorByGroup(this.tickersGroupID);
//			group = new SelectorByGroup( this.tickersGroupID , currentDate );
//			DataTable tickersFromGroup = group.GetTableOfSelectedTickers();
			
			List<string> tickersFromGroup = this.tickerSelectorByGroup.GetSelectedTickers(
				this.tickersGroupID , currentDate );
			
			List<string> oftenExchanged = this.getOftenExchanged( tickersFromGroup , history );
			
			EligibleTickers eligibleTickers = new EligibleTickers( oftenExchanged );
			
			return eligibleTickers;
		}
		#endregion getEligibleTickers_actually
		
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

		public EligibleTickers GetEligibleTickers(
			History history )
		{
			EligibleTickers eligibleTickers =
				this.getEligibleTickers_actually( history );
			this.getEligibleTickers_sendNewMessage( eligibleTickers );
			return eligibleTickers;
		}
		#endregion GetEligibleTickers
	}
}
