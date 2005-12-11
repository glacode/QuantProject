/*
QuantProject - Quantitative Finance Library

WFMultiOneRankRunInSampleDebug.cs
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
using System.Windows.Forms;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Class used to debug in sample results, starting from saved
	/// transactions
	/// </summary>
	public class WFMultiOneRankRunInSampleDebug
	{
		public WFMultiOneRankRunInSampleDebug()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public void Run()
		{
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			TransactionHistory transactionHistory =
				( TransactionHistory )visualObjectArchiver.Load(
				"Load transactions" , "qPt" , "Load transactions" );
			DateTime lastDateTime =
				((DateTime)transactionHistory.GetKey(
				transactionHistory.Count - 1 ) );
			RebuildableAccount account =
				new RebuildableAccount( "FromSerializedTransactions" );
			account.Add( transactionHistory );
			Report report = new Report( account ,
				new HistoricalAdjustedQuoteProvider() );
			report.Create( "WFT One Rank" , 1 ,
				new EndOfDayDateTime(
				lastDateTime ,
				EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
				"^SPX" );
			WFMultiOneRankReportDebugger wFMultiOneRankReportDebugger =
				new WFMultiOneRankReportDebugger( 6 ,
				120 , "^SPX" );
			report.TransactionGrid.MouseUp +=
				new MouseEventHandler(
				wFMultiOneRankReportDebugger.MouseClickEventHandler );
			report.Show();
		}
	}
}
