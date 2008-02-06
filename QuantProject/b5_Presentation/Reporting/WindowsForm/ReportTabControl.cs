/*
QuantProject - Quantitative Finance Library

ReportTabControl.cs
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
using System.Runtime.Serialization;
using System.Reflection;
using System.Windows.Forms;

using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Presentation.Charting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// TabControl for the report form
	/// </summary>
	[Serializable]
	public class ReportTabControl : TabControl, ISerializable
	{
		private AccountReport accountReport;
		private EquityChartTabPage equityChart;
		private SummaryTabPage summary;
		private ReportGridTabPage roundTrades;
		private ReportGridTabPage equity;
		private ReportGridTabPage transactions;
		private StatisticsSummaryTabPage statisticsSummary;

		public ReportGrid TransactionGrid
		{
			get { return this.transactions.ReportGrid; }
		}
		public Chart EquityChart
		{
			get { return this.equityChart.EquityChart; }
		}
		
		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ReportTabControl( SerializationInfo info , StreamingContext context ) :
							base()
		{
			// get the set of serializable members for this class and its base classes
			Type thisType = this.GetType();
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(
				thisType , context);

			// deserialize the fields from the info object
			for (Int32 i = 0 ; i < mi.Length; i++) 
			{
				FieldInfo fieldInfo = (FieldInfo) mi[i];

				// set the field to the deserialized value
				try
				{
					fieldInfo.SetValue( this ,
						info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
				}
				catch(Exception ex)
				{ex = ex;}
			}
		}

		/// <summary>
		/// serialize the set of serializable members for this class and base classes
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			// get the set of serializable members for this class and base classes
			Type thisType = this.GetType();
			MemberInfo[] mi = 
				FormatterServices.GetSerializableMembers( thisType , context);

			// serialize the fields to the info object
			for (Int32 i = 0 ; i < mi.Length; i++) 
			{
				info.AddValue(mi[i].Name, ((FieldInfo) mi[i]).GetValue(this));
			}
		}

		/// <summary>
		/// Contains all tab pages of a visual report
		/// </summary>
		/// <param name="accountReport">data for the report to be shown</param>
		/// <param name="showBenchmark">true iif the benchmark equity line
		/// is to be shown</param>
		public ReportTabControl( AccountReport accountReport ,
			bool showBenchmark )
		{
			this.accountReport = accountReport;
			this.Dock = DockStyle.Fill;
			this.equityChart = new EquityChartTabPage( this.accountReport ,
				showBenchmark );
			this.Controls.Add( this.equityChart );
			this.summary = new SummaryTabPage( this.accountReport );
			this.Controls.Add( this.summary );
			this.roundTrades = new ReportGridTabPage(
				"Round Trades" , this.accountReport.RoundTrades );
			this.Controls.Add( this.roundTrades );
			this.equity = new ReportGridTabPage(
				"Equity" , this.accountReport.Equity );
			this.Controls.Add( this.equity );
			this.transactions = new ReportGridTabPage(
				"Transactions" , this.accountReport.TransactionTable );
			this.Controls.Add( this.transactions );
			this.statisticsSummary = new StatisticsSummaryTabPage( this.accountReport );
			this.Controls.Add( this.statisticsSummary );
		}
	}
}
