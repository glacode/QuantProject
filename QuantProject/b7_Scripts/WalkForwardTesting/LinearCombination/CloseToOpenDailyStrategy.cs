/*
QuantProject - Quantitative Finance Library

CloseToOpenDailyStrategy.cs
Copyright (C) 2003
Marco Milletti

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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Close To Open daily strategy
	/// </summary>
	[Serializable]
	public class CloseToOpenDailyStrategy : IStrategy
	{
		private Account account;
		private WeightedPositions weightedPositions;
		
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		public CloseToOpenDailyStrategy( Account account ,
		                                WeightedPositions weightedPositions)
		{
			this.account = account;
			this.weightedPositions = weightedPositions;
		}
		
		private void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			AccountManager.ClosePositions(this.account);
		}
		private void fiveMinutesBeforeMarketCloseEventHandler( Object sender ,
		                                                     DateTime dateTime)
		{
		}
		private void marketCloseEventHandler( Object sender ,
		                                    DateTime dateTime)
		{
			if ( ( this.account.CashAmount == 0 ) &&
			    ( this.account.Transactions.Count == 0 ) )
				// cash has not been added yet
				this.account.AddCash( 15000 );
			AccountManager.OpenPositions(this.weightedPositions, this.account);
		}
		private void oneHourAfterMarketCloseEventHandler( Object sender ,
		                                                DateTime dateTime)
		{
		}
		
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.marketCloseEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
		}
	}
}
