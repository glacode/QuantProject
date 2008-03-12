/*
QuantProject - Quantitative Finance Library

PVO_CTCCorrelationChooser.cs
Copyright (C) 2008
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.TickersRelationships; 
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers
{
	/// <summary>
	/// PVO_CTCCorrelationChooser to be used for
	/// in sample optimization
	/// By means of correlation, the AnalyzeInSample method returns the 
	/// requested number of PVOPositions (positions for the PVO strategy)
	/// </summary>
	public class PVO_CTCCorrelationChooser : PVOCorrelationChooser
	{
		/// <summary>
		/// PVO_CTCCorrelationChooser to be used for
		/// in sample optimization
		/// </summary>
		/// <param name="numberOfBestTestingPositionsToBeReturned">
		/// The number of PVOPositions that the
		/// AnalyzeInSample method will return
		/// </param>
		public PVO_CTCCorrelationChooser(int numberOfBestTestingPositionsToBeReturned,
		                                 int closeToCloseReturnIntervalLength) :
																		 base(numberOfBestTestingPositionsToBeReturned,
			     															  closeToCloseReturnIntervalLength)
		{
			
		}
				
		protected override void setCorrelationProvider(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			DateTime firstDate = returnsManager.ReturnIntervals[0].Begin.DateTime;
			DateTime lastDate =  returnsManager.ReturnIntervals.LastEndOfDayDateTime.DateTime;
			this.correlationProvider =
				new CloseToCloseCorrelationProvider(eligibleTickers.Tickers, firstDate,
				                                    lastDate, this.numDaysForOscillatingPeriod,
				                                    0.0001f, 0.5f, "^GSPC");
		}
	}
}

