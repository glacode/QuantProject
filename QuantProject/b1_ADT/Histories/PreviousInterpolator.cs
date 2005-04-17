/*
QuantProject - Quantitative Finance Library

PreviousInterpolator.cs
Copyright (C) 2004 
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

namespace QuantProject.ADT.Histories
{
	/// <summary>
	/// When a time series value lacks, it is interpolated with the previous available value
	/// </summary>
	public class PreviousInterpolator : IInterpolationMethod
	{
		/// <summary>
		/// Interpolation method: if value is undefined,
		/// returns the previous available value
		/// </summary>
		public PreviousInterpolator()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// If value is undefined, returns the previous available value
		/// </summary>
		/// <param name="history"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public Object GetValue( History history , DateTime dateTime )
		{
			Object returnValue;
			if ( history.ContainsKey( dateTime ) )
				returnValue = history.GetValue( dateTime );
			else
				returnValue = history.GetValue( (DateTime)history.GetKeyOrPrevious( dateTime ) );
			return returnValue;				
		}
	}
}
