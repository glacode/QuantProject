using System;
using System.Data;

namespace QuantProject.ADT
{
	/// <summary>
	/// Extended DataTable
	/// </summary>
	public class ExtendedDataTable : DataTable
	{
		public ExtendedDataTable()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    
    /// <summary>
    /// Sort the given DataTable by the specified field, in a DESC mode
    /// </summary>
    
    public static void Sort(DataTable tableToSort, string sortingFieldName)
    {
      DataTable copyOfTableToSort = tableToSort.Copy();
      DataRow[] orderedRows = copyOfTableToSort.Select("", sortingFieldName + " DESC");
      int numRows = tableToSort.Rows.Count;
      int numColumns = tableToSort.Columns.Count;
      object[] valuesToAdd = new object[numColumns];
      tableToSort.Rows.Clear();
      for(int i = 0;i<numRows;i++)
      {
        for(int j = 0;j<numColumns;j++)
        {
          valuesToAdd[j]=orderedRows[i][j];
        }
        tableToSort.Rows.Add(valuesToAdd);
      }
      tableToSort.AcceptChanges();
      
    }
    public static void DeleteRows(DataTable tableWithRowsToDelete, long indexOfRowFromWhichDeletionHasToBeDone)
    {
      for(long i = indexOfRowFromWhichDeletionHasToBeDone;i<tableWithRowsToDelete.Rows.Count;i++)
      {
        tableWithRowsToDelete.Rows.RemoveAt((int)i);
      }
      tableWithRowsToDelete.AcceptChanges();
    }

	}
}
