using System;

using QuantProject.DataAccess;

namespace QuantProject.Data
{
	/// <summary>
	/// Extended DataTable
	/// </summary>
	public class ExtendedDataTable : QuantProject.ADT.ExtendedDataTable
	{
		private OleDbSingleTableAdapter oleDbSingleTableAdapter;

		public ExtendedDataTable( string sql )
		{
			this.oleDbSingleTableAdapter = new OleDbSingleTableAdapter( sql , this );
		}
		public void Update()
		{
			this.oleDbSingleTableAdapter.OleDbDataAdapter.Update( this );
		}
	}
}
