/*
QuantProject - Quantitative Finance Library

SummaryTabPage.cs
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
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;
using QuantProject.Presentation;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// TabPage to show the summary within the report form
	/// </summary>
	public class SummaryTabPage : TabPage
	{
		// constant values for label's placement
		private int labelRows = 10;
		private int xForLabels = 17;
		private int textLabelsWidth = 180;
		private int valueLablesWidth = 60;
		private int textToValueLabelSpacing = 8;
		private int valueToTextLabelSpacing = 50;
		private int yStart = 17;
		private int yStep = 25;

		private ArrayList summaryItems;
		/// <summary>
		/// used just to apply the GetType method
		/// </summary>
		private SummaryRow summaryRow;

		private System.Windows.Forms.Label lblValTotalNetProfit;
		private System.Windows.Forms.Label lblTotalNetProfit;
		private System.Windows.Forms.Label lblReturnOnAccount;
		private System.Windows.Forms.Label lblValReturnOnAccount;
		private System.Windows.Forms.Label lblAnnualSystemPercReturn;
		private System.Windows.Forms.Label lblValAnnualSystemPercReturn;
		private System.Windows.Forms.Label lblMaxEquityDrawDown;
		private System.Windows.Forms.Label lblValMaxEquityDrawDown;
		private System.Windows.Forms.Label lblTotalCommission;
		private System.Windows.Forms.Label lblValTotalCommission;
		private System.Windows.Forms.Label lblNumberWinningPeriods;
		private System.Windows.Forms.Label lblValNumberWinningPeriods;
		private System.Windows.Forms.Label lblNumberLosingPeriods;
		private System.Windows.Forms.Label lblValNumberLosingPeriods;
		private System.Windows.Forms.Label lblNumberEvenPeriods;
		private System.Windows.Forms.Label lblValNumberEvenPeriods;
		private System.Windows.Forms.Label lblPercentageWinningPeriods;
		private System.Windows.Forms.Label lblValPercentageWinningPeriods;
		private System.Windows.Forms.Label lblTotalNumberOfTrades;
		private System.Windows.Forms.Label lblValTotalNumberOfTrades;
		private System.Windows.Forms.Label lblNumberWinningTrades;
		private System.Windows.Forms.Label lblValNumberWinningTrades;
		private System.Windows.Forms.Label lblAverageTradePercReturn;
		private System.Windows.Forms.Label lblValAverageTradePercReturn;
		private System.Windows.Forms.Label lblLargestWinningTrade;
		private System.Windows.Forms.Label lblValLargestWinningTrade;
		private System.Windows.Forms.Label lblLargestLosingTrade;
		private System.Windows.Forms.Label lblValLargestLosingTrade;
		private System.Windows.Forms.Label lblTotalNumberOfLongTrades;
		private System.Windows.Forms.Label lblValTotalNumberOfLongTrades;
		private System.Windows.Forms.Label lblAverageLongTradePercReturn;
		private System.Windows.Forms.Label lblValAverageLongTradePercReturn;
		private System.Windows.Forms.Label lblNumberWinningShortTrades;
		private System.Windows.Forms.Label lblValNumberWinningShortTrades;
		private System.Windows.Forms.Label lblBenchmarkPercReturn;
		private System.Windows.Forms.Label lblValBenchmarkPercReturn;
		private System.Windows.Forms.Label lblNumberWinningLongTrades;
		private System.Windows.Forms.Label lblValNumberWinningLongTrades;
		private System.Windows.Forms.Label lblTotalNumberOfShortTrades;
		private System.Windows.Forms.Label lblValTotalNumberOfShortTrades;

		private AccountReport accountReport;

		private Point getPointForTextLabel( int labelPosition )
		{
			int x,y;
			if ( labelPosition < this.labelRows )
			{
				x = this.xForLabels;
				y = this.yStart + labelPosition * this.yStep;
			}
			else
			{
				x = this.xForLabels + this.textLabelsWidth + this.textToValueLabelSpacing +
					this.valueLablesWidth + this.valueToTextLabelSpacing;
				y = this.yStart + ( labelPosition - this.labelRows ) * this.yStep;
			}
			return new Point( x , y );
		}
		private void addTextLabel( Label label , string name , string text )
		{
			this.Controls.Add( label );
//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForTextLabel( ( this.Controls.Count - 1 ) / 2 );
			label.Width = this.textLabelsWidth;
			label.Name = name;
			label.Text = text;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		}
		private void addValueLabel( Label label , string name )
		{
			this.Controls.Add( label );
			//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForValueLabel( ( this.Controls.Count / 2 ) - 1 );
			label.Width = this.valueLablesWidth;
			label.Name = name;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		}

		private void addTextLabel( Label label )
		{
			this.Controls.Add( label );
			//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForTextLabel( ( this.Controls.Count - 1 ) / 2 );
			label.Width = this.textLabelsWidth;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		}
		private void addValueLabel( Label label )
		{
			this.Controls.Add( label );
			//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForValueLabel( ( this.Controls.Count / 2 ) - 1 );
			label.Width = this.valueLablesWidth;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		}

		private Point getPointForValueLabel( int labelPosition )
		{
			Point point = getPointForTextLabel( labelPosition ); 
			point.X += this.textLabelsWidth + this.textToValueLabelSpacing;
			return point;
		}
		private void myInitializeComponent_addItem( SummaryItem summaryItem )
		{
			this.addTextLabel( summaryItem.Description );
			this.addValueLabel( summaryItem.Value );
		}
		private void myInitializeComponent_addSummaryRow( PropertyInfo propertyInfo )
		{
			Object ob = propertyInfo.GetValue( this.accountReport.Summary , null );
			if ( ob is SummaryRow )
			{
				this.summaryItems.Add( new SummaryItem(
					(SummaryRow)ob ) );
			}
		}
		private void myInitializeComponent_addSummaryRows()
		{
			foreach ( PropertyInfo summaryProperty in
				this.accountReport.Summary.GetType().GetProperties() )
				myInitializeComponent_addSummaryRow( summaryProperty );
		}
		private void myInitializeComponent_old()
		{
			this.summaryItems = new ArrayList();
			myInitializeComponent_addSummaryRows();
			foreach( SummaryItem summaryItem in this.summaryItems )
				this.myInitializeComponent_addItem( summaryItem );

			this.lblTotalNetProfit = new System.Windows.Forms.Label();
			this.lblValTotalNetProfit = new System.Windows.Forms.Label();
			this.lblReturnOnAccount = new System.Windows.Forms.Label();
			this.lblValReturnOnAccount = new System.Windows.Forms.Label();
			this.lblBenchmarkPercReturn = new System.Windows.Forms.Label();
			this.lblValBenchmarkPercReturn = new System.Windows.Forms.Label();
			this.lblAnnualSystemPercReturn = new System.Windows.Forms.Label();
			this.lblValAnnualSystemPercReturn = new System.Windows.Forms.Label();
			this.lblMaxEquityDrawDown = new System.Windows.Forms.Label();
			this.lblValMaxEquityDrawDown = new System.Windows.Forms.Label();
			this.lblTotalCommission = new System.Windows.Forms.Label();
			this.lblValTotalCommission = new System.Windows.Forms.Label();
			this.lblNumberWinningPeriods = new System.Windows.Forms.Label();
			this.lblValNumberWinningPeriods = new System.Windows.Forms.Label();
			this.lblNumberLosingPeriods = new System.Windows.Forms.Label();
			this.lblValNumberLosingPeriods = new System.Windows.Forms.Label();
			this.lblNumberEvenPeriods = new System.Windows.Forms.Label();
			this.lblValNumberEvenPeriods = new System.Windows.Forms.Label();
			this.lblPercentageWinningPeriods = new System.Windows.Forms.Label();
			this.lblValPercentageWinningPeriods = new System.Windows.Forms.Label();
			this.lblTotalNumberOfTrades = new System.Windows.Forms.Label();
			this.lblValTotalNumberOfTrades = new System.Windows.Forms.Label();
			this.lblNumberWinningTrades = new System.Windows.Forms.Label();
			this.lblValNumberWinningTrades = new System.Windows.Forms.Label();
			this.lblAverageTradePercReturn = new System.Windows.Forms.Label();
			this.lblValAverageTradePercReturn = new System.Windows.Forms.Label();
			this.lblLargestWinningTrade = new System.Windows.Forms.Label();
			this.lblValLargestWinningTrade = new System.Windows.Forms.Label();
			this.lblLargestLosingTrade = new System.Windows.Forms.Label();
			this.lblValLargestLosingTrade = new System.Windows.Forms.Label();
			this.lblTotalNumberOfLongTrades = new System.Windows.Forms.Label();
			this.lblValTotalNumberOfLongTrades = new System.Windows.Forms.Label();
			this.lblNumberWinningLongTrades = new System.Windows.Forms.Label();
			this.lblValNumberWinningLongTrades = new System.Windows.Forms.Label();
			this.lblAverageLongTradePercReturn = new System.Windows.Forms.Label();
			this.lblValAverageLongTradePercReturn = new System.Windows.Forms.Label();
			this.lblTotalNumberOfShortTrades = new System.Windows.Forms.Label();
			this.lblValTotalNumberOfShortTrades = new System.Windows.Forms.Label();
			this.lblNumberWinningShortTrades = new System.Windows.Forms.Label();
			this.lblValNumberWinningShortTrades = new System.Windows.Forms.Label();
			// 
			// lblTotalNetProfit
			// 
			this.addTextLabel( lblTotalNetProfit , "lblTotalNetProfit" ,
				"Total net profit:" );
			// 
			// lblValTotalNetProfit
			// 
			this.addValueLabel( lblValTotalNetProfit , "lblValTotalNetProfit" );
			// 
			// lblReturnOnAccount
			// 
			this.addTextLabel( lblReturnOnAccount , "lblReturnOnAccount" ,
				"Return on account (%):" );
			// 
			// lblValReturnOnAccount
			// 
			this.addValueLabel( lblValReturnOnAccount , "lblValReturnOnAccount" );
			// 
			// lblBenchmarkPercReturn
			// 
			this.addTextLabel( lblBenchmarkPercReturn , "lblBenchmarkPercReturn" ,
				"Benchmark % return:" );
			// 
			// lblValBenchmarkPercReturn
			// 
			this.addValueLabel( lblValBenchmarkPercReturn , "lblValBuyAndHoldPercReturn" );
			// 
			// lblAnnualSystemPercReturn
			// 
			this.addTextLabel( lblAnnualSystemPercReturn , "lblAnnualSystemPercReturn" ,
				"Annual system % return:" );
			// 
			// lblValAnnualSystemPercReturn
			// 
			this.addValueLabel( lblValAnnualSystemPercReturn , "lblValAnnualSystemPercReturn" );
			// 
			// lblMaxEquityDrawDown
			// 
			this.addTextLabel( lblMaxEquityDrawDown , "lblMaxEquityDrawDown" ,
				"Max equity drawdown (%):" );
			// 
			// lblValMaxEquityDrawDown
			// 
			this.addValueLabel( lblValMaxEquityDrawDown , "lblValMaxEquityDrawDown" );
			// 
			// lblTotalCommission
			// 
			this.addTextLabel( this.lblTotalCommission , "lblTotalCommission" ,
				"Total Commission Amount:" );
			// 
			// lblValTotalCommission
			// 
			this.addValueLabel( this.lblValTotalCommission , "lblValTotalCommission" );
			// 
			// lblNumberWinningPeriods
			// 
			this.addTextLabel( this.lblNumberWinningPeriods , "lblNumberWinningPeriods" ,
				"Number Winning Periods:" );
			// 
			// lblValNumberWinningPeriods
			// 
			this.addValueLabel( this.lblValNumberWinningPeriods , "lblValNumberWinningPeriods" );
			// 
			// lblNumberLosingPeriods
			// 
			this.addTextLabel( this.lblNumberLosingPeriods , "lblNumberWinningPeriods" ,
				"Number Losing Periods:" );
			// 
			// lblValNumberLosingPeriods
			// 
			this.addValueLabel( this.lblValNumberLosingPeriods , "lblValNumberLosingPeriods" );
			// 
			// lblPercentageWinningPeriods
			// 
			this.addTextLabel( this.lblPercentageWinningPeriods , "lblPercentageWinningPeriods" ,
				"% Winning Periods:" );
			// 
			// lblValPercentageWinningPeriods
			// 
			this.addValueLabel( this.lblValPercentageWinningPeriods , "lblValPercentageWinningPeriods" );
			// 
			// lblTotalNumberOfTrades
			// 
			this.addTextLabel( lblTotalNumberOfTrades , "lblTotalNumberOfTrades" ,
				"Total # of trades:" );
			// 
			// lblValTotalNumberOfTrades
			// 
			this.addValueLabel( lblValTotalNumberOfTrades , "lblValTotalNumberOfTrades" );
			// 
			// lblNumberWinningTrades
			// 
			this.addTextLabel( lblNumberWinningTrades , "lblNumberWinningTrades" ,
				"Number winning trades:" );
			// 
			// lblValNumberWinningTrades
			// 
			this.addValueLabel( lblValNumberWinningTrades , "lblValNumberWinningTrades" );
			// 
			// lblAverageTradePercReturn
			// 
			this.addTextLabel( this.lblAverageTradePercReturn , "lblAverageTradePercReturn" ,
				"Average trade % return:" );
			// 
			// lblValAverageTradePercReturn
			// 
			this.addValueLabel( lblValAverageTradePercReturn , "lblValAverageTradePercReturn" );
			// 
			// lblLargestWinningTrade
			// 
			this.addTextLabel( lblLargestWinningTrade , "lblNumberWinningTrades" ,
				"Largest winning trade (%):" );
			// 
			// lblValLargestWinningTrade
			// 
			this.addValueLabel( lblValLargestWinningTrade , "lblValLargestWinningTrade" );
			// 
			// lblLargestLosingTrade
			// 
			this.addTextLabel( lblLargestLosingTrade , "lblNumberWinningTrades" ,
				"Largest losing trade:" );
			// 
			// lblValLargestLosingTrade
			// 
			this.addValueLabel( lblValLargestLosingTrade , "lblValLargestLosingTrade" );
			// 
			// lblTotalNumberOfLongTrades
			// 
			this.addTextLabel( lblTotalNumberOfLongTrades , "lblTotalNumberOfLongTrades" ,
				"Total # of long trades:" );
			// 
			// lblValTotalNumberOfLongTrades
			// 
			this.addValueLabel( lblValTotalNumberOfLongTrades , "lblValTotalNumberOfLongTrades" );
			// 
			// lblNumberWinningLongTrades
			// 
			this.addTextLabel( lblNumberWinningLongTrades , "lblNumberWinningLongTrades" ,
				"Number winning long trades:" );
			// 
			// lblValNumberWinningLongTrades
			// 
			this.addValueLabel( lblValNumberWinningLongTrades , "lblValNumberWinningLongTrades" );
			// 
			// lblAverageLongTradePercReturn
			// 
			this.addTextLabel( lblAverageLongTradePercReturn , "lblAverageLongTradePercReturn" ,
				"Average long trade % return:" );
			// 
			// lblValAverageLongTradePercReturn
			// 
			this.addValueLabel( lblValAverageLongTradePercReturn , "lblValAverageLongTradePercReturn" );
			// 
			// lblTotalNumberOfShortTrades
			// 
			this.addTextLabel( lblTotalNumberOfShortTrades , "lblTotalNumberOfShortTrades" ,
				"Total numbero of short trades:" );
			// 
			// lblValTotalNumberOfShortTrades
			// 
			this.addValueLabel( lblValTotalNumberOfShortTrades , "lblValTotalNumberOfShortTrades" );
			// 
			// lblNumberWinningShortTrades
			// 
			this.addTextLabel( lblNumberWinningShortTrades , "lblNumberWinningShortTrades" ,
				"Number winning short trades:" );
			// 
			// lblValNumberWinningShortTrades
			// 
			this.addValueLabel( lblValNumberWinningShortTrades , "lblValNumberWinningShortTrades" );
		}
	
		private void myInitializeComponent()
		{
			this.summaryItems = new ArrayList();
			myInitializeComponent_addSummaryRows();
			foreach( SummaryItem summaryItem in this.summaryItems )
				this.myInitializeComponent_addItem( summaryItem );
		}
	
		private void setSummaryValues()
		{
			this.lblValTotalNetProfit.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalPnl );
			this.lblValReturnOnAccount.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.ReturnOnAccount.Value );
			this.lblValBenchmarkPercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.BenchmarkPercentageReturn.Value );
			this.lblValAnnualSystemPercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.AnnualSystemPercentageReturn.Value );
//			this.lblValMaxEquityDrawDown.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.MaxEquityDrawDown );
			this.lblValTotalCommission.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.TotalCommissionAmount.Value );
			this.lblValNumberWinningPeriods.Text =
				this.accountReport.Summary.NumberWinningPeriods.ToString();
			this.lblValNumberLosingPeriods.Text =
				this.accountReport.Summary.NumberLosingPeriods.ToString();
			this.lblValPercentageWinningPeriods.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.PercentageWinningPeriods.Value );
//			this.lblValTotalNumberOfTrades.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalNumberOfTrades );
//			this.lblValNumberWinningTrades.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.NumberWinningTrades );
//			this.lblValAverageTradePercReturn.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.AverageTradePercentageReturn );
//			this.lblValLargestWinningTrade.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.LargestWinningTradePercentage.Value );
//			this.lblValLargestLosingTrade.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.LargestLosingTradePercentage.Value );
//			this.lblValTotalNumberOfLongTrades.Text =
//				FormatProvider.ConvertToStringWithZeroDecimals( (int)this.accountReport.Summary.TotalNumberOfLongTrades.Value );
//			this.lblValNumberWinningLongTrades.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.NumberWinningLongTrades.Value );
//			this.lblValAverageLongTradePercReturn.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.AverageLongTradePercentageReturn.Value );
//			this.lblValTotalNumberOfShortTrades.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.TotalNumberOfShortTrades.Value );
//			this.lblValNumberWinningShortTrades.Text =
//				FormatProvider.ConvertToStringWithTwoDecimals( (double)this.accountReport.Summary.NumberWinningShortTrades.Value );
		}
		public SummaryTabPage( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.summaryRow = new SummaryRow();
			this.myInitializeComponent();
			this.Text = "Summary";
//			this.setSummaryValues();
		}
	}
}
