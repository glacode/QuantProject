/*
QuantProject - Quantitative Finance Library

AccountReportRecord.cs
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

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Summary description for AccountReportRecord.
	/// </summary>
	internal class AccountReportRecord
	{
    private TimedTransaction transaction;
    private double totalCash;
    private double portfolioValue;
    private double accountValue;
    private double profitNetLoss;

    public TimedTransaction Transaction
    {
      set { transaction = value; }
      get { return transaction; }
    }
    public double TotalCash
    {
      set { totalCash = value; }
      get { return totalCash; }
    }
    public double PortfolioValue
    {
      set { portfolioValue = value; }
      get { return portfolioValue; }
    }
    public double AccountValue
    {
      set { accountValue = value; }
      get { return accountValue; }
    }
    public double ProfitNetLoss
    {
      set { profitNetLoss = value; }
      get { return profitNetLoss; }
    }
    public AccountReportRecord()
		{
		}
    public ArrayList GetHeaders()
    {
      ArrayList headers = new ArrayList();
      headers.Add( "DateTime" );
      headers.Add( "TransactionType" );
      headers.Add( "InstrumentKey" );
      headers.Add( "Quantity" );
      headers.Add( "Price" );
      headers.Add( "TransactionAmount" );
      headers.Add( "AccountCash" );
      headers.Add( "PortfolioValue" );
      headers.Add( "AccountValue" );
      headers.Add( "PnL" );
      return headers;
    }
    public ArrayList GetData()
    {
      ArrayList data = new ArrayList();
      data.Add( this.transaction.ExtendedDateTime.DateTime );
      data.Add( this.transaction.Type.ToString() );
      if ( this.transaction.Instrument != null )
        data.Add( this.transaction.Instrument.Key );
      else
        data.Add( "" );
      data.Add( this.transaction.Quantity );
      data.Add( this.transaction.InstrumentPrice );
      data.Add( this.transaction.InstrumentPrice * this.transaction.Quantity );
      data.Add( this.totalCash );
      data.Add( this.portfolioValue );
      data.Add( this.accountValue );
      data.Add( this.profitNetLoss );
      return data;
    }
  }
}
