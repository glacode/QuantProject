/*
QuantProject - Quantitative Finance Library

AnalyzerForLinearRegressionTestingPositions.cs
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

using QuantProject.Business.Scripting;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Analyzes a LinearRegressionTestingPositions that has been
	/// logged in sample
	/// </summary>
	[Serializable]
	public class AnalyzerForLinearRegressionTestingPositions : IExecutable
	{
		LinearRegressionTestingPositions linearRegressionTestingPositions;
		DateTime accountTimerDateTimeWhenThisObjectWasLogged;
		
		public AnalyzerForLinearRegressionTestingPositions(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
//			int numberOfInSampleDays ,
			DateTime accountTimerDateTimeWhenThisObjectWasLogged )
		{
			this.linearRegressionTestingPositions = linearRegressionTestingPositions;
			this.accountTimerDateTimeWhenThisObjectWasLogged =
				accountTimerDateTimeWhenThisObjectWasLogged;
		}
		public void Run()
		{
			// an idea would be to show some t-statistics for the
			// coefficients and then show which are those who seem to
			// be more significant
			System.Windows.Forms.MessageBox.Show(
				"A good analyzer has not yet been thought about, for " +
				"a LinearRegressionTestingPositions" );
		}
	}
}
