/*
QuantProject - Quantitative Finance Library

QuotesChart.cs
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
using NPlot;
using NPlot.Windows;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders.Quotes;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Base class for single ticker charting
	/// </summary>
	public class QuotesChart : Chart
	{
    protected string ticker;

    public string Ticker
    {
      get { return this.ticker; }
      set
			{
				this.ticker = value;
				this.Clear();
				this.Add( HistoricalQuotesProvider.GetAdjustedCloseHistory( this.ticker ) );
			}
    }

		public QuotesChart()
		{
		}

    protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
    {
      Console.WriteLine( "QuotesChart.PaintingHandler()" );
//      this.Clear();
//      this.Add( HistoricalDataProvider.GetCloseHistory( this.ticker ) );
      base.OnPaint( e );
    }
	}
}
