/*
QuantProject - Quantitative Finance Library

ComparableAccount.cs
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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Summary description for ComparableAccount.
	/// </summary>
	public class ComparableAccount : Account
	{
    private double maxAcceptableDrawDown = 0.3;

		private AccountReport accountReport;

		public AccountReport Report
		{
			get {	return this.accountReport; }
			set { this.accountReport = value; }
		}

		public double Goodness
		{
			get {	return this.goodness(); }
		}
		public ComparableAccount()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private double goodness()
		{
			double returnValue;
			if ( this.accountReport.Summary.MaxEquityDrawDown >= this.maxAcceptableDrawDown )
				returnValue = Double.MinValue;
			else
				// max draw down is acceptable
				returnValue = this.accountReport.Summary.TotalPnl /
					this.accountReport.Summary.BuyAndHoldPercentageReturn;
			return returnValue;
		}
	}
}
