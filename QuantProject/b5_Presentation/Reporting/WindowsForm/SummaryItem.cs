/*
QuantProject - Quantitative Finance Library

SummaryItem.cs
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
using System.Windows.Forms;

using QuantProject.Business.Financial.Accounting.Reporting.SummaryRows;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// A summary item: it contains both the item label and the item value
	/// </summary>
	[Serializable]
	public class SummaryItem
	{
		private Label description;
		private Label itemValue;

		public Label Description
		{
			get { return this.description; }
		}
		public Label Value
		{
			get { return this.itemValue; }
		}

		private void setPrivateMembers( string description , string itemValue )
		{
			this.description = new Label();
			this.description.Text = description;
			this.itemValue = new Label();
			this.itemValue.Text = itemValue;
		}
		public SummaryItem( string description , string itemValue )
		{
			this.setPrivateMembers( description , itemValue );
		}
		public SummaryItem( SummaryRow summaryRow )
		{
			this.setPrivateMembers( summaryRow.Description , summaryRow.FormattedValue );
		}
	}
}
