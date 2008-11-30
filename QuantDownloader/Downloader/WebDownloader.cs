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

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class WebDownloader : System.Windows.Forms.Form, ITickerSelector 
	{
    public OleDbConnection OleDbConnection1 = ConnectionProvider.OleDbConnection;
    public System.Windows.Forms.DataGrid dataGrid1;
    private System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1;
    private System.Data.OleDb.OleDbCommand oleDbSelectCommand1;
    private System.Data.OleDb.OleDbCommand oleDbInsertCommand1;
    private System.Data.OleDb.OleDbCommand oleDbUpdateCommand1;
    private System.Data.OleDb.OleDbCommand oleDbDeleteCommand1;
    private System.Data.OleDb.OleDbCommand oleDbCommand1;
    public DataSet1 DsTickerCurrentlyDownloaded = new DataSet1();
	  private System.Windows.Forms.Button buttonDownloadQuotesOfSelectedTickers;
	  private DataTable tableOfSelectedTickers;
    private System.Windows.Forms.Label labelStartingDateTime;
    private System.Windows.Forms.DateTimePicker dateTimePickerStartingDate;
    private System.Windows.Forms.GroupBox groupBoxUpdateDatabaseOptions;
    internal System.Windows.Forms.RadioButton radioButtonOverWriteYes;
    internal System.Windows.Forms.CheckBox checkBoxIsDicotomicSearchActivated;
    private System.Windows.Forms.RadioButton radioButtonDownloadOnlyAfterMax;
    internal System.Windows.Forms.CheckBox checkBoxComputeCloseToCloseValues;
    private System.Windows.Forms.ToolTip toolTip1;
    internal System.Windows.Forms.CheckBox checkBoxDownloadOnlyAfterCloseToCloseCheck;
    private System.Windows.Forms.DateTimePicker dateTimePickerSelectedDate;
    private System.Windows.Forms.RadioButton radioButtonDownloadSingleQuote;
    private System.ComponentModel.IContainer components;
    private Hashtable downloadingTickers;
    private int indexOfCurrentUpdatingTicker;
    private string lastQuoteInDB;
    private string currentState;
    private string databaseUpdated;
    private string adjustedClose;
    private string adjCloseToCloseRatio;
    private bool downloadingInProgress;
    
		public WebDownloader()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.commonInitialization();
      this.Text = "Download quotes of all tickers in the database"; 
			// TODO: set tableOfSelectedTickers by retrieving all the tickers'symbols stored in the DB
			this.initializeDownloadingTickers();
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
			this.tableOfSelectedTickers = tableOfSelectedTickers;
			this.initializeDownloadingTickers();
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
      this.radioButtonDownloadOnlyAfterMax.Checked = true;
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
      this.toolTip1.SetToolTip(this.checkBoxComputeCloseToCloseValues,
                                "It is possible to compute close to close " +
                                "ratios out of connection");
      this.lastQuoteInDB = "...";
      this.currentState = "...";
      this.databaseUpdated = "...";
      this.adjCloseToCloseRatio = "...";
      this.adjustedClose = "...";
    }
		
    private void initializeDownloadingTickers()
    {
    	this.downloadingTickers = new Hashtable();
    	for(int i = 0; i<this.tableOfSelectedTickers.Rows.Count; i++)
    		this.downloadingTickers.Add( this.tableOfSelectedTickers.Rows[i][0], i );
    }
    
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebDownloader));
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.oleDbDataAdapter1 = new System.Data.OleDb.OleDbDataAdapter();
			this.oleDbDeleteCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbInsertCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbSelectCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbUpdateCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbCommand1 = new System.Data.OleDb.OleDbCommand();
			this.buttonDownloadQuotesOfSelectedTickers = new System.Windows.Forms.Button();
			this.dateTimePickerStartingDate = new System.Windows.Forms.DateTimePicker();
			this.labelStartingDateTime = new System.Windows.Forms.Label();
			this.checkBoxIsDicotomicSearchActivated = new System.Windows.Forms.CheckBox();
			this.groupBoxUpdateDatabaseOptions = new System.Windows.Forms.GroupBox();
			this.radioButtonDownloadSingleQuote = new System.Windows.Forms.RadioButton();
			this.dateTimePickerSelectedDate = new System.Windows.Forms.DateTimePicker();
			this.radioButtonDownloadOnlyAfterMax = new System.Windows.Forms.RadioButton();
			this.radioButtonOnlyAddMissing = new System.Windows.Forms.RadioButton();
			this.radioButtonOverWriteYes = new System.Windows.Forms.RadioButton();
			this.checkBoxComputeCloseToCloseValues = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.groupBoxUpdateDatabaseOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGrid1
			// 
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Right;
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(278, 0);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.Size = new System.Drawing.Size(579, 472);
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
			this.oleDbDeleteCommand1.CommandText = resources.GetString("oleDbDeleteCommand1.CommandText");
			this.oleDbDeleteCommand1.Parameters.AddRange(new System.Data.OleDb.OleDbParameter[] {
									new System.Data.OleDb.OleDbParameter("Original_quId", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((byte)(10)), ((byte)(0)), "quId", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quDate", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quDate1", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quDate", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quTicker1", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null)});
			// 
			// oleDbInsertCommand1
			// 
			this.oleDbInsertCommand1.CommandText = "INSERT INTO quotes (quClose, quDate, quHigh, quLow, quOpen, quTicker, quVolume) V" +
			"ALUES (?, ?, ?, ?, ?, ?, ?)";
			this.oleDbInsertCommand1.Parameters.AddRange(new System.Data.OleDb.OleDbParameter[] {
									new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "quDate"),
									new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "quTicker"),
									new System.Data.OleDb.OleDbParameter("quVolume", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((byte)(10)), ((byte)(0)), "quVolume", System.Data.DataRowVersion.Current, null)});
			// 
			// oleDbSelectCommand1
			// 
			this.oleDbSelectCommand1.CommandText = "SELECT quClose, quDate, quHigh, quLow, quOpen, quTicker FROM quotes";
			// 
			// oleDbUpdateCommand1
			// 
			this.oleDbUpdateCommand1.CommandText = resources.GetString("oleDbUpdateCommand1.CommandText");
			this.oleDbUpdateCommand1.Parameters.AddRange(new System.Data.OleDb.OleDbParameter[] {
									new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "quDate"),
									new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Current, null),
									new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "quTicker"),
									new System.Data.OleDb.OleDbParameter("Original_quId", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((byte)(10)), ((byte)(0)), "quId", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quClose", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quDate", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quDate1", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quDate", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quHigh", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quLow", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((byte)(7)), ((byte)(0)), "quOpen", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null),
									new System.Data.OleDb.OleDbParameter("Original_quTicker1", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "quTicker", System.Data.DataRowVersion.Original, null)});
			// 
			// oleDbCommand1
			// 
			this.oleDbCommand1.CommandText = "DELETE quotes.* FROM quotes INNER JOIN tickers ON quotes.quTicker = tickers.tiTic" +
			"ker";
			// 
			// buttonDownloadQuotesOfSelectedTickers
			// 
			this.buttonDownloadQuotesOfSelectedTickers.Location = new System.Drawing.Point(85, 419);
			this.buttonDownloadQuotesOfSelectedTickers.Name = "buttonDownloadQuotesOfSelectedTickers";
			this.buttonDownloadQuotesOfSelectedTickers.Size = new System.Drawing.Size(112, 32);
			this.buttonDownloadQuotesOfSelectedTickers.TabIndex = 2;
			this.buttonDownloadQuotesOfSelectedTickers.Text = "Download quotes";
			this.buttonDownloadQuotesOfSelectedTickers.Click += new System.EventHandler(this.buttonDownloadQuotesOfSelectedTickers_Click);
			// 
			// dateTimePickerStartingDate
			// 
			this.dateTimePickerStartingDate.Location = new System.Drawing.Point(8, 36);
			this.dateTimePickerStartingDate.Name = "dateTimePickerStartingDate";
			this.dateTimePickerStartingDate.Size = new System.Drawing.Size(184, 20);
			this.dateTimePickerStartingDate.TabIndex = 6;
			// 
			// labelStartingDateTime
			// 
			this.labelStartingDateTime.Location = new System.Drawing.Point(8, 9);
			this.labelStartingDateTime.Name = "labelStartingDateTime";
			this.labelStartingDateTime.Size = new System.Drawing.Size(80, 23);
			this.labelStartingDateTime.TabIndex = 8;
			this.labelStartingDateTime.Text = "Starting date";
			// 
			// checkBoxIsDicotomicSearchActivated
			// 
			this.checkBoxIsDicotomicSearchActivated.Checked = true;
			this.checkBoxIsDicotomicSearchActivated.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxIsDicotomicSearchActivated.Location = new System.Drawing.Point(8, 306);
			this.checkBoxIsDicotomicSearchActivated.Name = "checkBoxIsDicotomicSearchActivated";
			this.checkBoxIsDicotomicSearchActivated.Size = new System.Drawing.Size(137, 24);
			this.checkBoxIsDicotomicSearchActivated.TabIndex = 13;
			this.checkBoxIsDicotomicSearchActivated.Text = "Use dicotomic search";
			// 
			// groupBoxUpdateDatabaseOptions
			// 
			this.groupBoxUpdateDatabaseOptions.Controls.Add(this.radioButtonDownloadSingleQuote);
			this.groupBoxUpdateDatabaseOptions.Controls.Add(this.dateTimePickerSelectedDate);
			this.groupBoxUpdateDatabaseOptions.Controls.Add(this.radioButtonDownloadOnlyAfterMax);
			this.groupBoxUpdateDatabaseOptions.Controls.Add(this.radioButtonOnlyAddMissing);
			this.groupBoxUpdateDatabaseOptions.Controls.Add(this.radioButtonOverWriteYes);
			this.groupBoxUpdateDatabaseOptions.Location = new System.Drawing.Point(8, 62);
			this.groupBoxUpdateDatabaseOptions.Name = "groupBoxUpdateDatabaseOptions";
			this.groupBoxUpdateDatabaseOptions.Size = new System.Drawing.Size(249, 238);
			this.groupBoxUpdateDatabaseOptions.TabIndex = 14;
			this.groupBoxUpdateDatabaseOptions.TabStop = false;
			this.groupBoxUpdateDatabaseOptions.Text = "Update Database options";
			// 
			// radioButtonDownloadSingleQuote
			// 
			this.radioButtonDownloadSingleQuote.Checked = true;
			this.radioButtonDownloadSingleQuote.Location = new System.Drawing.Point(8, 80);
			this.radioButtonDownloadSingleQuote.Name = "radioButtonDownloadSingleQuote";
			this.radioButtonDownloadSingleQuote.Size = new System.Drawing.Size(192, 24);
			this.radioButtonDownloadSingleQuote.TabIndex = 8;
			this.radioButtonDownloadSingleQuote.TabStop = true;
			this.radioButtonDownloadSingleQuote.Text = "Download single quote";
			this.radioButtonDownloadSingleQuote.CheckedChanged += new System.EventHandler(this.radioButtonDownloadSingleQuote_CheckedChanged);
			// 
			// dateTimePickerSelectedDate
			// 
			this.dateTimePickerSelectedDate.Enabled = false;
			this.dateTimePickerSelectedDate.Location = new System.Drawing.Point(16, 110);
			this.dateTimePickerSelectedDate.Name = "dateTimePickerSelectedDate";
			this.dateTimePickerSelectedDate.Size = new System.Drawing.Size(184, 20);
			this.dateTimePickerSelectedDate.TabIndex = 7;
			// 
			// radioButtonDownloadOnlyAfterMax
			// 
			this.radioButtonDownloadOnlyAfterMax.Checked = true;
			this.radioButtonDownloadOnlyAfterMax.Location = new System.Drawing.Point(8, 19);
			this.radioButtonDownloadOnlyAfterMax.Name = "radioButtonDownloadOnlyAfterMax";
			this.radioButtonDownloadOnlyAfterMax.Size = new System.Drawing.Size(192, 55);
			this.radioButtonDownloadOnlyAfterMax.TabIndex = 3;
			this.radioButtonDownloadOnlyAfterMax.TabStop = true;
			this.radioButtonDownloadOnlyAfterMax.Text = "Download only quotes after last quote (fastest)";
			// 
			// radioButtonOnlyAddMissing
			// 
			this.radioButtonOnlyAddMissing.Location = new System.Drawing.Point(6, 185);
			this.radioButtonOnlyAddMissing.Name = "radioButtonOnlyAddMissing";
			this.radioButtonOnlyAddMissing.Size = new System.Drawing.Size(237, 47);
			this.radioButtonOnlyAddMissing.TabIndex = 1;
			this.radioButtonOnlyAddMissing.Text = "Download all quotes from starting date, adding to database only the missing ones";
			// 
			// radioButtonOverWriteYes
			// 
			this.radioButtonOverWriteYes.Location = new System.Drawing.Point(6, 147);
			this.radioButtonOverWriteYes.Name = "radioButtonOverWriteYes";
			this.radioButtonOverWriteYes.Size = new System.Drawing.Size(237, 41);
			this.radioButtonOverWriteYes.TabIndex = 0;
			this.radioButtonOverWriteYes.Text = "Download all quotes from starting date, deleting all existing ones in database";
			// 
			// checkBoxComputeCloseToCloseValues
			// 
			this.checkBoxComputeCloseToCloseValues.Location = new System.Drawing.Point(8, 349);
			this.checkBoxComputeCloseToCloseValues.Name = "checkBoxComputeCloseToCloseValues";
			this.checkBoxComputeCloseToCloseValues.Size = new System.Drawing.Size(209, 24);
			this.checkBoxComputeCloseToCloseValues.TabIndex = 16;
			this.checkBoxComputeCloseToCloseValues.Text = "Compute close to close ratios (slower)";
			// 
			// checkBoxDownloadOnlyAfterCloseToCloseCheck
			// 
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck.Location = new System.Drawing.Point(8, 389);
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck.Name = "checkBoxDownloadOnlyAfterCloseToCloseCheck";
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck.Size = new System.Drawing.Size(230, 24);
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck.TabIndex = 17;
			this.checkBoxDownloadOnlyAfterCloseToCloseCheck.Text = "Download only after CTC check (slower)";
			this.toolTip1.SetToolTip(this.checkBoxDownloadOnlyAfterCloseToCloseCheck, "If checked, commit to database is performed only for tickers for which new adjust" +
						"ed values respect current close to close ratio  ");
			// 
			// WebDownloader
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(857, 472);
			this.Controls.Add(this.checkBoxDownloadOnlyAfterCloseToCloseCheck);
			this.Controls.Add(this.checkBoxComputeCloseToCloseValues);
			this.Controls.Add(this.groupBoxUpdateDatabaseOptions);
			this.Controls.Add(this.labelStartingDateTime);
			this.Controls.Add(this.dateTimePickerStartingDate);
			this.Controls.Add(this.buttonDownloadQuotesOfSelectedTickers);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.checkBoxIsDicotomicSearchActivated);
			this.Name = "WebDownloader";
			this.Text = "Web downloader";
			this.Load += new System.EventHandler(this.WebDownloader_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.WebDownloaderPaint);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebDownloaderFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.groupBoxUpdateDatabaseOptions.ResumeLayout(false);
			this.ResumeLayout(false);
    }
		private System.Windows.Forms.RadioButton radioButtonOnlyAddMissing;
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
    public DataTable GetTableOfSelectedTickers()
    {
      /*
      DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
      DataTable tableOfSelectedTickers = new DataTable();
      TickerDataTable.AddColumnsOfTickerTable(tableOfSelectedTickers);
      int indexOfRow = 0;
      while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
      {
        if(this.dataGrid1.IsSelected(indexOfRow))
        {
          DataRow dataRow = tableOfSelectedTickers.NewRow(); 
          dataRow["Ticker"] = (string)dataTableOfDataGrid1.Rows[indexOfRow][0];
          tableOfSelectedTickers.Rows.Add(dataRow);
        }
        indexOfRow++;
      }*/
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

    
//    private void downloadQuotes( )
//    {
////  		DataTable dt = this.DsTickerCurrentlyDownloaded.Tables["Tickers"];
//  		this.downloadingInProgress = true;
//  		try{
//  				TickerDownloader qd = new TickerDownloader( this );
//        	qd.DownloadTickers();
//  		}
//    	catch(Exception ex)
//    	{
//    		MessageBox.Show(ex.Message);
//    	}
//    	finally{
//    		this.downloadingInProgress = false;
//    		this.OleDbConnection1.Close();
//    	}
//    }
    
//    private void downloadQuotes_createTickerDataSet( DataSet ds )
//	  {
//		  System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1=new OleDbDataAdapter( "select * from tickers" , this.OleDbConnection1);
//		  oleDbDataAdapter1.Fill(ds);
//	  }	
//    
//    private void downloadQuotesOfAllTickers()
//    {
//      try
//      {
//        DataSet ds=new DataSet();
//        downloadQuotes_createTickerDataSet( ds );
//        downloadQuotes_withTickerDataSet( ds );
//        this.OleDbConnection1.Close();
//      }
//      catch(Exception ex)
//      {
//        MessageBox.Show(ex.ToString());
//      }
//  		
//      finally
//      {
//        
//      }
//
//    }
//    
      	
	  private void openDbAndSetOleDbCommand()
	  {
		  try
		  {
			  if (this.OleDbConnection1.State != ConnectionState.Open)
            this.OleDbConnection1.Open();
			  oleDbCommand1.Connection = this.OleDbConnection1;
			}
		  catch(Exception ex)
		  {
			  MessageBox.Show(ex.ToString());
		  }
  		finally
		  {
			  this.OleDbConnection1.Close();
		  }
 	  }

    private void buttonDownloadQuotesOfSelectedTickers_Click(object sender, System.EventArgs e)
    {
      this.buttonDownloadQuotesOfSelectedTickers.Enabled = false;
      this.openDbAndSetOleDbCommand();
      TickerDownloader tickerDownloader = 
      	new TickerDownloader(this);
      Thread downloadThread = new Thread( tickerDownloader.DownloadTickers );
      this.downloadingInProgress = true;
      downloadThread.Start();
		}

    private void radioButtonDownloadSingleQuote_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this.radioButtonDownloadSingleQuote.Checked)
        this.dateTimePickerSelectedDate.Enabled = true;
      else
        this.dateTimePickerSelectedDate.Enabled = false;
    }
    
    #region Properties
    
    public bool IsComputeCloseToCloseRatioSelected
    {
      get
      {
        return this.checkBoxComputeCloseToCloseValues.Checked;
      }
    }
    
    public bool IsOnlyAfterLastQuoteSelected
    {
      get
      {
        return this.radioButtonDownloadOnlyAfterMax.Checked;
      }
    }
    public bool IsDownloadAllSelected
    {
      get
      {
        return (this.radioButtonOnlyAddMissing.Checked ||
                this.radioButtonOverWriteYes.Checked);
      }
    }
    public bool IsOverWriteYesSelected
    {
      get
      {
        return this.radioButtonOverWriteYes.Checked;
      }
    }
    public bool IsOnlyAddMissingSelected
    {
      get
      {
        return this.radioButtonOnlyAddMissing.Checked;
      }
    }
    public bool IsCheckCloseToCloseSelected
    {
      get
      {
        return this.checkBoxDownloadOnlyAfterCloseToCloseCheck.Checked;
      }
    }
    public bool IsSingleQuoteSelected
    {
      get
      {
        return this.radioButtonDownloadSingleQuote.Checked;
      }
    }
    public DateTime SelectedDateForSingleQuote
    {
      get
      {
        return this.dateTimePickerSelectedDate.Value;
      }
    }
    public DateTime StartingDate
    {
      get
      {
        return this.dateTimePickerStartingDate.Value;
      }
    }
   
    public Hashtable DownloadingTickers
    {
      get
      {
        return this.downloadingTickers;
      }
    }
    public int IndexOfCurrentUpdatingTicker
    {
      get
      {
        return this.indexOfCurrentUpdatingTicker;
      }
      set
      {
      	this.indexOfCurrentUpdatingTicker = value;
      }
    }
		public string LastQuoteInDBForCurrentUpdatingTicker
    {
      get
      {
        return this.lastQuoteInDB;
      }
      set
      {
      	this.lastQuoteInDB = value;
      }
    }
    
    public string CurrentStateForCurrentUpdatingTicker
    {
      get
      {
        return this.currentState;
      }
      set
      {
      	this.currentState = value;
      }
    }
    public string DatabaseUpdatedInfoForCurrentUpdatingTicker
    {
      get
      {
        return this.databaseUpdated;
      }
      set
      {
      	this.databaseUpdated = value;
      }
    }
    public string AdjustedCloseInfoForCurrentUpdatingTicker
    {
      get
      {
        return this.adjustedClose;
      }
      set
      {
      	this.adjustedClose = value;
      }
    }
    public string AdjCloseToCloseRatioInfoForCurrentUpdatingTicker
    {
      get
      {
        return this.adjCloseToCloseRatio;
      }
      set
      {
      	this.adjCloseToCloseRatio = value;
      }
    }
    
    public bool DownloadingInProgress
    {
      get
      {
        return this.downloadingInProgress;
      }
      set
      {
      	this.downloadingInProgress = value;
      }
    }
    
		#endregion
		
		private void webDownloaderLoad_createDataSourceForDataGrid( )
    {
    	if (!this.DsTickerCurrentlyDownloaded.Tables.Contains( "Tickers" ))
      {
        //structure for table
      	this.DsTickerCurrentlyDownloaded.Tables.Add( "Tickers" );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( 
          new DataColumn( this.tableOfSelectedTickers.Columns[0].ColumnName , this.tableOfSelectedTickers.Columns[0].DataType ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "currentState" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "lastQuoteInDB" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "databaseUpdated" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "adjustedClose" , System.Type.GetType( "System.String" ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "adjCloseToCloseRatio" , System.Type.GetType( "System.String" ) );
        //populate table
        for(int i = 0; i < this.tableOfSelectedTickers.Rows.Count; i++)
        {
        	DataRow newRow = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
        	newRow[ 0 ] = this.tableOfSelectedTickers.Rows[ i ][ 0 ];
        	newRow[ "lastQuoteInDB" ] = "...";
		      newRow[ "currentState" ] = "...";
		      newRow[ "databaseUpdated" ] = "...";
		      newRow[ "adjustedClose"] = "...";
		      newRow[ "adjCloseToCloseRatio"] = "...";
		      this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
        }
        this.dataGrid1.DataSource = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ];
      }
    }
	
		private void webDownloaderLoad_setStyleForDataGrid()
		{
			DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
			dataGrid1TableStyle.MappingName = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].TableName;
			dataGrid1TableStyle.ColumnHeadersVisible = true;
			dataGrid1TableStyle.ReadOnly = true;
			dataGrid1TableStyle.SelectionBackColor = Color.DimGray ;
						
      DataGridTextBoxColumn columnStyle_Tickers = new DataGridTextBoxColumn();
			columnStyle_Tickers.MappingName = "tiTicker";
			columnStyle_Tickers.HeaderText = "Tickers";
			columnStyle_Tickers.TextBox.Enabled = false;
			columnStyle_Tickers.NullText = "";
			columnStyle_Tickers.Width = 58;
			
			DataGridTextBoxColumn columnStyle_lastQuoteInDB = new DataGridTextBoxColumn();
			columnStyle_lastQuoteInDB.MappingName = "lastQuoteInDB";
			columnStyle_lastQuoteInDB.HeaderText = "Last Quote In DB";
			columnStyle_lastQuoteInDB.TextBox.Enabled = false;
			columnStyle_lastQuoteInDB.NullText = "";
			columnStyle_lastQuoteInDB.Width = 95;
			
			DataGridTextBoxColumn columnStyle_currentState = new DataGridTextBoxColumn();
			columnStyle_currentState.MappingName = "currentState";
			columnStyle_currentState.HeaderText = "Current State";
			columnStyle_currentState.TextBox.Enabled = false;
			columnStyle_currentState.NullText = "";
			columnStyle_currentState.Width = 95;
      
			DataGridTextBoxColumn columnStyle_databaseUpdated = new DataGridTextBoxColumn();
      columnStyle_databaseUpdated.MappingName = "databaseUpdated";
      columnStyle_databaseUpdated.HeaderText = "DB Updated";
      columnStyle_databaseUpdated.TextBox.Enabled = false;
      columnStyle_databaseUpdated.NullText = "";
      columnStyle_databaseUpdated.Width = 80;

      DataGridTextBoxColumn columnStyle_adjustedClose = new DataGridTextBoxColumn();
      columnStyle_adjustedClose.MappingName = "adjustedClose";
      columnStyle_adjustedClose.HeaderText = "Adj Close";
      columnStyle_adjustedClose.TextBox.Enabled = false;
      columnStyle_adjustedClose.NullText = "";
      columnStyle_adjustedClose.Width = 70;
      
      DataGridTextBoxColumn columnStyle_adjCloseToCloseRatio = new DataGridTextBoxColumn();
      columnStyle_adjCloseToCloseRatio.MappingName = "adjCloseToCloseRatio";
      columnStyle_adjCloseToCloseRatio.HeaderText = "Adj Close To Close Ratio";
      columnStyle_adjCloseToCloseRatio.TextBox.Enabled = false;
      columnStyle_adjCloseToCloseRatio.NullText = "";
      columnStyle_adjCloseToCloseRatio.Width = 110;
      
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_Tickers);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_lastQuoteInDB);
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_currentState);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_databaseUpdated);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_adjustedClose);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_adjCloseToCloseRatio);
			
      this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
		}
	  
		private void WebDownloader_Load(object sender, System.EventArgs e)
    {
			this.webDownloaderLoad_createDataSourceForDataGrid();
			this.webDownloaderLoad_setStyleForDataGrid();
    }
    
    private void webDownloaderPaint_refreshTableForGrid()
		{
			lock( this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
				DataRow row = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows[ this.IndexOfCurrentUpdatingTicker ];
				row["lastQuoteInDB"] = this.LastQuoteInDBForCurrentUpdatingTicker;
				row["currentState"] = this.CurrentStateForCurrentUpdatingTicker;
				row["adjustedClose"] = this.AdjustedCloseInfoForCurrentUpdatingTicker;
				row["adjCloseToCloseRatio"] = this.AdjCloseToCloseRatioInfoForCurrentUpdatingTicker;
				row["databaseUpdated"] = this.DatabaseUpdatedInfoForCurrentUpdatingTicker;	
      }
		}
    
    void WebDownloaderPaint(object sender, PaintEventArgs e)
		{
    	if( this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] != null)
    		this.webDownloaderPaint_refreshTableForGrid();
    }
		
		void WebDownloaderFormClosing(object sender, FormClosingEventArgs e)
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
	}
}
