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

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class WebDownloader : System.Windows.Forms.Form, ITickerSelector 
	{
    public OleDbConnection OleDbConnection1 = ConnectionProvider.OleDbConnection;
    private System.Windows.Forms.Button button1;
    public System.Windows.Forms.DataGrid dataGrid1;
    private System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1;
    private System.Data.OleDb.OleDbCommand oleDbSelectCommand1;
    private System.Data.OleDb.OleDbCommand oleDbInsertCommand1;
    private System.Data.OleDb.OleDbCommand oleDbUpdateCommand1;
    private System.Data.OleDb.OleDbCommand oleDbDeleteCommand1;
//    private QuantProject.DataSet1 dataSet11;
    private System.Data.OleDb.OleDbCommand oleDbCommand1;
    public DataSet1 DsTickerCurrentlyDownloaded = new DataSet1();
	  private System.Windows.Forms.Button buttonDownloadQuotesOfSelectedTickers;
	  private DataTable tableOfSelectedTickers;
    internal System.Windows.Forms.Label labelNumberOfTickersToDownload;
    internal System.Windows.Forms.Label labelTickersLeft;
    private System.Windows.Forms.Label labelStartingDateTime;
    private System.Windows.Forms.DateTimePicker dateTimePickerStartingDate;
    private System.Windows.Forms.RadioButton radioButtonAllAvailableUntilNow;
    private System.Windows.Forms.GroupBox groupBoxWebDownloaderOptions;
    private System.Windows.Forms.GroupBox groupBoxUpdateDatabaseOptions;
    internal System.Windows.Forms.RadioButton radioButtonOverWriteYes;
    private System.Windows.Forms.RadioButton radioButtonOverWriteNo;
    private System.Windows.Forms.RadioButton radioButtonAllAvailableUntilNowSinceStartingDate;
    internal System.Windows.Forms.CheckBox checkBoxIsDicotomicSearchActivated;
    private System.Windows.Forms.RadioButton radioButtonDownloadBeforeMinAndAfterMax;
    private System.Windows.Forms.RadioButton radioButtonDownloadOnlyAfterMax;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WebDownloader()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.commonInitialization();
      
      //
			this.Text = "Download quotes of all tickers in the database"; 
			this.buttonDownloadQuotesOfSelectedTickers.Visible = false;
      // TODO: retrieve number of all the tickers symbol stored in DB
      this.labelNumberOfTickersToDownload.Visible = false;
      this.labelTickersLeft.Visible = false;
			//
			
		}

		public WebDownloader(DataTable tableOfSelectedTickers)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.commonInitialization();
			//
			this.Text = "Download quotes of selected tickers"; 
			this.button1.Visible = false;
			this.tableOfSelectedTickers = tableOfSelectedTickers;
      this.labelNumberOfTickersToDownload.Text = Convert.ToString(tableOfSelectedTickers.Rows.Count);
			//
			
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
      //this.dateTimePickerStartingDate.Refresh();
      this.radioButtonAllAvailableUntilNow.Checked = true;
      this.radioButtonDownloadOnlyAfterMax.Checked = true;
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
    }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.button1 = new System.Windows.Forms.Button();
      this.dataGrid1 = new System.Windows.Forms.DataGrid();
      this.oleDbDataAdapter1 = new System.Data.OleDb.OleDbDataAdapter();
      this.oleDbDeleteCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbInsertCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbSelectCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbUpdateCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbCommand1 = new System.Data.OleDb.OleDbCommand();
      this.buttonDownloadQuotesOfSelectedTickers = new System.Windows.Forms.Button();
      this.labelNumberOfTickersToDownload = new System.Windows.Forms.Label();
      this.labelTickersLeft = new System.Windows.Forms.Label();
      this.dateTimePickerStartingDate = new System.Windows.Forms.DateTimePicker();
      this.labelStartingDateTime = new System.Windows.Forms.Label();
      this.radioButtonAllAvailableUntilNow = new System.Windows.Forms.RadioButton();
      this.radioButtonAllAvailableUntilNowSinceStartingDate = new System.Windows.Forms.RadioButton();
      this.groupBoxWebDownloaderOptions = new System.Windows.Forms.GroupBox();
      this.checkBoxIsDicotomicSearchActivated = new System.Windows.Forms.CheckBox();
      this.groupBoxUpdateDatabaseOptions = new System.Windows.Forms.GroupBox();
      this.radioButtonDownloadOnlyAfterMax = new System.Windows.Forms.RadioButton();
      this.radioButtonDownloadBeforeMinAndAfterMax = new System.Windows.Forms.RadioButton();
      this.radioButtonOverWriteNo = new System.Windows.Forms.RadioButton();
      this.radioButtonOverWriteYes = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.groupBoxWebDownloaderOptions.SuspendLayout();
      this.groupBoxUpdateDatabaseOptions.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(88, 352);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(112, 32);
      this.button1.TabIndex = 0;
      this.button1.Text = "Download all Tickers\'quotes";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // dataGrid1
      // 
      this.dataGrid1.DataMember = "";
      this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGrid1.Location = new System.Drawing.Point(328, 8);
      this.dataGrid1.Name = "dataGrid1";
      this.dataGrid1.Size = new System.Drawing.Size(304, 456);
      this.dataGrid1.TabIndex = 1;
      // 
      // oleDbDataAdapter1
      // 
      this.oleDbDataAdapter1.DeleteCommand = this.oleDbDeleteCommand1;
      this.oleDbDataAdapter1.InsertCommand = this.oleDbInsertCommand1;
      this.oleDbDataAdapter1.SelectCommand = this.oleDbSelectCommand1;
      this.oleDbDataAdapter1.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                                                                                                new System.Data.Common.DataTableMapping("Table", "quotes", new System.Data.Common.DataColumnMapping[] {
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quId", "quId"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quTicker", "quTicker"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quDate", "quDate"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quOpen", "quOpen"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quHigh", "quHigh"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quLow", "quLow"),
                                                                                                                                                                                                        new System.Data.Common.DataColumnMapping("quClose", "quClose")})});
      this.oleDbDataAdapter1.UpdateCommand = this.oleDbUpdateCommand1;
      // 
      // oleDbDeleteCommand1
      // 
      this.oleDbDeleteCommand1.CommandText = @"DELETE FROM quotes WHERE (quId = ?) AND (quClose = ? OR ? IS NULL AND quClose IS NULL) AND (quDate = ? OR ? IS NULL AND quDate IS NULL) AND (quHigh = ? OR ? IS NULL AND quHigh IS NULL) AND (quLow = ? OR ? IS NULL AND quLow IS NULL) AND (quOpen = ? OR ? IS NULL AND quOpen IS NULL) AND (quTicker = ? OR ? IS NULL AND quTicker IS NULL)";
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quId", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "quId", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quDate", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate1", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quDate", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker1", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null));
      // 
      // oleDbInsertCommand1
      // 
      this.oleDbInsertCommand1.CommandText = "INSERT INTO quotes (quClose, quDate, quHigh, quLow, quOpen, quTicker, quVolume) V" +
        "ALUES (?, ?, ?, ?, ?, ?, ?)";
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Current, null));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "quDate"));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Current, null));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Current, null));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Current, null));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "quTicker"));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quVolume", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "quVolume", System.Data.DataRowVersion.Current, null));
      // 
      // oleDbSelectCommand1
      // 
      this.oleDbSelectCommand1.CommandText = "SELECT quClose, quDate, quHigh, quLow, quOpen, quTicker FROM quotes";
      // 
      // oleDbUpdateCommand1
      // 
      this.oleDbUpdateCommand1.CommandText = @"UPDATE quotes SET quClose = ?, quDate = ?, quHigh = ?, quLow = ?, quOpen = ?, quTicker = ? WHERE (quId = ?) AND (quClose = ? OR ? IS NULL AND quClose IS NULL) AND (quDate = ? OR ? IS NULL AND quDate IS NULL) AND (quHigh = ? OR ? IS NULL AND quHigh IS NULL) AND (quLow = ? OR ? IS NULL AND quLow IS NULL) AND (quOpen = ? OR ? IS NULL AND quOpen IS NULL) AND (quTicker = ? OR ? IS NULL AND quTicker IS NULL)";
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Current, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "quDate"));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Current, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Current, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Current, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "quTicker"));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quId", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "quId", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quClose", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quDate", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate1", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quDate", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quLow", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker1", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null));
      // 
      // oleDbCommand1
      // 
      this.oleDbCommand1.CommandText = "DELETE quotes.* FROM quotes INNER JOIN tickers ON quotes.quTicker = tickers.tiTic" +
        "ker";
      // 
      // buttonDownloadQuotesOfSelectedTickers
      // 
      this.buttonDownloadQuotesOfSelectedTickers.Location = new System.Drawing.Point(88, 392);
      this.buttonDownloadQuotesOfSelectedTickers.Name = "buttonDownloadQuotesOfSelectedTickers";
      this.buttonDownloadQuotesOfSelectedTickers.Size = new System.Drawing.Size(112, 32);
      this.buttonDownloadQuotesOfSelectedTickers.TabIndex = 2;
      this.buttonDownloadQuotesOfSelectedTickers.Text = "Download quotes";
      this.buttonDownloadQuotesOfSelectedTickers.Click += new System.EventHandler(this.buttonDownloadQuotesOfSelectedTickers_Click);
      // 
      // labelNumberOfTickersToDownload
      // 
      this.labelNumberOfTickersToDownload.Location = new System.Drawing.Point(152, 440);
      this.labelNumberOfTickersToDownload.Name = "labelNumberOfTickersToDownload";
      this.labelNumberOfTickersToDownload.Size = new System.Drawing.Size(48, 24);
      this.labelNumberOfTickersToDownload.TabIndex = 4;
      this.labelNumberOfTickersToDownload.Text = "0";
      // 
      // labelTickersLeft
      // 
      this.labelTickersLeft.Location = new System.Drawing.Point(8, 440);
      this.labelTickersLeft.Name = "labelTickersLeft";
      this.labelTickersLeft.Size = new System.Drawing.Size(136, 24);
      this.labelTickersLeft.TabIndex = 5;
      this.labelTickersLeft.Text = "Tickers Left to download:";
      // 
      // dateTimePickerStartingDate
      // 
      this.dateTimePickerStartingDate.Location = new System.Drawing.Point(96, 104);
      this.dateTimePickerStartingDate.Name = "dateTimePickerStartingDate";
      this.dateTimePickerStartingDate.Size = new System.Drawing.Size(184, 20);
      this.dateTimePickerStartingDate.TabIndex = 6;
      // 
      // labelStartingDateTime
      // 
      this.labelStartingDateTime.Location = new System.Drawing.Point(8, 104);
      this.labelStartingDateTime.Name = "labelStartingDateTime";
      this.labelStartingDateTime.Size = new System.Drawing.Size(80, 23);
      this.labelStartingDateTime.TabIndex = 8;
      this.labelStartingDateTime.Text = "Starting date";
      // 
      // radioButtonAllAvailableUntilNow
      // 
      this.radioButtonAllAvailableUntilNow.Location = new System.Drawing.Point(8, 24);
      this.radioButtonAllAvailableUntilNow.Name = "radioButtonAllAvailableUntilNow";
      this.radioButtonAllAvailableUntilNow.Size = new System.Drawing.Size(272, 24);
      this.radioButtonAllAvailableUntilNow.TabIndex = 10;
      this.radioButtonAllAvailableUntilNow.Text = "All available quotes until now, since starting date";
      this.radioButtonAllAvailableUntilNow.CheckedChanged += new System.EventHandler(this.radioButtonAllAvailableUntilNow_CheckedChanged);
      // 
      // radioButtonAllAvailableUntilNowSinceStartingDate
      // 
      this.radioButtonAllAvailableUntilNowSinceStartingDate.Location = new System.Drawing.Point(8, 56);
      this.radioButtonAllAvailableUntilNowSinceStartingDate.Name = "radioButtonAllAvailableUntilNowSinceStartingDate";
      this.radioButtonAllAvailableUntilNowSinceStartingDate.Size = new System.Drawing.Size(272, 24);
      this.radioButtonAllAvailableUntilNowSinceStartingDate.TabIndex = 12;
      this.radioButtonAllAvailableUntilNowSinceStartingDate.Text = "All available quotes until now, changing starting date";
      // 
      // groupBoxWebDownloaderOptions
      // 
      this.groupBoxWebDownloaderOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                               this.radioButtonAllAvailableUntilNowSinceStartingDate,
                                                                                               this.radioButtonAllAvailableUntilNow});
      this.groupBoxWebDownloaderOptions.Location = new System.Drawing.Point(8, 8);
      this.groupBoxWebDownloaderOptions.Name = "groupBoxWebDownloaderOptions";
      this.groupBoxWebDownloaderOptions.Size = new System.Drawing.Size(312, 88);
      this.groupBoxWebDownloaderOptions.TabIndex = 13;
      this.groupBoxWebDownloaderOptions.TabStop = false;
      this.groupBoxWebDownloaderOptions.Text = "Web Downloader options (source: Yahoo)";
      // 
      // checkBoxIsDicotomicSearchActivated
      // 
      this.checkBoxIsDicotomicSearchActivated.Checked = true;
      this.checkBoxIsDicotomicSearchActivated.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxIsDicotomicSearchActivated.Location = new System.Drawing.Point(16, 320);
      this.checkBoxIsDicotomicSearchActivated.Name = "checkBoxIsDicotomicSearchActivated";
      this.checkBoxIsDicotomicSearchActivated.Size = new System.Drawing.Size(272, 24);
      this.checkBoxIsDicotomicSearchActivated.TabIndex = 13;
      this.checkBoxIsDicotomicSearchActivated.Text = "Use dicotomic search";
      // 
      // groupBoxUpdateDatabaseOptions
      // 
      this.groupBoxUpdateDatabaseOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                                this.radioButtonDownloadOnlyAfterMax,
                                                                                                this.radioButtonDownloadBeforeMinAndAfterMax,
                                                                                                this.radioButtonOverWriteNo,
                                                                                                this.radioButtonOverWriteYes});
      this.groupBoxUpdateDatabaseOptions.Location = new System.Drawing.Point(8, 136);
      this.groupBoxUpdateDatabaseOptions.Name = "groupBoxUpdateDatabaseOptions";
      this.groupBoxUpdateDatabaseOptions.Size = new System.Drawing.Size(312, 176);
      this.groupBoxUpdateDatabaseOptions.TabIndex = 14;
      this.groupBoxUpdateDatabaseOptions.TabStop = false;
      this.groupBoxUpdateDatabaseOptions.Text = "Update Database options";
      // 
      // radioButtonDownloadOnlyAfterMax
      // 
      this.radioButtonDownloadOnlyAfterMax.Checked = true;
      this.radioButtonDownloadOnlyAfterMax.Location = new System.Drawing.Point(16, 24);
      this.radioButtonDownloadOnlyAfterMax.Name = "radioButtonDownloadOnlyAfterMax";
      this.radioButtonDownloadOnlyAfterMax.Size = new System.Drawing.Size(288, 24);
      this.radioButtonDownloadOnlyAfterMax.TabIndex = 3;
      this.radioButtonDownloadOnlyAfterMax.TabStop = true;
      this.radioButtonDownloadOnlyAfterMax.Text = "Download only quotes after last quote (fastest)";
      // 
      // radioButtonDownloadBeforeMinAndAfterMax
      // 
      this.radioButtonDownloadBeforeMinAndAfterMax.Location = new System.Drawing.Point(16, 56);
      this.radioButtonDownloadBeforeMinAndAfterMax.Name = "radioButtonDownloadBeforeMinAndAfterMax";
      this.radioButtonDownloadBeforeMinAndAfterMax.Size = new System.Drawing.Size(288, 32);
      this.radioButtonDownloadBeforeMinAndAfterMax.TabIndex = 2;
      this.radioButtonDownloadBeforeMinAndAfterMax.Text = "Download only quotes before first quote and after last quote";
      // 
      // radioButtonOverWriteNo
      // 
      this.radioButtonOverWriteNo.Location = new System.Drawing.Point(16, 96);
      this.radioButtonOverWriteNo.Name = "radioButtonOverWriteNo";
      this.radioButtonOverWriteNo.Size = new System.Drawing.Size(288, 32);
      this.radioButtonOverWriteNo.TabIndex = 1;
      this.radioButtonOverWriteNo.Text = "Download all quotes, adding to database only the missing ones";
      // 
      // radioButtonOverWriteYes
      // 
      this.radioButtonOverWriteYes.Location = new System.Drawing.Point(16, 136);
      this.radioButtonOverWriteYes.Name = "radioButtonOverWriteYes";
      this.radioButtonOverWriteYes.Size = new System.Drawing.Size(288, 32);
      this.radioButtonOverWriteYes.TabIndex = 0;
      this.radioButtonOverWriteYes.Text = "Download all quotes, overwriting the existing ones in database";
      // 
      // WebDownloader
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(664, 470);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.groupBoxUpdateDatabaseOptions,
                                                                  this.groupBoxWebDownloaderOptions,
                                                                  this.labelStartingDateTime,
                                                                  this.dateTimePickerStartingDate,
                                                                  this.labelTickersLeft,
                                                                  this.labelNumberOfTickersToDownload,
                                                                  this.buttonDownloadQuotesOfSelectedTickers,
                                                                  this.dataGrid1,
                                                                  this.button1,
                                                                  this.checkBoxIsDicotomicSearchActivated});
      this.Name = "WebDownloader";
      this.Text = "Web downloader";
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.groupBoxWebDownloaderOptions.ResumeLayout(false);
      this.groupBoxUpdateDatabaseOptions.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    #region Code not used anymore
	  /*
	  
    private void downloadQuotes_addRecord_toDatabase( string tiTicker , StreamReader sr)
    {
      string symbol=tiTicker;

      string[] ytemp=sr.ReadLine().Split(',');



      if(ytemp[0] != "Date")
      {
        oleDbDataAdapter1.InsertCommand.Connection.Open();

        oleDbDataAdapter1.InsertCommand.Parameters["quDate"].Value=ytemp[0];

        oleDbDataAdapter1.InsertCommand.Parameters["quOpen"].Value=ytemp[1];

        oleDbDataAdapter1.InsertCommand.Parameters["quHigh"].Value=ytemp[2];

        oleDbDataAdapter1.InsertCommand.Parameters["quLow"].Value=ytemp[3];

        oleDbDataAdapter1.InsertCommand.Parameters["quClose"].Value=ytemp[4];

        oleDbDataAdapter1.InsertCommand.Parameters["quVolume"].Value=ytemp[5];

        oleDbDataAdapter1.InsertCommand.Parameters["quTicker"].Value=symbol.ToString();


        try
        {
          oleDbDataAdapter1.InsertCommand.ExecuteNonQuery();
        }
        catch ( Exception ex )
        {
          MessageBox.Show( ex.ToString() );
        }

        oleDbDataAdapter1.InsertCommand.Connection.Close();
      }

    }
	
	
    private void downloadQuotes_deleteFrom_quotes()
    {
      FileInfo fi = new FileInfo("C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\quotes.csv");
      //FileStream fs = fi.Create();
      //fi.Delete();
      File.Copy("C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\empty_quotes.csv",
        "C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\quotes.csv" , true);
    }  
	
	private void downloadQuotes_forTicker_writeFile( string quTicker)
    {
      HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=1&b=1&c=1990&d=5&e=12&f=2003&s=" + quTicker + "&y=0&g=d&ignore=.csv");

      Req.Method = "GET";

      //this.textBox2.Text = "prima di GetResponse";
      //this.textBox2.Refresh();
      HttpWebResponse hwr = (HttpWebResponse)Req.GetResponse();
      //this.textBox2.Text = "prima di GetResponseStream";
      //this.textBox2.Refresh();
      Stream strm = hwr.GetResponseStream();
      //this.textBox2.Text = "Prima di StreamReader";
      //this.textBox2.Refresh();
      StreamReader sr = new StreamReader(strm);
      //this.textBox2.Text = "Prima di StreamWriter";
      //this.textBox2.Refresh();
      StreamWriter sw=new StreamWriter( "C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\" + quTicker + ".csv");
      //this.textBox2.Text = "Prima di ReadToEnd";
      //this.textBox2.Refresh();
      string myString = sr.ReadToEnd();
      //this.textBox2.Text = "Dopo ReadToEnd";
      //this.textBox2.Refresh();
      sw.Write(myString);
      //this.textBox2.Text = "file intermedio creato!";
      //this.textBox2.Refresh();
      sw.Close();
      sr.Close();
      strm.Close();
      //this.textBox2.Text = "file intermedio creato!";
      //this.textBox2.Refresh();
    }
    private void downloadQuotes_forTicker_importFile( string quTicker)
    {
      string sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            "C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\" +
            ";Extended Properties=\"Text;HDR=NO;FMT=Delimited\"";

      System.Data.OleDb.OleDbConnection objConn = new System.Data.OleDb.OleDbConnection(sConnectionString);
      objConn.Open();
      System.Data.OleDb.OleDbCommand odCommand = new System.Data.OleDb.OleDbCommand();
      odCommand.Connection = objConn;
      try
      {
        odCommand.CommandText = "insert into quotes.csv SELECT * FROM " + quTicker + ".csv";
        odCommand.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Console.WriteLine("{0} Exception caught.", ex);
      }
      objConn.Close();
    }
    private void downloadQuotes_forTicker( string quTicker)
    {
      downloadQuotes_forTicker_writeFile( quTicker );
      //this.textBox2.Text = "importo";
      //this.textBox2.Refresh();
      downloadQuotes_forTicker_importFile( quTicker );
    }
	  
    */
    #endregion
	  
    #region ITickerSelector's implementation
    public TickerDataTable GetTableOfSelectedTickers()
    {
      DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
      TickerDataTable tableOfSelectedTickers = new TickerDataTable();
      int indexOfRow = 0;
      while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
      {
        if(this.dataGrid1.IsSelected(indexOfRow))
        {
          DataRow dataRow = tableOfSelectedTickers.NewRow(); 
          dataRow[0] = (string)dataTableOfDataGrid1.Rows[indexOfRow][0];
          tableOfSelectedTickers.Rows.Add(dataRow);
        }
        indexOfRow++;
      }
      return tableOfSelectedTickers;
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


    private void downloadQuotes_withTickerDataSet_create_dsTickerCurrentlyDownloaded( DataTable dt )
    {
      if (!this.DsTickerCurrentlyDownloaded.Tables.Contains( "Tickers" ))
      {
        this.DsTickerCurrentlyDownloaded.Tables.Add( "Tickers" );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( 
          new DataColumn( dt.Columns[ "tiTicker" ].ColumnName , dt.Columns[ "tiTicker" ].DataType ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "currentState" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "adjustedClose" , System.Type.GetType( "System.String" ) );
        this.dataGrid1.DataSource = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ];
      }
    }

    private void downloadQuotes_withTickerDataSet( DataSet ds )
    {
      Cursor.Current = Cursors.WaitCursor;
      downloadQuotes_withTickerDataSet_create_dsTickerCurrentlyDownloaded( ds.Tables[0] );
      foreach (DataRow myRow in ds.Tables[0].Rows) 
      {
        //if (this.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Count>5)
        //  Monitor.Wait( this.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ] );
        TickerDownloader qd = new TickerDownloader( this , myRow , myRow["tiTicker"].ToString() , ds.Tables[0].Rows.Count );
        //Thread newThread = new Thread( new ThreadStart( qd.downloadTicker));
        //newThread.Start();
        if(this.radioButtonAllAvailableUntilNowSinceStartingDate.Checked)
        {
          qd.DownloadTicker(this.dateTimePickerStartingDate.Value);
        }
        else
        {
          qd.DownloadTicker();
        }
      Cursor.Current = Cursors.Default;
      
        //newThread.Join();
        //qd.downloadTicker();
      }
      //ImportQuotesCsv iqc = new ImportQuotesCsv();
    }
    
    private void downloadQuotes_createTickerDataSet( DataSet ds )
	  {
		  System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1=new OleDbDataAdapter( "select * from tickers" , this.OleDbConnection1);
		  oleDbDataAdapter1.Fill(ds);
	  }	
    
    private void downloadQuotesOfAllTickers()
    {
      DataSet ds=new DataSet();
      downloadQuotes_createTickerDataSet( ds );
      downloadQuotes_withTickerDataSet( ds );
      this.OleDbConnection1.Close();
    }
    

	  private void downloadQuotesOfSelectedTickers()
	  {
		  DataSet ds=new DataSet();
		  ds.Tables.Add(this.tableOfSelectedTickers);
      downloadQuotes_withTickerDataSet( ds );
		  this.OleDbConnection1.Close();
	  }
  	
	  private void openDbAndSetOleDbCommand()
	  {
		  try
		  {
			  Cursor.Current = Cursors.WaitCursor;
			  if (this.OleDbConnection1.State != ConnectionState.Open)
            this.OleDbConnection1.Open();
			  oleDbCommand1.Connection = this.OleDbConnection1;
			  //this.oleDbCommand1.ExecuteNonQuery();
			  // NOTE that the execution of the previous line
			  // causes the deletion of all records in quotes !
  	
		  }
		  catch(Exception ex)
		  {
			  MessageBox.Show(ex.ToString());
		  }
  		
		  finally
		  {
			  this.OleDbConnection1.Close();
			  Cursor.Current = Cursors.Default;
		  }
  	
	  }


    private void button1_Click(object sender, System.EventArgs e)
    {
	    this.openDbAndSetOleDbCommand();
	    this.downloadQuotesOfAllTickers();
      this.button1.Enabled = false;
    }

    private void buttonDownloadQuotesOfSelectedTickers_Click(object sender, System.EventArgs e)
    {
	    this.openDbAndSetOleDbCommand();
	    this.downloadQuotesOfSelectedTickers();
      this.buttonDownloadQuotesOfSelectedTickers.Enabled = false;
    }

    private void radioButtonAllAvailableUntilNow_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this.radioButtonAllAvailableUntilNow.Checked == true)
      {
        this.dateTimePickerStartingDate.Enabled = false;
      }
      else
      {
        this.dateTimePickerStartingDate.Enabled = true;
      }
    }

    public bool IsUpdateOptionSelected
    {
      get
      {
        return (this.radioButtonDownloadBeforeMinAndAfterMax.Checked || 
                this.radioButtonDownloadOnlyAfterMax.Checked);
      }
    }

    public bool IsOnlyAfterLastQuoteSelected
    {
      get
      {
        return this.radioButtonDownloadOnlyAfterMax.Checked;
      }
    }
  }
}
