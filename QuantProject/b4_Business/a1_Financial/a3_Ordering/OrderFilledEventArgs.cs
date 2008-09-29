/*
QuantProject - Quantitative Finance Library

NewTransactionEventArgs.cs
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
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Business.Financial.Ordering
{
	/// <summary>
	/// EventArgs for the OrderExecuted event
	/// </summary>
	public class OrderFilledEventArgs : EventArgs
	{
		private Order order;
		private TimedTransaction timedTransaction;

		public Order Order
		{
			get { return this.order; }
			set { this.order = value; }
		}

		public TimedTransaction TimedTransaction
		{
			get { return this.timedTransaction; }
			set { this.timedTransaction = value; }
		}

		public OrderFilledEventArgs(
			Order order , TimedTransaction timedTransaction )
		{
			this.order = order;
			this.timedTransaction = timedTransaction;
		}
	}
}
