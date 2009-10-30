/*
QuantProject - Quantitative Finance Library

GenomeManagerForWeightedOTC_EndOfDay.cs
Copyright (C) 2009 
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio with weights that best suites
	/// the Open To Close strategy using EOD data
	/// </summary>
	[Serializable]
	public class GenomeManagerForWeightedOTC_EndOfDay : GenomeManagerForOTC_EndOfDay
  {
    public GenomeManagerForWeightedOTC_EndOfDay(EligibleTickers eligibleTickers,
                           int numberOfTickersInPortfolio,
                           IDecoderForTestingPositions decoderForTestingPositions,
                           IFitnessEvaluator fitnessEvaluator,
                           GenomeManagerType genomeManagerType,
													 ReturnsManager returnsManager,
													 int seedForRandomGenerator)
                           :
														base(eligibleTickers,
														numberOfTickersInPortfolio,
														decoderForTestingPositions,
														fitnessEvaluator,		
														genomeManagerType,
														returnsManager,
														seedForRandomGenerator)
    {
			this.genomeSize = 2* numberOfTickersInPortfolio;
    }
  }
}
