using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for Validate.
	/// </summary>
	public class ValidateForm : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Label labelTickerIsLike;
    private System.Windows.Forms.Button buttonGo;
    private System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1;
    private ValidateDataTable validateDataTable;
	private DataTable tableOfTickersToBeValidated;
    private System.Data.OleDb.OleDbCommand oleDbSelectCommand1;
    private System.Data.OleDb.OleDbCommand oleDbInsertCommand1;
    private System.Data.OleDb.OleDbCommand oleDbUpdateCommand1;
    private System.Data.OleDb.OleDbCommand oleDbDeleteCommand1;
    private System.Windows.Forms.TextBox textBoxTickerIsLike;
    private System.Windows.Forms.Button buttonCommitAndRefresh;
    private System.Windows.Forms.TextBox textBoxSuspiciousRatio;
    private System.Windows.Forms.Label labelSuspiciousRatio;
    private QuantProject.Applications.Downloader.Validate.ValidateDataGrid validateDataGrid;
		private System.Windows.Forms.Button buttonGoValidateCurrentSelection;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn4;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn5;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn6;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn7;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn8;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn9;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn10;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn11;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn12;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn13;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn14;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn15;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn16;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn17;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn18;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ValidateForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.buttonGoValidateCurrentSelection.Visible = false;
			this.tableOfTickersToBeValidated = null;

		}
		
		public ValidateForm(DataTable dataTable)
		{
			InitializeComponent();
			this.labelTickerIsLike.Visible = false;
			this.textBoxTickerIsLike.Visible = false;
			this.buttonGo.Visible = false;
			this.Text = "Validate current selection";
			//the data to be validated comes from the data table
			// these two members are not used if the object is created
			// with the data table
			this.tableOfTickersToBeValidated = dataTable;
		}
		/// <summary>
		/// Clean up any resources being used.
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxTickerIsLike = new System.Windows.Forms.TextBox();
			this.labelTickerIsLike = new System.Windows.Forms.Label();
			this.buttonGo = new System.Windows.Forms.Button();
			this.oleDbDataAdapter1 = new System.Data.OleDb.OleDbDataAdapter();
			this.oleDbDeleteCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbInsertCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbSelectCommand1 = new System.Data.OleDb.OleDbCommand();
			this.oleDbUpdateCommand1 = new System.Data.OleDb.OleDbCommand();
			this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTableStyle2 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTextBoxColumn10 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn11 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn12 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn13 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn14 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn15 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn16 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn17 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn18 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.validateDataGrid = new QuantProject.Applications.Downloader.Validate.ValidateDataGrid();
			this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn5 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn6 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn7 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn8 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn9 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.buttonCommitAndRefresh = new System.Windows.Forms.Button();
			this.textBoxSuspiciousRatio = new System.Windows.Forms.TextBox();
			this.labelSuspiciousRatio = new System.Windows.Forms.Label();
			this.buttonGoValidateCurrentSelection = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.validateDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxTickerIsLike
			// 
			this.textBoxTickerIsLike.Location = new System.Drawing.Point(120, 16);
			this.textBoxTickerIsLike.Name = "textBoxTickerIsLike";
			this.textBoxTickerIsLike.TabIndex = 0;
			this.textBoxTickerIsLike.Text = "CCE";
			// 
			// labelTickerIsLike
			// 
			this.labelTickerIsLike.Location = new System.Drawing.Point(24, 16);
			this.labelTickerIsLike.Name = "labelTickerIsLike";
			this.labelTickerIsLike.Size = new System.Drawing.Size(80, 16);
			this.labelTickerIsLike.TabIndex = 1;
			this.labelTickerIsLike.Text = "Ticker is Like:";
			this.labelTickerIsLike.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonGo
			// 
			this.buttonGo.Location = new System.Drawing.Point(464, 16);
			this.buttonGo.Name = "buttonGo";
			this.buttonGo.TabIndex = 2;
			this.buttonGo.Text = "Go";
			this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
			// 
			// oleDbDataAdapter1
			// 
			this.oleDbDataAdapter1.DeleteCommand = this.oleDbDeleteCommand1;
			this.oleDbDataAdapter1.InsertCommand = this.oleDbInsertCommand1;
			this.oleDbDataAdapter1.SelectCommand = this.oleDbSelectCommand1;
			this.oleDbDataAdapter1.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
																										new System.Data.Common.DataTableMapping("Table", "quotes", new System.Data.Common.DataColumnMapping[] {
																																																				  new System.Data.Common.DataColumnMapping("Ticker", "Ticker"),
																																																				  new System.Data.Common.DataColumnMapping("TheDate", "TheDate"),
																																																				  new System.Data.Common.DataColumnMapping("TheOpen", "TheOpen"),
																																																				  new System.Data.Common.DataColumnMapping("High", "High"),
																																																				  new System.Data.Common.DataColumnMapping("Low", "Low"),
																																																				  new System.Data.Common.DataColumnMapping("TheClose", "TheClose"),
																																																				  new System.Data.Common.DataColumnMapping("Volume", "Volume"),
																																																				  new System.Data.Common.DataColumnMapping("AdjOpen", "AdjOpen"),
																																																				  new System.Data.Common.DataColumnMapping("AdjHigh", "AdjHigh"),
																																																				  new System.Data.Common.DataColumnMapping("AdjLow", "AdjLow"),
																																																				  new System.Data.Common.DataColumnMapping("AdjClose", "AdjClose"),
																																																				  new System.Data.Common.DataColumnMapping("AdjVolume", "AdjVolume"),
																																																				  new System.Data.Common.DataColumnMapping("ErrorDescription", "ErrorDescription")})});
			this.oleDbDataAdapter1.UpdateCommand = this.oleDbUpdateCommand1;
			// 
			// oleDbDeleteCommand1
			// 
			this.oleDbDeleteCommand1.CommandText = @"DELETE FROM quotes WHERE (quDate = ?) AND (quTicker = ?) AND (quAdjustedClose = ? OR ? IS NULL AND quAdjustedClose IS NULL) AND (quAdjustedHigh = ? OR ? IS NULL AND quAdjustedHigh IS NULL) AND (quAdjustedLow = ? OR ? IS NULL AND quAdjustedLow IS NULL) AND (quAdjustedOpen = ? OR ? IS NULL AND quAdjustedOpen IS NULL) AND (quAdjustedVolume = ? OR ? IS NULL AND quAdjustedVolume IS NULL) AND (quClose = ? OR ? IS NULL AND quClose IS NULL) AND (quHigh = ? OR ? IS NULL AND quHigh IS NULL) AND (quLow = ? OR ? IS NULL AND quLow IS NULL) AND (quOpen = ? OR ? IS NULL AND quOpen IS NULL)";
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "TheDate", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "Ticker", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedVolume", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedVolume1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Original, null));
			// 
			// oleDbInsertCommand1
			// 
			this.oleDbInsertCommand1.CommandText = "INSERT INTO quotes(quTicker, quDate, quOpen, quHigh, quLow, quClose, quVolume, qu" +
				"AdjustedOpen, quAdjustedHigh, quAdjustedLow, quAdjustedClose, quAdjustedVolume) " +
				"VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "Ticker"));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "TheDate"));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quVolume", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "Volume", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Current, null));
			this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedVolume", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Current, null));
			// 
			// oleDbSelectCommand1
			// 
			this.oleDbSelectCommand1.CommandText = @"SELECT quTicker AS Ticker, quDate AS TheDate, quOpen AS TheOpen, quHigh AS High, quLow AS Low, quClose AS TheClose, quVolume AS Volume, quAdjustedOpen AS AdjOpen, quAdjustedHigh AS AdjHigh, quAdjustedLow AS AdjLow, quAdjustedClose AS AdjClose, quAdjustedVolume AS AdjVolume, '' AS ErrorDescription FROM quotes ORDER BY quTicker, quDate";
			// 
			// oleDbUpdateCommand1
			// 
			this.oleDbUpdateCommand1.CommandText = @"UPDATE quotes SET quTicker = ?, quDate = ?, quOpen = ?, quHigh = ?, quLow = ?, quClose = ?, quVolume = ?, quAdjustedOpen = ?, quAdjustedHigh = ?, quAdjustedLow = ?, quAdjustedClose = ?, quAdjustedVolume = ? WHERE (quDate = ?) AND (quTicker = ?) AND (quAdjustedClose = ? OR ? IS NULL AND quAdjustedClose IS NULL) AND (quAdjustedHigh = ? OR ? IS NULL AND quAdjustedHigh IS NULL) AND (quAdjustedLow = ? OR ? IS NULL AND quAdjustedLow IS NULL) AND (quAdjustedOpen = ? OR ? IS NULL AND quAdjustedOpen IS NULL) AND (quAdjustedVolume = ? OR ? IS NULL AND quAdjustedVolume IS NULL) AND (quClose = ? OR ? IS NULL AND quClose IS NULL) AND (quHigh = ? OR ? IS NULL AND quHigh IS NULL) AND (quLow = ? OR ? IS NULL AND quLow IS NULL) AND (quOpen = ? OR ? IS NULL AND quOpen IS NULL)";
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "Ticker"));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quDate", System.Data.OleDb.OleDbType.DBDate, 0, "TheDate"));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quVolume", System.Data.OleDb.OleDbType.Integer, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(10)), ((System.Byte)(0)), "Volume", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("quAdjustedVolume", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Current, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "TheDate", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "Ticker", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjClose", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjHigh", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjLow", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedVolume", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quAdjustedVolume1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "AdjVolume", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quClose1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheClose", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quHigh1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "High", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quLow1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "Low", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Original, null));
			this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_quOpen1", System.Data.OleDb.OleDbType.Single, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(7)), ((System.Byte)(0)), "TheOpen", System.Data.DataRowVersion.Original, null));
			// 
			// dataGridTableStyle1
			// 
			this.dataGridTableStyle1.DataGrid = this.validateDataGrid;
			this.dataGridTableStyle1.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																												  this.dataGridTextBoxColumn1,
																												  this.dataGridTextBoxColumn2,
																												  this.dataGridTextBoxColumn3,
																												  this.dataGridTextBoxColumn4,
																												  this.dataGridTextBoxColumn5,
																												  this.dataGridTextBoxColumn6,
																												  this.dataGridTextBoxColumn7,
																												  this.dataGridTextBoxColumn8,
																												  this.dataGridTextBoxColumn9});
			this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle1.MappingName = "";
			// 
			// dataGridTableStyle2
			// 
			this.dataGridTableStyle2.DataGrid = this.validateDataGrid;
			this.dataGridTableStyle2.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																												  this.dataGridTextBoxColumn10,
																												  this.dataGridTextBoxColumn11,
																												  this.dataGridTextBoxColumn12,
																												  this.dataGridTextBoxColumn13,
																												  this.dataGridTextBoxColumn14,
																												  this.dataGridTextBoxColumn15,
																												  this.dataGridTextBoxColumn16,
																												  this.dataGridTextBoxColumn17,
																												  this.dataGridTextBoxColumn18});
			this.dataGridTableStyle2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle2.MappingName = "quotes";
			// 
			// dataGridTextBoxColumn10
			// 
			this.dataGridTextBoxColumn10.Format = "";
			this.dataGridTextBoxColumn10.FormatInfo = null;
			this.dataGridTextBoxColumn10.HeaderText = "Ticker";
			this.dataGridTextBoxColumn10.MappingName = "quTicker";
			this.dataGridTextBoxColumn10.Width = 39;
			// 
			// dataGridTextBoxColumn11
			// 
			this.dataGridTextBoxColumn11.Format = "";
			this.dataGridTextBoxColumn11.FormatInfo = null;
			this.dataGridTextBoxColumn11.HeaderText = "Date";
			this.dataGridTextBoxColumn11.MappingName = "quDate";
			this.dataGridTextBoxColumn11.Width = 32;
			// 
			// dataGridTextBoxColumn12
			// 
			this.dataGridTextBoxColumn12.Format = "";
			this.dataGridTextBoxColumn12.FormatInfo = null;
			this.dataGridTextBoxColumn12.HeaderText = "Open";
			this.dataGridTextBoxColumn12.MappingName = "quOpen";
			this.dataGridTextBoxColumn12.Width = 36;
			// 
			// dataGridTextBoxColumn13
			// 
			this.dataGridTextBoxColumn13.Format = "";
			this.dataGridTextBoxColumn13.FormatInfo = null;
			this.dataGridTextBoxColumn13.HeaderText = "High";
			this.dataGridTextBoxColumn13.MappingName = "quHigh";
			this.dataGridTextBoxColumn13.Width = 32;
			// 
			// dataGridTextBoxColumn14
			// 
			this.dataGridTextBoxColumn14.Format = "";
			this.dataGridTextBoxColumn14.FormatInfo = null;
			this.dataGridTextBoxColumn14.HeaderText = "Low";
			this.dataGridTextBoxColumn14.MappingName = "quLow";
			this.dataGridTextBoxColumn14.Width = 29;
			// 
			// dataGridTextBoxColumn15
			// 
			this.dataGridTextBoxColumn15.Format = "";
			this.dataGridTextBoxColumn15.FormatInfo = null;
			this.dataGridTextBoxColumn15.HeaderText = "Close";
			this.dataGridTextBoxColumn15.MappingName = "quClose";
			this.dataGridTextBoxColumn15.Width = 37;
			// 
			// dataGridTextBoxColumn16
			// 
			this.dataGridTextBoxColumn16.Format = "";
			this.dataGridTextBoxColumn16.FormatInfo = null;
			this.dataGridTextBoxColumn16.HeaderText = "Adj. Close";
			this.dataGridTextBoxColumn16.MappingName = "quAdjustedClose";
			this.dataGridTextBoxColumn16.Width = 60;
			// 
			// dataGridTextBoxColumn17
			// 
			this.dataGridTextBoxColumn17.Format = "";
			this.dataGridTextBoxColumn17.FormatInfo = null;
			this.dataGridTextBoxColumn17.HeaderText = "Warning";
			this.dataGridTextBoxColumn17.MappingName = "ValidationWarning";
			this.dataGridTextBoxColumn17.Width = 50;
			// 
			// dataGridTextBoxColumn18
			// 
			this.dataGridTextBoxColumn18.Format = "";
			this.dataGridTextBoxColumn18.FormatInfo = null;
			this.dataGridTextBoxColumn18.HeaderText = "Ew";
			this.dataGridTextBoxColumn18.MappingName = "Yuppy";
			this.dataGridTextBoxColumn18.Width = 24;
			// 
			// validateDataGrid
			// 
			this.validateDataGrid.DataMember = "";
			this.validateDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.validateDataGrid.Location = new System.Drawing.Point(16, 56);
			this.validateDataGrid.Name = "validateDataGrid";
			this.validateDataGrid.PreferredColumnWidth = 50;
			this.validateDataGrid.Size = new System.Drawing.Size(696, 224);
			this.validateDataGrid.TabIndex = 3;
			this.validateDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																										 this.dataGridTableStyle2,
																										 this.dataGridTableStyle1});
			// 
			// dataGridTextBoxColumn1
			// 
			this.dataGridTextBoxColumn1.Format = "";
			this.dataGridTextBoxColumn1.FormatInfo = null;
			this.dataGridTextBoxColumn1.HeaderText = "Ticker";
			this.dataGridTextBoxColumn1.MappingName = "quTicker";
			this.dataGridTextBoxColumn1.Width = 39;
			// 
			// dataGridTextBoxColumn2
			// 
			this.dataGridTextBoxColumn2.Format = "";
			this.dataGridTextBoxColumn2.FormatInfo = null;
			this.dataGridTextBoxColumn2.HeaderText = "Date";
			this.dataGridTextBoxColumn2.MappingName = "quDate";
			this.dataGridTextBoxColumn2.Width = 32;
			// 
			// dataGridTextBoxColumn3
			// 
			this.dataGridTextBoxColumn3.Format = "";
			this.dataGridTextBoxColumn3.FormatInfo = null;
			this.dataGridTextBoxColumn3.HeaderText = "Open";
			this.dataGridTextBoxColumn3.MappingName = "quOpen";
			this.dataGridTextBoxColumn3.Width = 36;
			// 
			// dataGridTextBoxColumn4
			// 
			this.dataGridTextBoxColumn4.Format = "";
			this.dataGridTextBoxColumn4.FormatInfo = null;
			this.dataGridTextBoxColumn4.HeaderText = "High";
			this.dataGridTextBoxColumn4.MappingName = "quHigh";
			this.dataGridTextBoxColumn4.Width = 32;
			// 
			// dataGridTextBoxColumn5
			// 
			this.dataGridTextBoxColumn5.Format = "";
			this.dataGridTextBoxColumn5.FormatInfo = null;
			this.dataGridTextBoxColumn5.HeaderText = "Low";
			this.dataGridTextBoxColumn5.MappingName = "quLow";
			this.dataGridTextBoxColumn5.Width = 29;
			// 
			// dataGridTextBoxColumn6
			// 
			this.dataGridTextBoxColumn6.Format = "";
			this.dataGridTextBoxColumn6.FormatInfo = null;
			this.dataGridTextBoxColumn6.HeaderText = "Close";
			this.dataGridTextBoxColumn6.MappingName = "quClose";
			this.dataGridTextBoxColumn6.Width = 37;
			// 
			// dataGridTextBoxColumn7
			// 
			this.dataGridTextBoxColumn7.Format = "";
			this.dataGridTextBoxColumn7.FormatInfo = null;
			this.dataGridTextBoxColumn7.HeaderText = "Adj. Close";
			this.dataGridTextBoxColumn7.MappingName = "quAdjustedClose";
			this.dataGridTextBoxColumn7.Width = 60;
			// 
			// dataGridTextBoxColumn8
			// 
			this.dataGridTextBoxColumn8.Format = "";
			this.dataGridTextBoxColumn8.FormatInfo = null;
			this.dataGridTextBoxColumn8.HeaderText = "Warning";
			this.dataGridTextBoxColumn8.MappingName = "ValidationWarning";
			this.dataGridTextBoxColumn8.Width = 50;
			// 
			// dataGridTextBoxColumn9
			// 
			this.dataGridTextBoxColumn9.Format = "";
			this.dataGridTextBoxColumn9.FormatInfo = null;
			this.dataGridTextBoxColumn9.HeaderText = "Ew";
			this.dataGridTextBoxColumn9.MappingName = "Yuppy";
			this.dataGridTextBoxColumn9.Width = 24;
			// 
			// buttonCommitAndRefresh
			// 
			this.buttonCommitAndRefresh.Location = new System.Drawing.Point(272, 296);
			this.buttonCommitAndRefresh.Name = "buttonCommitAndRefresh";
			this.buttonCommitAndRefresh.Size = new System.Drawing.Size(152, 23);
			this.buttonCommitAndRefresh.TabIndex = 4;
			this.buttonCommitAndRefresh.Text = "Commit && Refresh";
			this.buttonCommitAndRefresh.Click += new System.EventHandler(this.buttonCommitAndRefresh_Click);
			// 
			// textBoxSuspiciousRatio
			// 
			this.textBoxSuspiciousRatio.Location = new System.Drawing.Point(344, 16);
			this.textBoxSuspiciousRatio.Name = "textBoxSuspiciousRatio";
			this.textBoxSuspiciousRatio.TabIndex = 5;
			this.textBoxSuspiciousRatio.Text = "5";
			// 
			// labelSuspiciousRatio
			// 
			this.labelSuspiciousRatio.Location = new System.Drawing.Point(224, 16);
			this.labelSuspiciousRatio.Name = "labelSuspiciousRatio";
			this.labelSuspiciousRatio.Size = new System.Drawing.Size(120, 16);
			this.labelSuspiciousRatio.TabIndex = 6;
			this.labelSuspiciousRatio.Text = "Suspicious ratio:";
			this.labelSuspiciousRatio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonGoValidateCurrentSelection
			// 
			this.buttonGoValidateCurrentSelection.Location = new System.Drawing.Point(552, 16);
			this.buttonGoValidateCurrentSelection.Name = "buttonGoValidateCurrentSelection";
			this.buttonGoValidateCurrentSelection.TabIndex = 7;
			this.buttonGoValidateCurrentSelection.Text = "Go";
			this.buttonGoValidateCurrentSelection.Click += new System.EventHandler(this.buttonGoValidateCurrentSelection_Click);
			// 
			// ValidateForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(720, 341);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonGoValidateCurrentSelection,
																		  this.labelSuspiciousRatio,
																		  this.textBoxSuspiciousRatio,
																		  this.buttonCommitAndRefresh,
																		  this.validateDataGrid,
																		  this.buttonGo,
																		  this.labelTickerIsLike,
																		  this.textBoxTickerIsLike});
			this.Name = "ValidateForm";
			this.Text = "Validate";
			((System.ComponentModel.ISupportInitialize)(this.validateDataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

    #region "buttonGo_Click"
//      private void buttonGo_Click_setTableStyle()
//    {
//      DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();
//      dataGridTableStyle.ColumnHeadersVisible = true;
//      dataGridTableStyle.MappingName = "quotes";
//      DataGridTextBoxColumn dataGridColumnStyle = new DataGridTextBoxColumn();
//      dataGridColumnStyle.MappingName = "quTicker";
//      dataGridColumnStyle.HeaderText = "Ticker";
//      dataGridTableStyle.GridColumnStyles.Add( dataGridColumnStyle );
//      this.dataGrid1.TableStyles.Add( dataGridTableStyle );
//    }
    private void buttonGo_Click(object sender, System.EventArgs e)
    {
      this.validateDataTable = this.validateDataGrid.Validate( this.textBoxTickerIsLike.Text ,
        this.textBoxSuspiciousRatio.Text );
//      this.validateDataTable = new ValidateDataTable();
//      this.dataGrid1.DataSource = validateDataTable;
//      buttonGo_Click_setTableStyle();
//      validateDataTable.AddRows( textBoxTickerIsLike.Text , Convert.ToDouble( this.textBoxSuspiciousRatio.Text ) );
    }
	private void buttonGoValidateCurrentSelection_Click(object sender, System.EventArgs e)
	{
		try
		{
		Cursor.Current = Cursors.WaitCursor;
		this.validateDataTable = this.validateDataGrid.Validate(this.tableOfTickersToBeValidated,
															this.textBoxSuspiciousRatio.Text);
		}
		finally
		{
			Cursor.Current = Cursors.Default;
		}
		}

    #endregion


    private void buttonCommitAndRefresh_Click(object sender, System.EventArgs e)
    {
      validateDataTable.Update();
      buttonGo_Click( sender , e );
    }



	}
}
