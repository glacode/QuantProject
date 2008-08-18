/*
QuantProject - Quantitative Finance Library

Main.cs
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
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using QuantProject.Scripts;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.ADT.Optimizing.Genetic;

using QuantProject.Business.Strategies.Logging;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.SimpleTesting;
using QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;
using QuantProject.Scripts.WalkForwardTesting.PairsTrading;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag;
using QuantProject.Scripts.CallingReportsForRunScripts;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.ADT.Statistics;





namespace QuantProject.Principale
{
	/// <summary>
	/// Summary description for Principale.
	/// </summary>
	public class Principale : System.Windows.Forms.Form
	{
//    public static RunEfficientPorfolio runEfficientPortfolio;
		
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
    private System.Windows.Forms.MenuItem menuItem13;
    private System.Windows.Forms.MenuItem menuItem14;
    private System.Windows.Forms.MenuItem menuItem9;
    private System.Windows.Forms.MenuItem menuItemRun;
    private System.Windows.Forms.MenuItem menuItemSavedTests;
    private System.Windows.Forms.MenuItem menuItemAccountViewer;
    private System.Windows.Forms.MenuItem menuItemShowReportFromAccount;
    private System.Windows.Forms.MenuItem menuItemRunReleasingMode;
    private System.Windows.Forms.MenuItem menuItemShowSavedReport;
    private System.Windows.Forms.MenuItem menuItemShowReportFromTransactions;
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
      this.menuItemRun = new System.Windows.Forms.MenuItem();
      this.menuItemSavedTests = new System.Windows.Forms.MenuItem();
      this.menuItemAccountViewer = new System.Windows.Forms.MenuItem();
      this.menuItemShowReportFromAccount = new System.Windows.Forms.MenuItem();
      this.menuItemShowSavedReport = new System.Windows.Forms.MenuItem();
      this.menuItemRunReleasingMode = new System.Windows.Forms.MenuItem();
      this.menuItem13 = new System.Windows.Forms.MenuItem();
      this.menuItem14 = new System.Windows.Forms.MenuItem();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this.menuItem2 = new System.Windows.Forms.MenuItem();
      this.menuItem5 = new System.Windows.Forms.MenuItem();
      this.menuItem3 = new System.Windows.Forms.MenuItem();
      this.menuItem4 = new System.Windows.Forms.MenuItem();
      this.menuItem6 = new System.Windows.Forms.MenuItem();
      this.menuItem7 = new System.Windows.Forms.MenuItem();
      this.menuItemShowReportFromTransactions = new System.Windows.Forms.MenuItem();
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
                                                                               this.menuItemRun,
                                                                               this.menuItemSavedTests,
                                                                               this.menuItemRunReleasingMode});
      this.menuItem11.Text = "BackTest";
      // 
      // menuItemRun
      // 
      this.menuItemRun.Index = 0;
      this.menuItemRun.Text = "Run (debugging mode)";
      this.menuItemRun.Click += new System.EventHandler(this.menuItemRun_Click);
      // 
      // menuItemSavedTests
      // 
      this.menuItemSavedTests.Index = 1;
      this.menuItemSavedTests.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.menuItemAccountViewer,
                                                                                       this.menuItemShowReportFromAccount,
                                                                                       this.menuItemShowSavedReport,
                                                                                       this.menuItemShowReportFromTransactions});
      this.menuItemSavedTests.Text = "Saved Tests";
      // 
      // menuItemAccountViewer
      // 
      this.menuItemAccountViewer.Index = 0;
      this.menuItemAccountViewer.Text = "Account viewer";
      this.menuItemAccountViewer.Click += new System.EventHandler(this.menuItemAccountViewer_Click);
      // 
      // menuItemShowReportFromAccount
      // 
      this.menuItemShowReportFromAccount.Index = 1;
      this.menuItemShowReportFromAccount.Text = "Show report from account";
      this.menuItemShowReportFromAccount.Click += new System.EventHandler(this.menuItemShowReportFromAccount_Click);
      // 
      // menuItemShowSavedReport
      // 
      this.menuItemShowSavedReport.Index = 2;
      this.menuItemShowSavedReport.Text = "Show saved report";
      this.menuItemShowSavedReport.Click += new System.EventHandler(this.menuItemShowSavedReport_Click);
      // 
      // menuItemRunReleasingMode
      // 
      this.menuItemRunReleasingMode.Index = 2;
      this.menuItemRunReleasingMode.Text = "Run (releasing mode)";
      this.menuItemRunReleasingMode.Click += new System.EventHandler(this.menuItemRunReleasingMode_Click);
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
      // menuItemShowReportFromTransactions
      // 
      this.menuItemShowReportFromTransactions.Index = 3;
      this.menuItemShowReportFromTransactions.Text = "Show report from transactions";
      this.menuItemShowReportFromTransactions.Click += new System.EventHandler(this.menuItemShowReportFromTransactions_Click);
      // 
      // Principale
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 273);
      this.Menu = this.mainMenu1;
      this.Name = "Principale";
      this.Text = "Main";

    }
		#endregion

		private void menuItem9_Click(object sender, System.EventArgs e)
		{
//			TestDownloadedData testDownloadedData = new TestDownloadedData();
//			testDownloadedData.ShowDialog();
			BackTestLog backTestLog = LogArchiver.Load( "C:\\qpReports\\pairsTrading\\" );
			LogViewer logViewer =
				new LogViewer( backTestLog );
			logViewer.Show();
			
		}

    private void menuItem10_Click(object sender, System.EventArgs e)
    {
    }
    
    static void runMilloScripts()
    {
      /*QuantProject.Principale.Principale.runEfficientPortfolio = 
        new RunEfficientCTCPorfolio("Test",150,4,60,10,15000,"^SPX",
                                    new DateTime(2004,3,1),
											              new DateTime(2004,5,31),15,2,0.0,
                                    PortfolioType.ShortAndLong,0.05,
                                    2);*/
      /*QuantProject.Principale.Principale.runEfficientPortfolio = 
        new RunTestOptimizedCTCPortfolio("Test",150,4,60,10,25000,"^SPX",
        new DateTime(2004,3,1),
        new DateTime(2004,3,1).AddDays(60),15,2,0.015,
        PortfolioType.ShortAndLong,0.1,
        2);/*
    	/*QuantProject.Principale.Principale.runEfficientPortfolio =
    		new RunEfficientCTOPorfolio("Test",150,4,45,10,1000,
											        "^SPX",
											        new DateTime(2004,1,1),
											        new DateTime(2004,1,15),
											        0.0, PortfolioType.ShortAndLong, 2);*/
                              
