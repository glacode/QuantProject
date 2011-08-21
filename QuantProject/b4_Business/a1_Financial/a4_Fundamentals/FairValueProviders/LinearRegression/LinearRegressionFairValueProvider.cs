/*
QuantProject - Quantitative Finance Library

LinearRegressionFairValueProvider.cs
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
using QuantProject.ADT.Econometrics;
using QuantProject.Business.Financial.Fundamentals.FairValueProviders;

namespace QuantProject.Business.Financial.Fundamentals.FairValueProviders.LinearRegression
{
	/// <summary>
	/// Class implementing IFairValueProvider Interface
	/// using a linear regression model
	/// </summary>
	[Serializable]
	public class LinearRegressionFairValueProvider : IFairValueProvider
	{
		private ILinearRegressionValuesProvider linearRegressionValuesProvider;
		private IIndipendentValuesProvider indipendentValuesProvider;
		private double[] lastComputedRegressands;
		private double[] regressionCoefficients;
		//regressionCoefficients[0] is the intercept: if 0, then the
		//regression line goes through the origin (0,0)
		
		public LinearRegressionFairValueProvider(ILinearRegressionValuesProvider linearRegressionValuesProvider,
		                                         IIndipendentValuesProvider indipendentValuesProvider)
		{
			this.linearRegressionValuesProvider = linearRegressionValuesProvider;
			this.indipendentValuesProvider = indipendentValuesProvider;
		}
		
		public void Run(DateTime dateTime)
		{
			QuantProject.ADT.Econometrics.LinearRegression linearRegression = 
				new QuantProject.ADT.Econometrics.LinearRegression();
			this.lastComputedRegressands = 
				this.linearRegressionValuesProvider.GetRegressand(dateTime);
			linearRegression.RunRegression(this.lastComputedRegressands,
				this.linearRegressionValuesProvider.GetRegressors(dateTime));
			this.regressionCoefficients = linearRegression.EstimatedCoefficients;
		}
		
		public string Description
		{
			get
			{
				return "Num of Regressands: " +
					this.lastComputedRegressands.Length.ToString();
			}
		}
		
		public double GetFairValue( string ticker ,
		                     				DateTime dateOfFairValueComputation )
		{
			double returnValue = 0.0;
			double[] indipendentValues = 
				this.indipendentValuesProvider.GetIndipendentValues(ticker,
				                                       dateOfFairValueComputation);
			if(this.regressionCoefficients == null)
				this.Run(dateOfFairValueComputation);
			//indipendentValues length = regressionCoefficients length - 1 !!!
			for(int i = 0; i < this.regressionCoefficients.Length; i++)
			{
				if( i == 0 )
					returnValue = returnValue + this.regressionCoefficients[0];
				else
					returnValue = returnValue + 
						this.regressionCoefficients[ i ] * indipendentValues[ i - 1 ];
			}
			return returnValue;
		}
	}
}


