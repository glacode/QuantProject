/*
QuantProject - Quantitative Finance Library

WFLagLog.cs
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
using System.Reflection;
using System.Runtime.Serialization;

using QuantProject.ADT.Histories;
using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Log for a walk forward lag backtest
	/// </summary>
	[Serializable]
	public class WFLagLog : ISerializable
	{
		private int inSampleDays;
		private string benchmark;

		private TransactionHistory transactionHistory;
		private History logItemsHistory;
	
		public int InSampleDays
		{
			get { return this.inSampleDays; }
		}
		public string Benchmark
		{
			get { return this.benchmark; }
		}

		public TransactionHistory TransactionHistory
		{
			get { return this.transactionHistory; }
			set { this.transactionHistory = value; }
		}

		public History ChosenPositionsHistory
		{
			get { return this.logItemsHistory; }
		}
		public WFLagLog( int inSampleDays , string benchmark )
		{
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.logItemsHistory = new History();
		}
		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WFLagLog( SerializationInfo info , StreamingContext context )
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
				fieldInfo.SetValue( this ,
					info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
			}

		}
		#region GetObjectData
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
		#endregion

		public void Add( WFLagLogItem wFLagLogItem )
		{
			this.logItemsHistory.Add(
				wFLagLogItem.LastOptimizationDate ,
				wFLagLogItem );
		}
		public WFLagLogItem GetLogItem(
			DateTime transactionDateTime )
		{
			DateTime maxDateTimeForSettingRequestedChosenPosition =
				transactionDateTime.AddDays( - 1 );
			WFLagLogItem wFLagLogItem =
				(WFLagLogItem)this.logItemsHistory.GetByKeyOrPrevious(
				maxDateTimeForSettingRequestedChosenPosition );
			return wFLagLogItem;
		}
	}
}
