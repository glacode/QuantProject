using System;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;
using QuantProject.Business.Validation;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// TabPage for the QuotesEditor, containing a TabControl with
	/// a distinct tab for each kind of validation warning
	/// </summary>
	public class ValidationTabPage : TabPage
	{
    private ValidateDataTable validateDataTable = new ValidateDataTable();

    private string ticker;
    private double suspiciousRatio;

    public string Ticker
    {
      get { return this.ticker; }
      set { this.ticker = value; }
    }
    public double SuspiciousRatio
    {
      get { return this.suspiciousRatio; }
      set { this.suspiciousRatio = value; }
    }

    public ValidationTabPage()
		{
			this.Text = "Validation";
		}
    protected override void OnParentChanged( EventArgs e )
    {
      base.OnEnter( e );
      this.validateDataTable.Rows.Clear();
      this.validateDataTable.AddRows( this.ticker , this.suspiciousRatio );
    }
	}
}
