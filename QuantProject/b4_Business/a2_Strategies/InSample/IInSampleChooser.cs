/*
QuantProject - Quantitative Finance Library

IInSampleChooser.cs
Copyright (C) 2007
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

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// Interface for classes that perform in sample researches (usually optimization)
	/// </summary>
	public interface IInSampleChooser : IProgressNotifier , IMessageSender , ILogDescriptor
	{
		/// <summary>
		/// Analizes in sample data and returns an object
		/// </summary>
		/// <param name="eligibleTickers">eligible tickers for the in sample analysis</param>
		/// <param name="returnsManager">manager to efficiently handle in sample</param>
		/// <returns>interesting data (usually an optimization's optimal result) to be used
		/// to take decisions out of sample</returns>
		object AnalyzeInSample( EligibleTickers eligibleTickers ,
		                      ReturnsManager returnsManager );
////		                      EndOfDayDateTime currentOutOfSampleEndOfDayDateTime );
//		/// <summary>
//		/// Short description for the chooser
//		/// (it might be used in file names describing the strategy)
//		/// </summary>
//		string Description { get; }
	}
}
