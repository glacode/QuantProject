/*
QuantProject - Quantitative Finance Library

CandidateProperties.cs
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


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the class containing basic information
  /// for each candidate to be evaluated 
  /// </summary>
  [Serializable]
  public class CandidateProperties : IComparable
  {
    protected string ticker;
    protected float[] arrayOfRatesOfReturn;
    protected float[] oppositeArrayOfRatesOfReturn;
    protected bool longRatesOfReturn;
    protected double fitness;

    public CandidateProperties(string ticker, float[] arrayOfRatesOfReturn):
                    this(ticker,arrayOfRatesOfReturn,0.0)
    {
      
    }
    
    public CandidateProperties(string ticker, float[] arrayOfRatesOfReturn,
                               double fitness)
    {
      this.ticker = ticker;
      this.longRatesOfReturn = true;
      this.arrayOfRatesOfReturn = arrayOfRatesOfReturn;
      this.fitness = fitness;
    }
    
    private float[] arrayOfRatesOfReturn_getOppositeArrayOfRatesOfReturn()
    {
      if(this.oppositeArrayOfRatesOfReturn == null)
      //opposite array of rates of returns not set yet
      {
        this.oppositeArrayOfRatesOfReturn = new float[this.arrayOfRatesOfReturn.Length];
        for(int i = 0; i<this.arrayOfRatesOfReturn.Length; i++)
          oppositeArrayOfRatesOfReturn[i] = -this.arrayOfRatesOfReturn[i];
      }  
      return this.oppositeArrayOfRatesOfReturn;
    }
    //implementation of IComparable interface
    public int CompareTo(object obj) 
    {
      if(obj is CandidateProperties) 
      {
        CandidateProperties candidate = (CandidateProperties)obj;

        return this.Fitness.CompareTo(candidate.Fitness);
      }
        
      throw new ArgumentException("Object is not of CandidateProperties type!");    
    }
    //end of implementation of IComparable interface
    
    public float[] ArrayOfRatesOfReturn
    {
      get
      {
        float[] returnValue = this.arrayOfRatesOfReturn;
        if(!this.longRatesOfReturn)
          returnValue = this.arrayOfRatesOfReturn_getOppositeArrayOfRatesOfReturn();
        return returnValue;
      }
    }
    
    public bool LongRatesOfReturn
    {
      set{this.longRatesOfReturn = value;}
      get{return this.longRatesOfReturn;}
    }

    public string Ticker
    {
      get{return this.ticker;}
    }
    
    public virtual double Fitness
    {
      get{return this.fitness;}
    }
    
    public virtual void setFitness()
    {
      this.fitness = 0.0;
    }
		public float GetReturn( int arrayElementPosition ,
			bool isLong )
		{
			float returnValue = this.arrayOfRatesOfReturn[ arrayElementPosition ];
			if ( !isLong )
				// a reverse position return is requested
				returnValue = -returnValue;
			return returnValue;
		}
  }

}
