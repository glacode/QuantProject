using System;
using System.Collections;
using System.IO;
using QuantProject.ADT;

namespace QuantProject.ADT.Optimizing
{
	/// <summary>
	/// Summary description for Parameters.
	/// </summary>
	public class Parameters : RecursiveHashtable
	{
		public Parameters()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public override RecursiveHashtable Tail()
    {
      Parameters parameters = new Parameters();
      RecursiveHashtable recursiveHashtable = base.Tail();
      foreach (DictionaryEntry dictionaryEntry in recursiveHashtable)
        parameters.Add( (Parameter) dictionaryEntry.Value );
      return parameters;
    }

    public void Add( Parameter parameter )
    {
      base.Add( parameter.Key , parameter );
    }
    #region "SetNextValues"
    private bool setStartingValues()
    {
      foreach (Parameter parameter in this.Values)
        parameter.Value = parameter.LowerBound;
      return true;
    }

    private bool setNextValuesActually()
    {
      bool headOverflow = !((Parameter)this.Head()).SetNextValue();
      bool tailOverflow = false;
      Parameter head = (Parameter)this.Head();
      Parameters tail = (Parameters)this.Tail();
      if ( headOverflow )
      {
        tailOverflow = !tail.SetNextValues();
      }
      return !( headOverflow && tailOverflow );
    }

    private bool setNextValues_nonEmpty()
    {
      if (((Parameter)this.Head()).Value<((Parameter)this.Head()).LowerBound)
        return setStartingValues();
      else
        return setNextValuesActually();
    }

    public bool SetNextValues()
    {
      if (this.Count > 0)
        return setNextValues_nonEmpty();
      else
        return false;
    }
    #endregion

    public Parameters Copy()
    {
      Parameters parameters = new Parameters();
      foreach (DictionaryEntry dictionaryEntry in this)
      {
        parameters.Add( dictionaryEntry.Key , ((ICloneable)dictionaryEntry.Value).Clone() );
      }
      return parameters;
    }

    public void ReportToConsole()
    {
      foreach ( Parameter parameter in this.Values )
        Console.WriteLine( parameter.ToString() );
    }
  }
}
