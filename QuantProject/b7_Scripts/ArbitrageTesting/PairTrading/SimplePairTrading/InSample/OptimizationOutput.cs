/*
QuantProject - Quantitative Finance Library

OptimizationOutput.cs
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

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample
{
	/// <summary>
	/// Data to be saved/load to/from disk, resulting from the
	/// optimization process.
	/// </summary>
	[Serializable]
	public class OptimizationOutput : ArrayList
	{
//		public ArrayList BestGenomes
//		{
//			get { return this.bestGenomes; }
//		}
		public OptimizationOutput()
		{
		}
		/// <summary>
		/// Adds a genome representation
		/// </summary>
		/// <param name="genomeRepresentation"></param>
		public void Add( GenomeRepresentation genomeRepresentation )
		{
			base.Add( genomeRepresentation );
		}
	}
}
