/*
QuantProject - Quantitative Finance Library

PRESSBasedFitnessEvaluator.cs
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

using QuantProject.ADT.Econometrics;
using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Computes (in sample) the PSquare statistic
	/// </summary>
	[Serializable]
	public class PRESSBasedFitnessEvaluator : LinearRegressionFitnessEvaluator
	{
		public override string Description
		{
			get
			{
				return "lnrRgrssnPRESS";
			}
		}

		public PRESSBasedFitnessEvaluator(
			ILinearRegressionSetupManager linearRegressionSetupManger ) :
			base( linearRegressionSetupManger )
		{
		}
		
		protected override double getFitnessValue( ILinearRegression linearRegression )
		{
			double fitnessValue = linearRegression.CenteredPSquare;
			return fitnessValue;
		}
	}
}
