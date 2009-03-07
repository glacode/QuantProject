/*
QuantProject - Quantitative Finance Library

PairsViewer.cs
Copyright (C) 2009
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
using System.Drawing;
using System.Windows.Forms;

using QuantProject.ADT.Histories;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Form used to display two pairs trading candidates
	/// </summary>
	public partial class PairsViewer : Form
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private	WeightedPosition firstWeightedPosition;
		private	WeightedPosition secondWeightedPosition;
		
		public PairsViewer(
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			WeightedPosition firstWeightedPosition ,
			WeightedPosition secondWeightedPosition ,
			DateTime firstDateTime )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.firstWeightedPosition = firstWeightedPosition;
			this.secondWeightedPosition = secondWeightedPosition;
			this.dateTimePicker.Value = firstDateTime;
		}
		
		
		#region ButtonShowClick
		
		#region getHistory
		
		#region addItemsToHistory
		private DateTime getFirstDateTime()
		{
			DateTime firstDateTime =
				this.dateTimePicker.Value.AddMinutes( -10 );
//				HistoricalEndOfDayTimer.GetMarketOpen(
//					this.dateTimePicker.Value.AddDays( 1 ) );
			return firstDateTime;
		}
		private DateTime getLastDateTime()
		{
			DateTime firstDateTime = this.getFirstDateTime();
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstDateTime );
			return lastDateTime;
		}
		private void addItemToHistory(
			WeightedPosition weightedPosition , DateTime dateTime , History history )
		{
			try
			{
				double marketValue = this.historicalMarketValueProvider.GetMarketValue(
					weightedPosition.Ticker , dateTime );
				history.Add( dateTime , marketValue );
			}
			catch( TickerNotExchangedException tickerNotExchangedException )
			{
				string toAvoidWarning = tickerNotExchangedException.Message;
			}
		}
		private void addItemsToHistory( WeightedPosition weightedPosition , History history )
		{
			DateTime currentDateTime = this.getFirstDateTime();
			DateTime lastDateTime = this.getLastDateTime();
			while ( currentDateTime <= lastDateTime )
			{
				this.addItemToHistory( weightedPosition , currentDateTime , history );
				currentDateTime = currentDateTime.AddMinutes( 1 );
			}
		}
		#endregion addItemsToHistory
		
		private History getHistory( WeightedPosition weightedPosition )
		{
			History history = new History();
			this.addItemsToHistory( weightedPosition , history );
			return history;
		}
		#endregion getHistory
		
		private void showHistoriesPlots(
			History historyForFirstPosition , History historyForSecondPosition )
		{
			HistoriesViewer historiesViewer = new HistoriesViewer();
			historiesViewer.Add( historyForFirstPosition , Color.Green );
			historiesViewer.Add( historyForSecondPosition , Color.Red );
			historiesViewer.Show();
		}
		
		
		void ButtonShowClick(object sender, EventArgs e)
		{
			History historyForFirstPosition = this.getHistory( this.firstWeightedPosition );
			History historyForSecondPosition = this.getHistory(	this.secondWeightedPosition );
			this.showHistoriesPlots( historyForFirstPosition , historyForSecondPosition );
		}
		#endregion ButtonShowClick
	}
}
