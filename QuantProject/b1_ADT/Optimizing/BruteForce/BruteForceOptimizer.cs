/*
QuantProject - Quantitative Finance Library

BruteForceOptimizer.cs
Copyright (C) 2006
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

using QuantProject.ADT;

namespace QuantProject.ADT.Optimizing.BruteForce
{
	/// <summary>
	/// Optimizer that enumerates all possible parameter values
	/// </summary>
	public class BruteForceOptimizer : IProgressNotifier
	{
		private IBruteForceOptimizableParametersManager
			bruteForceOptimizableParametersManager;

		private BruteForceOptimizableParameters bestParameters;

		private int analizedItems;

		public event NewProgressEventHandler NewProgress;

		public BruteForceOptimizableParameters BestParameters
		{
			get { return this.bestParameters; }
		}

		public BruteForceOptimizer(
			IBruteForceOptimizableParametersManager bruteForceOptimizableParametersManager )
		{
			this.bruteForceOptimizableParametersManager =
				bruteForceOptimizableParametersManager;
		}
		#region Run
		private void handleProgress()
		{
			analizedItems ++ ;
			if ( analizedItems % 100000 == 0 )
			{
				NewProgressEventArgs newProgressEventArgs =
					new NewProgressEventArgs( analizedItems ,
					0 );
				if ( this.NewProgress != null )
					this.NewProgress( this , newProgressEventArgs );
			}
		}
		/// <summary>
		/// executes the optimization
		/// </summary>
		public void Run()
		{
			this.analizedItems = 0;
			this.bruteForceOptimizableParametersManager.Reset();
			this.bestParameters =
				(BruteForceOptimizableParameters)this.bruteForceOptimizableParametersManager.Current;
			while( this.bruteForceOptimizableParametersManager.MoveNext() )
			{
				BruteForceOptimizableParameters bruteForceOptimizableParameter =
					(BruteForceOptimizableParameters)this.bruteForceOptimizableParametersManager.Current;
				if ( this.bestParameters.Fitness < bruteForceOptimizableParameter.Fitness )
					this.bestParameters = bruteForceOptimizableParameter;
				this.handleProgress();
			}
		}
		#endregion
	}
}
