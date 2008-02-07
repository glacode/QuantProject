/*
QuantProject - Quantitative Finance Library

ConstantWeightedPositionsChooser.cs
Copyright (C) 2008
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
using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// This chooser returns always the same WeightedPositions
	/// (to be used for log item's debugging and/or for data snooping)
	/// </summary>
	public class ConstantWeightedPositionsChooser : IInSampleChooser
	{
		public event NewMessageEventHandler NewMessage;
		public event NewProgressEventHandler NewProgress;

		private WeightedPositions weightedPositions;

		public string Description
		{
			get{ return "ConstantWeightedPositions"; }
		}

		public ConstantWeightedPositionsChooser( WeightedPositions weightedPositions )
		{
			this.weightedPositions = weightedPositions;
		}
		public object AnalyzeInSample( EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , new NewMessageEventArgs( "New" ) );
			if ( this.NewProgress != null )
				this.NewProgress( this , new NewProgressEventArgs( 1 , 1 ) );
			return this.weightedPositions;
		}
	}
}
