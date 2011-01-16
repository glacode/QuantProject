/*
QuantProject - Quantitative Finance Library

FundamentalDataProvider.cs
Copyright (C) 2010
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;

namespace QuantProject.Business.Financial.Fundamentals
{
	/// <summary>
	/// Base class for fundamental data providers
	/// </summary>
	[Serializable]
	public class FundamentalDataProvider
	{
		protected int daysForAvailabilityOfData;
		/// <summary>
		/// FundamentalDataProvider's constructor
		/// </summary>
		/// <param name="daysForAvailabilityOfData">
		/// Each fundamental data refer to a specific date (for example: 
		/// long term debts, equity, and so on) or to a specific period
		/// (for example: earnings).
		/// In any case, such data are available after a variable
		/// (depending on data source) amounts of days from the specific date or the
		/// last date of the period data refer to 
		///</param>
		public FundamentalDataProvider(int daysForAvailabilityOfData)
		{
			this.daysForAvailabilityOfData = daysForAvailabilityOfData;
		}
	}
}
