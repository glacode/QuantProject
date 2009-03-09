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
			DateTime dateTime )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.firstWeightedPosition = firstWeightedPosition;
			this.secondWeightedPosition = secondWeightedPosition;
			this.dateTimePickerForFirstDateTime.Value =
				this.initializeFirstDateTime( dateTime );
			this.textBoxGreenTicker.Text = firstWeightedPosition.Ticker;
			this.textBoxRedTicker.Text = secondWeightedPosition.Ticker;
			
			this.dateTimePickerForLastDateTime.Value = this.initializeLastDateTime();
		}
		
		private DateTime initializeFirstDateTime( DateTime dateTime )
		{
			DateTime firstDateTime = dateTime.AddMinutes( -10 );
//				HistoricalEndOfDayTimer.GetMarketOpen(
//					this.dateTimePicker.Value.AddDays( 1 ) );
			return firstDateTime;
		}
		private DateTime initializeLastDateTime()
		{
			DateTime firstDateTime = this.dateTimePickerForFirstDateTime.Value;
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstDateTime );
			return lastDateTime;
		}
		
		#region ButtonShowClick
		
		#region getHistory
		
		#region addItemsToHistory

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
			DateTime currentDateTime = this.dateTimePickerForFirstDateTime.Value;
			DateTime lastDateTime = this.dateTimePickerForLastDateTime.Value;
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
		
		
		#region showHistoriesPlots
		private HistoriesViewer showHistoriesPlots(
			History firstHistory , History secondHistory , string formTitle ,
			int xPosition , int yPosition )
		{
			HistoriesViewer historiesViewer =
				new HistoriesViewer( formTitle );
			historiesViewer.StartPosition = FormStartPosition.Manual;
			historiesViewer.Location = new Point( xPosition , yPosition );
			historiesViewer.Add( firstHistory , Color.Green );
			historiesViewer.Add( secondHistory , Color.Red );
			historiesViewer.Show();
			return historiesViewer;
		}
		private void showHistoriesPlots(
			History firstTickerMarketValues , History secondTickerMarketValues ,
			History firstTickerReturns , History secondTickerReturns )
		{
			HistoriesViewer historiesViewer = this.showHistoriesPlots(
				firstTickerMarketValues , secondTickerMarketValues , "Market Values ($)" ,
				this.Location.X + this.Size.Width , this.Location.Y );
			this.showHistoriesPlots(
				firstTickerReturns , secondTickerReturns , "Returns" ,
				historiesViewer.Location.X ,
				historiesViewer.Location.Y + historiesViewer.Size.Height );
		}
		#endregion showHistoriesPlots
		

		
		void ButtonShowClick(object sender, EventArgs e)
		{
			History firstTickerMarketValues = this.getHistory( this.firstWeightedPosition );
			History secondTickerMarketValues = this.getHistory(	this.secondWeightedPosition );
			ReturnsComputer returnsComputer =
				new ReturnsComputer( this.historicalMarketValueProvider );
			History firstTickerReturns =
				returnsComputer.GetReturns( this.firstWeightedPosition , firstTickerMarketValues );
			History secondTickerReturns =
				returnsComputer.GetReturns( this.secondWeightedPosition , secondTickerMarketValues );
			this.showHistoriesPlots(
				firstTickerMarketValues , secondTickerMarketValues ,
				firstTickerReturns , secondTickerReturns );
		}
		#endregion ButtonShowClick
	}
}
