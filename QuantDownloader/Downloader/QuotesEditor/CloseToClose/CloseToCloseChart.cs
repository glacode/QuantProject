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
using NPlot;
using NPlot.Windows;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders.Quotes;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Displays the charts for the quotes within the interval around the quotes
	/// with suspicious Close To Close ratio
	/// </summary>
	public class CloseToCloseChart : VisualValidationChart
	{
    public CloseToCloseChart()
		{
		}
		protected override void addHistories()
		{
			History lowHistory = HistoricalQuotesProvider.GetCloseHistory( ((QuotesEditor)this.FindForm()).Ticker );
			this.add( lowHistory , Color.Green );
		}
  }
}
