/*
QuantProject - Quantitative Finance Library

Parameters.cs
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
