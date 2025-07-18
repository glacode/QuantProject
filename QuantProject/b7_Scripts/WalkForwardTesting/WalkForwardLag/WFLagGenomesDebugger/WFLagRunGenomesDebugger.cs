/*
QuantProject - Quantitative Finance Library

WFLagRunGenomesDebugger.cs
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
using System.Windows.Forms;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Script to debug positions chosen in a previous backtest
	/// and stored in a WFLagLog
	/// </summary>
	public class WFLagRunGenomesDebugger
	{
		private WFLagLog wFLagLog;
		private ICollection chosenPositionsCollection;
		private ArrayList chosenPositionsDebugInfoList;

		public WFLagRunGenomesDebugger()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region computeList
		private void computeList_setChosenPositionsCollection()
		{
			this.chosenPositionsCollection =
				this.wFLagLog.ChosenPositionsHistory.Values;
    }
		private void computeList_setChosenPositionsDebugInfoList_addDebugInfo(
			WFLagLogItem wFLagLogItem )
		{
			WFLagChosenPositionsDebugInfo wFLagChosenPositionsDebugInfo =
				new WFLagChosenPositionsDebugInfo(
					wFLagLogItem.WFLagWeightedPositions ,
					wFLagLogItem.LastOptimizationDate ,
					wFLagLogItem.Generation ,
					this.wFLagLog );
			this.chosenPositionsDebugInfoList.Add( wFLagChosenPositionsDebugInfo );
		}

		private void computeList_setChosenPositionsDebugInfoList()
		{
			this.chosenPositionsDebugInfoList = new ArrayList();
			foreach ( WFLagLogItem wFLagLogItem in
				this.chosenPositionsCollection )
				this.computeList_setChosenPositionsDebugInfoList_addDebugInfo(
					wFLagLogItem );
		}
		private void computeList()
		{
			this.computeList_setChosenPositionsCollection();
			this.computeList_setChosenPositionsDebugInfoList();
		}
		#endregion
		private void showList()
		{
			WFLagDebugChosenPositionsCollection wFLagDebugChosenPositionsCollection =
				new WFLagDebugChosenPositionsCollection( this.wFLagLog.InSampleDays ,
				this.wFLagLog.Benchmark , this.chosenPositionsDebugInfoList );
			wFLagDebugChosenPositionsCollection.Show();
		}
		public void Run( WFLagLog wFLagLog )
		{
			this.wFLagLog = wFLagLog;
			this.computeList();
			this.showList();
		}
	}
}
