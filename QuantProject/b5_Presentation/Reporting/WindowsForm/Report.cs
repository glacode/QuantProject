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
using System.Drawing;
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
		private MainMenu mainMenu;
		private System.Windows.Forms.MenuItem file;
		private System.Windows.Forms.MenuItem saveAccount;
		private System.Windows.Forms.MenuItem saveReport;
    private System.Windows.Forms.MenuItem saveTransactions;
    private SaveFileDialog saveFileDialog;

		public AccountReport AccountReport
		{
			get
			{
				AccountReport returnValue = this.accountReport;
				if ( this.accountReport == null )
					throw new Exception( "The AccountReport has not been created yet!" );
				return returnValue;
			}
		}
		public ReportGrid TransactionGrid
		{
			get { return this.reportTabControl.TransactionGrid; }
		}

		public Report( Account account , IHistoricalQuoteProvider historicalQuoteProvider )
		{
			this.account = account;
			this.historicalQuoteProvider = historicalQuoteProvider;
			this.initializeComponent();
		}
		public Report( AccountReport accountReport )
		{
			this.report( accountReport , true );
		}
		public Report( AccountReport accountReport , bool showBenchmark )
		{
			this.report( accountReport , showBenchmark );
		}
		private void report( AccountReport accountReport , bool showBenchmark )
		{
			this.accountReport = accountReport;
			this.account = this.accountReport.Account;
			this.initializeComponent();
			this.create_populateForm( showBenchmark );
		}
		
		private void initializeComponent()
		{
			this.mainMenu = new MainMenu();
		
			this.saveAccount = new MenuItem();
			this.saveAccount.Text = "Save Account";
			
			this.saveReport = new MenuItem();
			this.saveReport.Text = "Save Report";

      this.saveTransactions = new MenuItem();
      this.saveTransactions.Text = "Save Transactions";
			
			this.file = new MenuItem();
			this.file.Text = "File";
			this.mainMenu.MenuItems.AddRange(new MenuItem[]
			                                 {this.file});
			
			this.file.MenuItems.AddRange(new MenuItem[] 
			                             {this.saveAccount,
			                             	this.saveReport,
                                    this.saveTransactions});
			this.Menu = this.mainMenu;
			this.saveAccount.Click += new System.EventHandler(this.saveAccount_Click);
			this.saveReport.Click += new System.EventHandler(this.saveReport_Click);
			this.saveTransactions.Click += new System.EventHandler(this.saveTransactions_Click);
		}
		
		/// <summary>
		/// Populates the form and displays itself
		/// </summary>
		private void create_set_accountReport( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime , string buyAndHoldTicker )
		{
			if ( this.accountReport == null )
				this.accountReport = this.account.CreateReport( reportName ,
					numDaysForInterval , endDateTime , buyAndHoldTicker ,
					this.historicalQuoteProvider );
		}
		private void create_populateForm( bool showBenchmark )
		{
			this.Location = new System.Drawing.Point( 1000,500);
			this.Width = 700;
			this.Height = 500;
			this.Text = this.accountReport.Description;
			this.reportTabControl = new ReportTabControl( this.accountReport ,
				showBenchmark );
			this.Controls.Add( this.reportTabControl );
		}
		/// <summary>
		/// Creates the report data, but it doesn't show the report
		/// </summary>
		/// <param name="reportName"></param>
		/// <param name="numDaysForInterval"></param>
		/// <param name="endDateTime"></param>
		/// <param name="benchmark"></param>
		public void Create( string reportName ,	int numDaysForInterval ,
			EndOfDayDateTime endDateTime , string benchmark )
		{
			this.Create( reportName , numDaysForInterval , endDateTime ,
				benchmark , true );
		}
		public void Create( string reportName ,	int numDaysForInterval ,
			EndOfDayDateTime endDateTime , string benchmark , bool showBenchmark )
		{
			create_set_accountReport( reportName ,
				numDaysForInterval , endDateTime , benchmark );
			create_populateForm( showBenchmark );
		}
		public void Show( string reportName ,
			int numDaysForInterval , EndOfDayDateTime endDateTime , string benchmark )
		{
			if ( this.accountReport == null )
				this.Create( reportName ,	numDaysForInterval , endDateTime , benchmark );
			base.Show();
		}
