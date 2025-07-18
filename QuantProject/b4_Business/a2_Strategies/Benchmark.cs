using System;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Either a security or an index that is used to time a strategy and/or
	/// to compare a strategy's results
	/// </summary>
	[Serializable]
	public class Benchmark
	{
		private string ticker;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		
		public string Ticker
		{
			get { return this.ticker; }
		}

		public Benchmark( string ticker )
		{
			this.ticker = ticker;

			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}

		public Benchmark(
			string ticker , HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.ticker = ticker;

			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}

		#region GetTimeStep
		private void getTimeStep_checkParameters(
			DateTime timeStepBegin )
		{
			if ( !this.historicalMarketValueProvider.WasExchanged( this.ticker ,
			                                                      timeStepBegin ) )
				throw new Exception( "The benchmark '" + this.ticker +
				                    "' was not exchanged at the given timeStepBegin " +
				                    timeStepBegin.ToString() );
		}
		private void isExchanged_checkParameter(
			DateTime dateTime )
		{
			if ( dateTime.Year >
			    DateTime.Now.Year )
				throw new Exception( "It looks like this is a loop! " +
				                    "You have probably asked for a GetTimeStep, but " +
				                    "there is no other trading day for this benchmark: '" +
				                    this.ticker + "'" );
		}
		private bool isExchanged( DateTime dateTime )
		{
			this.isExchanged_checkParameter( dateTime );
			bool isTraded =	this.historicalMarketValueProvider.WasExchanged(
				this.ticker , dateTime );
			return isTraded;
		}
		private ReturnInterval getTimeStep_actually(
			DateTime timeStepBegin )
		{
			DateTime currentDateTime =
				HistoricalEndOfDayTimer.GetNextMarketStatusSwitch( timeStepBegin );
//				timeStepBegin.GetNextMarketStatusSwitch();
			while ( !this.isExchanged( currentDateTime ) )
				currentDateTime =
					HistoricalEndOfDayTimer.GetNextMarketStatusSwitch( currentDateTime );
//				currentEndOfDayDateTime =
//				currentEndOfDayDateTime.GetNextMarketStatusSwitch();
			ReturnInterval timeStep =
				new ReturnInterval( timeStepBegin , currentDateTime );
			return timeStep;
		}
		/// <summary>
		/// Returns the benchmark's time step that begins at timeStepBegin
		/// </summary>
		/// <param name="timeStepBegin"></param>
		/// <returns></returns>
		public ReturnInterval GetTimeStep( DateTime timeStepBegin )
		{
			this.getTimeStep_checkParameters( timeStepBegin );
			ReturnInterval timeStep =
				this.getTimeStep_actually( timeStepBegin );
			return timeStep;
		}
		#endregion GetTimeStep

		/// <summary>
		/// Returns either the next market close or the next market open
		/// (when the benchmark is exchanged), whichever is the nearest.
		/// If dateTime is either a market close or a market open
		/// when the benchmark is exchanged, then dateTime itself
		/// is returned
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public DateTime GetThisOrNextMarketStatusSwitch(
			DateTime dateTime )
		{
			DateTime currentDateTime = dateTime;
			if ( !HistoricalEndOfDayTimer.IsMarketStatusSwitch( currentDateTime ) )
				currentDateTime =
					HistoricalEndOfDayTimer.GetNextMarketStatusSwitch(
						currentDateTime );
			while ( !this.isExchanged( currentDateTime ) )
				currentDateTime =
					HistoricalEndOfDayTimer.GetNextMarketStatusSwitch(
						currentDateTime );
//					currentEndOfDayDateTime.GetNextMarketStatusSwitch();
			return currentDateTime;
		}

		/// <summary>
		/// Returns the End of Day History of the benchmark
		/// between the two given EndOfDayDateTimes
		/// </summary>
		public History GetEndOfDayHistory(
			DateTime firstDateTime,
			DateTime lastDateTime )
		{
			if( lastDateTime <=	firstDateTime )
				throw new Exception("lastDateTime has to be greater than " +
				                    "firstEndOfDayDateTime !!");
			History endOfDayHistory = new History();
			DateTime dateTimeToAddToHistory =
				ExtendedDateTime.Copy( firstDateTime );
//				firstDateTime.Copy();
			while( dateTimeToAddToHistory <= lastDateTime )
			{
				if( this.isExchanged( dateTimeToAddToHistory ) )
					endOfDayHistory.Add(
						dateTimeToAddToHistory ,
						dateTimeToAddToHistory );
				dateTimeToAddToHistory =
					HistoricalEndOfDayTimer.GetNextMarketStatusSwitch(
						dateTimeToAddToHistory );
			}
			return endOfDayHistory;
		}
		
		#region GetThisOrNextDateTimeWhenTraded
		private DateTime getDateTimeCandidate(
			DateTime minDateTimeToTry , Time dailyTime )
		{
			Date dateUsedToBuildDateTimeCandidate = new Date( minDateTimeToTry );
			Time timeForMinDateTimeToTry = new Time( minDateTimeToTry );
			if ( timeForMinDateTimeToTry > dailyTime )
				dateUsedToBuildDateTimeCandidate =
					dateUsedToBuildDateTimeCandidate.AddDays( 1 );
			DateTime dateTimeCandidate = new DateTime(
				dateUsedToBuildDateTimeCandidate.Year ,
				dateUsedToBuildDateTimeCandidate.Month ,
				dateUsedToBuildDateTimeCandidate.Day ,
				dailyTime.Hour ,
				dailyTime.Minute ,
				dailyTime.Second );
			return dateTimeCandidate;
		}
//		private bool isCandidateTooFarAhead(
//			DateTime startingDateTime , DateTime dateTimeCandidate ,
//			int maxNumberOfDaysToLookAheadFor )
//		{
//			bool isTooFarAhead;
//			TimeSpan timeSpan = dateTimeCandidate.Subtract( startingDateTime );
//			isTooFarAhead = ( timeSpan.Days > maxNumberOfDaysToLookAheadFor );
//			return isTooFarAhead;
//		}
		/// <summary>
		/// Starting from startingDateTime, this method searches for the next
		/// dateTime when the benchmark is traded at time dailyTime.
		/// The returned value is greater than or equal to startingDateTime,
		/// that is, startingDateTime itself is a valid candidate.
		/// If no solution is found within maxNumberOfDaysToLookAheadFor days,
		/// then an exception is returned
		/// </summary>
		/// <param name="startingDateTime"></param>
		/// <param name="dailyTime"></param>
		/// <param name="maxNumberOfDaysToLookAheadFor"></param>
		/// <returns></returns>
		public virtual DateTime GetThisOrNextDateTimeWhenTraded(
			DateTime startingDateTime ,
			Time dailyTime ,
			DateTime lastAcceptableDateTime )
		{
			DateTime dateTimeCandidate = this.getDateTimeCandidate(
				startingDateTime , dailyTime );
			DateTime thisOrNextDateTimeWhenTraded = DateTime.MinValue;
			while ( dateTimeCandidate <= lastAcceptableDateTime &&
//				!this.isCandidateTooFarAhead(
//				startingDateTime , dateTimeCandidate , maxNumberOfDaysToLookAheadFor ) &&
			       ( thisOrNextDateTimeWhenTraded == DateTime.MinValue ) )
			{
				// a valid dateTime when traded has not been found yet
				if ( this.isExchanged( dateTimeCandidate ) )
					thisOrNextDateTimeWhenTraded = dateTimeCandidate;
				else
					dateTimeCandidate = this.getDateTimeCandidate(
						dateTimeCandidate.AddSeconds( 1 ) , dailyTime );
			}
			if ( thisOrNextDateTimeWhenTraded == DateTime.MinValue )
				throw new Exception(
					"In the specified number of days, the benchmark is never " +
					"traded at the specified dailyTime!" );
			return thisOrNextDateTimeWhenTraded;
		}
		#endregion GetThisOrNextDateTimeWhenTraded
		
	}
}
