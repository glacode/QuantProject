/*
QuantProject - Quantitative Finance Library

ExcelManager.cs
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
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Threading;
using QuantProject.Business.Financial.Accounting.Reporting;
using Excel;

namespace QuantProject.Presentation.Reporting.MicrosoftExcel
{
	/// <summary>
	/// Summary description for ExcelManager.
	/// </summary>
	public class ExcelManager
	{
    private static ArrayList reportTables = new ArrayList();

		public ExcelManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    public static void Add( ReportTable reportTable )
    {
      reportTables.Add( reportTable );
    }
    public static void Add( AccountReport accountReport )
    {
      reportTables.Add( accountReport.Summary );
      reportTables.Add( accountReport.RoundTrades );
      reportTables.Add( accountReport.Equity );
      reportTables.Add( accountReport.TransactionTable );
    }
    #region "ShowReport"
    private static void drawHeaders( ReportTable reportTable , Worksheet excelWorkSheet )
    {
      DataColumnCollection dataColumnCollection = reportTable.DataTable.Columns;
//      AccountReportRecord reportRecord = (AccountReportRecord)((ArrayList)this.GetByIndex( 0 ))[ 0 ];
//      ArrayList headers = reportRecord.GetHeaders();
      for (int index = 0 ; index<dataColumnCollection.Count ; index++ )
      {
        excelWorkSheet.Cells[1,index+1] = dataColumnCollection[ index ].ColumnName;
      }
    }
    private static void drawRow( DataRowCollection dataRowCollection ,
      int rowIndex , Worksheet excelWorkSheet )
    {
      DataRow dataRow = dataRowCollection[ rowIndex ];
      //  ArrayList dataRow = reportRecord.GetData();
      for (int columnIndex = 0 ; columnIndex<dataRow.Table.Columns.Count ; columnIndex++ )
      {
        excelWorkSheet.Cells[rowIndex+2,columnIndex+1] = dataRow[ columnIndex ];
      }
    }
    private static void drawRows( ReportTable reportTable ,
      Worksheet excelWorkSheet )
    {
      try
      {
        DataRowCollection dataRowCollection = reportTable.DataTable.Rows;
        for (int index = 0 ; index<reportTable.DataTable.Rows.Count ; index++ )
        {
          //          ArrayList reportRecords = (ArrayList)this.GetByIndex( index );
          //          foreach (AccountReportRecord reportRecord in reportRecords)
          drawRow( dataRowCollection , index , excelWorkSheet );

          //          excelWorkSheet.Cells[ row , 1 ] = pnlHistory.GetKey( index );
          //          Object obj = pnlHistory[ (DateTime)pnlHistory.GetKey( index ) ];
          //          excelWorkSheet.Cells[ row , 2 ] = pnlHistory[ (DateTime)pnlHistory.GetKey( index ) ];
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
      }
    }
    private static void showReport_forCurrentWorksheet( int sheetIndex , ReportTable reportTable ,
      Excel.Workbook excelBook )
    {
      Excel.Worksheet excelSheet;
      if ( excelBook.Worksheets.Count >= sheetIndex )
        excelSheet = (Excel.Worksheet)excelBook.Worksheets.get_Item( sheetIndex );
      else
        excelSheet =
          (Excel.Worksheet)excelBook.Worksheets.Add(Missing.Value,
          excelBook.Worksheets.get_Item( excelBook.Worksheets.Count ),1,Missing.Value);       
      excelSheet.Name = reportTable.Name;
      ((_Worksheet)excelSheet).Activate();
      drawHeaders( reportTable , excelSheet );
      drawRows( reportTable , excelSheet );
    }

    public static void ShowReport()
    {
      if ( reportTables.Count > 0 )
      {
        Excel.Application excelApp = new Excel.ApplicationClass();
        excelApp.Visible = true;
 
        Excel.Workbook excelBook = excelApp.Workbooks.Add( Missing.Value );
        for ( int index=0; index<reportTables.Count; index++ )
        {
          showReport_forCurrentWorksheet( index + 1 , (ReportTable)reportTables[ index ] , excelBook );
        }
      }

      //reportToExcel_Dispose( excelSheet , excelBook , excelApp );
    }
    #endregion
	}
}
