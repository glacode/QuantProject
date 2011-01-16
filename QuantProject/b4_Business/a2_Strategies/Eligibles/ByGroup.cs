/*
QuantProject - Quantitative Finance Library

ByGroup.cs
Copyright (C) 2011
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
using System.Data;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.Data.Selectors;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Implements IEligiblesSelector for selecting all the tickers
	/// belonging to a given group
	/// </summary>
	[Serializable]
	public class ByGroup : IEligiblesSelector
	{
		public event NewMessageEventHandler NewMessage;
		
		private bool temporizedGroup;
		private string tickersGroupID;
		
		public string Description
		{
			get{
				return "From_" + this.tickersGroupID + " (temporized: " +
					this.temporizedGroup.ToString() + ")";
			}
		}
				
		public ByGroup(
			string tickersGroupID , bool temporizedGroup)
		{
			this.temporizedGroup = temporizedGroup;
			this.tickersGroupID = tickersGroupID;
		}

		private EligibleTickers getEligibleTickers_actually(
			History history )
		{
			DateTime currentDate = history.LastDateTime; 

			SelectorByGroup group;
			if(this.temporizedGroup)
			//the group is "temporized": returned set of tickers
			// depend on time
				group = new SelectorByGroup(this.tickersGroupID,
				                            currentDate);
      else//the group is not temporized
      	group = new SelectorByGroup(this.tickersGroupID);
      DataTable tickersFromGroup = group.GetTableOfSelectedTickers();

//			DataSet dataSet = new DataSet();
//			dataSet.Tables.Add( dataTableMostLiquid );
//			dataSet.WriteXml( "c:\\qpReports\\pairsTrading\\eligiblesCon_ByPriceMostLiquidAlwaysQuoted.xml" );

      return
				new EligibleTickers( tickersFromGroup );
		}
		
		private void getEligibleTickers_sendNewMessage(
			EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if(this.NewMessage != null)
				this.NewMessage( this , newMessageEventArgs );
		}
		
		/// <summary>
		/// Returns the eligible tickers
		/// </summary>
		/// <returns></returns>
		public EligibleTickers GetEligibleTickers(
			History history )
		{
			EligibleTickers eligibleTickers =
				this.getEligibleTickers_actually( history );
			this.getEligibleTickers_sendNewMessage( eligibleTickers );
			return eligibleTickers;
		}
	}
}
