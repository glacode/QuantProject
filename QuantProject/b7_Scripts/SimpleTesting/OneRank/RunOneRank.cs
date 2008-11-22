/*
QuantProject - Quantitative Finance Library

OneRank.cs
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
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// Script to test the One Rank strategy on a single ticker
	/// </summary>
	public class RunOneRank : Script
	{
		private DateTime startDateTime = new DateTime( 2003 , 1 , 1 );
		private DateTime endDateTime = new DateTime( 2003 , 12 , 31 );
		private Account account;
		private HistoricalMarketValueProvider historicalMarketValueProvider =
			new HistoricalAdjustedQuoteProvider();
		/// <summary>
		/// Script to test the One Rank strategy on a single ticker
		/// </summary>
		public RunOneRank()
		{
		}
		private void showTransactionDataGridContextMenu( object sender ,
			MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
//			MessageBox.Show( dataRow[ "DateTime" ].ToString() );
			DateTime rowDateTime = (DateTime)dataRow[ "DateTime" ];
			string rowTicker = (string)dataRow[ "InstrumentKey"];
			OneRankForm oneRankForm = new OneRankForm();
			oneRankForm.FirstDateTime = rowDateTime.AddDays( -30 );
			oneRankForm.LastDateTime = rowDateTime;
			oneRankForm.Ticker = rowTicker;
			oneRankForm.Show();
		}
		private void mouseEventHandler( object sender , MouseEventArgs eventArgs )
		{
			if ( eventArgs.Button == MouseButtons.Right )
				this.showTransactionDataGridContextMenu( sender , eventArgs );
		}
		public override void Run()
		{
//			HistoricalDataProvider.MinDate = this.startDateTime;
//			HistoricalDataProvider.MaxDate = this.endDateTime.AddDays( 10 );
			HistoricalEndOfDayTimer historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
					HistoricalEndOfDayTimer.GetMarketOpen( this.startDateTime ) ,
//				new EndOfDayDateTime( this.startDateTime ,
//				EndOfDaySpecificTime.MarketOpen ) ,
					"^GSPC" );

//			with IB commission
//			this.account = new Account( "MSFT" , historicalEndOfDayTimer ,
//				new HistoricalDataStreamer( historicalEndOfDayTimer ,
//				this.historicalQuoteProvider ) ,
//				new HistoricalOrderExecutor( historicalEndOfDayTimer ,
//				this.historicalQuoteProvider ) ,
//				new IBCommissionManager() );

//			with no commission
			this.account = new Account( "MSFT" , historicalEndOfDayTimer ,
				new HistoricalDataStreamer( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) ,
				new HistoricalOrderExecutor( historicalEndOfDayTimer ,
				this.historicalMarketValueProvider ) );
			OneRank oneRank = new OneRank( account ,
				this.endDateTime );
			Report report = new Report( this.account , this.historicalMarketValueProvider );
			report.Create(
				"WFT One Rank" , 1 ,
				HistoricalEndOfDayTimer.GetMarketClose( this.endDateTime ) ,
//				new EndOfDayDateTime( this.endDateTime , EndOfDaySpecificTime.MarketClose ) ,
				"MSFT" );
			report.TransactionGrid.MouseUp +=
				new MouseEventHandler( this.mouseEventHandler );
//			ObjectArchiver.Archive( report.AccountReport ,
//				@"C:\Documents and Settings\Glauco\Desktop\reports\runOneRank.qPr" );
			report.Show();
		}
	}
}
