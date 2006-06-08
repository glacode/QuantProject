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
		private int labelRows = 14;
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
		private void myInitializeComponent()
		{
			this.summaryItems = new ArrayList();
			myInitializeComponent_addSummaryRows();
			foreach( SummaryItem summaryItem in this.summaryItems )
				this.myInitializeComponent_addItem( summaryItem );
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
