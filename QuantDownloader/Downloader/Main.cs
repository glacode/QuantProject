using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantDownloader
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
    private System.Windows.Forms.MenuItem menuItem8;
    private System.Windows.Forms.MenuItem menuItem10;
    private System.Windows.Forms.MenuItem menuItem11;
    private System.Windows.Forms.MenuItem menuItem12;
    private System.Windows.Forms.MenuItem menuItem13;
    private System.Windows.Forms.MenuItem menuItem14;
    private System.Windows.Forms.MenuItem menuItem9;
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
      this.menuItem8 = new System.Windows.Forms.MenuItem();
      this.menuItem10 = new System.Windows.Forms.MenuItem();
      this.menuItem9 = new System.Windows.Forms.MenuItem();
      this.menuItem11 = new System.Windows.Forms.MenuItem();
      this.menuItem12 = new System.Windows.Forms.MenuItem();
      this.menuItem13 = new System.Windows.Forms.MenuItem();
      this.menuItem14 = new System.Windows.Forms.MenuItem();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this.menuItem2 = new System.Windows.Forms.MenuItem();
      this.menuItem5 = new System.Windows.Forms.MenuItem();
      this.menuItem3 = new System.Windows.Forms.MenuItem();
      this.menuItem4 = new System.Windows.Forms.MenuItem();
      this.menuItem6 = new System.Windows.Forms.MenuItem();
      this.menuItem7 = new System.Windows.Forms.MenuItem();
      // 
      // mainMenu1
      // 
      this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                              this.menuItem8,
                                                                              this.menuItem11,
                                                                              this.menuItem13});
      // 
      // menuItem8
      // 
      this.menuItem8.Index = 0;
      this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                              this.menuItem10,
                                                                              this.menuItem9});
      this.menuItem8.Text = "Data";
      // 
      // menuItem10
      // 
      this.menuItem10.Index = 0;
      this.menuItem10.Text = "Download";
      this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
      // 
      // menuItem9
      // 
      this.menuItem9.Index = 1;
      this.menuItem9.Text = "Test";
      this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
      // 
      // menuItem11
      // 
      this.menuItem11.Index = 1;
      this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menuItem12});
      this.menuItem11.Text = "BackTest";
      // 
      // menuItem12
      // 
      this.menuItem12.Index = 0;
      this.menuItem12.Text = "Go";
      this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
      // 
      // menuItem13
      // 
      this.menuItem13.Index = 2;
      this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menuItem14});
      this.menuItem13.Text = "Trade";
      // 
      // menuItem14
      // 
      this.menuItem14.Index = 0;
      this.menuItem14.Text = "Go";
      this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
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
      // Principale
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 273);
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
        Application.Run(new Principale());  //togli il commento per riavere la windows application
        //new RunMSFTsimpleTest().Run();
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

	}
}
