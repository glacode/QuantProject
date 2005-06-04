/*
QuantProject - Quantitative Finance Library

ReportTabControl.cs
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
using System.Windows.Forms;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// TabControl for the report form
	/// </summary>
	public class ReportTabControl : TabControl
	{
		private AccountReport accountReport;
		private EquityChartTabPage equityChart;
		private SummaryTabPage summary;
		private ReportGridTabPage roundTrades;
		private ReportGridTabPage equity;
		private ReportGridTabPage transactions;

		public ReportGrid TransactionGrid
		{
			get { return this.transactions.ReportGrid; }
		}

		public ReportTabControl( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.Dock = DockStyle.Fill;
			this.equityChart = new EquityChartTabPage( this.accountReport );
			this.Controls.Add( this.equityChart );
			this.summary = new SummaryTabPage( this.accountReport );
			this.Controls.Add( this.summary );
			this.roundTrades = new ReportGridTabPage(
				"Round Trades" , this.accountReport.RoundTrades );
			this.Controls.Add( this.roundTrades );
			this.equity = new ReportGridTabPage(
				"Equity" , this.accountReport.Equity );
			this.Controls.Add( this.equity );
			this.transactions = new ReportGridTabPage(
				"Transactions" , this.accountReport.TransactionTable );
			this.Controls.Add( this.transactions );
		}
	}
}
