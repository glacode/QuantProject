/*
QuantProject - Quantitative Finance Library

NumberWinningPeriods.cs
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
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Financial.Instruments;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary row that computes the Equity Line vs Benchmark comparison
	/// </summary>
	[Serializable]
	public class NumberWinningPeriods : IntegerSummaryRow
	{
		private Summary summary;

		private double numberWinningPeriods;
		private double numberLosingPeriods;
		private double numberEvenPeriods;
		private double numberPositivePeriods;
		private double numberNegativePeriods;
		private double numberZeroPeriods;

		private void setWinningLosingAndEvenPeriods_forPeriod( int i )
		{
			double equityHistoryGain =
				( (double)this.summary.AccountReport.EquityLine.GetByIndex( i + 1 ) -
				(double)this.summary.AccountReport.EquityLine.GetByIndex( i ) ) /
				(double)this.summary.AccountReport.EquityLine.GetByIndex( i );
			double benchmarkGain =
				( Convert.ToDouble( this.summary.AccountReport.BenchmarkEquityLine.GetByIndex( i + 1 ) ) -
				Convert.ToDouble( this.summary.AccountReport.BenchmarkEquityLine.GetByIndex( i ) ) ) /
				Convert.ToDouble( this.summary.AccountReport.BenchmarkEquityLine.GetByIndex( i ) );
			if ( ( equityHistoryGain - benchmarkGain ) > ConstantsProvider.MinForDifferentGains )
				this.numberWinningPeriods++;
			else
			{
				if ( ( benchmarkGain - equityHistoryGain ) > ConstantsProvider.MinForDifferentGains )
					this.numberLosingPeriods++;
				else
					this.numberEvenPeriods++;
			}
			if ( equityHistoryGain < 0 )
				this.numberNegativePeriods ++;
			else
			{
				if ( equityHistoryGain > 0 )
					this.numberPositivePeriods ++;
				else
					// equityHistoryGain == 0
					this.numberZeroPeriods ++;
			}

		}

		public void SetWinningLosingPositiveAndNegativePeriods()
		{
			this.numberWinningPeriods = 0;
			this.numberLosingPeriods = 0;
			this.numberEvenPeriods = 0;
			this.numberPositivePeriods = 0;
			this.numberNegativePeriods = 0;
			this.numberZeroPeriods = 0;
			for ( int i=0; i<this.summary.AccountReport.EquityLine.Count - 1 ; i++ )
				this.setWinningLosingAndEvenPeriods_forPeriod( i );
		}
		public NumberWinningPeriods( Summary summary ) : base()
		{
			this.summary = summary;
			this.SetWinningLosingPositiveAndNegativePeriods();
			this.rowDescription = "# winning periods";
			this.format = ConstantsProvider.FormatWithZeroDecimals;
			this.rowValue = this.numberWinningPeriods;
		}
		public double NumberLosingPeriods
		{
			get { return this.numberLosingPeriods; }
		}
		public double NumberPositivePeriods
		{
			get { return this.numberPositivePeriods; }
		}
		public double NumberNegativePeriods
		{
			get { return this.numberNegativePeriods; }
		}
	}
}
