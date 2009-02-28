/*
QuantProject - Quantitative Finance Library

HistoriesViewer.cs
Copyright (C) 2009
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

using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Presentation.Charting;

namespace QuantProject.Presentation
{
	/// <summary>
	/// Form used to visualize (and compare) histories
	/// </summary>
	public partial class HistoriesViewer : Form
	{
		private Chart chart;
		
		public HistoriesViewer()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.addChart();
		}
		
		private void addChart()
		{
			this.chart = new Chart();
			this.Controls.Add( chart );
		}
		
		public void Add( History historyToBePlotted , Color color )
		{
			this.chart.Add( historyToBePlotted , color );
		}
		
//		#region buildTheForm
//
//		private void addChart()
//		{
//			Chart chart = new Chart();
//			this.Controls.Add( chart );
//		}
//
//		#region populateChart
//
//		#region addChart
//
//		#region getHistoryToBePlotted
//		private History getHistoryToBePlotted( WeightedPosition weightedPosition )
//		{
//
//		}
//		#endregion getHistoryToBePlotted
//
//		private void addHistory( WeightedPosition weightedPosition )
//		{
//			History historyToBePlotted =
//				this.getHistoryToBePlotted( weightedPosition );
//			this.addHistory( historyToBePlotted );
//		}
//		#endregion addChart
//
//		private void populateChart()
//		{
//			foreach ( WeightedPosition weightedPosition in
//			         this.pairsTradingTestingPositions.WeightedPositions )
//				this.addHistory( weightedPosition );
//		}
//		#endregion populateChart
//
//		private void buildTheForm()
//		{
//			this.addChart();
//			this.populateChart();
//		}
//		#endregion buildTheForm
	}
}
