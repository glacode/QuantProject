using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for Principale.
	/// </summary>
	public class Principale : System.Windows.Forms.Form
	{
    private System.Windows.Forms.MainMenu mainMenu1;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.Windows.Forms.MenuItem menuItem2;
    private System.Windows.Forms.MenuItem menuItem3;
    private System.Windows.Forms.MenuItem menuItem4;
    private System.Windows.Forms.MenuItem menuItem5;
    private System.Windows.Forms.MenuItem menuItem6;
    private System.Windows.Forms.MenuItem menuItem7;
    private System.Windows.Forms.MenuItem menuImport;
    private System.Windows.Forms.MenuItem subMenuFromTheWeb;
    private System.Windows.Forms.MenuItem menuValidate;
    private System.Windows.Forms.MenuItem subMenuValidateGo;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemTickerViewer;
		private System.Windows.Forms.MenuItem menuItemTickerGroupsViewer;
		private System.Windows.Forms.MenuItem menuItemQuotesEditor;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Principale()
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemTickerViewer = new System.Windows.Forms.MenuItem();
			this.menuItemTickerGroupsViewer = new System.Windows.Forms.MenuItem();
			this.menuImport = new System.Windows.Forms.MenuItem();
			this.subMenuFromTheWeb = new System.Windows.Forms.MenuItem();
			this.menuValidate = new System.Windows.Forms.MenuItem();
			this.subMenuValidateGo = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItemQuotesEditor = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItemOpen,
																																							this.menuImport,
																																							this.menuValidate});
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																								 this.menuItemTickerViewer,
																																								 this.menuItemTickerGroupsViewer,
																																								 this.menuItemQuotesEditor});
			this.menuItemOpen.Text = "Open";
			// 
			// menuItemTickerViewer
			// 
			this.menuItemTickerViewer.Index = 0;
			this.menuItemTickerViewer.Text = "Ticker Viewer";
			this.menuItemTickerViewer.Click += new System.EventHandler(this.menuItemTickerViewer_Click);
			// 
			// menuItemTickerGroupsViewer
			// 
			this.menuItemTickerGroupsViewer.Index = 1;
			this.menuItemTickerGroupsViewer.Text = "Ticker-Groups Viewer";
			this.menuItemTickerGroupsViewer.Click += new System.EventHandler(this.menuItemTickerGroupsViewer_Click);
			// 
			// menuImport
			// 
			this.menuImport.Index = 1;
			this.menuImport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							 this.subMenuFromTheWeb});
			this.menuImport.Text = "Import";
			// 
			// subMenuFromTheWeb
			// 
			this.subMenuFromTheWeb.Index = 0;
			this.subMenuFromTheWeb.Text = "From the Web";
			this.subMenuFromTheWeb.Click += new System.EventHandler(this.menuItem10_Click);
			// 
			// menuValidate
			// 
			this.menuValidate.Index = 2;
			this.menuValidate.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																								 this.subMenuValidateGo});
			this.menuValidate.Text = "Validate";
			// 
			// subMenuValidateGo
			// 
			this.subMenuValidateGo.Index = 0;
			this.subMenuValidateGo.Text = "Go";
			this.subMenuValidateGo.Click += new System.EventHandler(this.subMenuValidateGo_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = -1;
			this.menuItem1.Text = "";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = -1;
			this.menuItem2.Text = "";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = -1;
			this.menuItem5.Text = "";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = -1;
			this.menuItem3.Text = "";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = -1;
			this.menuItem4.Text = "";
			// 
			// menuItem6
			// 
			this.menuItem6.Index = -1;
			this.menuItem6.Text = "";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = -1;
			this.menuItem7.Text = "";
			// 
			// menuItemQuotesEditor
			// 
			this.menuItemQuotesEditor.Index = 2;
			this.menuItemQuotesEditor.Text = "Quotes Editor";
			this.menuItemQuotesEditor.Click += new System.EventHandler(this.menuItemQuotesEditor_Click);
			// 
			// Principale
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(232, 177);
			this.Menu = this.mainMenu1;
			this.Name = "Principale";
			this.Text = "Principale";

		}
		#endregion

    private void menuItem9_Click(object sender, System.EventArgs e)
    {
      TestDownloadedData testDownloadedData = new TestDownloadedData();
      testDownloadedData.ShowDialog();
    }

    private void menuItem12_Click(object sender, System.EventArgs e)
    {
//      BackTestForm backTestForm = new BackTestForm();
//      backTestForm.ShowDialog();
    }

    private void menuItem10_Click(object sender, System.EventArgs e)
    {
      WebDownloader webDownloader = new WebDownloader();
      webDownloader.ShowDialog( this );
    }


    [STAThread]
    static void Main() 
    {
      //try
      {
        Application.Run(new Principale());  //togli il commento per riavere il downloader
//        Application.Run(new QuotesEditor( "RYVYX" ));
//        Application.Run(new TestScpl());
      } 
      //catch ( Exception ex )
      {
        //MessageBox.Show( ex.ToString() ) ;
      }
    }

    private void menuItem14_Click(object sender, System.EventArgs e)
    {
//      BackTestForm form1 = new BackTestForm();
//      form1.ShowDialog( this );
    }

    private void subMenuValidateGo_Click(object sender, System.EventArgs e)
    {
      QuantProject.Applications.Downloader.Validate.ValidateForm validateForm =
        new QuantProject.Applications.Downloader.Validate.ValidateForm();
      validateForm.ShowDialog();
    }

		private void menuItemTickerViewer_Click(object sender, System.EventArgs e)
		{
			TickerViewer tickerViewer = new TickerViewer();
			tickerViewer.Show();
		}

		private void menuItemTickerGroupsViewer_Click(object sender, System.EventArgs e)
		{
			TickerGroupsViewer tickerGroupsViewer = new TickerGroupsViewer();
			tickerGroupsViewer.Show();
		}

		private void menuItemQuotesEditor_Click(object sender, System.EventArgs e)
		{
			QuotesEditor quotesEditor = new QuotesEditor( "MSFT" );
			quotesEditor.ShowDialog();
		}

	}
}
