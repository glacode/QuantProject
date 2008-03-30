/*
QuantProject - Quantitative Finance Library

DummyInSampleChooser.cs
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
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// Selects no TestingPosition(s) at all. To be used for log item's
	/// debugging
	/// </summary>
	public class DummyInSampleChooser : IInSampleChooser
	{
		public event NewMessageEventHandler NewMessage;
		public event NewProgressEventHandler NewProgress;

		private WeightedPositions weightedPositions;

		public string Description
		{
			get{ return "ConstantWeightedPositions"; }
		}

		public DummyInSampleChooser()
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
			TestingPositions[] dummyTestingPositionsArray =
				new TestingPositions[ 1 ];
			TestingPositions dummyTestingPositions =
				new TestingPositions();
			dummyTestingPositionsArray[ 0 ] = dummyTestingPositions;
			return dummyTestingPositionsArray;
		}
	}
}
