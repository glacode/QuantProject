/*
QuantProject - Quantitative Finance Library

BackTesterReportViewer.cs
Copyright (C) 2008
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.General.Reporting
{
	/// <summary>
	/// Displays a report for a given EndOfDayStrategyBackTester
	/// </summary>
	public class BackTesterReportViewer
	{
		public BackTesterReportViewer()
		{
		}
		/// <summary>
		/// Displays a report for a given EndOfDayStrategyBackTester
		/// </summary>
		/// <param name="lastDateTimeRequestedForTheScript"></param>
		/// <param name="endOfDayStrategyBackTester"></param>
		public static void ShowReport(
			DateTime lastDateTimeRequestedForTheScript ,
			EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{
			DateTime lastReportDateTime =
				endOfDayStrategyBackTester.ActualLastDateTime;
			Report report = new Report(
				endOfDayStrategyBackTester.AccountReport ,
				true );
			report.Create( endOfDayStrategyBackTester.DescriptionForLogFileName , 1 ,
				new EndOfDayDateTime( lastReportDateTime ,
				EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
				endOfDayStrategyBackTester.Benchmark.Ticker );
			report.Show();
		}
	}
}
