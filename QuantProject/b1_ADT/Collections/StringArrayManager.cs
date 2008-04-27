/*
QuantProject - Quantitative Finance Library

StringArrayManager.cs
Copyright (C) 2008
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
	/// This class provides methods to manage arrays of string
	/// </summary>
	public class StringArrayManager
	{
		public StringArrayManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region SubArray
		private static void subArray_checkParameters( string[] arrayOfString ,
			int firstPosition ,	int numberOfElementsToBeSelected )
		{
			if ( firstPosition < 0 )
				throw new Exception( "firstPosition is less than zero!" );
			if ( firstPosition >= arrayOfString.Length )
				throw new Exception( "firstPosition is >= than arrayOfInt.Length!" );
			if ( numberOfElementsToBeSelected < 0 )
				throw new Exception(
					"numberOfElementsToBeSelected is less than zero!" );
			if ( firstPosition + numberOfElementsToBeSelected > arrayOfString.Length )
				throw new Exception(
					"firstPosition + numberOfElementsToBeSelected is >= arrayOfInt.Length !!" );
		}

		/// <summary>
		/// Returns a subarray of arrayOfString
		/// </summary>
		/// <param name="arrayOfString"></param>
		/// <param name="firstPosition"></param>
		/// <param name="numberOfElementsToBeSelected"></param>
		/// <returns></returns>
		public static string[] SubArray( string[] arrayOfString , int firstPosition ,
			int numberOfElementsToBeSelected )
		{
			subArray_checkParameters( arrayOfString , firstPosition ,
				numberOfElementsToBeSelected );
			string[] subArray = new string[ numberOfElementsToBeSelected ];
			for ( int i = 0 ; i < numberOfElementsToBeSelected ; i++ )
			{
				int currentPosition = firstPosition + i;
				subArray[ i ] =	arrayOfString[ currentPosition ];
			}
			return subArray;
		}
		#endregion SubArray

	}
}
