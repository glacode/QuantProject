using System;

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for Keyed.
	/// </summary>
  [Serializable]
  public class Keyed
  {
    private string key;
    
    public string Key
    {
      get
      {
        return key;
      }
      set
      {
        key = value;
      }
    }

    public Keyed()
    {
      //
      // TODO: Add constructor logic here
      //
    }
  
    public Keyed( string key )
    {
      Key = key;
    }
  }
}
