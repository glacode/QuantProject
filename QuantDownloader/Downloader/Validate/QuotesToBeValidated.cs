using System;
using System.Data;
using System.Data.OleDb;
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
      this.oleDbDataAdapter = new OleDbDataAdapter( selectStatement , AdoNetTools.OleDbConnection );
      try
      {
        this.oleDbDataAdapter.Fill( this );
      }
      catch (Exception exception)
      {
        Console.WriteLine( exception.ToString() );
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
