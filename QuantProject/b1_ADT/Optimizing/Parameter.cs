/*
QuantProject - Quantitative Finance Library

Parameter.cs
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
using QuantProject.ADT;

namespace QuantProject.ADT.Optimizing
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
	public class Parameter : Keyed , ICloneable
	{
    private double lowerBound;
    private double upperBound;
    private double step;
    private double privateValue = double.MinValue;

    private bool overflow;

    public double LowerBound
    {
      get
      {
        return lowerBound;
      }
      set
      {
        lowerBound = value;
      }
    }

    public double UpperBound
    {
      get
      {
        return upperBound;
      }
      set
      {
        upperBound = value;
      }
    }

    public double Step
    {
      get
      {
        return step;
      }
      set
      {
        step = value;
      }
    }

    public double Value
    {
      get
      {
        return privateValue;
      }
      set
      {
        privateValue = value;
      }
    }

    public bool Overflow
    {
      get
      {
        return overflow;
      }
    }

    public Parameter( string key , double lowerBound , double upperBound , double step )
		{
      Key = key;
			LowerBound = lowerBound;
      UpperBound = upperBound;
      Step = step;
		}

    public bool SetNextValue()
    {
      overflow = false;
      if ( Value == double.MinValue )
        Value = lowerBound;
      else
      {
        if ( Value < UpperBound )
          Value = Math.Min( UpperBound , Value + Step );
        else
          // Value == UpperBound
        {
          overflow = true;
          Value = lowerBound;
        }
      }
      return (!Overflow);
    }

    public Object Clone()
    {
      Parameter parameter = new Parameter( this.Key , this.LowerBound , this.UpperBound , this.Step );
      parameter.Value = this.Value;
      return parameter;
    }

    public override string ToString()
    {
      return
        "\nParameter Key : " + this.Key +
        "\nParameter Value : " + this.Value;
    }
	}
}
