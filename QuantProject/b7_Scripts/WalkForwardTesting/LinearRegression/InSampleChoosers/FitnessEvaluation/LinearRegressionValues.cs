/*
QuantProject - Quantitative Finance Library

LinearRegressionValues.cs
Copyright (C) 2010
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

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Values needed to set up a regression
	/// </summary>
	[Serializable]
	public class LinearRegressionValues : ILinearRegressionValues
	{
		private double[] regressand;
		
		public double[] Regressand {
			get { return this.regressand; }
		}
		
		private double[,] regressors;
		
		public double[,] Regressors {
			get { return this.regressors; }
		}
		
		private double[] regressorWeights;
		
		public double[] RegressorWeights {
			get { return this.regressorWeights; }
		}
		
		public LinearRegressionValues(
			double[] regressand ,
			double[,] regressors ,
			double[] regressorWeights )
		{
			this.regressand = regressand;
			this.regressors = regressors;
			this.regressorWeights = regressorWeights;
		}
	}
}
