/*
QuantProject - Quantitative Finance Library

BasicFunctions.cs
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

namespace QuantProject.ADT.Statistics
{
	/// <summary>
	/// Implements some basic statistical functions
	/// </summary>
	public class BasicFunctions
	{
		
		static public  double Sum( double[] data ) 
		{
			double sum = 0;
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				sum += data[ i ];
			}
			return	sum;
		}
		static public  double SumOfSquares( double[] data ) 
		{
			double sumOfSquares = 0;
			for( int i = 0; i < data.Length ; i ++ ) 
			{
				sumOfSquares += data[ i ]*data[ i ];
			}
			return	sumOfSquares;
		}
		
		static public double SimpleAverage( double[] data ) 
		{
			return	BasicFunctions.Sum(data)/data.Length;
		}

		static public double Variance( double[] data ) 
		{
			double sum = BasicFunctions.Sum(data);
			double sumOfSquares = BasicFunctions.SumOfSquares(data);
			return	(sumOfSquares - sum*sum/data.Length)/data.Length;
		}

		static public double StdDev( double[] data ) 
		{
			return	System.Math.Sqrt(BasicFunctions.Variance(data));
		}
	}	
}		

		