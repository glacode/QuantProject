/*
QuantProject - Quantitative Finance Library

AverageReturnOnTuesdayWithOpenPositions.cs
Copyright (C) 2008 
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

using QuantProject.Business.Financial.Accounting.Reporting.Tables;

namespace QuantProject.Business.Financial.Accounting.Reporting.StatisticsSummaryRows
{
	/// <summary>
	/// Summary row containing the average return of the strategy on Tuesday
	/// </summary>
	[Serializable]
	public class AverageReturnOnTuesdayWithOpenPositions : AverageReturnOnDayOfWeekWithOpenPositions
	{
		public AverageReturnOnTuesdayWithOpenPositions( StatisticsSummary statisticsSummary )
																: base( statisticsSummary )
		{
		}

		protected override string getRowDescription()
		{
			return "Average % return on Tuesday (with opened positions)";
		}
		
		protected override DayOfWeek getSpecificDayOfWeek()
		{
			return DayOfWeek.Tuesday;
		}

	}
}
