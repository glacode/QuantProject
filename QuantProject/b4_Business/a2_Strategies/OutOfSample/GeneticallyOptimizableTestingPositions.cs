/*
QuantProject - Quantitative Finance Library

GeneticallyOptimizableTestingPositions.cs
Copyright (C) 2010
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

using QuantProject.Business.Strategies;

namespace QuantProject.Business.Strategies.OutOfSample
{
	/// <summary>
	/// This is the class representing a generic genetically 
	/// optimizable TestingPositions, which can be used 
	/// in several different strategies
	/// </summary>
	[Serializable]
	public class GeneticallyOptimizableTestingPositions : TestingPositions, IGeneticallyOptimizable
	{
		private int generation;
		
		//explicit interface implementation
		//the property can be used only by a interface
		//instance or through a cast to the interface
		int IGeneticallyOptimizable.Generation
		{
			get{return this.generation;}
			set{this.generation = value;}
		}
		
		public new GeneticallyOptimizableTestingPositions Copy()
		{
			return new GeneticallyOptimizableTestingPositions( this.WeightedPositions,
			                         this.generation );
		}

		// creates an empty TestingPositions: to be used to give a meaning with
		// GeneticallyOptimizableTestingPositions type to undecodables
		public GeneticallyOptimizableTestingPositions() : base()
		{
		}
		public GeneticallyOptimizableTestingPositions(WeightedPositions weightedPositions, int generation) :
			base(weightedPositions)
			
		{
			this.generation = generation;
		}
		public GeneticallyOptimizableTestingPositions(WeightedPositions weightedPositions) :
			base(weightedPositions)
			
		{
			this.generation = -1;
		}
	}
}
