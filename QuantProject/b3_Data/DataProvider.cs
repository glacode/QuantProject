using System;
using System.Collections;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;

namespace QuantProject.Data
{
	/// <summary>
	/// Summary description for DataProvider.
	/// </summary>
	public class DataProvider
	{
    private static Hashtable cachedHistories = new Hashtable();

		public DataProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    // to be deleted
    public static void Add( string instrumentKey )
    {
      Hashtable barComponentHistories = new Hashtable();
      cachedHistories.Add( instrumentKey , barComponentHistories );
    }

    /// <summary>
    /// Adds a new instrument quote history to be cached in memory
    /// </summary>
    /// <param name="instrument">Instrument to be monitored</param>
    /// <param name="barComponent">Bar component to be monitored (Open, High, Low, Close or Volume)</param>
    public static void Add( string instrumentKey , BarComponent barComponent )
    {
      if ( !cachedHistories.ContainsKey( instrumentKey ) )
        cachedHistories.Add( instrumentKey , new Hashtable() );
      ((Hashtable) cachedHistories[ instrumentKey ]).Add(
        barComponent , barComponent );
    }

    //public static void

    public static void SetCachedHistories(
      DateTime startDateTime , DateTime endDateTime )
    {
      ArrayList keyArray = new ArrayList();
      foreach (string instrumentKey in cachedHistories.Keys)
        keyArray.Add( instrumentKey );
      foreach (string instrumentKey in keyArray )
      {
        Hashtable barComponents = new Hashtable();
        foreach (BarComponent barComponent in
          (( Hashtable )cachedHistories[ instrumentKey ]).Keys )
          barComponents.Add( barComponent , barComponent );
        Hashtable histories = DataBase.GetHistories(
          instrumentKey , barComponents , startDateTime , endDateTime );
        cachedHistories[ instrumentKey ] = histories;
      }
    }

    public static History GetOpenHistory( string instrumentKey )
    {
      return (History)((Hashtable)cachedHistories[ instrumentKey ])[
        BarComponent.Open ];
    }

    public static History GetCloseHistory( string instrumentKey )
    {
      return (History)((Hashtable)cachedHistories[ instrumentKey ])[
        BarComponent.Close ];
    }

    public static double GetMarketValue( string instrumentKey , ExtendedDateTime extendedDateTime )
    {
      //DateTime dateTime = 
      return Convert.ToDouble(
        ( (History) ((Hashtable)
          cachedHistories[ instrumentKey ])[ extendedDateTime.BarComponent ] ).GetByIndex(
          ( (History) ((Hashtable) cachedHistories[ instrumentKey ])[ extendedDateTime.BarComponent ]
            ).IndexOfKeyOrPrevious( extendedDateTime.DateTime ) ) );
    }
	}
}
