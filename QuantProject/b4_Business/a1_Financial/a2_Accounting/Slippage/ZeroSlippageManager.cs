/*
QuantProject - Quantitative Finance Library

ZeroSlippageManager.cs
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

namespace QuantProject.Business.Financial.Accounting.Slippage
{
	/// <summary>
	/// Slippage Manager for Zero value slippage.
	/// This is the slippage manager used by an Account
	/// if no ISlippageManager is specified.
	/// </summary>
	[Serializable]
  public class ZeroSlippageManager : ISlippageManager
	{
		public ZeroSlippageManager()
		{
		}
		public double GetSlippage( Ordering.Order order )
		{
			return 0.0;
		}
	}
}
