/*
QuantProject - Quantitative Finance Library

Transaction.cs
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
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Accounting.Commissions;

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Summary description for Transaction.
	/// </summary>
  [Serializable]
  public class Transaction
  {
    private TransactionType transactionType;
    private double amount;
    private Instrument instrument;
    private long quantity; 
    private double instrumentPrice;
		private Commission commission;

    public TransactionType Type
    {
      get
      {
        return transactionType;
      }
    }

    public Double Amount
    {
      get
      {
        return amount;
      }
    }

    public Instrument Instrument
    {
      get
      {
        return instrument;
      }
    }

    public long Quantity
    {
      get
      {
        return quantity;
      }
    }

		public Commission Commission
		{
			get
			{
				return this.commission;
			}
			set
			{
				this.commission = value;
			}
		}

		public double InstrumentPrice
    {
      get { return instrumentPrice; }
    }

    public Transaction( TransactionType transactionType , Double transactionAmount )
    {
      this.transactionType = transactionType;
      this.amount = transactionAmount;
    }

    public Transaction( TransactionType transactionType , Instrument instrument , long quantity , double instrumentPrice )
    {
      this.transactionType = transactionType;
      this.instrument = instrument;
      this.quantity = quantity;
      this.instrumentPrice = instrumentPrice;
      this.amount = instrumentPrice * quantity;
    }

    public override string ToString()
    {
      if (this.transactionType == TransactionType.AddCash)
        return
          "\nTransaction:" +
          "\n   Type: " + this.transactionType.ToString() +
          "\n   Amount: " + this.Amount;
      else
        return
          "\nTransaction:" +
          "\n   Type: " + this.transactionType.ToString() +
          "\n   Instrument: " + this.instrument.Key +
          "\n   Quantity: " + this.quantity +
          "\n   Price: " + this.InstrumentPrice +
          "\n   Amount: " + this.Amount;
    }
    public double CashFlow()
    {
      double cashFlow = 0;
      switch (this.Type)
      {
        case TransactionType.AddCash:
          cashFlow = this.Amount;
          break;
        case TransactionType.BuyLong:
        case TransactionType.Cover:
          cashFlow = -this.InstrumentPrice * this.Quantity;
          break;
        case TransactionType.Sell:
        case TransactionType.SellShort:
          cashFlow = +this.InstrumentPrice * this.Quantity;
          break;
      }
      return cashFlow;
    }
  }
}
