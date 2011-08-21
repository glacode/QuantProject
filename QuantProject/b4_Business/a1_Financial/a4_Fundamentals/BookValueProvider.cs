/*
QuantProject - Quantitative Finance Library

BookValueProvider.cs
Copyright (C) 2011
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
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;

namespace QuantProject.Business.Financial.Fundamentals
{
	/// <summary>
	/// Class implementing FundamentalDataProvider.
	/// It provides the last available BookValue
	/// </summary>
	[Serializable]
	public class BookValueProvider : FundamentalDataProvider
	{
		
		public BookValueProvider(int daysForAvailabilityOfData) : 
											  base(daysForAvailabilityOfData)
		{
		}
		
		public override double GetValue( string ticker , DateTime atDate )
		{
			double returnValue;
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
			returnValue = 
				FinancialValues.GetLastFinancialValueForTicker(ticker, FinancialValueType.BookValuePerShare, 12,
			                                    limitDateForEndingPeriodDate);
//				FinancialValues.GetFinancialValue(ticker, FinancialValueType.BookValuePerShare, 12,
//			                                    limitDateForEndingPeriodDate);
			return returnValue;
		}
	}
}
