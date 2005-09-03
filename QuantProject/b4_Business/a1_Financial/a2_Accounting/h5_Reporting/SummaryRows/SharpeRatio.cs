/*
QuantProject - Quantitative Finance Library

SharpeRatio.cs
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
using QuantProject.ADT.Statistics;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.SummaryRows
{
	/// <summary>
	/// Summary row containing the Sharpe Ratio calculation for
	/// the account report equity line
	/// </summary>
	[Serializable]
  public class SharpeRatio : DoubleSummaryRow
	{
		private ArrayList getReturns( History equityLine )
		{
			ArrayList returnValue = new ArrayList();
			for ( int i=0 ; i < equityLine.Count - 1 ; i++ )
			{
				if ( Convert.ToDouble( equityLine.GetByIndex( i ) ) <= 0 )
					throw new Exception( "Equity line is expected to always being " +
						"strictly positive. equityLine[ i ] is negative or equal to zero!" );
				double periodReturn =
					( Convert.ToDouble( equityLine.GetByIndex( i + 1 ) ) -
					Convert.ToDouble( equityLine.GetByIndex( i ) ) ) /
					Convert.ToDouble( equityLine.GetByIndex( i ) );
				returnValue.Add( periodReturn );
			}
			return returnValue;
		}
		public SharpeRatio( History equityLine ) : base( 2 )
		{
      this.rowDescription = "Sharpe ratio";
			ICollection returns = this.getReturns( equityLine );
			this.rowValue = AdvancedFunctions.GetSharpeRatio(
				returns );
    }
	}
}
