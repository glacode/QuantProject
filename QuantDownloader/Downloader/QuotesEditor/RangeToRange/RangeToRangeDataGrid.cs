using System;
using QuantProject.Applications.Downloader.Validate;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// VisualValidationDataGrid to contain quotes with
	/// Range to Range suspicious ratios
	/// </summary>
	public class RangeToRangeDataGrid : VisualValidationDataGrid
	{
		public RangeToRangeDataGrid()
		{
			this.validationWarning = ValidationWarning.SuspiciousRangeToRangeRatio;
		}
		override protected void confirmVisualValidation( string ticker , DateTime quoteDate )
		{
//			VisuallyValidatedTickers.ValidateRangeToRange( ((QuotesEditor)this.FindForm()).Ticker );
			Quotes quotes = new Quotes( ticker );
			VisuallyValidatedQuotes.ValidateRangeToRange( quotes , quoteDate );
		}
	}
}
