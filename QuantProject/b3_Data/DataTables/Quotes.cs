using System;
using System.Data;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for quotes table data
	/// </summary>
	public class Quotes : DataTable
	{
		private void fillDataTable( string ticker , DateTime startDate , DateTime endDate )
		{
			QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
				ticker , startDate , endDate , this );
		}
		public Quotes( string ticker , DateTime startDate , DateTime endDate )
		{
			this.fillDataTable( ticker , startDate , endDate );
		}
		public Quotes( string ticker )
		{
			this.fillDataTable( 
				ticker ,
				QuantProject.DataAccess.Tables.Quotes.GetStartDate( ticker ) ,
				QuantProject.DataAccess.Tables.Quotes.GetEndDate( ticker ) );
		}
	}
}
