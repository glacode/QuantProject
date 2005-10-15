/*
QuantProject - Quantitative Finance Library

EquityLine.cs
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
using QuantProject.ADT.Histories;
using QuantProject.Business.Financial.Instruments;

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Equity line
	/// </summary>
	/// 
	[Serializable]
	public class EquityLine : History
	{
		public EquityLine() : base()
		{
		}
		/// <summary>
		/// Computes the return history, for the equity line
		/// </summary>
		/// <returns></returns>
		public History GetReturns()
		{
			History returns = new History();
			for ( int i = 0 ; i < this.Count - 1 ; i++ )
			{
				if ( Convert.ToDouble( this.GetByIndex( i ) ) <= 0 )
					throw new Exception( "This equity line contains a " +
						"non positive value. An equity line must be strictly positive " );
				returns.Add( this.GetKey( i + 1 ) ,
					( Convert.ToDouble( this.GetByIndex( i+1 ) ) -
					Convert.ToDouble( this.GetByIndex( i ) ) ) /
					Convert.ToDouble( this.GetByIndex( i ) ) );
			}
			return returns;
		}
	}
}
