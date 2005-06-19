/*
QuantProject - Quantitative Finance Library

ComparableAccount.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Summary description for ComparableAccount.
	/// </summary>
	[Serializable]
	public class ComparableAccount : Account
	{
    private double maxAcceptableDrawDown = 30;

		private double minAcceptableWinningPeriods = 52;

		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();

		private AccountReport accountReport;

		public AccountReport Report
		{
			get {	return this.accountReport; }
			set { this.accountReport = value; }
		}

		public double Goodness
		{
			get {	return this.goodness(); }
		}
		public ComparableAccount( string accountName , IEndOfDayTimer endOfDayTimer ,
			IDataStreamer dataStreamer , IOrderExecutor orderExecutor ) : base( accountName ,
			endOfDayTimer , dataStreamer , orderExecutor )
		{
		}
		private double goodness()
		{
			double returnValue;
			if ( this.accountReport == null )
				this.accountReport = this.CreateReport( this.Key , 1 ,
					this.EndOfDayTimer.GetCurrentTime() , this.Key ,
					this.historicalQuoteProvider );

			// old goodness computation
			if ( ( (double)this.accountReport.Summary.MaxEquityDrawDown.Value >=
				this.maxAcceptableDrawDown )
				|| ( this.accountReport.Summary.TotalPnl <=
				(double)this.accountReport.Summary.BenchmarkPercentageReturn.Value ) )
				returnValue = Double.MinValue;
			else
				// max draw down is acceptable and the strategy is better than buy and hold
				returnValue = (double)this.accountReport.Summary.ReturnOnAccount.Value -
					(double)this.accountReport.Summary.BenchmarkPercentageReturn.Value;

			// new goodness computation
//			if ( this.accountReport.Summary.NumberWinningPeriods < this.minAcceptableWinningPeriods )
//				returnValue = Double.MinValue;
//			else
//				returnValue = Convert.ToDouble( this.accountReport.Summary.PercentageWinningPeriods );
			if ( (double)this.accountReport.Summary.PercentageWinningPeriods.Value <
				this.minAcceptableWinningPeriods )
				returnValue = Double.MinValue;
			else
				returnValue = Convert.ToDouble( - (double)this.accountReport.Summary.MaxEquityDrawDown.Value );

			return returnValue;
		}
		public override double GetFitnessValue()
		{
			return this.goodness();
		}
	}
}
