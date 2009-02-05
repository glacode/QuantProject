/*
QuantProject - Quantitative Finance Library

SimpleScriptsContainer.cs
Copyright (C) 2003 
Marco Milletti

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
using System.Data;
using System.Windows.Forms;
using System.IO;

using QuantProject.DataAccess.Tables;
using QuantProject.Data.Selectors;
using QuantProject.Data.Selectors.ByLinearIndipendence;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for SimpleScriptsContainer.
	/// </summary>
	public class SimpleScriptsContainer 
	{
    #region Execute
		
		public static void Execute()
    {
      QuantProject.Data.Selectors.ByLinearIndipendence.SelectorByMaxLinearIndipendence selector = 
        new QuantProject.Data.Selectors.ByLinearIndipendence.SelectorByMaxLinearIndipendence(
        "SP500",new DateTime(2004,1,1),new DateTime(2004,5,31),5,3,2000,"^GSPC");

      DataTable indipendentTickers = selector.GetTableOfSelectedTickers();
      SelectorByCloseToCloseCorrelationToBenchmark selector2 = 
          new SelectorByCloseToCloseCorrelationToBenchmark("SP500",(string)indipendentTickers.Rows[0][0],
                                                           false,new DateTime(2004,1,1),new DateTime(2004,5,31),
                                                           20,false);
      DataTable correlatedTickers = selector2.GetTableOfSelectedTickers();
    }
  	#endregion Execute
		
  	#region ImportCsvFilesIntoDatabase
  	
  	static int numberOfCsvFilesImported = 0;
  	static DateTime beginOfImportSession;
  	
    private static void importFromCsvFiles_importFile_openFileAndWriteToDb_addBar(string ticker,
                         string exchange, long intervalFrameInSeconds, string lineRepresentingBar)
    {
    	
    	//2006-01-03;09:30:00;33.400000;33.400000;33.380000;33.380000;232600
    	string[] fields = lineRepresentingBar.Split(';');
    	string[] subStringsForDate = fields[0].Split('-');
    	string[] subStringsForTime = fields[1].Split(':');
    	DateTime dateTimeForOpen = new DateTime( Convert.ToInt32(subStringsForDate[0]),
    	                                         Convert.ToInt32(subStringsForDate[1]),
    	                                         Convert.ToInt32(subStringsForDate[2]),
    	                                         Convert.ToInt32(subStringsForTime[0]),
    	                                         Convert.ToInt32(subStringsForTime[1]),
    	                                         Convert.ToInt32(subStringsForTime[2]) );
    	double open, high, low, close, volume;
    	open = Double.Parse(fields[2]);
    	high = Double.Parse(fields[3]);
    	low = Double.Parse(fields[4]);
    	close = Double.Parse(fields[5]);
    	volume = Double.Parse(fields[6]);
    	
    	Bars.AddBar(ticker, exchange, dateTimeForOpen, intervalFrameInSeconds, open, high, low, close, volume);
    }
    
    private static void writeToLogFile(string ticker, int numOfRowsAdded,
                                       DateTime dateTimeOfFirstRowAdded )
    {
    	string fileNamePathForLog;
    	string applicationDir = Application.ExecutablePath.Substring(0,Application.ExecutablePath.LastIndexOf('\\'));
    	fileNamePathForLog = applicationDir + "\\" + "SessionImportLog" + 
    		beginOfImportSession.Year.ToString() + "_" + 
    		beginOfImportSession.Month.ToString() + "_" + 
    		beginOfImportSession.Day.ToString() + "_" +
    		beginOfImportSession.Hour.ToString() + "_" +
    		beginOfImportSession.Minute.ToString() + "_" + 
    		beginOfImportSession.Second.ToString() + "_" +
    		"Requested_" + numberOfCsvFilesImported.ToString() + 
    		"_files.txt";
    	StreamWriter writer;
    	if (File.Exists(fileNamePathForLog))
    		writer = File.AppendText(fileNamePathForLog);
    	else
    		{
     			writer = File.CreateText(fileNamePathForLog);
     			writer.WriteLine("ticker;rows added;date time of 1° row added;date time of last row added;minutes elapsed;seconds elapsed;rows added per second"); 		
     		}
    	TimeSpan timeSpan = DateTime.Now.Subtract(dateTimeOfFirstRowAdded);
    	int rowsAddedPerSecond = 0;
    	int totalOfSeconds = timeSpan.Minutes*60+timeSpan.Seconds;
    	if ( totalOfSeconds != 0 )
    		rowsAddedPerSecond = numOfRowsAdded / totalOfSeconds;
     	string lineToWrite = ticker + ";" + numOfRowsAdded.ToString() + ";" +
    		dateTimeOfFirstRowAdded.ToLongTimeString() + ";" +
    		DateTime.Now.ToLongTimeString() + ";" +
    		timeSpan.Minutes + ";" + timeSpan.Seconds + ";" +
    		rowsAddedPerSecond.ToString();
    		    	
    	writer.WriteLine(lineToWrite);
    	writer.Close();
    }
    
    private static void importFromCsvFiles_importFile_openFileAndWriteToDb(string filePath, string ticker,
                         string exchange, long intervalFrameInSeconds)
    {
    	StreamReader csvReader = new StreamReader(filePath);
    	string line;
    	int numOfRowsAdded = 0;
    	if( csvReader == null )
        return;
      line = csvReader.ReadLine();//first line contains column headers
      line = csvReader.ReadLine();
      DateTime dateTimeOfFirstRowAdded = DateTime.Now;
  		while ( line != null )
      {
        try{
  				numOfRowsAdded++;
  				importFromCsvFiles_importFile_openFileAndWriteToDb_addBar(ticker, exchange, intervalFrameInSeconds,
  			                                                          line);
  				
  	    }
  			catch(Exception ex){
  				string s = ex.ToString();
  				numOfRowsAdded--;
				}  				
  			line = csvReader.ReadLine();
      }
  		csvReader.Close();
  		writeToLogFile(ticker, numOfRowsAdded, dateTimeOfFirstRowAdded);
    }
        
    private static void importFromCsvFiles_importFile(string filePath)
    {
    	//A_1_N_20060101000000_20090101230000.csv
    	int previousIndexBeforeFileName = filePath.LastIndexOf('\\');
    	string fileName = filePath.Substring(previousIndexBeforeFileName + 1);
    	int posOfFirstSep = fileName.IndexOf('_');
    	int posOfSecondSep = fileName.IndexOf('_', posOfFirstSep + 1);
    	int posOfThirdSep = fileName.IndexOf('_', posOfSecondSep + 1);
    	string ticker = fileName.Substring(0, posOfFirstSep);
    	long intervalFrameInMinutes = Convert.ToInt64(
    		fileName.Substring(posOfFirstSep + 1, posOfSecondSep - posOfFirstSep - 1) );
    	string exchange = 
    		fileName.Substring(posOfSecondSep + 1, 1);
    	importFromCsvFiles_importFile_openFileAndWriteToDb(filePath, ticker,
    	                                                   exchange, intervalFrameInMinutes * 60);
    }
        
    public static void ImportFromCsvFiles()
    {
    	OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select csv files to import in db";
			openFileDialog.Multiselect = true;
			openFileDialog.CheckFileExists = true;
			openFileDialog.Filter = "csv files|*.csv";
			openFileDialog.ShowDialog();
			string[] files = openFileDialog.FileNames;
			if( files!= null)
			{
				numberOfCsvFilesImported = files.Length;
				beginOfImportSession = DateTime.Now;
				for ( int i = 0; i < files.Length; i++ )
					importFromCsvFiles_importFile( files[i] );
			}
    }
    
    #endregion ImportCsvFilesIntoDatabase
  }
}
