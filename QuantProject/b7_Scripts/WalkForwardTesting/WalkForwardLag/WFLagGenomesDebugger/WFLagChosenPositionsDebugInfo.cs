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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Debug info related to a single genome
	/// </summary>
	public class WFLagChosenPositionsDebugInfo
	{
		private WFLagChosenPositions wFLagChosenPositions;
		private WFLagLog wFLagLog;
		private double inSampleSharpeRatio;
		private double preSampleSharpeRatio30;
		private double postSampleSharpeRatio30;

		public string DrivingPositions
		{
			get { return this.wFLagChosenPositions.DrivingPositions.KeyConcat; }
		}
		public string PortfolioPositions
		{
			get { return this.wFLagChosenPositions.PortfolioPositions.KeyConcat; }
		}
		public DateTime LastOptimization
		{
			get { return this.wFLagChosenPositions.LastOptimizationDate; }
		}
		public int InSampleDays
		{
			get { return this.wFLagLog.InSampleDays; }
		}
		public double InSampleSharpeRatio
		{
			get { return this.inSampleSharpeRatio; }
		}
		public double PreSampleSharpeRatio30
		{
			get { return this.preSampleSharpeRatio30; }
		}
		public double PostSampleSharpeRatio30
		{
			get { return this.postSampleSharpeRatio30; }
		}
		public WFLagChosenPositionsDebugInfo(
			WFLagChosenPositions wFLagChosenPositions ,
			WFLagLog wFLagLog )
		{
			//
			// TODO: Add constructor logic here
			//
			this.wFLagChosenPositions = wFLagChosenPositions;
			this.wFLagLog = wFLagLog;
			this.inSampleSharpeRatio = this.getInSampleSharpeRatio();
			this.preSampleSharpeRatio30 = this.getPreSampleSharpeRatio( 30 );
			this.postSampleSharpeRatio30 = this.getPostSampleSharpeRatio( 30 );
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
				this.wFLagChosenPositions , firstDateTime ,
				this.LastOptimization );
		}
		private double getPreSampleSharpeRatio( int days )
		{
			DateTime lastDateTime = this.getInSampleFirstDate();
			DateTime firstDateTime = lastDateTime.AddDays( -days - 1 );  // I subtract one more day, so I have days daily returns
			return WFLagSharpeRatioComputer.GetSharpeRatio(
				this.wFLagChosenPositions , firstDateTime ,
				lastDateTime );
		}
		private double getPostSampleSharpeRatio( int days )
		{
			DateTime firstDateTime =
				this.LastOptimization;	// add one day if you want to exclude the first day after the optimization;
			// consider that the real out of sample jumps out that day
			DateTime lastDateTime = firstDateTime.AddDays( days + 1 );  // I add one day, so I have days daily returns
			return WFLagSharpeRatioComputer.GetSharpeRatio(
				this.wFLagChosenPositions , firstDateTime ,
				lastDateTime );
		}
	}
}
