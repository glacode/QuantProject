/*
QuantProject - Quantitative Finance Library

CalculusApproximation.cs
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
using QuantProject.ADT.Statistics;

namespace QuantProject.ADT.Calculus
{
	/// <summary>
	/// Class providing static methods for calculus approximation
	/// </summary>
  public class CalculusApproximation 
	{
	  /// <summary>
		/// Computes area using a standard approximation method
		/// </summary>
	 	public static double GetArea(IPdfDefiner distributionWithPDF,
	 	                             double a, double b,
	 	                             int numInterval)
		{
			double h = 0;
			double area = 0;
			if (numInterval <= 0)
			 	 throw new Exception("Number of intervals must be > 0!");
			if (a > b)
         throw new Exception("< a > must be less than < b >");
      h = (b - a)/numInterval;
			area = distributionWithPDF.GetProbabilityDensityValue(b);
			area += distributionWithPDF.GetProbabilityDensityValue(a);
			area = area/2;
			for (int conta = 1; conta < numInterval; conta++)
			{
				area +=  distributionWithPDF.GetProbabilityDensityValue(a+conta*h);
			}
			area = area * h;
	
			return area;

		}
       
	}
}
