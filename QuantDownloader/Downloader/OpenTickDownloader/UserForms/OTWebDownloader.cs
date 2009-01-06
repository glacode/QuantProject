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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.IO;
using System.Threading;

using QuantProject.ADT.Timing;
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
			this.checkedListOfDailyTimes = new System.Windows.Forms.CheckedListBox();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.groupBoxWebDownloaderOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.timeFrameInSeconds)).BeginInit();
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
			this.dataGrid1.Size = new System.Drawing.Size(352, 469);
			this.dataGrid1.TabIndex = 1;
			// 
			// buttonDownloadQuotesOfSelectedTickers
			// 
			this.buttonDownloadQuotesOfSelectedTickers.Location = new System.Drawing.Point(5, 418);
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
			this.groupBoxWebDownloaderOptions.Location = new System.Drawing.Point(12, 122);
			this.groupBoxWebDownloaderOptions.Name = "groupBoxWebDownloaderOptions";
			this.groupBoxWebDownloaderOptions.Size = new System.Drawing.Size(336, 141);
			this.groupBoxWebDownloaderOptions.TabIndex = 13;
			this.groupBoxWebDownloaderOptions.TabStop = false;
			this.groupBoxWebDownloaderOptions.Text = "Web Downloader options (source: OpenTick)";
			// 
			// radioButtonDownloadOnlyAfterMax
			// 
			this.radioButtonDownloadOnlyAfterMax.Location = new System.Drawing.Point(6, 79);
			this.radioButtonDownloadOnlyAfterMax.Name = "radioButtonDownloadOnlyAfterMax";
			this.radioButtonDownloadOnlyAfterMax.Size = new System.Drawing.Size(307, 55);
			this.radioButtonDownloadOnlyAfterMax.TabIndex = 3;
			this.radioButtonDownloadOnlyAfterMax.Text = "Download only quotes after last available quote in the DB (in case no quotes are " +
			"available in the local DB, quotes are downloaded from starting date)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 269);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 41);
			this.label1.TabIndex = 16;
			this.label1.Text = "OverWrite quotes before";
			// 
			// dateTimeOverwriteQuotesBefore
			// 
			this.dateTimeOverwriteQuotesBefore.Location = new System.Drawing.Point(98, 269);
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
			this.timeFrameInSeconds.Location = new System.Drawing.Point(299, 55);
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
			this.label2.Location = new System.Drawing.Point(191, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 20);
			this.label2.TabIndex = 18;
			this.label2.Text = "Time Frame (sec.)";
			// 
			// txtOpenTickUser
			// 
			this.txtOpenTickUser.Location = new System.Drawing.Point(63, 382);
			this.txtOpenTickUser.Name = "txtOpenTickUser";
			this.txtOpenTickUser.Size = new System.Drawing.Size(78, 20);
			this.txtOpenTickUser.TabIndex = 28;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 382);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 23);
			this.label8.TabIndex = 29;
			this.label8.Text = "user";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(169, 382);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(58, 23);
			this.label9.TabIndex = 31;
			this.label9.Text = "password";
			// 
			// txtOpenTickPassword
			// 
			this.txtOpenTickPassword.Location = new System.Drawing.Point(247, 382);
			this.txtOpenTickPassword.Name = "txtOpenTickPassword";
			this.txtOpenTickPassword.Size = new System.Drawing.Size(78, 20);
			this.txtOpenTickPassword.TabIndex = 30;
			this.txtOpenTickPassword.UseSystemPasswordChar = true;
			// 
			// checkBoxCheckingForMissingQuotes
			// 
			this.checkBoxCheckingForMissingQuotes.Location = new System.Drawing.Point(12, 352);
			this.checkBoxCheckingForMissingQuotes.Name = "checkBoxCheckingForMissingQuotes";
			this.checkBoxCheckingForMissingQuotes.Size = new System.Drawing.Size(315, 24);
			this.checkBoxCheckingForMissingQuotes.TabIndex = 32;
			this.checkBoxCheckingForMissingQuotes.Text = "Check for missing quotes";
			this.checkBoxCheckingForMissingQuotes.UseVisualStyleBackColor = true;
			// 
			// checkBoxOverWrite
			// 
			this.checkBoxOverWrite.Location = new System.Drawing.Point(12, 308);
			this.checkBoxOverWrite.Name = "checkBoxOverWrite";
			this.checkBoxOverWrite.Size = new System.Drawing.Size(246, 38);
			this.checkBoxOverWrite.TabIndex = 33;
			this.checkBoxOverWrite.Text = "OverWrite all quotes,  also after the given date";
			this.checkBoxOverWrite.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(131, 23);
			this.label10.TabIndex = 34;
			this.label10.Text = "New York Local Times";
			// 
			// endingDownloadingTimeLabel
			// 
			this.endingDownloadingTimeLabel.Location = new System.Drawing.Point(83, 452);
			this.endingDownloadingTimeLabel.Name = "endingDownloadingTimeLabel";
			this.endingDownloadingTimeLabel.Size = new System.Drawing.Size(262, 20);
			this.endingDownloadingTimeLabel.TabIndex = 36;
			this.endingDownloadingTimeLabel.Text = ".";
			// 
			// startingDownloadingTimeLabel
			// 
			this.startingDownloadingTimeLabel.Location = new System.Drawing.Point(83, 405);
			this.startingDownloadingTimeLabel.Name = "startingDownloadingTimeLabel";
			this.startingDownloadingTimeLabel.Size = new System.Drawing.Size(262, 22);
			this.startingDownloadingTimeLabel.TabIndex = 37;
			this.startingDownloadingTimeLabel.Text = ".";
			// 
			// signallingLabel
			// 
			this.signallingLabel.Location = new System.Drawing.Point(83, 427);
			this.signallingLabel.Name = "signallingLabel";
			this.signallingLabel.Size = new System.Drawing.Size(274, 23);
			this.signallingLabel.TabIndex = 38;
			// 
			// checkedListOfDailyTimes
			// 
			this.checkedListOfDailyTimes.FormattingEnabled = true;
			this.checkedListOfDailyTimes.Location = new System.Drawing.Point(12, 22);
			this.checkedListOfDailyTimes.Name = "checkedListOfDailyTimes";
			this.checkedListOfDailyTimes.Size = new System.Drawing.Size(173, 94);
			this.checkedListOfDailyTimes.TabIndex = 39;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(227, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(66, 23);
			this.button1.TabIndex = 40;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// OTWebDownloader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(734, 469);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.checkedListOfDailyTimes);
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
			this.ResumeLayout(false);
			this.PerformLayout();
    }
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckedListBox checkedListOfDailyTimes;
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
		private System.Windows.Forms.DateTimePicker dateTimeOverwriteQuotesBefore;
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
	  		if(this.CheckedDailyTimes.Count == 0)
	  			throw new Exception("Check at least one daily time!");
	  		OTTickerDownloader tickerDownloader =
	  			new OTTickerDownloader(
	  				this.TickersToDownload,
	  				this.StartingNewYorkDateTime,
	  				this.CheckedDailyTimes[0].Hour ,
	  				this.CheckedDailyTimes[0].Minute ,
	  				this.CheckedDailyTimes[0].Second ,
	  				Convert.ToInt32( this.timeFrameInSeconds.Value ) ,
	  				1 ,
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
      	return Time.GetDateTimeFromMerge( new DateTime(this.dateTimePickerStartingDate.Value.Year,
      			             											this.dateTimePickerStartingDate.Value.Month,
      			             											this.dateTimePickerStartingDate.Value.Day) ,
      			             									new Time((string)this.checkedListOfDailyTimes.CheckedItems[0]) );
      }
    }
    public int TimeFrameInSeconds
    {
      get
      {
      	return Convert.ToInt32(this.timeFrameInSeconds.Value);
      }
    }
    
    public List<Time> CheckedDailyTimes
    {
     	get
      {
     		List<Time> checkedDailyTimes = 
     			new List<Time>();
     		for(int i = 0; 
     		    i < this.checkedListOfDailyTimes.CheckedItems.Count;
     		    i++)
     			checkedDailyTimes.Add(new Time((string)this.checkedListOfDailyTimes.CheckedItems[i]) );
     			
      	return checkedDailyTimes;
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
    
    #endregion properties
    
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
    		
    private void otWebDownloaderLoad_populateCheckedListOfDailyTimes()
    {
    	Time firstTimeToAdd = new Time(9,30,0);
    	Time lastTimeToAdd = new Time(16,0,0);
    	Time currentTimeToAdd = firstTimeToAdd;
    	while( currentTimeToAdd <= lastTimeToAdd )
    	{
    		this.checkedListOfDailyTimes.Items.Add(currentTimeToAdd.GetFormattedString(), false);
    		currentTimeToAdd = currentTimeToAdd.AddMinutes( 1 );
    	}
    }
    
    private void otWebDownloaderLoad(object sender, EventArgs e)
		{
			this.otWebDownloaderLoad_fillDataGridWithTickersToBeDownloaded();
			this.otWebDownloaderLoad_populateCheckedListOfDailyTimes();
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
		
		void Button1Click(object sender, EventArgs e)
		{
			List<Time> listDT = 
				this.CheckedDailyTimes;
		}
	}
}
