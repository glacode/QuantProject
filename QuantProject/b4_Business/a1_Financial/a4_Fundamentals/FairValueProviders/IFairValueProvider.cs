/*
QuantProject - Quantitative Finance Library

IFairValueProvider.cs
Copyright (C) 2010
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;
using QuantProject.Business.Strategies.Logging;

namespace QuantProject.Business.Financial.Fundamentals.FairValueProviders
{
	/// <summary>
	/// Interface to be implemented by objects implementing a model which computes
	/// the fair value for a given ticker at a given date, using 
	/// available fundamental data since another previous date
	/// </summary>
	public interface IFairValueProvider : ILogDescriptor
	{
		void Run(DateTime dateOfFairValueComputation);
		//for IFairValueProvider objects of some complexity,
		//a run method may be required before
		//calling GetFairValue, for providing
		//the actual computation of the fair value
		double GetFairValue( string ticker ,
		                     DateTime dateOfFairValueComputation );
	}
}
