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

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class WebDownloader : System.Windows.Forms.Form
	{
    private static DataBaseLocator dataBaseLocator = new DataBaseLocator("MDB"); 
    private static string mdbPath = dataBaseLocator.Path;
    private static string connectionString =
      @"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;Data Source=" +
      mdbPath +
      @";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
    public OleDbConnection OleDbConnection1 = new OleDbConnection( connectionString );
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

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
      //this.oleDbDataAdapter1.Fill(this.dataSet11);
      this.OleDbConnection1.Open();
      //this.OleDbConnection1.Close();
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
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(48, 24);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(112, 24);
      this.button1.TabIndex = 0;
      this.button1.Text = "button1";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // dataGrid1
      // 
      this.dataGrid1.DataMember = "";
      this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGrid1.Location = new System.Drawing.Point(176, 16);
      this.dataGrid1.Name = "dataGrid1";
      this.dataGrid1.Size = new System.Drawing.Size(336, 216);
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
      // WebDownloader
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(592, 285);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.dataGrid1,
                                                                  this.button1});
      this.Name = "WebDownloader";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
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

    private void downloadQuotes_createTickerDataSet( DataSet ds )
    {
      System.Data.OleDb.OleDbDataAdapter oleDbDataAdapter1=new OleDbDataAdapter( "select * from tickers" , this.OleDbConnection1);
      oleDbDataAdapter1.Fill(ds);
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

    private void downloadQuotes_withTickerDataSet_create_dsTickerCurrentlyDownloaded( DataTable dt )
    {
      if (!this.DsTickerCurrentlyDownloaded.Tables.Contains( "Tickers" ))
      {
        this.DsTickerCurrentlyDownloaded.Tables.Add( "Tickers" );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( 
          new DataColumn( dt.Columns[ "tiTicker" ].ColumnName , dt.Columns[ "tiTicker" ].DataType ) );
        this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Columns.Add( "currentState" , System.Type.GetType( "System.String" ) );
        this.dataGrid1.DataSource = this.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ];
      }
      
    }

    private void downloadQuotes_withTickerDataSet( DataSet ds )
    {
      downloadQuotes_withTickerDataSet_create_dsTickerCurrentlyDownloaded( ds.Tables[0] );
      foreach (DataRow myRow in ds.Tables[0].Rows) 
      {
        //if (this.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Count>5)
        //  Monitor.Wait( this.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ] );

        TickerDownloader qd = new TickerDownloader( this , myRow , myRow["tiTicker"].ToString() , ds.Tables[0].Rows.Count );
        //Thread newThread = new Thread( new ThreadStart( qd.downloadTicker));
        //newThread.Start();
        qd.downloadTicker();
        //newThread.Join();
        //qd.downloadTicker();
      }
      //ImportQuotesCsv iqc = new ImportQuotesCsv();
    }

    private void downloadQuotes()
    {
      DataSet ds=new DataSet();
      //downloadQuotes_deleteFrom_quotes() - csv files aren't used anymore;
      downloadQuotes_createTickerDataSet( ds );
      //this.OleDbConnection1.Open();
      downloadQuotes_withTickerDataSet( ds );
      this.OleDbConnection1.Close();
    }

    private void button1_Click(object sender, System.EventArgs e)
    {
      oleDbCommand1.Connection = this.OleDbConnection1;
      this.oleDbCommand1.ExecuteNonQuery();
      downloadQuotes();
    }

    private void Form1_Load(object sender, System.EventArgs e)
    {
    
    }
	}

  }
