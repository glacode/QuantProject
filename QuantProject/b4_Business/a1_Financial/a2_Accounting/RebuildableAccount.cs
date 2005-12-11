/*
QuantProject - Quantitative Finance Library

RebuildableAccount.cs
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

using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Account that can be recreated from a transaction history
	/// </summary>
	public class RebuildableAccount : Account
	{
		public RebuildableAccount( string name ) : base( name )
		{
		}
		/// <summary>
		/// Recreate the account from the transaction history
		/// </summary>
		/// <param name="transactions"></param>
		public void Add( TransactionHistory transactions )
		{
			foreach( Object key in transactions.Keys )
			{
				foreach( EndOfDayTransaction transaction
									 in (ArrayList)transactions[key] )
				{
					this.Add( transaction );
				}
			}
		}
	}
}
