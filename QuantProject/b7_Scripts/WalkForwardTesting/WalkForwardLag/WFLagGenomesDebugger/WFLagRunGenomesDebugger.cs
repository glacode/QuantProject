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
		private ArrayList additionalInfosList;

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
		private void computeList_setAdditionalInfosList( WFLagChosenPositions wFLagChosenPositions )
		{
			
		}
		private void computeList_setAdditionalInfosList()
		{
			this.additionalInfosList = new ArrayList();
			foreach ( WFLagChosenPositions wFLagChosenPositions
									in this.chosenPositionsCollection )
				this.computeList_setAdditionalInfosList( wFLagChosenPositions );
		}
		private void computeList()
		{
			this.computeList_setChosenPositionsCollection();
			this.computeList_setAdditionalInfosList();
		}
		#endregion
		private void showList()
		{
			WFLagDebugChosenPositionsCollection wFLagDebugChosenPositionsCollection =
				new WFLagDebugChosenPositionsCollection( this.chosenPositionsCollection );
			wFLagDebugChosenPositionsCollection.ShowDialog();
		}
		public void Run( WFLagLog wFLagLog )
		{
			this.wFLagLog = wFLagLog;
			this.computeList();
			this.showList();
		}
	}
}
