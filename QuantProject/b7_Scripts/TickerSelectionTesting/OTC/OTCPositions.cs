/*
QuantProject - Quantitative Finance Library

OTCPositions.cs
Copyright (C) 2009
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Business.DataProviders;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC
{
	/// <summary>
	/// This is the class representing a TestingPositions for the
	/// Open To Close strategy
	/// </summary>
	[Serializable]
	public class OTCPositions : TestingPositions, IGeneticallyOptimizable
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
		
		public new OTCPositions Copy()
		{
			return new OTCPositions( this.WeightedPositions,
			                         this.generation );
		}

		// creates an empty TestingPositions: to be used to give a meaning with
		// OTCPositions type to undecodables
		public OTCPositions() : base()
		{
		}
		public OTCPositions(WeightedPositions weightedPositions, int generation) :
			base(weightedPositions)
			
		{
			this.generation = generation;
		}
		public OTCPositions(WeightedPositions weightedPositions) :
			base(weightedPositions)
			
		{
			this.generation = -1;
		}
	}
}
