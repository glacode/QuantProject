/*
QuantProject - Quantitative Finance Library

BasicIndipendentValuesProvider.cs
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
using QuantProject.Business.Financial.Fundamentals;

namespace QuantProject.Business.Financial.Fundamentals.FairValueProviders.LinearRegression
{
	/// <summary>
	/// Class implementing IIndipendentValuesProvider Interface
	/// In this basic implementation, indipendent values
	/// (to be passed to a LinearRegressionFairValueProvider)
	/// are fundamental values 
	/// </summary>
	[Serializable]
	public class BasicIndipendentValuesProvider : IIndipendentValuesProvider
	{
		private FundamentalDataProvider[] fundamentalDataProviders;
		
		public BasicIndipendentValuesProvider(FundamentalDataProvider[] fundamentalDataProviders)
		{
			this.fundamentalDataProviders = fundamentalDataProviders;
		}
		
		public virtual double[] GetIndipendentValues(string ticker,
				                          			 DateTime dateForIndipendentValues)
		{
			double[] returnValue = new double[this.fundamentalDataProviders.Length];
			for(int i = 0; i < this.fundamentalDataProviders.Length; i++)
				returnValue[i] =
					this.fundamentalDataProviders[i].GetValue(ticker, dateForIndipendentValues);
			return returnValue;
		}
	}
}
