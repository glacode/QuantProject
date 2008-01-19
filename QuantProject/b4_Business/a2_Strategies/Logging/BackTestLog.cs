/*
QuantProject - Quantitative Finance Library

BackTestLog.cs
Copyright (C) 2007
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

using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Business.Strategies.Logging
{
	/// <summary>
	/// Useful information to be saved for a back test
	/// </summary>
	[Serializable]
	public class BackTestLog : System.Collections.CollectionBase
	{
		private TransactionHistory transactionHistory;

		private string backTestId;
		private DateTime firstDate;
		private DateTime lastDate;
		private Benchmark benchmark;

		public TransactionHistory TransactionHistory
		{
			get { return this.transactionHistory; }
			set { this.transactionHistory = value; }
		}
		public BackTestLog( string backTestId , DateTime firstDate ,
			DateTime lastDate , Benchmark benchmark )
		{
			this.backTestId = backTestId;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.benchmark = benchmark;
		}
		public LogItem this[ int index ]  
		{
			get  
			{
				return( (LogItem) this.List[ index ] );
			}
			set  
			{
				this.List[ index ] = value;
			}
		}
		public void Add( LogItem logItem )
		{
			this.List.Add( logItem );
		}
	}
}
