/*
QuantProject - Quantitative Finance Library

CloseToCloseChart.cs
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
using scpl;
using scpl.Windows;
using QuantProject.ADT.Histories;
using QuantProject.Data;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Displays the charts for the quotes within the interval around the quotes
	/// with suspicious Close To Close ratio
	/// </summary>
	public class CloseToCloseChart : Chart
	{
    private string ticker;
    private DateTime suspiciousDateTime;
    private History history;
    private DateTime startDateTime;
    private DateTime endDateTime;

    public string Ticker
    {
      get { return this.ticker; }
      set { this.ticker = value; }
    }
    public DateTime SuspiciousDateTime
    {
      get { return this.suspiciousDateTime; }
      set { this.suspiciousDateTime = value; }
    }
    public CloseToCloseChart()
		{
			InitializeComponent();
		}

    private void InitializeComponent()
    {
      this.Name = "CloseToCloseChart";
    }

    #region "OnPaint"
    protected void onPaint_setTimeInterval()
    {
      this.startDateTime = (DateTime) this.history.GetKey( Math.Max( 0 ,
        this.history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) - 20 ) );
      this.endDateTime = (DateTime) this.history.GetKey( Math.Min( this.history.Count - 1 ,
        this.history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) ) + 20 );
    }
    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
      Console.WriteLine( "CloseToCloseChart.OnPaint()" );
      this.Clear();
      this.history = DataProvider.GetCloseHistory( ((QuotesEditor)this.FindForm()).Ticker );
      this.onPaint_setTimeInterval();
      this.Add( this.history , Color.Red , this.startDateTime , this.endDateTime );
      base.OnPaint( e );
    }
    #endregion
  }
}
