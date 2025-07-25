/*
QuantDownloader - Quantitative Finance Library

TickerViewerMenu.cs
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
using QuantProject.Applications.Downloader.OpenTickDownloader.UserForms;


namespace QuantProject.Applications.Downloader.TickerSelectors
{
	/// <summary>
	/// The Context Menu used by TickerViewer
	/// It is the base class from which it is possible to derive
	/// other context menus used by any Form implementing ITickerSelector
	/// </summary>
	public class TickerViewerMenu : ContextMenu  
	{
    // the form which contains the context Menu
    protected Form parentForm;
    private MenuItem menuItemSelectAll = new MenuItem("&Select all items");
    private MenuItem menuItemDownloadFromYahoo = new MenuItem("&Download selection - Yahoo");
    private MenuItem menuItemDownloadFromOpenTick = new MenuItem("&Download selection - OpenTick");
    private MenuItem menuItemValidate = new MenuItem("&Validate selection");
    private MenuItem menuItemCopy = new MenuItem("&Copy selection");
    private MenuItem menuItemComputeCloseToCloseRatios = 
                                    new MenuItem("C&ompute Close to Close ratios");
    private MenuItem menuItemQuotesEditor = new MenuItem("&Open Quotes Editor");
    private MenuItem menuItemTickerSelectorForm = new MenuItem("&Open Ticker Selector");
    
    public TickerViewerMenu(Form ITickerSelectorForm) 
    {
      this.parentForm = ITickerSelectorForm;
      //this.parentForm.ContextMenu = this;
      this.menuItemSelectAll.Click += new System.EventHandler(this.selectAllTickers);
      this.menuItemDownloadFromYahoo.Click += new System.EventHandler(this.downloadSelectedTickersFromYahoo);
      this.menuItemDownloadFromOpenTick.Click += new System.EventHandler(this.downloadSelectedTickersFromOpenTick);
      this.menuItemValidate.Click += new System.EventHandler(this.validateSelectedTickers);
      this.menuItemCopy.Click += new System.EventHandler(this.copySelectedTickers);
      this.menuItemQuotesEditor.Click += new System.EventHandler(this.openQuotesEditor);
      this.menuItemComputeCloseToCloseRatios.Click += new System.EventHandler(this.computeCloseToCloseRatios);
      this.menuItemTickerSelectorForm.Click += new System.EventHandler(this.openTickerSelectorForm);

      this.MenuItems.Add(this.menuItemSelectAll);
      this.MenuItems.Add(this.menuItemDownloadFromYahoo);
      this.MenuItems.Add(this.menuItemDownloadFromOpenTick);
      this.MenuItems.Add(this.menuItemValidate);
      this.MenuItems.Add(this.menuItemCopy);
      this.MenuItems.Add(this.menuItemQuotesEditor);
      this.MenuItems.Add(this.menuItemTickerSelectorForm);
      this.MenuItems.Add(this.menuItemComputeCloseToCloseRatios);
      
    }
    
    private void selectAllTickers(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      iTickerSelector.SelectAllTickers();
    }
    private void copySelectedTickers(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      TickerDataTable.Clipboard = iTickerSelector.GetTableOfSelectedTickers();
    }
    
    private void downloadSelectedTickersFromYahoo(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();
      
      if(tableOfSelectedTickers.Rows.Count == 0)
      {
        this.displayMessageNoTickersSelected();
        return;
      }
      WebDownloader webDownloader = new WebDownloader(tableOfSelectedTickers);
      webDownloader.Show();
    }
    private void downloadSelectedTickersFromOpenTick(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();
      
      if(tableOfSelectedTickers.Rows.Count == 0)
      {
        this.displayMessageNoTickersSelected();
        return;
      }
      OTWebDownloader webDownloader = new OTWebDownloader(tableOfSelectedTickers);
      webDownloader.Show();
    }
    private void validateSelectedTickers(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();      
      if(tableOfSelectedTickers.Rows.Count == 0)
      {
        this.displayMessageNoTickersSelected();
        return;
      }
      if(tableOfSelectedTickers.Rows.Count != 1)
      {
        this.displayMessageTooManyTickersSelected();
        return;
      }
      QuantProject.Applications.Downloader.Validate.ValidateForm validateForm = 
        new Validate.ValidateForm(tableOfSelectedTickers);
      validateForm.Show();
    }
    private void openQuotesEditor(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();      
      
      if(tableOfSelectedTickers.Rows.Count != 1)
      {
        this.displayMessageTooManyTickersSelected();
        return;
      }
      QuotesEditor quotesEditor = 
        new QuotesEditor((string)tableOfSelectedTickers.Rows[0][0]);
      quotesEditor.Show();
    }

    private void computeCloseToCloseRatios(object sender, System.EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();
      string currentTicker;
      //user confirmation
      if(tableOfSelectedTickers.Rows.Count > 5)
      { 
        if(DialogResult.No == MessageBox.Show( "This operation may be very long: \n" + 
                                            "Do you want to continue?", "Confirmation",
                                            MessageBoxButtons.YesNo))
        { 
          Cursor.Current = Cursors.Default;
          return;
        }
      }
      //ok
      foreach(DataRow row in tableOfSelectedTickers.Rows)
      {
        currentTicker = (string)row[Tickers.Ticker];
        QuantProject.DataAccess.Tables.Quotes.ComputeAndCommitCloseToCloseRatios(currentTicker);
      }
      Cursor.Current = Cursors.Default; 


    }
    private void openTickerSelectorForm(object sender, System.EventArgs e)
    {
      ITickerSelector iTickerSelector = (ITickerSelector)this.parentForm;
      DataTable tableOfSelectedTickers = iTickerSelector.GetTableOfSelectedTickers();      
      TickerSelectorForm selectorForm = new TickerSelectorForm(tableOfSelectedTickers);
      selectorForm.Show();
    }
    
    private void displayMessageNoTickersSelected()
    {
      MessageBox.Show("No ticker has been selected!\n\n" + 
        "Click on the grey area on the left to " +
        "select a ticker", "Error message",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    
    private void displayMessageTooManyTickersSelected()
    {
      MessageBox.Show("Choose just one ticker for this selection!\n\n" + 
        "Click on the grey area on the left to " +
        "select only one ticker", "Error message",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  } 
}
