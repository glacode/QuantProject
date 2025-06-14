/*
QuantProject - Quantitative Finance Library

ShowReportFromFile.cs
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
using System.Collections;
using QuantProject.Scripts;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.DataProviders;
using QuantProject.ADT.FileManaging;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.CallingReportsForRunScripts
{
	public class ShowReportFromFile
	{
		public ShowReportFromFile()
		{
			
		}

		
		public static void ShowReportFromSerializedAccount(string serializedAccountFullPath)
		{
			try
			{
				Account account = 
						(Account)ObjectArchiver.Extract(serializedAccountFullPath);
				Report report = new Report(account, new HistoricalAdjustedQuoteProvider());
				
        report.Text = 
							serializedAccountFullPath.Substring(serializedAccountFullPath.LastIndexOf("\\") + 1);
				ReportShower reportShower = new ReportShower(report);
        reportShower.Show(); 
			}
			catch(System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.ToString());
			}
		}
    
    public static Report ShowReportFromSerializedAccountReport(
			string serializedAccountReportFullPath )
    {
    	Report report = null;
      try
      {
        AccountReport accountReport = 
          (AccountReport)ObjectArchiver.Extract(serializedAccountReportFullPath);
        report = new Report(accountReport);
        report.Text = 
        	serializedAccountReportFullPath.Substring(serializedAccountReportFullPath.LastIndexOf("\\") + 1);
        report.Show();
      }
      catch(System.Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ex.ToString());
      }
      return report;
    }
    



    public static void ShowReportFromSerializedTransactionHistory(string serializedTransactionHistoryFullPath)
    {
      try
      {
        TransactionHistory transactions = 
          (TransactionHistory)ObjectArchiver.Extract(serializedTransactionHistoryFullPath);
        Account account = new Account("FromSerializedTransactions");
        foreach(Object key in transactions.Keys)
        {
          foreach(TimedTransaction timedTransaction in (ArrayList)transactions[key])
          {
            account.Add(timedTransaction);
          }
        }
        Report report = new Report(account, new HistoricalAdjustedQuoteProvider());
        ReportShower reportShower = new ReportShower(report);
        reportShower.Show(); 
      }
      catch(System.Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ex.ToString());
      }
    }
	}			
}
