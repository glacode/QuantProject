using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using QuantProject.Business.Financial.Accounting;
using QuantProject.ADT.FileManaging;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataProviders;
using QuantProject.Business.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;





namespace QuantProject.Principale
{
	/// <summary>
	/// AccountViewer.
	/// </summary>
	public class AccountViewer : System.Windows.Forms.Form
	{
    private ArrayList accountsArrayList = new ArrayList();
    private System.Windows.Forms.Button buttonAddAccounts;
    private System.Windows.Forms.DataGrid dataGridAccounts;
    private DataTable accountsTable;
    private System.Windows.Forms.Button buttonViewReport;
		
		private System.ComponentModel.Container components = null;

		public AccountViewer()
		{
			
			//
			InitializeComponent();
      this.setTableAndGrid();

			//
			// TODO: 
			//
		}
    
    private void fillAccountsArrayList(IList accountList)
    {
      foreach(object item in accountList)
      {
        this.accountsArrayList.Add((Account)item);
      }
      //
    }

    public AccountViewer(IList accountList)
    {
			
      //
      InitializeComponent();
      this.fillAccountsArrayList(accountList);
      this.setTableAndGrid();
      this.updateForm();
      //
      // TODO: 
      //
    }

    private void setTableAndGrid()
    {
      this.accountsTable = new DataTable();
      this.accountsTable.Columns.Add("AccountName", System.Type.GetType("System.String"));
      this.accountsTable.Columns.Add("Fitness", Type.GetType("System.Double"));
      this.accountsTable.Columns.Add("Account", Type.GetType("System.Object"));
      this.dataGridAccounts.DataSource = this.accountsTable;
    }
		/// <summary>
		/// Clean up
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		//do not modify with code editor
		private void InitializeComponent()
		{
      this.buttonAddAccounts = new System.Windows.Forms.Button();
      this.dataGridAccounts = new System.Windows.Forms.DataGrid();
      this.buttonViewReport = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridAccounts)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonAddAccounts
      // 
      this.buttonAddAccounts.Location = new System.Drawing.Point(8, 160);
      this.buttonAddAccounts.Name = "buttonAddAccounts";
      this.buttonAddAccounts.Size = new System.Drawing.Size(96, 32);
      this.buttonAddAccounts.TabIndex = 1;
      this.buttonAddAccounts.Text = "Add Accounts ...";
      this.buttonAddAccounts.Click += new System.EventHandler(this.buttonAddAccounts_Click);
      // 
      // dataGridAccounts
      // 
      this.dataGridAccounts.DataMember = "";
      this.dataGridAccounts.Dock = System.Windows.Forms.DockStyle.Top;
      this.dataGridAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGridAccounts.Name = "dataGridAccounts";
      this.dataGridAccounts.Size = new System.Drawing.Size(464, 144);
      this.dataGridAccounts.TabIndex = 2;
      // 
      // buttonViewReport
      // 
      this.buttonViewReport.Location = new System.Drawing.Point(336, 160);
      this.buttonViewReport.Name = "buttonViewReport";
      this.buttonViewReport.Size = new System.Drawing.Size(96, 32);
      this.buttonViewReport.TabIndex = 3;
      this.buttonViewReport.Text = "View Report";
      this.buttonViewReport.Click += new System.EventHandler(this.buttonViewReport_Click);
      // 
      // AccountViewer
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(464, 266);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.buttonViewReport,
                                                                  this.dataGridAccounts,
                                                                  this.buttonAddAccounts});
      this.Name = "AccountViewer";
      this.Text = "AccountViewer";
      ((System.ComponentModel.ISupportInitialize)(this.dataGridAccounts)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion
    
    
    private void updateForm_updateGrid()
    {
      this.dataGridAccounts.Refresh();
    }
    
    private void updateForm_updateTable()
    {
      Account account;
      foreach(Object item in this.accountsArrayList)
      {
        DataRow rowToAdd = this.accountsTable.NewRow();
        account = (Account)item;
        rowToAdd["AccountName"] = account.Key;
        rowToAdd["Fitness"] = account.GetFitnessValue();
        rowToAdd["Account"] = account;
        this.accountsTable.Rows.Add(rowToAdd);
      }
    }
    
    private void updateForm()
    {
      this.updateForm_updateTable();
      this.updateForm_updateGrid();
    }
 
    #region Add Accounts

    private string[] buttonAddAccounts_Click_extractAccounts_selectAccounts()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select accounts please ...";
      openFileDialog.Multiselect = true;
      openFileDialog.CheckFileExists = true;
      openFileDialog.ShowDialog();
      return openFileDialog.FileNames;
    }

    private void buttonAddAccounts_Click_extractAccounts()
    {
      string[] accountsSelection = this.buttonAddAccounts_Click_extractAccounts_selectAccounts();
      foreach(string fileName in accountsSelection)
      {
        this.accountsArrayList.Add((Account)ObjectArchiver.Extract(fileName));
      }
    }

    private void buttonAddAccounts_Click(object sender, System.EventArgs e)
    {
      this.accountsArrayList.Clear();
      this.buttonAddAccounts_Click_extractAccounts();
      this.updateForm();
    }
    #endregion

    private void buttonViewReport_Click(object sender, System.EventArgs e)
    {
      DataTable selectedAccountTable = 
                  TickerSelector.GetTableOfManuallySelectedTickers(this.dataGridAccounts);
      if(selectedAccountTable.Rows.Count != 1 )
      //only a single row is a valid selection
        MessageBox.Show("You can't view more than one report!");
      else
      {
        Report report = new Report((Account)selectedAccountTable.Rows[0]["Account"],
                                  new HistoricalAdjustedQuoteProvider());
        ReportShower reportShower = new ReportShower(report);
        reportShower.Show(); 
      }
     
    }
	
  }
}
