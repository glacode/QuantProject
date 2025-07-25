/*
QuantProject - Quantitative Finance Library

GenomeComparer.cs
Copyright (C) 2003 
Marco Milletti

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

namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// Compares genomes by fitness
	/// </summary>
	[Serializable]
	public sealed class GenomeComparer : IComparer
	{
		public GenomeComparer()
		{
		}
		public int Compare( object x, object y)
		{
			int returnValue = 0;
			
			if((x is Genome) && (y is Genome))	
			{
				if ( ((Genome) x).Fitness > ((Genome) y).Fitness )
					returnValue = 1;
				else if ( ((Genome) x).Fitness < ((Genome) y).Fitness )
					returnValue = -1;
			}
			else
        throw new ArgumentException("Both objects to compare must be genomes!");
      //old implementation
      //else if ( !(x is Genome) && (y is Genome))
				//returnValue = -1;
			//else if	((x is Genome) && !(y is Genome))				
				//returnValue = 1;
										
			return returnValue;
		}
	}
}
