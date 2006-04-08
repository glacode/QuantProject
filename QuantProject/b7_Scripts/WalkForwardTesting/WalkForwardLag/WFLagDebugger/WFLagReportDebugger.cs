/*
QuantProject - Quantitative Finance Library

WFLagReportDebugger.cs
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
using System.Drawing;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Script to debug the walk forward Lag Strategy backtest
	/// </summary>
	public class WFLagReportDebugger
	{
		private WFLagLog wFLagLog;
		private string benchmark;

		public WFLagReportDebugger( WFLagLog wFLagLog ,
			string benchmark )
		{
			this.wFLagLog = wFLagLog;
			this.benchmark = benchmark;
		}
		#region MouseClickEventHandler
		private int rightClickEventHandler_getRowNumber(  object sender ,
			MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			return hitTestInfo.Row;
		}
		private DateTime getRowDateTime( DataGrid dataGrid , int rowNumber )
		{
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			DataRow dataRow = dataTable.Rows[ rowNumber ];
			DateTime dateTime = (DateTime)dataRow[ "DateTime" ];
			return dateTime;
		}
		private void rightClickEventHandler_withRowNumber( DataGrid dataGrid ,
			int rowNumber )
		{
			DateTime transactionDateTime =
				this.getRowDateTime( dataGrid , rowNumber );
			WFLagChosenPositions wFLagChosenPositions =
				this.wFLagLog.GetChosenPositions( transactionDateTime );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( wFLagChosenPositions ,
				transactionDateTime , 30 ,
				this.wFLagLog.InSampleDays , 30 ,
				this.wFLagLog.Benchmark );
			wFLagDebugPositions.Run();
			//			WFMultiOneRankDebugInSample wFMultiOneRankDebugInSample =
			//				new WFMultiOneRankDebugInSample( signedTickers , firstDateTime ,
			//				lastDateTime ,
			//				this.benchmark );
			//			wFMultiOneRankDebugInSample.Run();
		}
		private void rightClickEventHandler( object sender ,
			MouseEventArgs eventArgs )
		{
//			rightClickEventHandler_checkExceptions( sender ,
//				eventArgs );
			int rowNumber = rightClickEventHandler_getRowNumber(
				sender , eventArgs );
			rightClickEventHandler_withRowNumber(
				(DataGrid)sender , rowNumber );
			//			MessageBox.Show( dataRow[ "DateTime" ].ToString() );
		}
		public void MouseClickEventHandler( object sender ,
			MouseEventArgs eventArgs )
		{
			if ( eventArgs.Button == MouseButtons.Right )
				this.rightClickEventHandler( sender , eventArgs );
		}
		#endregion
	}
}
