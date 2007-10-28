using System;
using System.Windows.Forms;

namespace QuantProject.ADT
{
	/// <summary>
	/// Exception thrown when the key to find is less than the first object
	/// </summary>
	public class IndexOfKeyOrPreviousException : Exception
	{
		public IndexOfKeyOrPreviousException(
			IComparable firstKeyInTheAdvancedSortedList , Object key )
		{
			string message = "The given key is less than the first object " +
				"in the AdvancedSortedList";
			message = message + "";
//			MessageBox.Show( this.message );
		}
	}
}
