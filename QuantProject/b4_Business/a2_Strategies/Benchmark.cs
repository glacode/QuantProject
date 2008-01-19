using System;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Either a security or an index that is used to time a strategy and/or
	/// to compare a strategy's results
	/// </summary>
	public class Benchmark
	{
		private string ticker;
		private HistoricalQuoteProvider historicalQuoteProvider;
		
		public string Ticker
		{
			get { return this.ticker; }
		}

		public Benchmark( string ticker )
		{
			this.ticker = ticker;

			this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}

		#region GetTimeStep
		private void getTimeStep_checkParameters(
			EndOfDayDateTime timeStepBegin )
		{
			if ( !this.historicalQuoteProvider.WasExchanged( this.ticker ,
				timeStepBegin ) )
				throw new Exception( "The benchmark '" + this.ticker +
					"' was not exchanged at the given timeStepBegin " +
					timeStepBegin.ToString() );
		}
		private void isExchanged_checkParameter(
			EndOfDayDateTime endOfDayDateTime )
		{
			if ( endOfDayDateTime.DateTime.Year >
				DateTime.Now.Year )
				throw new Exception( "It looks like this is a loop! " +
					"You have probably asked for a GetTimeStep, but " +
					"there is no other trading day for this benchmark: '" +
					this.ticker + "'" );
		}
		private bool isExchanged( EndOfDayDateTime endOfDayDateTime )
		{
			this.isExchanged_checkParameter( endOfDayDateTime );
			bool isTraded =	this.historicalQuoteProvider.WasExchanged(
				this.ticker , endOfDayDateTime );
			return isTraded;
		}
		private ReturnInterval getTimeStep_actually(
			EndOfDayDateTime timeStepBegin )
		{
			EndOfDayDateTime currentEndOfDayDateTime =
				timeStepBegin.GetNextMarketStatusSwitch();
			while ( !this.isExchanged( currentEndOfDayDateTime ) )
				currentEndOfDayDateTime =
				currentEndOfDayDateTime.GetNextMarketStatusSwitch();
			ReturnInterval timeStep =
				new ReturnInterval( timeStepBegin , currentEndOfDayDateTime );
			return timeStep;
		}
		/// <summary>
		/// Returns the benchmark's time step that begins at timeStepBegin
		/// </summary>
		/// <param name="timeStepBegin"></param>
		/// <returns></returns>
		public ReturnInterval GetTimeStep( EndOfDayDateTime timeStepBegin )
		{
			this.getTimeStep_checkParameters( timeStepBegin );
			ReturnInterval timeStep = this.getTimeStep_actually( timeStepBegin );
			return timeStep;
		}
		#endregion GetTimeStep

		/// <summary>
		/// Returns either the next market close or the next market open
		/// (when the benchmark is exchanged), whichever is the nearest. 
		/// If endOfDayDateTime is either a market close or a market open
		/// when the benchmark is exchanged, then endOfDayDateTime itself
		/// is returned
		/// </summary>
		/// <param name="endOfDayDateTime"></param>
		/// <returns></returns>
		public EndOfDayDateTime GetThisOrNextMarketStatusSwitch(
			EndOfDayDateTime endOfDayDateTime )
		{
			EndOfDayDateTime currentEndOfDayDateTime = endOfDayDateTime;
			while ( !this.isExchanged( currentEndOfDayDateTime ) )
				currentEndOfDayDateTime =
					currentEndOfDayDateTime.GetNextMarketStatusSwitch();
			return currentEndOfDayDateTime;
		}

	}
}
