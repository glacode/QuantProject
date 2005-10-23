/*
QuantProject - Quantitative Finance Library

GenomeMeaning.cs
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
using System.Data;
using System.Collections;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the class representing the meaning for genome
  /// </summary>
  [Serializable]
  public class GenomeMeaning
  {
  	private string[] tickers;
  	private double returnAtLastDayInSample;
  	private double averageReturnInSample;
  	private double varianceReturnInSample;
    
  	public string[] Tickers
    {
      get{return this.tickers;}
    }
    
    public double ReturnAtLastDayInSample
    {
      get{return this.returnAtLastDayInSample;}
    }
    
    public double AverageReturnInSample
    {
      get{return this.averageReturnInSample;}
    }
    
    public double VarianceReturnInSample
    {
      get{return this.varianceReturnInSample;}
    }
   
    public GenomeMeaning(string[] tickers)
    {
 			this.tickers = tickers;
		}
    
    public GenomeMeaning(string[] tickers,
                            double returnAtLastDayInSample, 
  													 double averageReturnInSample,
  													 double varianceReturnInSample)
    {
 			this.tickers = tickers;
 			this.returnAtLastDayInSample = returnAtLastDayInSample;
 			this.averageReturnInSample = averageReturnInSample;
 			this.varianceReturnInSample = varianceReturnInSample;
		}
    
    
  }

}
