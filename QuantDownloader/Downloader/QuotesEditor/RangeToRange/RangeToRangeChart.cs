using System;
using System.Drawing;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// VisualValidationChart to contain charts for quotes with 
	/// Range to Range suspicious ratios
	/// </summary>
	public class RangeToRangeChart : VisualValidationChart
	{
		public RangeToRangeChart()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		protected override void addHistories()
		{
			History lowHistory = HistoricalDataProvider.GetLowHistory( ((QuotesEditor)this.FindForm()).Ticker );
			this.add( lowHistory , Color.Green );
			History highHistory = HistoricalDataProvider.GetHighHistory( ((QuotesEditor)this.FindForm()).Ticker );
			this.add( highHistory , Color.Blue );
		}
	}
}
