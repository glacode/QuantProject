using System;
using System.Collections;

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for RecursiveHashtable.
	/// </summary>
	public class RecursiveHashtable : Hashtable
	{
    public RecursiveHashtable() : base()
    {
    }

    public bool IsEmpty()
    {
      return ( this.Count == 0 );
    }

    public Keyed Head()
    {
      IDictionaryEnumerator enumerator;
      enumerator = this.GetEnumerator();
      //enumerator.Reset();
      if ( enumerator.MoveNext() )
      {
        return ( Keyed )enumerator.Value;
      }
      else
      {
        // an exception will rise
        return ( Keyed )enumerator.Value;
      }
    }

    public virtual RecursiveHashtable Tail()
    {
      RecursiveHashtable tail = new RecursiveHashtable();
      IDictionaryEnumerator enumerator;
      enumerator = this.GetEnumerator();
      enumerator.Reset();
      enumerator.MoveNext();
      while ( enumerator.MoveNext() )
      {
        Keyed keyed = ( Keyed )enumerator.Value;
        tail.Add( keyed.Key , keyed );
      }
      return tail;
    }

    public RecursiveHashtable Cons( Keyed head , RecursiveHashtable tail )
    {
      RecursiveHashtable recursiveHashtable = new RecursiveHashtable();
      IDictionaryEnumerator enumerator;

      recursiveHashtable.Add( head.Key , head );

      enumerator = this.GetEnumerator();
      while ( !enumerator.MoveNext() )
      {
        Keyed keyed = ( Keyed )enumerator.Current;
        recursiveHashtable.Add( keyed.Key , keyed );
      }
      return recursiveHashtable;
    }

    public override string ToString()
    {
      string toString = "\n";
      foreach (Object obj in this.Values)
        toString += obj.ToString();   
      return toString;
    }
	}
}