//    public new void Show()
//    {
//      if(this.accountReport != null)
//        this.show_populateForm();
//      
//      base.Show();
//    }
    /// <summary>
    /// Clears the existing account report, so that a new one can be created
    /// </summary>
    public void Clear()
    {
      this.accountReport = null;
    }
    
    /// <summary>
    /// Gets the date of the last trade
    /// </summary>
    public DateTime GetLastTradeDate()
    {
      object returnValue;
      returnValue = this.account.Transactions.GetKey(this.account.Transactions.Count -1);
      return (DateTime)returnValue;
    }
		
    #region save account or report or transactions
    
    private void saveAccount_Click(object sender, System.EventArgs e)
    {
    	this.saveObject((MenuItem)sender);
    }
 
   
		private void saveReport_Click(object sender, System.EventArgs e)
    {
			this.saveObject((MenuItem)sender);
	  }
    
    private void saveTransactions_Click(object sender, System.EventArgs e)
    {
      this.saveObject((MenuItem)sender);
    }
    
    private void saveObject_setSaveFileDialog(MenuItem sender)
    {
      this.saveFileDialog = new SaveFileDialog();
      if(sender.Text.EndsWith("Report"))
        //if text property of the menu item sender contains at the end
        // the word "Report", then it will be saved an account Report object
      {
        this.saveFileDialog.DefaultExt = "qPr";
        this.saveFileDialog.InitialDirectory = 
          System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"];
      }
      else if(sender.Text.EndsWith("Account"))
        //else the text property of the menu item sender contains at the end
        // the word "Account"; so it will be saved an account object
      { 
        this.saveFileDialog.DefaultExt = "qPa";
        this.saveFileDialog.InitialDirectory =
           System.Configuration.ConfigurationSettings.AppSettings["AccountsArchive"];
      }
      else if(sender.Text.EndsWith("Transactions"))
        //else the text property of the menu item sender contains at the end
        // the word "Transactions"; so it will be saved a TransactionHistory object
      { 
        this.saveFileDialog.DefaultExt = "qPt";
      }
      
      this.saveFileDialog.AddExtension = true;
      this.saveFileDialog.CreatePrompt = true;
      this.saveFileDialog.OverwritePrompt = true;
      this.saveFileDialog.Title = sender.Text;
      //the saveFileDialog title is the same as the
      //menu item clicked by the user
      this.saveFileDialog.CheckPathExists = true;

			this.saveFileDialog.FileName = this.Text;
   }

    private void saveObject(MenuItem sender)
    {
      this.saveObject_setSaveFileDialog(sender);
      this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.fileOk_Click);
      this.saveFileDialog.ShowDialog();
    }
    
    private void fileOk_Click(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(((SaveFileDialog)sender).Title.EndsWith("Report"))
        QuantProject.ADT.FileManaging.ObjectArchiver.Archive(this.accountReport,
                                                           this.saveFileDialog.FileName);
      else if(((SaveFileDialog)sender).Title.EndsWith("Account"))
        QuantProject.ADT.FileManaging.ObjectArchiver.Archive(this.account,
          this.saveFileDialog.FileName);
      else if(((SaveFileDialog)sender).Title.EndsWith("Transactions"))
        QuantProject.ADT.FileManaging.ObjectArchiver.Archive(this.account.Transactions,
          this.saveFileDialog.FileName);
    }




		#endregion
		public void AddEquityLine( EquityLine equityLine ,
			Color color )
		{
			this.reportTabControl.EquityChart.Add( equityLine , color );
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
