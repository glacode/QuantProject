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
using System.Drawing;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Presentation;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// TabPage to show the summary within the report form
	/// </summary>
	public class SummaryTabPage : TabPage
	{
//		private int currentLabel;
		private int xForLabels = 17;
		private int labelsWidth = 150;
		private int yStart = 17;
		private int yStep = 25;

		private System.Windows.Forms.Label lblValTotalNetProfit;
		private System.Windows.Forms.Label lblTotalNetProfit;
		private System.Windows.Forms.Label lblReturnOnAccount;
		private System.Windows.Forms.Label lblValReturnOnAccount;
		private System.Windows.Forms.Label lblAnnualSystemPercReturn;
		private System.Windows.Forms.Label lblValAnnualSystemPercReturn;
		private System.Windows.Forms.Label lblMaxEquityDrawDown;
		private System.Windows.Forms.Label lblValMaxEquityDrawDown;
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
		private System.Windows.Forms.Label lblBuyAndHoldPercReturn;
		private System.Windows.Forms.Label lblValBuyAndHoldPercReturn;
		private System.Windows.Forms.Label lblNumberWinningLongTrades;
		private System.Windows.Forms.Label lblValNumberWinningLongTrades;
		private System.Windows.Forms.Label lblTotalNumberOfShortTrades;
		private System.Windows.Forms.Label lblValTotalNumberOfShortTrades;

		private AccountReport accountReport;

		private Point getPointForTextLabel( int labelPosition )
		{
			return new Point( xForLabels , yStart + labelPosition * yStep );
		}
		private void addTextLabel( Label label , string name , string text )
		{
			this.Controls.Add( label );
//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForTextLabel( ( this.Controls.Count - 1 ) / 2 );
			label.Width = this.labelsWidth;
			label.Name = name;
			label.Text = text;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		}
		private void addValueLabel( Label label , string name )
		{
			this.Controls.Add( label );
			//			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.Location = getPointForValueLabel( ( this.Controls.Count / 2 ) - 1 );
			label.Name = name;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		}

		private Point getPointForValueLabel( int labelPosition )
		{
			return new Point( xForLabels + labelsWidth + 3 ,
				yStart + labelPosition * yStep );
		}
		private void myInitializeComponent()
		{
			this.lblTotalNetProfit = new System.Windows.Forms.Label();
			this.lblValTotalNetProfit = new System.Windows.Forms.Label();
			this.lblReturnOnAccount = new System.Windows.Forms.Label();
			this.lblValReturnOnAccount = new System.Windows.Forms.Label();
			this.lblBuyAndHoldPercReturn = new System.Windows.Forms.Label();
			this.lblValBuyAndHoldPercReturn = new System.Windows.Forms.Label();
			this.lblAnnualSystemPercReturn = new System.Windows.Forms.Label();
			this.lblValAnnualSystemPercReturn = new System.Windows.Forms.Label();
			this.lblMaxEquityDrawDown = new System.Windows.Forms.Label();
			this.lblValMaxEquityDrawDown = new System.Windows.Forms.Label();
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
				"Return on account:" );
			// 
			// lblValReturnOnAccount
			// 
			this.addValueLabel( lblValReturnOnAccount , "lblValReturnOnAccount" );
			// 
			// lblBuyAndHoldPercReturn
			// 
			this.addTextLabel( lblBuyAndHoldPercReturn , "lblBuyAndHoldPercReturn" ,
				"Buy & hold % return:" );
			// 
			// lblValBuyAndHoldPercReturn
			// 
			this.addValueLabel( lblValBuyAndHoldPercReturn , "lblValBuyAndHoldPercReturn" );
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
				"Max equity drawydown (%):" );
			// 
			// lblValMaxEquityDrawDown
			// 
			this.addValueLabel( lblValMaxEquityDrawDown , "lblValMaxEquityDrawDown" );
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
			this.addTextLabel( lblNumberWinningTrades , "lblNumberWinningTrades" ,
				"Average trade % return:" );
			// 
			// lblValAverageTradePercReturn
			// 
			this.addValueLabel( lblValAverageTradePercReturn , "lblValAverageTradePercReturn" );
			// 
			// lblLargestWinningTrade
			// 
			this.addTextLabel( lblNumberWinningTrades , "lblNumberWinningTrades" ,
				"Largest winning trade:" );
			// 
			// lblValLargestWinningTrade
			// 
			this.addValueLabel( lblValLargestWinningTrade , "lblValLargestWinningTrade" );
			// 
			// lblLargestLosingTrade
			// 
			this.addTextLabel( lblNumberWinningTrades , "lblNumberWinningTrades" ,
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
	
		private void addControls()
		{
//			this.Controls.Add( this.lblValTotalNetProfit );
//			this.Controls.Add( this.lblReturnOnAccount );
//			this.Controls.Add( this.lblValReturnOnAccount );
			this.Controls.Add( this.lblBuyAndHoldPercReturn );
			this.Controls.Add( this.lblValBuyAndHoldPercReturn );
			this.Controls.Add( this.lblAnnualSystemPercReturn );
			this.Controls.Add( this.lblValAnnualSystemPercReturn );
			this.Controls.Add( this.lblMaxEquityDrawDown );
			this.Controls.Add( this.lblValMaxEquityDrawDown );
			this.Controls.Add( this.lblTotalNumberOfShortTrades );
			this.Controls.Add( this.lblValTotalNumberOfTrades );
			this.Controls.Add( this.lblAverageTradePercReturn );
			this.Controls.Add( this.lblValAverageTradePercReturn );
			this.Controls.Add( this.lblLargestWinningTrade );
			this.Controls.Add( this.lblValLargestWinningTrade );
			this.Controls.Add( this.lblLargestLosingTrade );
			this.Controls.Add( this.lblValLargestLosingTrade );
			this.Controls.Add( this.lblTotalNumberOfLongTrades );
			this.Controls.Add( this.lblValTotalNumberOfLongTrades );
			this.Controls.Add( this.lblNumberWinningLongTrades );
			this.Controls.Add( this.lblValNumberWinningLongTrades );
			this.Controls.Add( this.lblAverageLongTradePercReturn );
			this.Controls.Add( this.lblValAverageLongTradePercReturn );
			this.Controls.Add( this.lblTotalNumberOfShortTrades );
			this.Controls.Add( this.lblValTotalNumberOfTrades );
			this.Controls.Add( this.lblNumberWinningShortTrades );
			this.Controls.Add( this.lblValNumberWinningShortTrades );
		}
		private void setSummaryValues()
		{
			this.lblValTotalNetProfit.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalPnl );
			this.lblValReturnOnAccount.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.ReturnOnAccount );
			this.lblValBuyAndHoldPercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.BuyAndHoldPercentageReturn );
			this.lblValAnnualSystemPercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.AnnualSystemPercentageReturn );
			this.lblValMaxEquityDrawDown.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.MaxEquityDrawDown );
			this.lblValTotalNumberOfTrades.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalNumberOfTrades );
			this.lblValAverageTradePercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.AverageTradePercentageReturn );
			this.lblValLargestWinningTrade.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.LargestWinningTradePercentage );
			this.lblValLargestLosingTrade.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.LargestLosingTradePercentage );
			this.lblValTotalNumberOfLongTrades.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalNumberOfLongTrades );
			this.lblValNumberWinningLongTrades.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.NumberWinningLongTrades );
			this.lblValAverageLongTradePercReturn.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.AverageLongTradePercentageReturn );
			this.lblValTotalNumberOfShortTrades.Text =
				FormatProvider.ConvertToStringWithTwoDecimals( this.accountReport.Summary.TotalNumberOfShortTrades );
		}
		public SummaryTabPage( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.myInitializeComponent();
			this.addControls();
			this.Text = "Summary";
			this.setSummaryValues();
		}
	}
}
