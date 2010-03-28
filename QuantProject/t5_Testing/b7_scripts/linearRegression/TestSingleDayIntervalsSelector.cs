/*
QuantProject - Quantitative Finance Library

TestSingleDayIntervalsSelector.cs
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
using System.Collections.Generic;
using NUnit.Framework;

using QuantProject.ADT.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	public class BenchmarkToTestSingleDayIntervalsSelector : Benchmark
	{
		private List<DateTime> dateTimes;
//		private int indexForTheNextDateTimeToBeChosen;
		
		public BenchmarkToTestSingleDayIntervalsSelector( List<DateTime> dateTimes ) :
			base( "dummyTicker" )
		{
			this.dateTimes = dateTimes;
//			this.indexForTheNextDateTimeToBeChosen = 0;
		}
		public override DateTime GetThisOrNextDateTimeWhenTraded(
			DateTime startingDateTime ,
			Time dailyTime ,
			DateTime lastAcceptableDateTime )
		{
			DateTime thisOrNextDateTimeWhenTraded = DateTime.MinValue;
			int currentIndex = 0;
			while ( ( currentIndex < this.dateTimes.Count ) &&
			       ( this.dateTimes[ currentIndex ] <
			        startingDateTime ) )
				currentIndex++;
			if ( currentIndex < this.dateTimes.Count )
				// at least a DateTime greater or equal than startingDateTime has been found
				thisOrNextDateTimeWhenTraded =
					this.dateTimes[ currentIndex ];
//			thisOrNextDateTimeWhenTraded = this.dateTimes[ currentIndex ];
			return thisOrNextDateTimeWhenTraded;
		}

	}
	
	[TestFixture]
	/// <summary>
	/// Test for the SingleDayIntervalsSelector class
	/// </summary>
	public class TestSingleDayIntervalsSelector
	{
		
		#region TestMethod
		private BenchmarkToTestSingleDayIntervalsSelector getBenchmark()
		{
			List<DateTime> dateTimesForBenchmark = new List<DateTime>();
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 10 , 16 , 0 , 0 ) ); // wednesday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 11 , 16 , 0 , 0 ) ); // thursday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 12 , 16 , 0 , 0 ) ); // friday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 15 , 16 , 0 , 0 ) ); // monday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 16 , 16 , 0 , 0 ) ); // tuesday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 17 , 16 , 0 , 0 ) ); // wednesday
			dateTimesForBenchmark.Add( new DateTime( 2010 , 2 , 18 , 16 , 0 , 0 ) ); // thursday
			BenchmarkToTestSingleDayIntervalsSelector benchmark =
				new BenchmarkToTestSingleDayIntervalsSelector( dateTimesForBenchmark );
			return benchmark;
		}
		[Test]
		public void TestMethod()
		{
			BenchmarkToTestSingleDayIntervalsSelector benchmark = this.getBenchmark();
			List<DayOfWeek> acceptableDaysOfTheWeekForTheEndOfEachInterval =
				new List<DayOfWeek>(
					new DayOfWeek[] {
						DayOfWeek.Monday , DayOfWeek.Tuesday ,		// wednesday is not acceptable!
						DayOfWeek.Thursday , DayOfWeek.Friday } );
			SingleDayIntervalsSelector singleDayIntervalsSelector = new
				SingleDayIntervalsSelector(
					benchmark , new Time( 16 , 0 , 0 ) ,
					acceptableDaysOfTheWeekForTheEndOfEachInterval ,
					TimeSpan.FromDays( 20 ) );
			ReturnInterval firstReturnInterval =
				singleDayIntervalsSelector.GetFirstInterval(
					new DateTime( 2010 , 2 , 10 , 16 , 0 , 0 ) );
			Assert.AreEqual( firstReturnInterval.Begin , new DateTime( 2010 , 2 , 10 , 16 , 0 , 0 ) );	// wednesday
			Assert.AreEqual( firstReturnInterval.End , new DateTime( 2010 , 2 , 11 , 16 , 0 , 0 ) );	// thursday
			ReturnIntervals returnIntervals = new ReturnIntervals( firstReturnInterval );
			ReturnInterval secondReturnInterval =
				singleDayIntervalsSelector.GetNextInterval( returnIntervals );
			Assert.AreEqual( secondReturnInterval.Begin , new DateTime( 2010 , 2 , 11 , 16 , 0 , 0 ) );	// thursday
			Assert.AreEqual( secondReturnInterval.End , new DateTime( 2010 , 2 , 12 , 16 , 0 , 0 ) );	// friday
			returnIntervals.Add( secondReturnInterval );
			ReturnInterval thirdReturnInterval =
				singleDayIntervalsSelector.GetNextInterval( returnIntervals );
			Assert.AreEqual( thirdReturnInterval.Begin , new DateTime( 2010 , 2 , 15 , 16 , 0 , 0 ) );	// monday
			Assert.AreEqual( thirdReturnInterval.End , new DateTime( 2010 , 2 , 16 , 16 , 0 , 0 ) );	// tuesday
			returnIntervals.Add( thirdReturnInterval );
			ReturnInterval fourthReturnInterval =
				singleDayIntervalsSelector.GetNextInterval( returnIntervals );
			Assert.AreEqual( fourthReturnInterval.Begin , new DateTime( 2010 , 2 , 17 , 16 , 0 , 0 ) );	// wednesday
			Assert.AreEqual( fourthReturnInterval.End , new DateTime( 2010 , 2 , 18 , 16 , 0 , 0 ) );	// thursday
			returnIntervals.Add( fourthReturnInterval );
			ReturnInterval fifthReturnInterval =
				singleDayIntervalsSelector.GetNextInterval( returnIntervals );
			Assert.IsNull( fifthReturnInterval );	// no more intervals are available
		}
		#endregion TestMethod
	}
}
