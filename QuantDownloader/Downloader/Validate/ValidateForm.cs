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
		private System.Windows.Forms.Button buttonGoValidateCurrentSelection;
    private ValidateDataGrid validateDataGrid = new ValidateDataGrid();
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

      this.validateDataGrid.Location = new System.Drawing.Point(16, 56);
      this.validateDataGrid.Width = this.Width - 32;
      this.validateDataGrid.Height = this.Height - 130;
      this.Controls.Add( this.validateDataGrid );
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
      this.buttonCommitAndRefresh = new System.Windows.Forms.Button();
      this.textBoxSuspiciousRatio = new System.Windows.Forms.TextBox();
      this.labelSuspiciousRatio = new System.Windows.Forms.Label();
      this.buttonGoValidateCurrentSelection = new System.Windows.Forms.Button();
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
      // buttonCommitAndRefresh
      // 
      this.buttonCommitAndRefresh.Location = new System.Drawing.Point(272, 312);
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
                                                                  this.buttonGo,
                                                                  this.labelTickerIsLike,
                                                                  this.textBoxTickerIsLike});
      this.Name = "ValidateForm";
      this.Text = "Validate";
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
