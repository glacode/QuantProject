/*
QuantProject - Quantitative Finance Library

Set.cs
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
using System.Collections;

namespace QuantProject.ADT.Collections
{
	/// <summary>
	/// A mathematical set
	/// </summary>
	[Serializable]
	public class Set
	{
		// TO DO: it should implement ICollection
		private Hashtable elements;

		public Set()
		{
			this.elements = new Hashtable();
		}
		/// <summary>
		/// Adds a new 
		/// </summary>
		/// <param name="obj"></param>
		public void Add( object element )
		{
			if ( !this.Contains( element ) )
				this.elements.Add( element , null );
		}
		/// <summary>
		/// Checks if element belongs to this set
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public bool Contains( object element )
		{
			return this.elements.ContainsKey( element );
		}
		
		/// <summary>
		/// removes all elements in the set
		/// </summary>
		public void Clear()
		{
			this.elements.Clear();
		}
	}
}
