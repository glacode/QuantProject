/*
QuantProject - Quantitative Finance Library

IBCommission.cs
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

namespace QuantProject.Business.Financial.Accounting.Commissions
{
	/// <summary>
	/// A sample Commission
	/// </summary>
	[Serializable]
  public class IBCommission : Commission
	{
		public override double Value
		{
			get
			{
				double returnValue;
				if  ( ( this.transaction.Type == TransactionType.AddCash ) ||
					( this.transaction.Type == TransactionType.Withdraw ) )
					returnValue = 0;
				else
				{
					returnValue = this.transaction.Quantity * 0.005;
					//USD 0.005 per share
					returnValue = Math.Min( Math.Max( returnValue , 1.0 ),
																	0.005 * this.transaction.Amount);
					//0.5% of trade value plus exchange, ECN, and specialist fees
				}
				return returnValue;
			}
		}
		public IBCommission( Transaction transaction ) : base( transaction )
		{
		}
	}
}
