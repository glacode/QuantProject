/*
QuantDownloader - Quantitative Finance Library

OTWebDownloader.cs
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.IO;
using System.Threading;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;
using QuantProject.Applications.Downloader.TickerSelectors;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;

namespace QuantProject.Applications.Downloader.OpenTickDownloader.UserForms
{
	/// <summary>
	/// Form to be used for downloading quotes from OpenTick provider
	/// </summary>
	public class OTWebDownloader : System.Windows.Forms.Form, ITickerSelector 
	{
    public System.Windows.Forms.DataGrid dataGrid1;
		public DataSet1 DsTickerCurrentlyDownloaded = new DataSet1();
	  private System.Windows.Forms.Button buttonDownloadQuotesOfSelectedTickers;
	  private DataTable tableOfSelectedTickers;
    private System.Windows.Forms.Label labelStartingDateTime;
    private System.Windows.Forms.DateTimePicker dateTimePickerStartingDate;
    private System.Windows.Forms.RadioButton radioButtonAllAvailableUntilNow;
    private System.Windows.Forms.GroupBox groupBoxWebDownloaderOptions;
    private System.Windows.Forms.RadioButton radioButtonDownloadOnlyAfterMax;
    private Thread downloadThread;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.ComponentModel.IContainer components;
    private bool downloadingInProgress;
    private string textForStartingDownloadingTimeLabel;
    private string textForEndingDownloadingTimeLabel;
    private string[] tickersToDownload;
    private SortedList downloadingTickersSortedList;
    private int indexOfCurrentUpdatingTicker;
    private DateTime currentUpdatingTickerDateTimeOfLastBarUpdate;
    
		public OTWebDownloader(DataTable tableOfSelectedTickers)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.commonInitialization();
			this.tableOfSelectedTickers = tableOfSelectedTickers;
			this.initializeDownloadingTickersSortedList();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
    /// <summary>
    /// common initialization of the controls of the form
    /// </summary>
    private void commonInitialization()
    {
      this.dateTimePickerStartingDate.Value = QuantProject.ADT.ConstantsProvider.InitialDateTimeForDownload;
      this.radioButtonAllAvailableUntilNow.Checked = true;
      this.radioButtonDownloadOnlyAfterMax.Checked = false;
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
      this.Closing += new CancelEventHandler(this.OTWebDownloader_Closing);
      this.indexOfCurrentUpdatingTicker = -1;
    }

    private void initializeDownloadingTickersSortedList()
    {
    	this.downloadingTickersSortedList = new SortedList();
    	for(int i = 0; i<this.tableOfSelectedTickers.Rows.Count; i++)
    		this.downloadingTickersSortedList.Add( this.tableOfSelectedTickers.Rows[i][0], i );
    }
    
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.buttonDownloadQuotesOfSelectedTickers = new System.Windows.Forms.Button();
			this.dateTimePickerStartingDate = new System.Windows.Forms.DateTimePicker();
			this.labelStartingDateTime = new System.Windows.Forms.Label();
			this.radioButtonAllAvailableUntilNow = new System.Windows.Forms.RadioButton();
			this.groupBoxWebDownloaderOptions = new System.Windows.Forms.GroupBox();
			this.radioButtonDownloadOnlyAfterMax = new System.Windows.Forms.RadioButton();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.dateTimeOverwriteQuotesBefore = new System.Windows.Forms.DateTimePicker();
			this.timeFrameInSeconds = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.fromHour = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.fromMin = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.numberOfBars = new System.Windows.Forms.NumericUpDown();
			this.fromSec = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.txtOpenTickUser = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.txtOpenTickPassword = new System.Windows.Forms.TextBox();
			this.checkBoxCheckingForMissingQuotes = new System.Windows.Forms.CheckBox();
			this.checkBoxOverWrite = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.endingDownloadingTimeLabel = new System.Windows.Forms.Label();
			this.startingDownloadingTimeLabel = new System.Windows.Forms.Label();
			this.signallingLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.groupBoxWebDownloaderOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.timeFrameInSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fromHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fromMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numberOfBars)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fromSec)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid1
			// 
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Right;
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(382, 0);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.Size = new System.Drawing.Size(352, 459);
			this.dataGrid1.TabIndex = 1;
			// 
			// buttonDownloadQuotesOfSelectedTickers
			// 
			this.buttonDownloadQuotesOfSelectedTickers.Location = new System.Drawing.Point(6, 408);
			this.buttonDownloadQuotesOfSelectedTickers.Name = "buttonDownloadQuotesOfSelectedTickers";
			this.buttonDownloadQuotesOfSelectedTickers.Size = new System.Drawing.Size(72, 32);
			this.buttonDownloadQuotesOfSelectedTickers.TabIndex = 2;
			this.buttonDownloadQuotesOfSelectedTickers.Text = "Download";
			this.buttonDownloadQuotesOfSelectedTickers.Click += new System.EventHandler(this.buttonDownloadQuotesOfSelectedTickers_Click);
			// 
			// dateTimePickerStartingDate
			// 
			this.dateTimePickerStartingDate.Location = new System.Drawing.Point(86, 49);
			this.dateTimePickerStartingDate.Name = "dateTimePickerStartingDate";
			this.dateTimePickerStartingDate.Size = new System.Drawing.Size(231, 20);
			this.dateTimePickerStartingDate.TabIndex = 6;
			this.dateTimePickerStartingDate.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
			// 
			// labelStartingDateTime
			// 
			this.labelStartingDateTime.Location = new System.Drawing.Point(8, 49);
			this.labelStartingDateTime.Name = "labelStartingDateTime";
			this.labelStartingDateTime.Size = new System.Drawing.Size(80, 23);
			this.labelStartingDateTime.TabIndex = 8;
			this.labelStartingDateTime.Text = "Starting date";
			// 
			// radioButtonAllAvailableUntilNow
			// 
			this.radioButtonAllAvailableUntilNow.Checked = true;
			this.radioButtonAllAvailableUntilNow.Location = new System.Drawing.Point(6, 19);
			this.radioButtonAllAvailableUntilNow.Name = "radioButtonAllAvailableUntilNow";
			this.radioButtonAllAvailableUntilNow.Size = new System.Drawing.Size(272, 24);
			this.radioButtonAllAvailableUntilNow.TabIndex = 10;
			this.radioButtonAllAvailableUntilNow.TabStop = true;
			this.radioButtonAllAvailableUntilNow.Text = "All available quotes until now, since starting date";
			// 
			// groupBoxWebDownloaderOptions
			// 
			this.groupBoxWebDownloaderOptions.Controls.Add(this.radioButtonDownloadOnlyAfterMax);
			this.groupBoxWebDownloaderOptions.Controls.Add(this.radioButtonAllAvailableUntilNow);
			this.groupBoxWebDownloaderOptions.Controls.Add(this.dateTimePickerStartingDate);
			this.groupBoxWebDownloaderOptions.Controls.Add(this.labelStartingDateTime);
			this.groupBoxWebDownloaderOptions.Location = new System.Drawing.Point(10, 97);
			this.groupBoxWebDownloaderOptions.Name = "groupBoxWebDownloaderOptions";
			this.groupBoxWebDownloaderOptions.Size = new System.Drawing.Size(336, 134);
			this.groupBoxWebDownloaderOptions.TabIndex = 13;
			this.groupBoxWebDownloaderOptions.TabStop = false;
			this.groupBoxWebDownloaderOptions.Text = "Web Downloader options (source: OpenTick)";
			// 
			// radioButtonDownloadOnlyAfterMax
			// 
			this.radioButtonDownloadOnlyAfterMax.Location = new System.Drawing.Point(8, 75);
			this.radioButtonDownloadOnlyAfterMax.Name = "radioButtonDownloadOnlyAfterMax";
			this.radioButtonDownloadOnlyAfterMax.Size = new System.Drawing.Size(307, 55);
			this.radioButtonDownloadOnlyAfterMax.TabIndex = 3;
			this.radioButtonDownloadOnlyAfterMax.Text = "Download only quotes after last available quote in the DB (in case no quotes are " +
			"available in the local DB, quotes are downloaded from starting date)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 243);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 41);
			this.label1.TabIndex = 16;
			this.label1.Text = "OverWrite quotes before";
			// 
			// dateTimeOverwriteQuotesBefore
			// 
			this.dateTimeOverwriteQuotesBefore.Location = new System.Drawing.Point(96, 247);
			this.dateTimeOverwriteQuotesBefore.Name = "dateTimeOverwriteQuotesBefore";
			this.dateTimeOverwriteQuotesBefore.Size = new System.Drawing.Size(229, 20);
			this.dateTimeOverwriteQuotesBefore.TabIndex = 15;
			this.dateTimeOverwriteQuotesBefore.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			// 
			// timeFrameInSeconds
			// 
			this.timeFrameInSeconds.Increment = new decimal(new int[] {
									60,
									0,
									0,
									0});
			this.timeFrameInSeconds.Location = new System.Drawing.Point(114, 56);
			this.timeFrameInSeconds.Maximum = new decimal(new int[] {
									3600,
									0,
									0,
									0});
			this.timeFrameInSeconds.Minimum = new decimal(new int[] {
									60,
									0,
									0,
									0});
			this.timeFrameInSeconds.Name = "timeFrameInSeconds";
			this.timeFrameInSeconds.Size = new System.Drawing.Size(49, 20);
			this.timeFrameInSeconds.TabIndex = 17;
			this.timeFrameInSeconds.Value = new decimal(new int[] {
									60,
									0,
									0,
									0});
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 20);
			this.label2.TabIndex = 18;
			this.label2.Text = "Time Frame (sec.)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 23);
			this.label3.TabIndex = 19;
			this.label3.Text = "From:";
			// 
			// fromHour
			// 
			this.fromHour.Location = new System.Drawing.Point(72, 25);
			this.fromHour.Maximum = new decimal(new int[] {
									16,
									0,
									0,
									0});
			this.fromHour.Minimum = new decimal(new int[] {
									9,
									0,
									0,
									0});
			this.fromHour.Name = "fromHour";
			this.fromHour.Size = new System.Drawing.Size(42, 20);
			this.fromHour.TabIndex = 20;
			this.fromHour.Value = new decimal(new int[] {
									9,
									0,
									0,
									0});
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(44, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(22, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "H.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(120, 27);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(30, 13);
			this.label5.TabIndex = 23;
			this.label5.Text = "Min.";
			// 
			// fromMin
			// 
			this.fromMin.Location = new System.Drawing.Point(156, 25);
			this.fromMin.Maximum = new decimal(new int[] {
									59,
									0,
									0,
									0});
			this.fromMin.Name = "fromMin";
			this.fromMin.Size = new System.Drawing.Size(45, 20);
			this.fromMin.TabIndex = 22;
			this.fromMin.Value = new decimal(new int[] {
									30,
									0,
									0,
									0});
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(169, 58);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(68, 20);
			this.label6.TabIndex = 25;
			this.label6.Text = "n° of bars";
			// 
			// numberOfBars
			// 
			this.numberOfBars.Location = new System.Drawing.Point(243, 56);
			this.numberOfBars.Maximum = new decimal(new int[] {
									10000,
									0,
									0,
									0});
			this.numberOfBars.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.numberOfBars.Name = "numberOfBars";
			this.numberOfBars.Size = new System.Drawing.Size(59, 20);
			this.numberOfBars.TabIndex = 24;
			this.numberOfBars.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			// 
			// fromSec
			// 
			this.fromSec.Location = new System.Drawing.Point(257, 24);
			this.fromSec.Maximum = new decimal(new int[] {
									59,
									0,
									0,
									0});
			this.fromSec.Name = "fromSec";
			this.fromSec.Size = new System.Drawing.Size(45, 20);
			this.fromSec.TabIndex = 26;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(219, 26);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(30, 13);
			this.label7.TabIndex = 27;
			this.label7.Text = "Sec.";
			// 
			// txtOpenTickUser
			// 
			this.txtOpenTickUser.Location = new System.Drawing.Point(63, 361);
			this.txtOpenTickUser.Name = "txtOpenTickUser";
			this.txtOpenTickUser.Size = new System.Drawing.Size(78, 20);
			this.txtOpenTickUser.TabIndex = 28;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 361);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 23);
			this.label8.TabIndex = 29;
			this.label8.Text = "user";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(169, 361);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(58, 23);
			this.label9.TabIndex = 31;
			this.label9.Text = "password";
			// 
			// txtOpenTickPassword
			// 
			this.txtOpenTickPassword.Location = new System.Drawing.Point(247, 361);
			this.txtOpenTickPassword.Name = "txtOpenTickPassword";
			this.txtOpenTickPassword.Size = new System.Drawing.Size(78, 20);
			this.txtOpenTickPassword.TabIndex = 30;
			this.txtOpenTickPassword.UseSystemPasswordChar = true;
			// 
			// checkBoxCheckingForMissingQuotes
			// 
			this.checkBoxCheckingForMissingQuotes.Location = new System.Drawing.Point(12, 331);
			this.checkBoxCheckingForMissingQuotes.Name = "checkBoxCheckingForMissingQuotes";
			this.checkBoxCheckingForMissingQuotes.Size = new System.Drawing.Size(315, 24);
			this.checkBoxCheckingForMissingQuotes.TabIndex = 32;
			this.checkBoxCheckingForMissingQuotes.Text = "Check for missing quotes";
			this.checkBoxCheckingForMissingQuotes.UseVisualStyleBackColor = true;
			// 
			// checkBoxOverWrite
			// 
			this.checkBoxOverWrite.Location = new System.Drawing.Point(12, 287);
			this.checkBoxOverWrite.Name = "checkBoxOverWrite";
			this.checkBoxOverWrite.Size = new System.Drawing.Size(246, 38);
			this.checkBoxOverWrite.TabIndex = 33;
			this.checkBoxOverWrite.Text = "OverWrite all quotes,  also after the given date";
			this.checkBoxOverWrite.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(93, -1);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(131, 23);
			this.label10.TabIndex = 34;
			this.label10.Text = "New York Local Time";
			// 
			// endingDownloadingTimeLabel
			// 
			this.endingDownloadingTimeLabel.Location = new System.Drawing.Point(84, 436);
			this.endingDownloadingTimeLabel.Name = "endingDownloadingTimeLabel";
			this.endingDownloadingTimeLabel.Size = new System.Drawing.Size(262, 23);
			this.endingDownloadingTimeLabel.TabIndex = 36;
			this.endingDownloadingTimeLabel.Text = ".";
			// 
			// startingDownloadingTimeLabel
			// 
			this.startingDownloadingTimeLabel.Location = new System.Drawing.Point(84, 392);
			this.startingDownloadingTimeLabel.Name = "startingDownloadingTimeLabel";
			this.startingDownloadingTimeLabel.Size = new System.Drawing.Size(262, 23);
			this.startingDownloadingTimeLabel.TabIndex = 37;
			this.startingDownloadingTimeLabel.Text = ".";
			// 
			// signallingLabel
			// 
			this.signallingLabel.Location = new System.Drawing.Point(84, 415);
			this.signallingLabel.Name = "signallingLabel";
			this.signallingLabel.Size = new System.Drawing.Size(274, 12);
			this.signallingLabel.TabIndex = 38;
			// 
			// OTWebDownloader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(734, 459);
			this.Controls.Add(this.signallingLabel);
			this.Controls.Add(this.startingDownloadingTimeLabel);
			this.Controls.Add(this.endingDownloadingTimeLabel);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkBoxOverWrite);
			this.Controls.Add(this.checkBoxCheckingForMissingQuotes);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.txtOpenTickPassword);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtOpenTickUser);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.fromSec);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.numberOfBars);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.fromMin);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.fromHour);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.timeFrameInSeconds);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dateTimeOverwriteQuotesBefore);
			this.Controls.Add(this.groupBoxWebDownloaderOptions);
			this.Controls.Add(this.buttonDownloadQuotesOfSelectedTickers);
			this.Controls.Add(this.dataGrid1);
			this.Name = "OTWebDownloader";
			this.Text = "OT Web downloader";
			this.Load += new System.EventHandler(this.otWebDownloaderLoad);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OTWebDownloaderPaint);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.groupBoxWebDownloaderOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.timeFrameInSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fromHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fromMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numberOfBars)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fromSec)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
    }
		private System.Windows.Forms.Label signallingLabel;
		private System.Windows.Forms.Label endingDownloadingTimeLabel;
		private System.Windows.Forms.Label startingDownloadingTimeLabel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkBoxOverWrite;
		private System.Windows.Forms.CheckBox checkBoxCheckingForMissingQuotes;
		private System.Windows.Forms.TextBox txtOpenTickPassword;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtOpenTickUser;
		private System.Windows.Forms.NumericUpDown timeFrameInSeconds;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown fromSec;
		private System.Windows.Forms.NumericUpDown numberOfBars;
		private System.Windows.Forms.DateTimePicker dateTimeOverwriteQuotesBefore;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown fromMin;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown fromHour;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		#endregion
	  
    #region ITickerSelector's implementation
    public DataTable GetTableOfSelectedTickers()
    {
      return TickerSelector.GetTableOfManuallySelectedTickers(this.dataGrid1);
    }

    public void SelectAllTickers()
    {
      DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
      int indexOfRow = 0;
      while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
      {
        this.dataGrid1.Select(indexOfRow);
        indexOfRow++;
      }
    }    
    #endregion

    #region form options
        
    public bool IsOnlyAfterLastQuoteSelected
    {
      get
      {
        return this.radioButtonDownloadOnlyAfterMax.Checked;
      }
    }
    public bool IsOverWriteSelected
    {
      get
      {
        return this.checkBoxOverWrite.Checked;
      }
    }
    #endregion
    
	  private void buttonDownloadQuotesOfSelectedTickers_Click(object sender, System.EventArgs e)
    {
	  	try{
	  		OTTickerDownloader tickerDownloader =
	  			new OTTickerDownloader(
	  				this.TickersToDownload,
	  				this.StartingNewYorkDateTime,
	  				Convert.ToInt32( this.fromHour.Value ) ,
	  				Convert.ToInt32( this.fromMin.Value ) ,
	  				Convert.ToInt32( this.fromSec.Value ) ,
	  				Convert.ToInt32( this.timeFrameInSeconds.Value ) ,
	  				Convert.ToInt32( this.numberOfBars.Value ) ,
	  				this.dateTimeOverwriteQuotesBefore.Value,
	  				this.checkBoxCheckingForMissingQuotes.Checked,
	  				this.checkBoxOverWrite.Checked,
	  				this.radioButtonDownloadOnlyAfterMax.Checked,
	  				this.txtOpenTickUser.Text,
	  				this.txtOpenTickPassword.Text);
	  		tickerDownloader.DownloadingStarted +=
	  			new DownloadingStartedEventHandler(this.setStartingTime_atDownloadedStarted);
	  		tickerDownloader.DatabaseUpdated +=
	  			new DatabaseUpdatedEventHandler(this.refreshIndexAndDateTimeForCurrentUpdatingTicker);
	  		tickerDownloader.DownloadingCompleted +=
	  			new DownloadingCompletedEventHandler(this.setEndingTime_atDownloadedCompleted);
	  		this.buttonDownloadQuotesOfSelectedTickers.Enabled = false;
	  		this.downloadThread = new Thread( tickerDownloader.DownloadTickers );
	  		this.downloadThread.Start();
	  	}
	  	catch(Exception ex)
    	{
    		MessageBox.Show(ex.Message);
    	}
    }
		
	  #region Form's methods called by the thread started at OTTickerDownloader's method 
		
	   private void refreshIndexAndDateTimeForCurrentUpdatingTicker(object sender, DatabaseUpdatedEventArgs eventArgs)
    {
    	lock(this.downloadingTickersSortedList)
    	{
	    	this.currentUpdatingTickerDateTimeOfLastBarUpdate = 
	    		eventArgs.DateTimeOfLastBarUpdated;
	    	int sortedListIndexOfKey = 
	    		this.downloadingTickersSortedList.IndexOfKey(eventArgs.Ticker);
	    	this.indexOfCurrentUpdatingTicker = 
	    		(int)this.downloadingTickersSortedList.GetByIndex(sortedListIndexOfKey);
	     	this.downloadingInProgress = true; //if a database
	    	//update event has been risen, downloading has to be
	    	//in progress
	    	this.Invalidate();//this forces the form to repaint itself
	    	//in a thread - safe manner
	   	}
    }
    
    private void setStartingTime_atDownloadedStarted(object sender, DownloadingStartedEventArgs eventArgs)
    {
    	lock(this.startingDownloadingTimeLabel)
    	{
    	    this.textForStartingDownloadingTimeLabel =
    	     	"Downloading started at: " + eventArgs.StartingDateTime.ToString();
	   			this.downloadingInProgress = true;
    	}
    }
    private void setEndingTime_atDownloadedCompleted(object sender, DownloadingCompletedEventArgs eventArgs)
    {
    	lock(this.endingDownloadingTimeLabel)
    	{
    		this.textForEndingDownloadingTimeLabel = "Downloading completed at: " +
	    		eventArgs.EndingDateTime.ToString();
	    	this.downloadingInProgress = false;
    	}
    }
	  
    #endregion
        
    #region properties
    
    public DateTime StartingNewYorkDateTime
    {
      get
      {
      	return new DateTime(this.dateTimePickerStartingDate.Value.Year,
      			             this.dateTimePickerStartingDate.Value.Month,
      			             this.dateTimePickerStartingDate.Value.Day,
      			             Convert.ToInt32(this.fromHour.Value),
      			             Convert.ToInt32(this.fromMin.Value),
      			             Convert.ToInt32(this.fromSec.Value));
      }
    }
    public int TimeFrameInSeconds
    {
      get
      {
      	return Convert.ToInt32(this.timeFrameInSeconds.Value);
      }
    }
    public int NumberOfBars
    {
      get
      {
      	return Convert.ToInt32(this.numberOfBars.Value);
      }
    }
    public string[] TickersToDownload
    {
    	get
      {
    		if(this.tickersToDownload == null)
    		{
    			this.tickersToDownload = 
    				new string[this.tableOfSelectedTickers.Rows.Count];
    			for(int i = 0; i < this.tickersToDownload.Length; i++)
    				this.tickersToDownload[i] = 
    					(string)this.tableOfSelectedTickers.Rows[i][0];
    		}
    		return this.tickersToDownload;
      }
    }
    public bool DownloadInProgress
    {
    	get
      {
      	return this.downloadingInProgress;
      }
    }
    
    #endregion
    
    private void otWebDownloaderLoad_setStyleForDataGrid()
		{
			DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
			dataGrid1TableStyle.MappingName = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].TableName;
			dataGrid1TableStyle.ColumnHeadersVisible = true;
			dataGrid1TableStyle.ReadOnly = true;
			dataGrid1TableStyle.SelectionBackColor = Color.DimGray ;
			
			
      DataGridTextBoxColumn columnStyle_Tickers = new DataGridTextBoxColumn();
			columnStyle_Tickers.MappingName = "tickers";
			columnStyle_Tickers.HeaderText = "Tickers";
			columnStyle_Tickers.TextBox.Enabled = false;
			columnStyle_Tickers.NullText = "";
			columnStyle_Tickers.Width = 60;
			
			DataGridTextBoxColumn columnStyle_dbUpdated = new DataGridTextBoxColumn();
			columnStyle_dbUpdated.MappingName = "dbUpdated";
			columnStyle_dbUpdated.HeaderText = "DB Updated";
			columnStyle_dbUpdated.TextBox.Enabled = false;
			columnStyle_dbUpdated.NullText = "";
			columnStyle_dbUpdated.Width = 70;
      
			DataGridTextBoxColumn columnStyle_dateTimeOfLastBar = new DataGridTextBoxColumn();
      columnStyle_dateTimeOfLastBar.MappingName = "dateTimeOfLastBar";
      columnStyle_dateTimeOfLastBar.HeaderText = "Date Time of Last Bar";
      columnStyle_dateTimeOfLastBar.TextBox.Enabled = false;
      columnStyle_dateTimeOfLastBar.NullText = "";
      columnStyle_dateTimeOfLastBar.Width = 120;

      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_Tickers);
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_dbUpdated);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_dateTimeOfLastBar);
     
			this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
		}
    
    private void otWebDownloaderLoad_fillDataGridWithTickersToBeDownloaded()
    {
      //prepares the table for the dataGrid
    	if (!this.DsTickerCurrentlyDownloaded.Tables.Contains( "Tickers" ))
      {
        this.DsTickerCurrentlyDownloaded.Tables.Add( "Tickers" );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( 
          new DataColumn( "tickers" , this.tableOfSelectedTickers.Columns[0].DataType ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "dbUpdated" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "dateTimeOfLastBar" , System.Type.GetType( "System.DateTime" ) );
      }
    	//populates the dataGrid with the selectedTickers to be downloaded
    	for (int i = 0; i<this.tableOfSelectedTickers.Rows.Count; i++)
    	{
    		DataRow newRow = this.DsTickerCurrentlyDownloaded.Tables["Tickers"].NewRow();
    		newRow[0] = this.tableOfSelectedTickers.Rows[i][0];
    		newRow[1] = "No";//databaseUpdated
    		newRow[2] = DBNull.Value;//dateTime of the last updated bar
    		this.DsTickerCurrentlyDownloaded.Tables["Tickers"].Rows.Add(newRow);
    	}
    	//link of dataGrid to the table
    	this.dataGrid1.DataSource = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ];
    	this.otWebDownloaderLoad_setStyleForDataGrid();
    }
    
		private void otWebDownloaderLoad(object sender, EventArgs e)
		{
			this.otWebDownloaderLoad_fillDataGridWithTickersToBeDownloaded();
		}
								
		private void OTWebDownloader_Closing(Object sender, CancelEventArgs e)
		{
       if (this.downloadingInProgress)
       {
          e.Cancel = true;
          MessageBox.Show("You can't close the form if downloading is still in progress!");
       }
       else
       {
          e.Cancel = false;
       }
    }
		
		private void OTWebDownloaderPaint_refreshSignallingLabel()
		{
			if(this.downloadingInProgress)
			{
				if(this.signallingLabel.Text.Length<60)
					this.signallingLabel.Text += "--";
				else
					this.signallingLabel.Text = "";
			}
			else
			{
				this.signallingLabel.Text = "";
			}
		}
		
		private void OTWebDownloaderPaint_refreshTableOfSelectedTickers()
		{
      lock(this.downloadingTickersSortedList)
			{
				DataTable tickersCurrentylDownloaded =
		    	this.DsTickerCurrentlyDownloaded.Tables["Tickers"];
		    if(this.indexOfCurrentUpdatingTicker != -1)
		    	//a databaseUpdated event has been risen
		    {
			   	tickersCurrentylDownloaded.Rows[this.indexOfCurrentUpdatingTicker][1] = "Yes";
			   	tickersCurrentylDownloaded.Rows[this.indexOfCurrentUpdatingTicker][2]	= 
			   		this.currentUpdatingTickerDateTimeOfLastBarUpdate;
		    }
      }
    }
		
		private void OTWebDownloaderPaint_refreshEndingDownloadingTimeLabel()
		{
      lock(this.endingDownloadingTimeLabel)
			{
				this.endingDownloadingTimeLabel.Text = 
					this.textForEndingDownloadingTimeLabel;
			}
	  }
		
		private void OTWebDownloaderPaint_refreshStartingDownloadingTimeLabel()
		{
      lock(this.startingDownloadingTimeLabel)
			{
				this.startingDownloadingTimeLabel.Text = 
					this.textForStartingDownloadingTimeLabel;
			}
	  }
		
		//this event handler should be called only by the 
		//thread that owns the form. With these locks,
		//information coming from the thread that owns the
		//OT ticker downloader should be "treated"
		//(written and displayed by the form) in a thread-safe manner
		void OTWebDownloaderPaint(object sender, PaintEventArgs e)
		{
			this.OTWebDownloaderPaint_refreshStartingDownloadingTimeLabel();
			this.OTWebDownloaderPaint_refreshSignallingLabel();
			this.OTWebDownloaderPaint_refreshTableOfSelectedTickers();
			this.OTWebDownloaderPaint_refreshEndingDownloadingTimeLabel();
		}
	}
}