//    	QuantProject.Principale.Principale.runEfficientPortfolio = 
//    		new RunTestOptimizedCTOPorfolio("Test",150,5,45,10,50000,
//											        "^SPX",
//											        new DateTime(2004,3,1),
//											        new DateTime(2004,3,1).AddDays(45),
//											        0.0005, PortfolioType.ShortAndLong, 12);
                              
    		/*new RunTestOptimizedCTCPortfolio("Test",150,6,90,10,20000,
    		                                 "^SPX",
    		                                 new DateTime(2002,10,1),
        	                        				new DateTime(2002,10,1).AddDays(120),
        	                        				5,2,0.0,
        	                        				PortfolioType.ShortAndLong, 0.50, 10);*/
    		                                 
//    	QuantProject.Principale.Principale.runEfficientPortfolio.Run();
 /*     QuantProject.Principale.Principale.runEfficientPortfolio = 
        new RunTestOptimizedCTOPorfolio("Test",150,6,45,10,150000,
        "^SPX",
        new DateTime(2003,1,1),
        new DateTime(2003,1,1).AddDays(45),
        0.0, PortfolioType.OnlyShort, 12);
      
      QuantProject.Principale.Principale.runEfficientPortfolio.Run();  */
      /*
    	QuantProject.Principale.Principale.runEfficientPortfolio =
    	new RunTestOptimizedCTCPortfolio("Test",100,5,120,10,150000,
    	                            "^SPX",
    	                            new DateTime(2002,10,1),
        	                        new DateTime(2002,10,1).AddDays(120),
        	                        5, 0.0,
        	                       	PortfolioType.ShortAndLong, 0.50, 10);
    	QuantProject.Principale.Principale.runEfficientPortfolio.Run();
    	
      QuantProject.Principale.Principale.runEfficientPortfolio =
        new RunTestOptimizedCTCPortfolio("Test",150,5,80,15,60000,
        "^SPX",
        new DateTime(2002,10,1),
        new DateTime(2002,10,1).AddDays(80),
        5, 0.0,
        PortfolioType.OnlyLong , 0.50, 10);
      QuantProject.Principale.Principale.runEfficientPortfolio.Run();

      QuantProject.Principale.Principale.runEfficientPortfolio =
        new RunTestOptimizedCTCPortfolio("Test",150,5,160,15,60000,
        "^SPX",
        new DateTime(2002,10,1),
        new DateTime(2002,10,1).AddDays(160),
        5, 0.0,
        PortfolioType.ShortAndLong, 0.50, 10);
      QuantProject.Principale.Principale.runEfficientPortfolio.Run();
			*/
         	
    	/*
    	    	QuantProject.Principale.Principale.runEfficientPortfolio =  		
    		new RunEfficientCTOPorfolio("Test",500,6,45,15,50000,
											        "^SPX",
											        new DateTime(2002,1,1),
											        new DateTime(2004,12,31),
											        0.0, PortfolioType.OnlyLong);
    	
    	 QuantProject.Principale.Principale.runEfficientPortfolio.Run();
    	*/
    			
    }

    [STAThread]
    static void Main() 
    {
//      try
//      {
        //new RunMSFTsimpleTest().Run();
//		new RunMSFTsimpleTest_2().Run();
				//new RunOneRankWithExcelReport().Run();

				//new RunOneRankWithWindowsReport().Run();
        //new RunBestTwoIndipendent().Run();
        //Principale.geneticOptimizerTest();
        //new RunEfficientCTOPorfolio().Run();
        //new RunEfficientCTCPorfolio().Run();
				//new RunEfficientPorfolio().Run();

//				QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases.FixedLengthTwoPhasesMain
//					myMain =
//				new QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases.FixedLengthTwoPhasesMain().Run;
        Application.Run(new Principale());
        //geneticOptimizerTest();
        //Application.Run(new OneRankForm());
        //runMilloScripts();
        
        //new RunEfficientCTOPorfolio("Test",200,5,60,10,5000, "^SPX",
        //                            new DateTime(2000,1,1),
        //                            new DateTime(2001,12,31)).Run();
      //  new RunMSFTwalkForward().Run();

	//		new RunOneRankWithWindowsReport().Run();
				//new RunMSFTwalkForward().Run();

        //new RunMultiTestOneRank().Run();

				//new RunWalkForwardOneRank().Run();

				//new RunEfficientCTOPorfolio().Run();
				//new RunWalkForwardOneRank().Run();

//      } 
//      catch ( Exception ex )
//      {
////      	QuantProject.Principale.Principale.runEfficientPortfolio.SaveScriptResults();
//      	writeExceptionToLogFile(ex);
//      }
    }
    static private void writeExceptionToLogFile(Exception ex)
    {
    	string path = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\'))
										+ @"\ExceptionLog.txt";
    	if(File.Exists(path))
    	{
    	  StreamWriter w = File.AppendText(path);
	      w.Write("\r\nTime of exception : ");
	      w.WriteLine("{0} {1}", DateTime.Now.ToLongDateString(),
	                  DateTime.Now.ToLocalTime());
//	      w.Write("\r\nScript name: {0}",
//	              QuantProject.Principale.Principale.runEfficientPortfolio.ScriptName );
//	      w.Write("\r\nTimer date: {0}",
//	              QuantProject.Principale.Principale.runEfficientPortfolio.TimerLastDate.ToLongDateString() );
//	      
	      w.WriteLine("\n  :{0}", ex.ToString());
	      w.WriteLine("\n  :{0}", ex.StackTrace.ToString());
	      w.WriteLine ("\n-----------------------------------");
	      // Update the underlying file.
	      w.Flush();
	      w.Close();
    	}
    	

    }
    
    private static void geneticOptimizerTest()
    {
      IGenomeManager genomeManagerTest = new GenomeManagerTest(5,1,10);     
      GeneticOptimizer GO = new GeneticOptimizer(genomeManagerTest,1000,10);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.Run(true);
      System.Console.WriteLine("\n\nThe best solution found is: " + (string)GO.BestGenome.Meaning +
                                " with {0} generations", GO.GenerationCounter);
    }
    
    private void menuItem14_Click(object sender, System.EventArgs e)
    {
    }
    //run scripts in debugging mode
    private void menuItemRun_Click(object sender, System.EventArgs e)
    {

//      try
//      {//call here your scripts
	//new RunWalkForwardOneRank().Run();
//			new WFLagMain().ShowDialog();

			new FixedLengthTwoPhasesMain().Run();
//			new PairsTradingMain().Run();
        
//			new RunOneRank().Run();
        //new RunEfficientCTCPorfolio("Test",400,5,90,10,10000,
        //	                         	"^SPX",
        //	                          new DateTime(2004,1,1),
        //	                          new DateTime(2004,2,29),
        //	                          3).Run();
        
        //this.Close();
//        runMilloScripts();
        
        //new RunEfficientCTOPorfolio("Test",200,5,60,2,500, "^SPX",
        //                            new DateTime(2004,1,1),
        //                            new DateTime(2004,1,15)).Run();
        
//      }
//      catch ( Exception ex )
//      {
//        string notUsed = ex.ToString();
//        //in this way qP shouldn't stop if running a single script fails ...
//      }

    }

    private void menuItemAccountViewer_Click(object sender, System.EventArgs e)
    {
      AccountViewer accountViewer = new AccountViewer();
      accountViewer.Show();
    }

    private string getPath(string dialogTitle)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = dialogTitle;
      openFileDialog.Multiselect = false;
      openFileDialog.CheckFileExists = true;
      openFileDialog.ShowDialog();
      return openFileDialog.FileName;
    }

    private void menuItemShowReportFromAccount_Click(object sender, System.EventArgs e)
    {
      string chosenPath = this.getPath("Select a serialized account please ...");
      if(chosenPath != "")
        ShowReportFromFile.ShowReportFromSerializedAccount(chosenPath);
    }

    
    private void menuItemRunReleasingMode_Click(object sender, System.EventArgs e)
    {
      try
      { 
        this.Cursor = Cursors.WaitCursor;
        //call here your scripts
        //new RunWalkForwardOneRank().Run();			
       

      }
      catch ( Exception ex )
      {
        ex = ex;
        //in this way qP shouldn't stop if running a single script fails ...
      }
      finally
      {
        this.Cursor = Cursors.Default;
      }
    }

    private void menuItemShowSavedReport_Click(object sender, System.EventArgs e)
    {
      string chosenPath = this.getPath("Select a serialized report please ...");
      if(chosenPath != "")
        ShowReportFromFile.ShowReportFromSerializedAccountReport(chosenPath);
    }

    private void menuItemShowReportFromTransactions_Click(object sender, System.EventArgs e)
    {
      string chosenPath = this.getPath("Select a serialized TransactionHistory please ...");
      if(chosenPath != "")
        ShowReportFromFile.ShowReportFromSerializedTransactionHistory(chosenPath);
    }




	}
}
