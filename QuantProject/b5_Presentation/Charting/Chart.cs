/*
QuantProject - Quantitative Finance Library

Chart.cs
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
using System.Data;
using System.Drawing;
//using scpl;
using NPlot;
using QuantProject.ADT.Histories;

namespace QuantProject.Presentation.Charting
{
	/// <summary>
	/// Base class for QuantProject charting capabilities. Single interface point to
	/// the scpl library. All scpl dependent code must be written within this class.
	/// </summary>
	public class Chart : NPlot.Windows.PlotSurface2D
	{
    private ArrayList chartPlots;

		public Chart()
		{
			this.chartPlots = new ArrayList();
//      this.Paint += new System.Windows.Forms.PaintEventHandler(this.Chart_Paint);
    }
    public new void Clear()
    {
      this.chartPlots.Clear();
      base.Clear();
    }
    /// <summary>
    /// Adds a new ChartPlot to the Chart, using default values for the Color,
    /// the StartDateTime and the EndDateTime
    /// </summary>
    /// <param name="history">History for the new ChartPlot</param>
		public void Add( History history )
		{
			ChartPlot chartPlot = new ChartPlot( history , Color.Red );
			this.chartPlots.Add( chartPlot );
		}
		public void Add( History history , Color color )
		{
			ChartPlot chartPlot = new ChartPlot( history , color );
			this.chartPlots.Add( chartPlot );
		}
		/// <summary>
    /// Adds a new ChartPlot to the Chart, using the given arguments for the Color,
    /// the StartDate and the EndDate
    /// </summary>
    /// <param name="history">History for the new Chart Plot</param>
    /// <param name="color">Color for the new Chart Plot</param>
    /// <param name="startDateTime">StartDateTime for the new Chart Plot</param>
    /// <param name="endDateTime">EndDateTime for the new Chart Plot</param>
    public void Add( History history , Color color , DateTime startDateTime , DateTime endDateTime )
    {
      ChartPlot chartPlot = new ChartPlot( history , color , startDateTime , endDateTime );
      this.chartPlots.Add( chartPlot );
    }
    #region "OnPaint"
		private void onPaint_addLinePlot_ok( ChartPlot chartPlot )
		{
			int npt=chartPlot.History.Count;
			int startIndex = chartPlot.History.IndexOfKeyOrPrevious( chartPlot.StartDateTime );
			int endIndex = chartPlot.History.IndexOfKeyOrPrevious( chartPlot.EndDateTime );
			int plotLength = endIndex - startIndex + 1;
			
      DataTable dataTable = new DataTable();
			dataTable.Columns.Add( "X" , DateTime.Now.GetType() );
			dataTable.Columns.Add( "Y" , System.Type.GetType( "System.Single" ) );

			for ( int i=startIndex ; i<=endIndex ; i++ )
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[ "X" ] = (DateTime)chartPlot.History.GetKey( i );
				dataRow[ "Y" ] = Convert.ToDouble( chartPlot.History.GetByIndex( i ) );
				dataTable.Rows.Add( dataRow );
			}

			LinePlot lp = new LinePlot();
			lp.DataSource = dataTable;
			lp.AbscissaData = "X";
//			lp.ValueData = "Y";
			lp.OrdinateData = "Y";

			Pen p=new Pen( chartPlot.Color );
			lp.Pen=p;

			//      base.Clear();
			this.Add(lp);
		}
		private void onPaint_addLinePlot( ChartPlot chartPlot )
		{
			int npt=chartPlot.History.Count;
			int startIndex = chartPlot.History.IndexOfKeyOrPrevious( chartPlot.StartDateTime );
			int endIndex = chartPlot.History.IndexOfKeyOrPrevious( chartPlot.EndDateTime );
			int plotLength = endIndex - startIndex + 1;
			float [] x = new float[ plotLength ];
			float [] y = new float[ plotLength ];

			float step=1.0F;
			for ( int i=startIndex ; i<=endIndex ; i++ )
			{
				x[i-startIndex]=i*step;
				y[i-startIndex]=(float)chartPlot.History.GetByIndex( i );
			}

//			LinePlot lp=new LinePlot( new ArrayAdapter(x,y) );  commentata per avere compilazione; rimuovi commenti per farlo funzionare con la vecchia scpl
			Pen p=new Pen( chartPlot.Color );
//			lp.Pen=p;

			//      base.Clear();
//			this.Add(lp);
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
      Console.WriteLine( "Chart.OnPaint()" );
      foreach ( ChartPlot chartPlot in this.chartPlots )
        onPaint_addLinePlot_ok( chartPlot );
      base.OnPaint( e );
    }
    #endregion
	}
}
