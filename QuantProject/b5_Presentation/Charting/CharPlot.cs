/*
QuantProject - Quantitative Finance Library

DataBase.cs
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
using QuantProject.ADT.Histories;

namespace QuantProject.Presentation.Charting
{
	/// <summary>
	/// Single plot to be charted by the Chart class
	/// </summary>
	public class ChartPlot
	{
    private History history;
    private Color color;
    private DateTime startDateTime;
    private DateTime endDateTime;

    public Color Color
    {
      get { return this.color; }
      set { this.color = value; }
    }

    public DateTime StartDateTime
    {
      get { return this.startDateTime; }
      set { this.startDateTime = value; }
    }

    public DateTime EndDateTime
    {
      get { return this.endDateTime; }
      set { this.endDateTime = value; }
    }

    public History History
    {
      get { return this.history; }
      set { this.history = value; }
    }


		private void chartPlot( History history , Color color ,
			DateTime startDateTime , DateTime endDateTime )
		{
			this.history = history;
			this.color = color;
			this.startDateTime = startDateTime;
			this.endDateTime = endDateTime;
		}
		public ChartPlot( History history , Color color )
		{
			this.chartPlot( history , color ,
				(DateTime)history.GetKey( 0 ) , (DateTime)history.GetKey( history.Count - 1 ) );
		}

//		public ChartPlot( History history )
//		{
//			ChartPlot( history , Color.Red );
//		}

		public ChartPlot( History history , Color color , DateTime startDateTime , DateTime endDateTime )
    {
			this.chartPlot( history , color , startDateTime , endDateTime );
    }
  }
}
