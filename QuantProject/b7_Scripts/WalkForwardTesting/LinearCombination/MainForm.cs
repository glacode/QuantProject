using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{

//		private int numberOfTickersToBeSavedFromEachGeneration = 5;

		private string tickerGroupID;
		private int numberOfEligibleTickers;
		private int numberOfTickersToBeChosen;
		//		private int numDaysForLiquidity;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDate;
		private DateTime lastDate;
		private double targetReturn;
		private PortfolioType portfolioType;
		private GeneticOptimizer GO;

		private OptimizationOutput optimizationOutput;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm( string tickerGroupID, int numberOfEligibleTickers, 
			int numberOfTickersToBeChosen, int numDaysForLiquidity, 
			int generationNumberForGeneticOptimizer,
			int populationSizeForGeneticOptimizer, string benchmark,
			DateTime firstDate, DateTime lastDate, double targetReturn,
			PortfolioType portfolioType)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.tickerGroupID = tickerGroupID;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			//				this.numDaysForLiquidity = numDaysForLiquidity;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem2,
																																							this.menuItem3});
			this.menuItem1.Text = "File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Create Optimized Genomes";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Test Optimized Genomes";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "MainForm";

		}
		#endregion

		#region createOptimizedGenomes
		protected DataTable getSetOfTickersToBeOptimized_quickly()
		{
			SelectorByGroup selectorByGroup =
				new SelectorByGroup( "20020106" );
			DataTable returnValue = selectorByGroup.GetTableOfSelectedTickers();
			return returnValue;
		}
		protected DataTable getSetOfTickersToBeOptimized()
		{
//			SelectorByLiquidity mostLiquid = new SelectorByLiquidity(this.tickerGroupID, false,
//				this.firstDate, this.lastDate,
//				this.numberOfEligibleTickers);
//			DataTable eligibleTickers = mostLiquid.GetTableOfSelectedTickers();

			
			SelectorByGroup selectorByGroup =
				new SelectorByGroup( "SP500" , this.lastDate );
			DataTable eligibleTickers = selectorByGroup.GetTableOfSelectedTickers();
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
				new SelectorByQuotationAtEachMarketDay( eligibleTickers,
				false, this.firstDate,
				this.lastDate, this.numberOfEligibleTickers, this.benchmark);
			return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
		}
		private void newGenerationEventHandler_updateProgressBar(
			NewGenerationEventArgs newGenerationEventArgs )
		{
//			if ( ( newGenerationEventArgs.GenerationCounter == 1 ) ||
//				( newGenerationEventArgs.GenerationCounter /	100 * 100 ==
//				newGenerationEventArgs.GenerationCounter ) )
				Console.WriteLine(
					newGenerationEventArgs.GenerationCounter.ToString() + " / " +
					newGenerationEventArgs.GenerationNumber.ToString() +
					" - " + DateTime.Now.ToString() );
		}
		private void newGenerationEventHandler( object sender ,
			NewGenerationEventArgs newGenerationEventArgs )
		{
			newGenerationEventHandler_updateProgressBar( newGenerationEventArgs );
			ArrayList generation = newGenerationEventArgs.Generation;
			if ( this.optimizationOutput.Count == 0 )
				// this is the first generation created and this.bestGenomes is still empty
				this.optimizationOutput.Add( new GenomeRepresentation(
					(Genome)generation[ generation.Count - 1 ] ,
					this.firstDate ,
					this.lastDate ,
					newGenerationEventArgs.GenerationCounter ) );
			for ( int i=0 ; i < generation.Count ; i++ )
				if ( ((Genome)generation[ i ]).Fitness >
					((GenomeRepresentation)this.optimizationOutput[
					this.optimizationOutput.Count - 1 ]).Fitness )
					// generation[ i ] is a genome better than the best already stored
					this.optimizationOutput.Add( new GenomeRepresentation(
						(Genome)generation[ i ] ,
						this.firstDate ,
						this.lastDate ,
						newGenerationEventArgs.GenerationCounter ) );
		}
		private void writeOptimizedGenomesToDisk()
		{
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			OptimizationOutput optimizationOutput =
				new OptimizationOutput();
			visualObjectArchiver.Save( optimizationOutput , "bgn" ,
				"Save best genomes" );
		}

		private void createOptimizedGenomes_go()
		{
		}
		private void createOptimizedGenomes()
		{
			this.optimizationOutput = new OptimizationOutput();
//			DataTable setOfTickersToBeOptimized =
//				this.getSetOfTickersToBeOptimized_quickly();
			DataTable setOfTickersToBeOptimized =
				this.getSetOfTickersToBeOptimized();
			GenomeManagerForEfficientCTOPortfolio genManEfficientCTOPortfolio = 
				new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
				this.firstDate,
				this.lastDate,
				this.numberOfTickersToBeChosen,
				this.targetReturn,
				this.portfolioType);
        
			this.GO = new GeneticOptimizer(genManEfficientCTOPortfolio,
				this.populationSizeForGeneticOptimizer,
				this.generationNumberForGeneticOptimizer);
			this.GO.NewGeneration += new NewGenerationEventHandler(
				this.newGenerationEventHandler );

//			Thread thread = new Thread(new ThreadStart(this.createOptimizedGenomes_go));
//			thread.IsBackground = true;
//			thread.Start();

			GO.Run(false);
       
			this.writeOptimizedGenomesToDisk();
		}
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.createOptimizedGenomes();
		}

		#endregion

		#region testOptimizedGenomes
		private void loadBestGenomesFromDisk()
		{
			if ( this.optimizationOutput == null )
			{
				VisualObjectArchiver visualObjectArchiver =
					new VisualObjectArchiver();
				this.optimizationOutput =
          (OptimizationOutput)visualObjectArchiver.Load(
					"Load best genomes" , "bgn" , "Load Genomes");
				if ( this.optimizationOutput.Count == 0 )
					throw new Exception( "The loaded optimization output contains no element!" );
				this.firstDate = ((GenomeRepresentation)(
					this.optimizationOutput[ 0 ])).FirstOptimizationDate;
				this.lastDate = ((GenomeRepresentation)(
					this.optimizationOutput[ 0 ])).LastOptimizationDate;
			}
		}
		private void testOptimizedGenomesActually()
		{
			TestDisplayer testDisplayer = new TestDisplayer(
				this.firstDate , this.lastDate , this.optimizationOutput );
			this.optimizationOutput = null;
			testDisplayer.Show();
		}
		private void testOptimizedGenomes()
		{
			this.loadBestGenomesFromDisk();
			this.testOptimizedGenomesActually();
		}
		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			this.testOptimizedGenomes();
		}
		#endregion
	}
}
