using System;
using System.Data;
using System.Windows.Forms;


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
    public static void Sort(DataTable tableToSort, string sortingFieldName, bool sortByASC)
    {
      DataTable copyOfTableToSort = tableToSort.Copy();
      string sortDirection = " DESC";
      if(sortByASC)
        sortDirection = " ASC";
      DataRow[] orderedRows = copyOfTableToSort.Select("", sortingFieldName + sortDirection);
      tableToSort.Rows.Clear();
      for(int i = 0;i<orderedRows.Length;i++)
      {
        tableToSort.ImportRow(orderedRows[i]);
      }
    }
    
    /// <summary>
    /// Copy the given DataTable into another DataTable, sorting by the specified field, in a DESC mode
    /// </summary>
    public static DataTable CopyAndSort(DataTable tableToCopyAndSort, string sortingFieldName, bool sortByASC)
    {
      DataTable copyOfTableToCopyAndSort = tableToCopyAndSort.Clone();
      string sortDirection = " DESC";
      if(sortByASC)
         sortDirection = " ASC";
      DataRow[] orderedRows = tableToCopyAndSort.Select("", sortingFieldName + sortDirection);
      for(int i = 0;i<orderedRows.Length;i++)
      {
        copyOfTableToCopyAndSort.ImportRow(orderedRows[i]);
      }
      return copyOfTableToCopyAndSort;
    }
    
    public static void DeleteRows(DataTable table, long fromIndex)
    {
      for(long i = table.Rows.Count - 1;i>=fromIndex; i=table.Rows.Count-1)
      {
        table.Rows.RemoveAt((int)i);
      }
    }
    
    /// <summary>
    /// Get an array of float corresponding to a column compatible with the float type in a given data table
    /// </summary>
    public static float[] GetArrayOfFloatFromColumn(DataTable table,
                                                      string columnName)
    {
      int numRows = table.Rows.Count;
      float[] arrayOfFloat = new float[numRows];
      int index = 0;
      try
      {
        for(; index!= numRows; index++)
        {
          arrayOfFloat[index] = (float) table.Rows[index][columnName];
        }

      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
        index = numRows;
      }
      return arrayOfFloat;
      
    }
    
	}
}
