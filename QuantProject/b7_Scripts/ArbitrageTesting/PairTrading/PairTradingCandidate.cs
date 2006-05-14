/*
QuantProject - Quantitative Finance Library

PairTradingCandidate.cs
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


namespace QuantProject.Scripts.ArbitrageTesting.PairTrading
{
  /// <summary>
  /// This is the class containing basic information
  /// for each candidate to be evaluated for the pair trading strategies
  /// </summary>
  [Serializable]
  public class PairTradingCandidate
  {
    protected string ticker;
    protected float[] arrayOfAdjustedCloseQuotes;
        
    public PairTradingCandidate(string ticker, float[] arrayOfAdjustedCloseQuotes)
    {
      this.ticker = ticker;
      this.arrayOfAdjustedCloseQuotes = arrayOfAdjustedCloseQuotes;
    }
    
    public float[] ArrayOfAdjustedCloseQuotes
    {
      get
      {
        return this.arrayOfAdjustedCloseQuotes;
      }
    }
    
    public string Ticker
    {
      get{return this.ticker;}
    }
    
    public float GetAdjustedCloseQuote( int arrayElementPosition )
		{
			return this.arrayOfAdjustedCloseQuotes[ arrayElementPosition ];
		}

  }

}
