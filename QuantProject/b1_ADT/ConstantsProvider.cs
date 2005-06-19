using System;

namespace QuantProject.ADT
{
	/// <summary>
	/// Provides constants to be used by the whole application
	/// </summary>
	public class ConstantsProvider
	{
		public ConstantsProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static int SuspiciousRatio = 7;
		public static int PrecedingDaysForVisualValidation = 20;
		public static int DaysForMovingAverageForSuspiciousRatioValidation = 20;
		public static DateTime InitialDateTimeForDownload = new DateTime(1985,1,1);
    public static int TimeOutValue = 15000;
    public static int NumberOfCheckToPerformOnAdjustedValues = 3;
    public static double MaxDifferenceForAdjustedValues = 0.01;
    public static double MaxDifferenceForCloseToCloseRatios = 0.005;
		// threshold above which the equity line gain and the benchmark gain are considered different
		public static double MinForDifferentGains = 0.0003;
    // max num days allowed by the data source (yahoo)
    public static double MaxNumDaysDownloadedAtEachConnection = 200;
		public static string FormatWithZeroDecimals = "#,#.";
		public static string FormatWithOneDecimal = "#,#.0";
		public static string FormatWithTwoDecimals = "#,#.00";
		public static DateTime MinQuoteDateTime = new DateTime( 1950 , 1 , 1 );
		public static int CachePages = 1000;
		public static int PagesToBeRemovedFromCache = 500; // for Garbage Collection
	}
}
