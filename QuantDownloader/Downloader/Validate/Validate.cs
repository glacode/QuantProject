using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for Validate.
	/// </summary>
	public class Validate : System.Windows.Forms.Form
	{
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label labelTickerIsLike;
    private System.Windows.Forms.Button buttonGo;
    private System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1;
    private QuantProject.Applications.Downloader.ValidateDataset validateDataset1;
    private System.Data.OleDb.OleDbConnection oleDbConnection2;
    private System.Windows.Forms.DataGrid dataGrid1;
    private System.Data.OleDb.OleDbCommand oleDbSelectCommand1;
    private System.Data.OleDb.OleDbCommand oleDbInsertCommand1;
    private System.Data.OleDb.OleDbCommand oleDbUpdateCommand1;
    private System.Data.OleDb.OleDbCommand oleDbDeleteCommand1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Validate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.labelTickerIsLike = new System.Windows.Forms.Label();
      this.buttonGo = new System.Windows.Forms.Button();
      this.oleDbDataAdapter1 = new System.Data.OleDb.OleDbDataAdapter();
      this.oleDbDeleteCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbConnection2 = AdoNetTools.OleDbConnection;
      this.oleDbInsertCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbSelectCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbUpdateCommand1 = new System.Data.OleDb.OleDbCommand();
      this.validateDataset1 = new QuantProject.Applications.Downloader.ValidateDataset();
      this.dataGrid1 = new System.Windows.Forms.DataGrid();
      ((System.ComponentModel.ISupportInitialize)(this.validateDataset1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.SuspendLayout();
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(120, 16);
      this.textBox1.Name = "textBox1";
      this.textBox1.TabIndex = 0;
      this.textBox1.Text = "MS?T";
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
      this.buttonGo.Location = new System.Drawing.Point(368, 16);
      this.buttonGo.Name = "buttonGo";
      this.buttonGo.TabIndex = 2;
      this.buttonGo.Text = "Go";
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
      this.oleDbDeleteCommand1.Connection = this.oleDbConnection2;
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
      // oleDbInsertCommand1
      // 
      this.oleDbInsertCommand1.CommandText = "INSERT INTO quotes(quTicker, quDate, quOpen, quHigh, quLow, quClose, quVolume, qu" +
        "AdjustedOpen, quAdjustedHigh, quAdjustedLow, quAdjustedClose, quAdjustedVolume) " +
        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
      this.oleDbInsertCommand1.Connection = this.oleDbConnection2;
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
      this.oleDbSelectCommand1.Connection = this.oleDbConnection2;
      // 
      // oleDbUpdateCommand1
      // 
      this.oleDbUpdateCommand1.CommandText = @"UPDATE quotes SET quTicker = ?, quDate = ?, quOpen = ?, quHigh = ?, quLow = ?, quClose = ?, quVolume = ?, quAdjustedOpen = ?, quAdjustedHigh = ?, quAdjustedLow = ?, quAdjustedClose = ?, quAdjustedVolume = ? WHERE (quDate = ?) AND (quTicker = ?) AND (quAdjustedClose = ? OR ? IS NULL AND quAdjustedClose IS NULL) AND (quAdjustedHigh = ? OR ? IS NULL AND quAdjustedHigh IS NULL) AND (quAdjustedLow = ? OR ? IS NULL AND quAdjustedLow IS NULL) AND (quAdjustedOpen = ? OR ? IS NULL AND quAdjustedOpen IS NULL) AND (quAdjustedVolume = ? OR ? IS NULL AND quAdjustedVolume IS NULL) AND (quClose = ? OR ? IS NULL AND quClose IS NULL) AND (quHigh = ? OR ? IS NULL AND quHigh IS NULL) AND (quLow = ? OR ? IS NULL AND quLow IS NULL) AND (quOpen = ? OR ? IS NULL AND quOpen IS NULL)";
      this.oleDbUpdateCommand1.Connection = this.oleDbConnection2;
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
      // validateDataset1
      // 
      this.validateDataset1.DataSetName = "ValidateDataset";
      this.validateDataset1.Locale = new System.Globalization.CultureInfo("en-US");
      this.validateDataset1.Namespace = "http://www.tempuri.org/ValidateDataset.xsd";
      // 
      // dataGrid1
      // 
      this.dataGrid1.DataMember = "";
      this.dataGrid1.DataSource = this.validateDataset1.quotes;
      this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGrid1.Location = new System.Drawing.Point(16, 56);
      this.dataGrid1.Name = "dataGrid1";
      this.dataGrid1.PreferredColumnWidth = 50;
      this.dataGrid1.Size = new System.Drawing.Size(696, 224);
      this.dataGrid1.TabIndex = 3;
      // 
      // Validate
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(720, 341);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.dataGrid1,
                                                                  this.buttonGo,
                                                                  this.labelTickerIsLike,
                                                                  this.textBox1});
      this.Name = "Validate";
      this.Text = "Validate";
      ((System.ComponentModel.ISupportInitialize)(this.validateDataset1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

	}
}
