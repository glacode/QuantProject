/*
QuantProject - Quantitative Finance Library

TestDownloadedData.cs
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Principale
{
	/// <summary>
	/// Summary description for Test.
	/// </summary>
	public class TestDownloadedData : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Button button1;
    private System.Data.DataSet dsTestErrors;
    private System.Windows.Forms.DataGrid dgTestErrors;
    private System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1;
    private System.Data.OleDb.OleDbCommand oleDbSelectCommand1;
    private System.Data.OleDb.OleDbCommand oleDbInsertCommand1;
    private System.Data.OleDb.OleDbCommand oleDbUpdateCommand1;
    private System.Data.OleDb.OleDbCommand oleDbDeleteCommand1;
    private System.Data.OleDb.OleDbConnection oleDbConnection1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestDownloadedData()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
      this.dgTestErrors = new System.Windows.Forms.DataGrid();
      this.dsTestErrors = new System.Data.DataSet();
      this.button1 = new System.Windows.Forms.Button();
      this.oleDbDataAdapter1 = new System.Data.OleDb.OleDbDataAdapter();
      this.oleDbSelectCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbInsertCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbUpdateCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbDeleteCommand1 = new System.Data.OleDb.OleDbCommand();
      this.oleDbConnection1 = new System.Data.OleDb.OleDbConnection();
      ((System.ComponentModel.ISupportInitialize)(this.dgTestErrors)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dsTestErrors)).BeginInit();
      this.SuspendLayout();
      // 
      // dgTestErrors
      // 
      this.dgTestErrors.DataMember = "";
      this.dgTestErrors.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dgTestErrors.Location = new System.Drawing.Point(16, 40);
      this.dgTestErrors.Name = "dgTestErrors";
      this.dgTestErrors.Size = new System.Drawing.Size(416, 216);
      this.dgTestErrors.TabIndex = 0;
      // 
      // dsTestErrors
      // 
      this.dsTestErrors.DataSetName = "dsTestErrors";
      this.dsTestErrors.Locale = new System.Globalization.CultureInfo("en-US");
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(8, 8);
      this.button1.Name = "button1";
      this.button1.TabIndex = 1;
      this.button1.Text = "Test Now";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // oleDbDataAdapter1
      // 
      this.oleDbDataAdapter1.DeleteCommand = this.oleDbDeleteCommand1;
      this.oleDbDataAdapter1.InsertCommand = this.oleDbInsertCommand1;
      this.oleDbDataAdapter1.SelectCommand = this.oleDbSelectCommand1;
      this.oleDbDataAdapter1.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                                                                                                new System.Data.Common.DataTableMapping("Table", "testErrors", new System.Data.Common.DataColumnMapping[] {
                                                                                                                                                                                                            new System.Data.Common.DataColumnMapping("teTicker", "teTicker"),
                                                                                                                                                                                                            new System.Data.Common.DataColumnMapping("teDate", "teDate"),
                                                                                                                                                                                                            new System.Data.Common.DataColumnMapping("teDescription", "teDescription")})});
      this.oleDbDataAdapter1.UpdateCommand = this.oleDbUpdateCommand1;
      // 
      // oleDbSelectCommand1
      // 
      this.oleDbSelectCommand1.CommandText = "SELECT teDate, teDescription, teTicker FROM testErrors";
      this.oleDbSelectCommand1.Connection = this.oleDbConnection1;
      // 
      // oleDbInsertCommand1
      // 
      this.oleDbInsertCommand1.CommandText = "INSERT INTO testErrors(teDate, teDescription, teTicker) VALUES (?, ?, ?)";
      this.oleDbInsertCommand1.Connection = this.oleDbConnection1;
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teDate", System.Data.OleDb.OleDbType.DBDate, 0, "teDate"));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teDescription", System.Data.OleDb.OleDbType.VarWChar, 50, "teDescription"));
      this.oleDbInsertCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "teTicker"));
      // 
      // oleDbUpdateCommand1
      // 
      this.oleDbUpdateCommand1.CommandText = "UPDATE testErrors SET teDate = ?, teDescription = ?, teTicker = ? WHERE (teDate =" +
        " ?) AND (teTicker = ?) AND (teDescription = ? OR ? IS NULL AND teDescription IS " +
        "NULL)";
      this.oleDbUpdateCommand1.Connection = this.oleDbConnection1;
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teDate", System.Data.OleDb.OleDbType.DBDate, 0, "teDate"));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teDescription", System.Data.OleDb.OleDbType.VarWChar, 50, "teDescription"));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("teTicker", System.Data.OleDb.OleDbType.VarWChar, 8, "teTicker"));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDate", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teTicker", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDescription", System.Data.OleDb.OleDbType.VarWChar, 50, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDescription", System.Data.DataRowVersion.Original, null));
      this.oleDbUpdateCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDescription1", System.Data.OleDb.OleDbType.VarWChar, 50, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDescription", System.Data.DataRowVersion.Original, null));
      // 
      // oleDbDeleteCommand1
      // 
      this.oleDbDeleteCommand1.CommandText = "DELETE FROM testErrors WHERE (teDate = ?) AND (teTicker = ?) AND (teDescription =" +
        " ? OR ? IS NULL AND teDescription IS NULL)";
      this.oleDbDeleteCommand1.Connection = this.oleDbConnection1;
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDate", System.Data.OleDb.OleDbType.DBDate, 0, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDate", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teTicker", System.Data.OleDb.OleDbType.VarWChar, 8, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teTicker", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDescription", System.Data.OleDb.OleDbType.VarWChar, 50, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDescription", System.Data.DataRowVersion.Original, null));
      this.oleDbDeleteCommand1.Parameters.Add(new System.Data.OleDb.OleDbParameter("Original_teDescription1", System.Data.OleDb.OleDbType.VarWChar, 50, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), "teDescription", System.Data.DataRowVersion.Original, null));
      // 
      // oleDbConnection1
      // 
      this.oleDbConnection1.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;Data Source=C:\Documents and Settings\Glauco\My Documents\Visual Studio Projects\QuantProject\QuantProject.mdb;Mode=Share Deny None;Extended Properties="""";Jet OLEDB:System database="""";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
      // 
      // Test
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(472, 273);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.button1,
                                                                  this.dgTestErrors});
      this.Name = "Test";
      this.Text = "Test";
      ((System.ComponentModel.ISupportInitialize)(this.dgTestErrors)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dsTestErrors)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

    private string teDescription( double quOpen , double quLow , double quHigh , double quClose )
    {
      string toReturn = "";
      if (quOpen<quLow)
        toReturn = "Open less than Low";
      else
        if (quOpen>quHigh)
          toReturn = "Open greater than High";
            else
          if (quLow>quHigh)
            toReturn = "Low greater than High";
              else
            if (quClose<quLow)
              toReturn = "Closs less than Open";
                else
              if (quClose>quHigh)
                toReturn = "Close greater than High";
      toReturn = toReturn + ". " + quOpen + " " + quLow + " " + quHigh + " " + quClose;
      return ( toReturn );
    }

    private void test_go_update_dataTable_testErrors()
    {
      System.Data.DataTable dtTestErrors = this.dsTestErrors.Tables[ "testErrors" ];
      System.Data.DataTable dtTestErrors_temp = new System.Data.DataTable();
      System.Data.OleDb.OleDbDataAdapter oda = new System.Data.OleDb.OleDbDataAdapter();
      System.Data.OleDb.OleDbCommand odc = new System.Data.OleDb.OleDbCommand();
      odc.CommandText = 
        "select * from quotes where " +
        "quOpen<quLow or quOpen>quHigh or quLow>quHigh or quClose<quLow or quClose>quHigh";
      odc.Connection = this.oleDbConnection1;
      oda.SelectCommand = odc;
      oda.Fill( dtTestErrors_temp );
      foreach (System.Data.DataRow dr in dtTestErrors_temp.Rows )
      {
        System.Data.DataRow newRow = dtTestErrors.NewRow();
        newRow[ "teTicker" ] = dr[ "quTicker" ];
        newRow[ "teDate" ] = dr[ "quDate" ];
        double myTemp = Double.Parse( dr[ "quOpen" ].ToString() );
        newRow[ "teDescription" ] =
          teDescription( Double.Parse( dr[ "quOpen" ].ToString() ) ,
          Double.Parse( dr[ "quLow" ].ToString() ) , Double.Parse( dr[ "quHigh" ].ToString() ) ,
          Double.Parse( dr[ "quClose" ].ToString() ) );
        dtTestErrors.Rows.Add( newRow );
      }
    }

    private void test_go()
    {
      this.oleDbDataAdapter1.Fill( this.dsTestErrors , "testErrors" );
      foreach (System.Data.DataRow dr in this.dsTestErrors.Tables[ "testErrors" ].Rows)
        dr.Delete();
      this.dgTestErrors.DataSource = this.dsTestErrors.Tables[ "testErrors" ];
      test_go_update_dataTable_testErrors();
      this.oleDbDataAdapter1.Update( this.dsTestErrors.Tables[ "testErrors" ] );
    }
    private void button1_Click(object sender, System.EventArgs e)
    {
      test_go();
    }
	}
}
