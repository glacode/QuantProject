/*
QuantProject - Quantitative Finance Library

NewGenerationEventArgs.cs
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

namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// EventArgs for the NewGeneration event
	/// </summary>
	[Serializable]
	public class NewGenerationEventArgs
	{
		private ArrayList generation;
		private int generationNumber;
		private int generationCounter;
		private GeneticOptimizer currentGeneticOptimizer;

		/// <summary>
		/// Current generation
		/// </summary>
		public ArrayList Generation
		{
			get { return this.generation; }
		}
		/// <summary>
		/// Total number of generations to be created
		/// </summary>
		public int GenerationNumber
		{
			get { return this.generationNumber; }
		}
		/// <summary>
		/// Number of the current generation
		/// </summary>
		public int GenerationCounter
		{
			get { return this.generationCounter; }
		}
		/// <summary>
		/// Genetic optimizer that
		/// has created the current generation 
		/// </summary>
		public GeneticOptimizer CurrentGeneticOptimizer
		{
			get { return this.currentGeneticOptimizer; }
		}
		
		public NewGenerationEventArgs( ArrayList generation )
		{
			this.generation = generation;
		}
		public NewGenerationEventArgs( GeneticOptimizer currentGeneticOptimizer,
		                               ArrayList generation ,
																	 int generationCounter , 
																	 int generationNumber )
		{
			this.currentGeneticOptimizer = currentGeneticOptimizer;
			this.generation = generation;
			this.generationCounter = generationCounter;
			this.generationNumber = generationNumber;
		}
	}
}
