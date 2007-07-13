/*
QuantProject - Quantitative Finance Library

FloatArrayManager.cs
Copyright (C) 2006
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

namespace QuantProject.ADT.Collections
{
	/// <summary>
	/// This class provides functions to manage array of floats
	/// </summary>
	public class FloatArrayManager
	{
		public FloatArrayManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region GetRatios
		private static float getRatio( float[] values , int i )
		{
			return ( values[ i + 1 ] / values[ i ] );
		}
		/// <summary>
		/// If values contains n elements (V0, V1, ... Vn-1),
		/// GetRatios returns n-1 elements (E0, E1, ... En-2) where
		/// Ei = Vi+1 / Vi, for all i=0,1,...n-2
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public static float[] GetRatios( float[] values )
		{
			float[] ratios = new float[ values.Length - 1 ];
			for ( int i=0 ; i < values.Length ; i++ )
				ratios[ i ] = getRatio( values , i );
			return ratios;
		}
		#endregion
	}
}
