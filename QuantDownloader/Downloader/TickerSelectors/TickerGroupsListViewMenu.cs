/*
QuantDownloader - Quantitative Finance Library

TickerGroupsListViewMenu.cs
Copyright (C) 2003 
Marco Milletti

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
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;

namespace QuantProject.Applications.Downloader.TickerSelectors
{
	/// <summary>
	/// It is the context menu used by the list view of the ticker groups viewer
	/// </summary>
	public class TickerGroupsListViewMenu : TickerViewerMenu  
	{
    private MenuItem menuItemPaste = new MenuItem("&Paste copied tickers");
    private MenuItem menuItemRemoveSelectedItems = new MenuItem("&Remove selected items");
    
    public TickerGroupsListViewMenu(Form ITickerSelectorForm) : base(ITickerSelectorForm)
    {
      this.menuItemPaste.Click += new System.EventHandler(this.paste);
      this.menuItemRemoveSelectedItems.Click += new System.EventHandler(this.removeSelectedItems);

      this.MenuItems.Add(this.menuItemPaste);
      this.MenuItems.Add(this.menuItemRemoveSelectedItems);
    }
    
    private void paste(object sender, System.EventArgs e)
    {
      ITickerReceiver iTickerReceiver = (ITickerReceiver)this.parentForm;
      iTickerReceiver.ReceiveTickers(TickerDataTable.Clipboard);
    }
    private void removeSelectedItems(object sender, System.EventArgs e)
    {
      //;
    }
    


  } 
}
