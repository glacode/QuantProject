using System;

using QuantProject.DataAccess;

namespace QuantProject.Data
{
	/// <summary>
	/// Extended DataTable
	/// </summary>
	public class ExtendedDataTable : QuantProject.ADT.ExtendedDataTable
	{
		private SingleTableDbDataAdapter oleDbSingleTableAdapter;

		public ExtendedDataTable( string sql )
		{
			this.oleDbSingleTableAdapter = new SingleTableDbDataAdapter( sql , this );
		}
		public void Update()
		{
			this.oleDbSingleTableAdapter.DbDataAdapter.Update( this );
		}
	}
}
