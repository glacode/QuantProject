using System;
using System.Data;
using System.Data.OleDb;
using QuantProject.DataAccess;
using QuantProject.Applications.Downloader.Validate.Validators;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for QuotesToBeValidated.
	/// </summary>
	public class QuotesToBeValidated : DataTable
	{
    private string selectStatement;
    private OleDbDataAdapter oleDbDataAdapter;

    private double suspiciousRatio;

    public double SuspiciousRatio
    {
      get { return this.suspiciousRatio; }
      set { this.suspiciousRatio = value; }
    }
    
    public QuotesToBeValidated( string tickerIsLike )
	{
//      this.selectStatement =
//        "select * from quotes where quTicker like '" + tickerIsLike + "'";
      this.selectStatement =
        "select * from quotes where quTicker = '" + tickerIsLike + "'";
      this.oleDbDataAdapter =
        new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
      try
      {
        this.oleDbDataAdapter.Fill( this );
      }
      catch (Exception exception)
      {
        Console.WriteLine( exception.ToString() );
      }
	}

	public QuotesToBeValidated(DataTable tableOfTickersToBeValidated)
	{	
		this.oleDbDataAdapter =
				new OleDbDataAdapter( "" , ConnectionProvider.OleDbConnection );
		foreach(DataRow row in tableOfTickersToBeValidated.Rows)
		{
			this.oleDbDataAdapter.SelectCommand.CommandText = 
						"select * from quotes where quTicker = '" + 
						(string)row[0] + "'";
			try
			{
				this.oleDbDataAdapter.Fill( this );
			}
			catch (Exception exception)
			{
				Console.WriteLine( exception.ToString() );
			}
		}

	}

//    public delegate void SuspiciousDataRowEventHandler(
//      Object sender , SuspiciousDataRowEventArgs eventArgs );

    public event SuspiciousDataRowEventHandler SuspiciousDataRow;

    #region "Validate"
    private void suspiciousDataRowHandler( Object sender ,
      SuspiciousDataRowEventArgs eventArgs )
    {
      SuspiciousDataRow( this , eventArgs );
    }
    public void Validate()
    {
//      QuantProject.Applications.Downloader.Validate.Validators.OHLCvalidator oHLCvalidator =
//        new QuantProject.Applications.Downloader.Validate.Validators.OHLCvalidator();
//      oHLCvalidator.SuspiciousDataRow +=
//        new SuspiciousDataRowEventHandler( suspiciousDataRowHandler );
//      oHLCvalidator.Validate( this );
      MultiValidator multiValidator = new MultiValidator();
      multiValidator.SuspiciousRatio = this.suspiciousRatio;
      multiValidator.SuspiciousDataRow +=
        new SuspiciousDataRowEventHandler( this.suspiciousDataRowHandler );
      multiValidator.Validate( this );
    }
	}
  #endregion
}
