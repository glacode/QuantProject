using System;
using QuantProject.Applications.Downloader.Validate;

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
	}
}
