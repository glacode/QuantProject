using System;

namespace QuantProject.Data
{
	/// <summary>
	/// Filters to be used within the DataTables namespace
	/// </summary>
	public class FilterBuilder
	{
		public FilterBuilder()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		internal static string GetDateConstant( DateTime dateTime )
		{
			string getDateConstant;
			getDateConstant = "#" + dateTime.Month + "/" + dateTime.Day + "/" +
				dateTime.Year + "#";
			return getDateConstant;
		}
	}
}
