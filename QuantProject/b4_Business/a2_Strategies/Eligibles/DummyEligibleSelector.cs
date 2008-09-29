/*
QuantProject - Quantitative Finance Library

DummyEligibleSelector.cs
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
using System.Data;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Selects no tickers at all. To be used for log item's
	/// debugging
	/// </summary>
	[Serializable]
	public class DummyEligibleSelector : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;

		public string Description
		{
			get
			{
				return "Elgbls_dummy";
			}
		}

		/// <summary>
		/// Returns an empty EligibleTickers object
		/// </summary>
		/// <param name="groupID"></param>
		public DummyEligibleSelector()
		{
		}
		public EligibleTickers GetEligibleTickers(
			History history )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , new NewMessageEventArgs( "NewEligibles" ) );
			EligibleTickers eligibleTickers =
				new EligibleTickers( new DataTable() );
//			eligibleTickers = eligibleTickers;
			return eligibleTickers;
		}
	}
}
