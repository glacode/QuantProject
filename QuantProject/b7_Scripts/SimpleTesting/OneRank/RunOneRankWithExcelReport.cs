/*
QuantProject - Quantitative Finance Library

RunOneRankWithExcelReport.cs
Copyright (C) 2003 
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
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Scripting;
using QuantProject.Presentation.Reporting.MicrosoftExcel;

namespace QuantProject.Scripts
{
	/// <summary>
	/// An example script to show the Excel report feature
	/// </summary>
	public class RunOneRankWithExcelReport : RunOneRank
	{
    public RunOneRankWithExcelReport()
    {
    }
    public override void Run()
    {
			base.Run();
			AccountReport accountReport = this.account.CreateReport( this.ticker , 7 ,
				new EndOfDayDateTime( endDateTime , EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
				ticker );
			ExcelManager.Add( accountReport );
			ExcelManager.ShowReport();
		}
	}
}
