/*
QuantProject - Quantitative Finance Library

SelectTopEligiblesInSampleChooser.cs
Copyright (C) 2011
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
	[Serializable]
	public class SelectTopEligiblesInSampleChooser : IInSampleChooser
	{
		public event NewMessageEventHandler NewMessage;
		public event NewProgressEventHandler NewProgress;
		protected int maxNumOfTestingPositionsToBeReturned;
		protected int numOfTickersInEachTestingPosition;
		
		public string Description
		{
			get{ return "TopElgblsInSmplChsr"; }
		}

		public SelectTopEligiblesInSampleChooser(int numOfTickersInEachTestingPosition,
		                                         int maxNumOfTestingPositionsToBeReturned)
		{
			this.numOfTickersInEachTestingPosition = 
				numOfTickersInEachTestingPosition;
			this.maxNumOfTestingPositionsToBeReturned =
				maxNumOfTestingPositionsToBeReturned;
		}
		
		protected virtual string analyzeInSample_getTestingPositionsArray_getTicker(EligibleTickers eligibleTickers,
		                                                                            int idxOfTicker)
		{
			string returnValue =
				eligibleTickers.Tickers[idxOfTicker];
			
			return returnValue;
		}
		
		
		protected GeneticallyOptimizableTestingPositions[] analyzeInSample_getTestingPositionsArray(EligibleTickers eligibleTickers)
		{
			if( eligibleTickers.Count < 
			    (this.maxNumOfTestingPositionsToBeReturned *
			     this.numOfTickersInEachTestingPosition) )
				throw new Exception("Not enough eligibles !");
			
			GeneticallyOptimizableTestingPositions[] returnValue =
				new GeneticallyOptimizableTestingPositions[ this.maxNumOfTestingPositionsToBeReturned ];
			int idxEligible = 0;
			string[] signedTickers = new string[numOfTickersInEachTestingPosition];
			for ( int i = 0; i < returnValue.Length ; i++)
			{
				for ( int j = 0;
				      idxEligible < eligibleTickers.Count && j < numOfTickersInEachTestingPosition;
				      j++ )
					signedTickers[j] = 
						this.analyzeInSample_getTestingPositionsArray_getTicker(
							eligibleTickers, idxEligible + j);
				
				returnValue[i] =
						new GeneticallyOptimizableTestingPositions(new WeightedPositions(new SignedTickers(signedTickers)));
				idxEligible =+ numOfTickersInEachTestingPosition;
			}
			return returnValue;
		}
		
		public object AnalyzeInSample( EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , new NewMessageEventArgs( "New" ) );
			if ( this.NewProgress != null )
				this.NewProgress( this , new NewProgressEventArgs( 1 , 1 ) );
			GeneticallyOptimizableTestingPositions[] returnValue =
				this.analyzeInSample_getTestingPositionsArray(eligibleTickers);
			
			return returnValue;
		}
	}
}
