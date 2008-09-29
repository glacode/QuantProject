using System;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Reporting.StatisticsSummaryRows;

namespace QuantProject.Business.Financial.Accounting.Reporting.Tables
{
	/// <summary>
	/// Summary description for StatisticsSummary.
	/// </summary>
	[Serializable]
	public class StatisticsSummary : ReportTable, ISerializable
	{
		private AccountReport accountReport;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		
		private AverageReturnOnMondayWithOpenPositions averageReturnOnMondayWithOpenPositions;
		private AverageReturnOnTuesdayWithOpenPositions averageReturnOnTuesdayWithOpenPositions;
		private AverageReturnOnWednesdayWithOpenPositions averageReturnOnWednesdayWithOpenPositions;
		private AverageReturnOnThursdayWithOpenPositions averageReturnOnThursdayWithOpenPositions;
		private AverageReturnOnFridayWithOpenPositions averageReturnOnFridayWithOpenPositions;
		private AverageReturnOnDayWithOpenPositions averageReturnOnDayWithOpenPositions;
		
		public AccountReport AccountReport
		{
			get { return accountReport; }
		}
		
		public AverageReturnOnMondayWithOpenPositions AverageReturnOnMondayWithOpenPositions
		{
			get { return this.averageReturnOnMondayWithOpenPositions; }
		}

		public AverageReturnOnTuesdayWithOpenPositions AverageReturnOnTuesdayWithOpenPositions
		{
			get { return this.averageReturnOnTuesdayWithOpenPositions; }
		}
		
		public AverageReturnOnWednesdayWithOpenPositions AverageReturnOnWednesdayWithOpenPositions
		{
			get { return this.averageReturnOnWednesdayWithOpenPositions; }
		}
		
		public AverageReturnOnThursdayWithOpenPositions AverageReturnOnThursdayWithOpenPositions
		{
			get { return this.averageReturnOnThursdayWithOpenPositions; }
		}
		
		public AverageReturnOnFridayWithOpenPositions AverageReturnOnFridayWithOpenPositions
		{
			get { return this.averageReturnOnFridayWithOpenPositions; }
		}
		
		public AverageReturnOnDayWithOpenPositions AverageReturnOnDayWithOpenPositions
		{
			get { return this.averageReturnOnDayWithOpenPositions; }
		}

		private void statisticsSummary( AccountReport accountReport )
		{
			this.accountReport = accountReport;
			this.getStatisticsSummary();
		}
		
		public StatisticsSummary( AccountReport accountReport ) :
			base( accountReport.Name + " - StatisticsSummary" )
		{
			this.statisticsSummary( accountReport );
		}
		public StatisticsSummary( AccountReport accountReport ,
		                         HistoricalMarketValueProvider historicalMarketValueProvider ) :
			base( accountReport.Name + " - StatisticsSummary" )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.statisticsSummary( accountReport );
		}

		#region Serialization

		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected StatisticsSummary( SerializationInfo info , StreamingContext context ) :
			base( "Summary" )
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
				catch (Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
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
		#endregion

		#region "getStatisticsSummary"

		private void getStatisticsSummary()
		{
			if ( this.accountReport.Equity.DataTable.Rows.Count == 0 )
				throw new Exception( "A StatisticsSummary computation has been requested, but the equity line is empty" );
			
			this.averageReturnOnMondayWithOpenPositions = new AverageReturnOnMondayWithOpenPositions(this);
			this.averageReturnOnTuesdayWithOpenPositions = new AverageReturnOnTuesdayWithOpenPositions(this);
			this.averageReturnOnWednesdayWithOpenPositions = new AverageReturnOnWednesdayWithOpenPositions(this);
			this.averageReturnOnThursdayWithOpenPositions = new AverageReturnOnThursdayWithOpenPositions(this);
			this.averageReturnOnFridayWithOpenPositions = new AverageReturnOnFridayWithOpenPositions(this);
			this.averageReturnOnDayWithOpenPositions =
				new AverageReturnOnDayWithOpenPositions(this);
		}

		#endregion

	}
}
