/*
QuantProject - Quantitative Finance Library

TestingPositions.cs
Copyright (C) 2008
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
  /// This is the base class representing positions that 
  /// should be tested out of sample in a back test
  /// Every strategy should test out of sample an object
  /// of this type or an object derived from this class
  /// </summary>
  [Serializable]
  public class TestingPositions
  {
  	private WeightedPositions weightedPositions;
  	private string hashCodeForTickerComposition;
  	private double fitnessInSample;
  	  	 
		public WeightedPositions WeightedPositions
		{
			get{return this.weightedPositions;}
		}
    
		public double FitnessInSample
		{
			get{return this.fitnessInSample;}
			set{this.fitnessInSample = value;}
		}
		
		
		public string HashCodeForTickerComposition
    {
      get
      {
      	if(this.hashCodeForTickerComposition == null && 
      	   this.weightedPositions != null)
      	//if hashCodeForTickerComposition has not been computed yet and
      	//the current instance is not empty
      	{
      		ArrayList listOfTickers = 
      			new ArrayList(this.weightedPositions.SignedTickers.Tickers);
		      listOfTickers.Sort();
		      foreach(string tickerCode in listOfTickers)
		        this.hashCodeForTickerComposition += tickerCode + ";";
      	}
      	return this.hashCodeForTickerComposition;
			}
    }
		
		public TestingPositions(WeightedPositions weightedPositions)
    {
 			this.weightedPositions = weightedPositions;
 			this.fitnessInSample = double.MinValue;
		}
		
		public TestingPositions(WeightedPositions weightedPositions,
		                        double fitnessInSample)
    {
 			this.weightedPositions = weightedPositions;
 			this.fitnessInSample = fitnessInSample;
		}
		
		//it creates an empty TestingPositions
		public TestingPositions()
    {
 			this.weightedPositions = null;
 			this.fitnessInSample = double.MinValue;
		}
  }
}
