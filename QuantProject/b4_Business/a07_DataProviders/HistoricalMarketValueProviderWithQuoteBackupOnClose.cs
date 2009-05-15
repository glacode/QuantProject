/*
QuantProject - Quantitative Finance Library

HistoricalMarketValueProviderWithQuoteBackupOnClose.cs
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Returns market values from the given historicalMarketValueProvider, but
	/// if the requested market value is at market close, and the
	/// historicalMarketValueProvider doesn't have that quote, than
	/// the raw close is returned (from the quotes table)
	/// </summary>
	[Serializable]
	public class HistoricalMarketValueProviderWithQuoteBackupOnClose :
		HistoricalMarketValueProvider
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;

		private HistoricalQuoteProvider historicalQuoteProvider;
		
		public HistoricalMarketValueProviderWithQuoteBackupOnClose(
			HistoricalMarketValueProvider historicalMarketValueProvider,
		  HistoricalQuoteProvider historicalQuoteProviderBackUp)
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.historicalQuoteProvider = historicalQuoteProviderBackUp;
		}
		protected override string getDescription()
		{
			string description =
				this.historicalMarketValueProvider.Description + "_whtBckpOnCls";
			return description;
		}

		#region GetMarketValue
		private double getCloseIfTheCase( string ticker , DateTime dateTime )
		{
			double marketValue = double.NaN;
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				// dateTime is at market close
				marketValue = this.historicalQuoteProvider.GetMarketValue(
					ticker , dateTime );
			else
				// dateTime is not at market close
				throw new TickerNotExchangedException( ticker , dateTime );
			return marketValue;
		}
		public override double GetMarketValue( string ticker , DateTime dateTime )
		{
			double marketValue;
			if ( this.historicalMarketValueProvider.WasExchanged( ticker , dateTime ) )
				marketValue = this.historicalMarketValueProvider.GetMarketValue(
					ticker , dateTime );
			else
				// this.historicalMarketValueProvider doesn't have a market value for
				// the given ticker, at the given dateTime
				marketValue = this.getCloseIfTheCase( ticker , dateTime );
			return marketValue;
		}
		#endregion GetMarketValue
			
		
		#region WasExchanged
		
		private bool isCloseAnAvailableBackup( string ticker , DateTime dateTime )
		{
			bool isBackupAvailable = HistoricalEndOfDayTimer.IsMarketClose( dateTime );
			isBackupAvailable = isBackupAvailable &&
				this.historicalQuoteProvider.WasExchanged( ticker , dateTime );
			return isBackupAvailable;
		}
		public override bool WasExchanged( string ticker , DateTime dateTime )
		{
			bool wasExchanged = this.historicalMarketValueProvider.WasExchanged(
				ticker , dateTime );
			wasExchanged = wasExchanged ||
				this.isCloseAnAvailableBackup( ticker , dateTime );
			return wasExchanged;
		}
		#endregion WasExchanged
	}
}
