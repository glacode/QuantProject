/*
QuantProject - Quantitative Finance Library

RecursiveHashTable.cs
Copyright (C) 2003 
Glauco Siliprandi

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
