/*
QuantProject - Quantitative Finance Library

OHLCdataGrid.cs
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
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Contains the quotes with Open High Low Close inconsistency
	/// </summary>
	public class OHLCdataGrid : ValidationDataGrid
	{
		public OHLCdataGrid()
		{
//      this.Dock = DockStyle.Fill;
			this.Anchor = AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
		}
    public override void DataBind()
    {
      ValidateDataTable validateDataTable = ((QuotesEditor)this.FindForm()).ValidateDataTable;
      DataView dataView = new DataView( validateDataTable );
      dataView.RowFilter = "ValidationWarning=" +
        Convert.ToInt16( ValidationWarning.OpenHighLowCloseLogicalInconsistency );
      this.DataSource = dataView;
      //      foreach ( DataRow dataRow in dataRows )
      //        this.openHighLowCloseDataTable.Rows.Add( dataRow );
    }
	}
}
