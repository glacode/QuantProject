/*
QuantProject - Quantitative Finance Library

TransactionHistory.cs
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
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Financial.Accounting.Transactions
{
	/// <summary>
	/// Transaction history
	/// </summary>
  [Serializable]
  public class TransactionHistory : History
	{
		public TransactionHistory()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public double TotalAddedCash
    {
      get 
      {
        double totalAddedCash = 0;
        foreach ( ArrayList arrayList in this.Values )
          foreach ( Transaction transaction in arrayList )
            if ( transaction.Type == TransactionType.AddCash )
              totalAddedCash += transaction.Amount;
        return totalAddedCash;
      }
    }

    public double TotalWithdrawn
    {
      get 
      {
        double totalWithdrawn = 0;
        foreach ( ArrayList arrayList in this.Values )
          foreach ( Transaction transaction in arrayList )
            if ( transaction.Type == TransactionType.Withdraw )
              totalWithdrawn += transaction.Amount;
        return totalWithdrawn;
      }
    }

//		public void Add( TimedTransaction transaction )
//		{
//			base.MultiAdd( transaction.ExtendedDateTime.DateTime , transaction );
//		}

		public void Add( TimedTransaction transaction )
		{
			base.MultiAdd( transaction.DateTime , transaction );
		}

		public override string ToString()
    {
      string toString = "";
      foreach ( ArrayList arrayList in this.Values )
        foreach ( Transaction transaction in arrayList )
          toString += transaction.ToString();
      return toString;
    }
  }
}
