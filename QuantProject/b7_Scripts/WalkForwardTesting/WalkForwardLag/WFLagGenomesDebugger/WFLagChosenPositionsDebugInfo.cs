/*
QuantProject - Quantitative Finance Library

WFLagChosenPositionsDebugInfo.cs
Copyright (C) 2003 
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
using System.Collections;

using QuantProject.Business.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Debug info related to a single genome
	/// </summary>
	public class WFLagChosenPositionsDebugInfo
	{
		private double dummyValue;
		private int maxPreSampleDays;
		private WFLagWeightedPositions wFLagWeightedPositions;
		private int generation;
		private DateTime lastOptimizationDate;
		private WFLagLog wFLagLog;
		private double inSampleSharpeRatio;
		private double preSampleSharpeRatio30;
		private double preSampleSharpeRatio150;
		private double preSampleMaxSharpeRatio;
		private double postSampleSharpeRatio30;
		private double postSampleSharpeRatio150;
		private double inSampleExpectancyScore;

		public string DrivingPositions
		{
			get
			{
				return this.wFLagWeightedPositions.DrivingWeightedPositions.ToString();
			}
		}
		public string PortfolioPositions
		{
			get
			{
				return this.wFLagWeightedPositions.PortfolioWeightedPositions.ToString();
			}
		}
		public DateTime LastOptimization
		{
			get { return this.lastOptimizationDate; }
		}
		public int InSampleDays
		{
			get { return this.wFLagLog.InSampleDays; }
		}
		/// <summary>
		/// The number of the genetic optimizer's generation
		/// when the corresponding genome was created
		/// </summary>
		public int Generation
		{
			get { return this.generation; }
		}
		public double InSampleSharpeRatio
		{
			get { return this.inSampleSharpeRatio; }
		}
		public double InSampleExpectancyScore
		{
			get { return this.inSampleExpectancyScore; }
		}
		public int MaxPreSampleDays
		{
			get
			{
				if ( this.maxPreSampleDays == int.MinValue )
					this.setMaxPreSampleDays();
				return this.maxPreSampleDays;
			}
		}
		public double PreSample30SharpeRatio
		{
			get { return this.preSampleSharpeRatio30; }
		}
		public double PreSampleMaxSharpeRatio
		{
			get { return this.preSampleMaxSharpeRatio; }
		}
		public double PreSample150SharpeRatio
		{
			get { return this.preSampleSharpeRatio150; }
		}
		public double PostSample30SharpeRatio
		{
			get { return this.postSampleSharpeRatio30; }
		}
		public double PostSample150SharpeRatio
		{
			get { return this.postSampleSharpeRatio150; }
		}
		public WFLagChosenPositionsDebugInfo(
			WFLagWeightedPositions wFLagWeightedPositions ,
			DateTime lastOptimizationDate ,
			int generation ,
			WFLagLog wFLagLog )
		{
			//
			// TODO: Add constructor logic here
			//
			this.dummyValue = -999;

			this.wFLagWeightedPositions = wFLagWeightedPositions;
			this.generation = generation;
			this.lastOptimizationDate = lastOptimizationDate;
			this.wFLagLog = wFLagLog;
			this.inSampleSharpeRatio = this.getInSampleSharpeRatio();
			this.inSampleExpectancyScore = this.getInSampleExpectancyScore();
			this.setMaxPreSampleDays();
			this.preSampleSharpeRatio30 = this.getPreSampleSharpeRatio( 30 );
			this.preSampleSharpeRatio150 = this.getPreSampleSharpeRatio( 150 );
			if ( this.MaxPreSampleDays > 1 )
				this.preSampleMaxSharpeRatio = this.getPreSampleSharpeRatio(
					this.MaxPreSampleDays - 1 );
			this.postSampleSharpeRatio30 = this.getPostSampleSharpeRatio( 30 );
			this.postSampleSharpeRatio150 = this.getPostSampleSharpeRatio( 150 );
		}
		public static string[] GetDrivingAndPortfolioTickers(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			int size = wFLagWeightedPositions.DrivingWeightedPositions.Count +
				wFLagWeightedPositions.PortfolioWeightedPositions.Count;
			string[] drivingAndPortfolioTickers = new string[ size ];
			int i = 0;
			foreach ( string signedTicker in
				wFLagWeightedPositions.DrivingWeightedPositions.Keys )
			{
				drivingAndPortfolioTickers[ i ] = SignedTicker.GetTicker( signedTicker );
				i++;
			}
			foreach ( string signedTicker in
				wFLagWeightedPositions.PortfolioWeightedPositions.Keys )
			{
				drivingAndPortfolioTickers[ i ] = SignedTicker.GetTicker( signedTicker );
				i++;
			}
			return drivingAndPortfolioTickers;
		}
		/// <summary>
		/// returns the chosen positions. A method is used instead of a property, because
		/// we don't want this as a column displayed in the grid
		/// </summary>
		public WFLagWeightedPositions GetChosenPositions()
		{
			return this.wFLagWeightedPositions;
		}
		private ArrayList getMinDatesForTickers()
		{
			string[] tickers = GetDrivingAndPortfolioTickers( this.wFLagWeightedPositions );
			ArrayList minDatesForTickers = new ArrayList();
			foreach ( string ticker in tickers )
				minDatesForTickers.Add(
					QuantProject.Data.DataTables.Quotes.GetFirstQuoteDate( ticker ) );
			return minDatesForTickers;
		}
		private DateTime getFirstCommonDateForTickers()
		{
			ArrayList minDatesForTickers =
				this.getMinDatesForTickers();
			minDatesForTickers.Sort();
			return (DateTime)minDatesForTickers[ minDatesForTickers.Count - 1 ];
		}
		private void setMaxPreSampleDays()
		{
			DateTime firstCommonDateForTickers =
				this.getFirstCommonDateForTickers();
			TimeSpan timeSpan = this.getInSampleFirstDate() -
				firstCommonDateForTickers;
			this.maxPreSampleDays = timeSpan.Days;
			// the following statement is used for the rare case when the
			// firstCommonDateForTickers comes after this.getInSampleFirstDate() 
			// It may happen when this.getInSampleFirstDate()
			// falls in a non market day and one of the involved tickers begins being
			// quoted from the following market day
			this.maxPreSampleDays = Math.Max( this.maxPreSampleDays , 0 );
		}
		private DateTime getInSampleFirstDate()
		{
			DateTime inSampleFirstDate = this.LastOptimization.AddDays(
				-( this.wFLagLog.InSampleDays - 1 ) );
			return inSampleFirstDate;
		}
		private double getInSampleSharpeRatio()
		{
			DateTime firstDateTime = this.getInSampleFirstDate();
			return WFLagSharpeRatioComputer.GetSharpeRatio(
				this.wFLagWeightedPositions , firstDateTime ,
				this.LastOptimization );
		}
		private double getInSampleExpectancyScore()
		{
			DateTime firstDateTime = this.getInSampleFirstDate();
			return WFLagSharpeRatioComputer.GetExpectancyScore(
				this.wFLagWeightedPositions , firstDateTime ,
				this.LastOptimization );
		}
		private double getPreSampleSharpeRatio( int days )
		{
			double preSampleSharpeRatio = this.dummyValue;
			if ( this.MaxPreSampleDays > days )
			{
				DateTime lastDateTime = this.getInSampleFirstDate();
				DateTime firstDateTime = lastDateTime.AddDays( -days - 1 );  // I subtract one more day, so I have days daily returns
				preSampleSharpeRatio = WFLagSharpeRatioComputer.GetSharpeRatio(
					this.wFLagWeightedPositions , firstDateTime ,
					lastDateTime );
			}
			return preSampleSharpeRatio;
		}
		private double getPostSampleSharpeRatio( int days )
		{
			DateTime firstDateTime =
				this.LastOptimization;	// add one day if you want to exclude the first day after the optimization;
			// consider that the real out of sample jumps out that day
			DateTime lastDateTime = firstDateTime.AddDays( days + 1 );  // I add one day, so I have days daily returns
			return WFLagSharpeRatioComputer.GetSharpeRatio(
				this.wFLagWeightedPositions , firstDateTime ,
				lastDateTime );
		}
	}
}
