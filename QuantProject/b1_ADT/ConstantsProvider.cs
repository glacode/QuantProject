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
		public static int SuspiciousRatio = 3;
		public static int PrecedingDaysForVisualValidation = 20;
    public static DateTime InitialDateTimeForDownload = new DateTime(1985,1,1);
    public static int TimeOutValue = 15000;
    public static int NumberOfCheckToPerformOnAdjustedValues = 3;
    public static double MaxRelativeDifferenceForAdjustedValues = 0.0005;
    public static double MaxRelativeDifferenceForCloseToCloseRatios = 0.0001;
    public static double MaxNumDaysDownloadedAtEachConnection = 200;
	}
}
