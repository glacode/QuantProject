/*
QuantProject - Quantitative Finance Library

WFLagLog.cs
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

using QuantProject.ADT.Histories;

using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Log for a walk forward lag backtest
	/// </summary>
	[Serializable]
	public class WFLagLog
	{
		private int inSampleDays;
		private string benchmark;

		private TransactionHistory transactionHistory;
		private History chosenPositionsHistory;
	
		public int InSampleDays
		{
			get { return this.inSampleDays; }
		}
		public string Benchmark
		{
			get { return this.benchmark; }
		}

		public TransactionHistory TransactionHistory
		{
			get { return this.transactionHistory; }
			set { this.transactionHistory = value; }
		}

		public WFLagLog( int inSampleDays , string benchmark )
		{
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.chosenPositionsHistory = new History();
		}
		public void Add( WFLagChosenPositions wFLagChosenPositions )
		{
			this.chosenPositionsHistory.Add(
				wFLagChosenPositions.LastOptimizationDate ,
				wFLagChosenPositions );
		}
		public WFLagChosenPositions GetChosenPositions(
			DateTime transactionDateTime )
		{
			DateTime maxDateTimeForSettingRequestedChosenPosition =
				transactionDateTime.AddDays( - 1 );
			WFLagChosenPositions wFLagChosenPositions =
				(WFLagChosenPositions)this.chosenPositionsHistory.GetByKeyOrPrevious(
				maxDateTimeForSettingRequestedChosenPosition );
			return wFLagChosenPositions;
		}
	}
}
