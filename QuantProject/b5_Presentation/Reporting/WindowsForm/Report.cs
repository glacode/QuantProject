/*
QuantProject - Quantitative Finance Library

Report.cs
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
using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Timing;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// Windows Form account report
	/// </summary>
	
  [Serializable]
	public class Report : Form
	{
		private Account account;
		private IHistoricalQuoteProvider historicalQuoteProvider;
		private AccountReport accountReport;
		private ReportTabControl reportTabControl;

		public Report( Account account , IHistoricalQuoteProvider historicalQuoteProvider )
		{
			this.account = account;
			this.historicalQuoteProvider = historicalQuoteProvider;
		}
		public Report( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.account = this.accountReport.Account;
		}

		/// <summary>
		/// Populates the form and displays itself
		/// </summary>
		private void show_set_accountReport( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime , string buyAndHoldTicker )
		{
			if ( this.accountReport == null )
				this.accountReport = this.account.CreateReport( reportName ,
					numDaysForInterval , endDateTime , buyAndHoldTicker ,
					this.historicalQuoteProvider );
		}
		private void show_populateForm()
		{
			this.Location = new System.Drawing.Point( 1000,500);
			this.Width = 700;
			this.Height = 500;
			this.reportTabControl = new ReportTabControl( this.accountReport );
			this.Controls.Add( this.reportTabControl );
		}
		public void Show( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime , string buyAndHoldTicker )
		{
			this.show_set_accountReport( reportName ,
				numDaysForInterval , endDateTime , buyAndHoldTicker );
			this.show_populateForm();
			base.ShowDialog();
		}
    public new void Show()
    {
      if(this.accountReport != null)
        this.show_populateForm();
      
      base.ShowDialog();
    }
    /// <summary>
    /// Clears the existing account report, so that a new one can be created
    /// </summary>
    public void Clear()
    {
      this.accountReport = null;
    }
  
//    /// <summary>
//    /// Imports an existing account report
//    /// </summary>
//    public void Import(AccountReport accountReportToBeImported)
//    {
//      this.accountReport = accountReportToBeImported;
//    }
	}
}
