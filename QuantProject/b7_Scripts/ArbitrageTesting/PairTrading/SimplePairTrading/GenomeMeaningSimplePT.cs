/*
QuantProject - Quantitative Finance Library

GenomeMeaningSimplePT.cs
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

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
  /// <summary>
  /// This is the class representing the meaning for genome
  /// found by a GeneticOptimizer initialized by GenomeManagerForSimplePT
  /// </summary>
  [Serializable]
  public class GenomeMeaningSimplePT
  {
  	private string firstTicker;
    private string secondTicker;
  	private int numOfDaysForGap;
    private double averageGap;
    private double stdDevGap;
  	
  	public string FirstTicker
    {
      get{return this.firstTicker;}
    }
    
    public string SecondTicker
    {
      get{return this.secondTicker;}
    }
    
    public int NumOfDaysForGap
    {
      get{return this.numOfDaysForGap;}
    }
    
    public double AverageGap
    {
      get{return this.averageGap;}
    }
    
    public double StdDevGap
    {
      get{return this.stdDevGap;}
    }
    
    public GenomeMeaningSimplePT(int numOfDaysForGap, double averageGap,
                                 double stdDevGap,
                                  string firstTicker, string secondTicker)
    {
 			this.numOfDaysForGap = numOfDaysForGap;
      this.firstTicker = firstTicker;
      this.secondTicker = secondTicker;
      this.averageGap = averageGap;
      this.stdDevGap = stdDevGap;
		}
  }
}
