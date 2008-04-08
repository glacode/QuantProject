/*
QuantProject - Quantitative Finance Library

OutOfRangeException.cs
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

namespace QuantProject.ADT
{
	/// <summary>
	/// It should be thrown when a given number
	/// is out of the desired range
	/// </summary>
	public class OutOfRangeException : Exception
	{
		private string outOfRangeNumber;
		private double minimumForValidRange;
		private double maximumForValidRange;
		
		public override string Message
		{
			get
			{
				return this.outOfRangeNumber + 
							 " is out of range! It should be between " +
							 this.minimumForValidRange.ToString() + " and " +
					     this.maximumForValidRange.ToString();
			}
		}
		public OutOfRangeException( string outOfRangeNumber,
		                            double minimumForValidRange,
		                            double maximumForValidRange)
		{
			this.outOfRangeNumber = outOfRangeNumber;
			this.minimumForValidRange = minimumForValidRange;
			this.maximumForValidRange = maximumForValidRange;
		}
	}
}
