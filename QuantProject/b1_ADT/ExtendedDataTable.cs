using System;
using System.Collections;
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
      ExtendedDataTable.DeleteRows(table, fromIndex, table.Rows.Count - 1);
      //for(long i = table.Rows.Count - 1;i>=fromIndex; i=table.Rows.Count-1)
      //{
        //table.Rows.RemoveAt((int)i);
      //}
    }
    
    public static void DeleteRows(DataTable table, long fromIndex, long toIndex)
    {
      for(long i = fromIndex; i <= toIndex; i++)
      {
        table.Rows.RemoveAt((int)fromIndex);
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
    /// <summary>
    /// Get an array of float corresponding to a column compatible with the float type in a given data table,
    /// filtered by the given filterExpression
    /// </summary>
    public static float[] GetArrayOfFloatFromColumn(DataTable table,
                                                    string columnName,
                                                    string filterExpression)
    {
      DataRow[] selectedRows = table.Select(filterExpression);
      int numRows = selectedRows.Length;
      float[] arrayOfFloat = new float[numRows];
      int index = 0;
      try
      {
        for(; index!= numRows; index++)
        {
          arrayOfFloat[index] = (float)selectedRows[index][columnName];
        }

      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
        index = numRows;
      }
      return arrayOfFloat;
    }
    /// <summary>
    /// Get an array of float corresponding to the ratio between columnA and columnB
    /// </summary>
    public static float[] GetArrayOfFloatFromRatioOfColumns(DataTable table,
                                                            string columnAName, string columnBName)
    {
      int numRows = table.Rows.Count;
      float[] arrayOfFloat = new float[numRows];
      int index = 0;
      try
      {
        for(; index!= numRows; index++)
        {
          arrayOfFloat[index] = (float) table.Rows[index][columnAName] / 
                                (float) table.Rows[index][columnBName];
        }

      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
        index = numRows;
      }
      return arrayOfFloat;
      
    }
    
    /// <summary>
	  /// It returns a hashtable containing the common values in two
	  /// columns of two given Data tables (the two columns must contain unique values!)
	  /// </summary>
	  /// <param name="firstDataTable">The first table that contains the first column</param>
	  /// <param name="secondDataTable">The second table that contains the second column</param>
	  /// <param name="indexOfColumnOfFirstTable">The index of the first column in the first table</param>
    /// <param name="indexOfColumnOfSecondTable">The index of the second column in the second table</param>
    public static Hashtable GetCommonValues(DataTable firstDataTable, DataTable secondDataTable,
                                           int indexOfColumnOfFirstTable,
                                           int indexOfColumnOfSecondTable)
    {
      
      Hashtable hashTable = new Hashtable();
    
      string columnNameTable1 = firstDataTable.Columns[indexOfColumnOfFirstTable].ColumnName;
      string columnNameTable2 = firstDataTable.Columns[indexOfColumnOfSecondTable].ColumnName;
      DataRow[] orderedRowsTable1 = firstDataTable.Select(columnNameTable1 + "<>' '", columnNameTable1 + " DESC");
      DataRow[] orderedRowsTable2 = secondDataTable.Select(columnNameTable2 + "<>' '", columnNameTable2 + " DESC");
      int j = 0;
      bool found;
      for(int i=0; i != orderedRowsTable1.Length; i++)
      {
        found = false;
        for(; j != orderedRowsTable2.Length && !found; j++)
        {
          int currentIndex = j;
          object object1 = orderedRowsTable1[i][indexOfColumnOfFirstTable];
          object object2 = orderedRowsTable2[j][indexOfColumnOfSecondTable];
          if( (string)object1 == (string)object2 )
          {
            found = true;
            j = currentIndex;
            hashTable.Add(object1, object2);
          }
        }
        if( !found )
        {
          j = 0;
        }

      }
      return hashTable;

    }
    

	}
}
