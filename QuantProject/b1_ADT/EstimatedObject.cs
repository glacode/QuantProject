/*
QuantProject - Quantitative Finance Library

EstimatedObject.cs
Copyright (C) 2003 
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

namespace QuantProject.ADT
{
	/// <summary>
	/// An object along with its estimation
	/// </summary>
	public class EstimatedObject : IComparable
	{
		private object objectToBeEstimated;
		private IComparable estimation;

		public object ObjectToBeEstimated
		{
			get { return this.objectToBeEstimated; }
		}
		public IComparable Estimation
		{
			get { return this.estimation; }
		}

		public EstimatedObject( object objectToBeEstimated , IComparable estimation )
		{
			this.objectToBeEstimated = objectToBeEstimated;
			this.estimation = estimation;
		}

		public int CompareTo( object obj )
		{
			int returnValue = -5;
			try
			{
				returnValue = this.estimation.CompareTo( ((EstimatedObject)obj).Estimation );
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
			return returnValue;
		}
	}
}
