/*
QuantProject - Quantitative Finance Library

DummyTesterForTestingPositions.cs
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
using System.Windows.Forms;

using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.General.Logging
{
	[Serializable]
	/// <summary>
	/// It doesn't test a TestingPositions object:
	/// its only function is to show the TestingPositions
	/// through a ExecutablesListViewer
	/// </summary>
	public class DummyTesterForTestingPositions : IExecutable
	{
		private TestingPositions testingPositions;
		private int numberOfInSampleDays;
		private EndOfDayDateTime endOfDayDateTimeWhenThisObjectWasLogged;
		
		/// <summary>
		/// Generation when the TestingPositions object has been created
		/// (if genetically optimized)
		/// </summary>
		public int Generation
		{
			get
			{
				int generation = -1;
				if( this.testingPositions is IGeneticallyOptimizable )
					generation = 
						((IGeneticallyOptimizable)this.testingPositions).Generation;
				return generation;
			}
		}
		public double FitnessInSample
		{
			get { return this.testingPositions.FitnessInSample; }
		}
		public string ShortDescription
		{
			get { return this.testingPositions.WeightedPositions.Description; }
		}
		public DummyTesterForTestingPositions(
			TestingPositions testingPositions ,
			int numberOfInSampleDays ,
			EndOfDayDateTime endOfDayDateTimeWhenThisObjectWasLogged )
		{
			this.testingPositions = testingPositions;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.endOfDayDateTimeWhenThisObjectWasLogged =
				endOfDayDateTimeWhenThisObjectWasLogged;
		}
						
		public void Run()
		{
			MessageBox.Show("No run has been implemented for " +
			                "this kind of object");
		}
	}
}
