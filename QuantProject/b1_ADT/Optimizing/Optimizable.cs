/*
QuantProject - Quantitative Finance Library

History.cs
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

namespace QuantProject.ADT.Optimizing
{
	/// <summary>
	/// Summary description for Optimizable.
	/// </summary>
	public abstract class Optimizable
	{
    public Parameters Parameters = new Parameters();
    public Parameters OptimalParameters;

		public Optimizable()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public abstract double Objective();

    private void updateOptimalParameters()
    {
      //OptimalParameters.Clear();
      //foreach (Parameter parameter in this.Parameters )
        //OptimalParameters.Add( parameter );
      OptimalParameters = (Parameters) this.Parameters.Copy();
    }

    public void Optimize()
    {
      double minUntilNow = double.MaxValue;
      //bool doLoop = Parameters.SetNextValues();
      while ( Parameters.SetNextValues() )
      {
        double objectiveValue = this.Objective();
        if (objectiveValue<minUntilNow)
        {
          updateOptimalParameters();
          minUntilNow = objectiveValue;
        }
      }
    }
	}
}
