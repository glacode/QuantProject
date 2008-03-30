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
		private int numberOfTopBestParameters;

		private int numberOfAnalizedItemsForNewProgess;
		private int totalNumberOfItemsToBeAnalized;

//		private BruteForceOptimizableParameters bestParameters;
		private BestParametersManager bestParametersManager;

		private int analizedItems;
		private bool runIsComplete;

		public event NewProgressEventHandler NewProgress;

		public BruteForceOptimizableParameters BestParameters
		{
			get { return this.TopBestParameters[ 0 ]; }
		}
		public BruteForceOptimizableParameters[] TopBestParameters
		{
			get
			{
				if ( !this.runIsComplete )
					throw new Exception( "This property cannot be invoked until " +
						"the Run execution is complete!" );
				return this.bestParametersManager.TopBestParameters;
			}
		}


		/// <summary>
		/// Optimizer that enumerates all possible parameter values
		/// </summary>
		/// <param name="bruteForceOptimizableParametersManager"></param>
		/// <param name="numberOfTopBestParameters">number of the best
		/// optimizable parameters to be returned by the TopBestParameters
		/// property</param>
		public BruteForceOptimizer(
			IBruteForceOptimizableParametersManager bruteForceOptimizableParametersManager ,
			int numberOfTopBestParameters )
		{
			this.bruteForceOptimizableParametersManager =
				bruteForceOptimizableParametersManager;
			this.numberOfTopBestParameters = numberOfTopBestParameters;

			this.numberOfAnalizedItemsForNewProgess = 100000;
			this.totalNumberOfItemsToBeAnalized = 0;
			this.runIsComplete = false;
		}
		#region Run
		private void handleProgress()
		{
			this.analizedItems ++ ;
			if ( analizedItems % this.numberOfAnalizedItemsForNewProgess == 0 )
			{
				NewProgressEventArgs newProgressEventArgs =
					new NewProgressEventArgs( analizedItems ,
					this.totalNumberOfItemsToBeAnalized );
				if ( this.NewProgress != null )
					this.NewProgress( this , newProgressEventArgs );
			}
		}

		#region Run
//		private void handleCurrentOptimizableParameters(
//			BruteForceOptimizableParameters bruteForceOptimizableParameters )
//		{
//			if ( !this.topBestParameters.IsFull )
//				// this.topBestParameters still contains less than
//				// this.numberOfTopBestParametersToBeReturned items. It means that we are still in
//				// the initializing phase, when the first this.numberOfTopBestParametersToBeReturned
//				// items are to be added to the topBestParameters
//				this.topBestParameters.Analize( bruteForceOptimizableParameters )
//					else if ( this.topBestParameters.LowestFitness < bruteForceOptimizableParameters.Fitness )
//				this.updateTopBestParameters( bruteForceOptimizableParameters );
//		}
		public void createTopBestParameters()
		{
			this.bestParametersManager = new BestParametersManager(
				this.numberOfTopBestParameters ,
				this.bruteForceOptimizableParametersManager );
			this.analizedItems = 0;
			this.bruteForceOptimizableParametersManager.Reset();
//			this.bestParameters =
//				(BruteForceOptimizableParameters)this.bruteForceOptimizableParametersManager.Current;
			do
			{
				BruteForceOptimizableParameters bruteForceOptimizableParameters =
					(BruteForceOptimizableParameters)this.bruteForceOptimizableParametersManager.Current;
				this.bestParametersManager.Analize( bruteForceOptimizableParameters );
				this.handleProgress();
				
			}while (this.bruteForceOptimizableParametersManager.MoveNext());
		}
		private void sortTopBestParametersDescending()
		{
			this.bestParametersManager.SortTopBestParametersDescending();
		}
		/// <summary>
		/// executes the optimization
		/// </summary>
		public void Run()
		{
			this.createTopBestParameters();
			this.sortTopBestParametersDescending();
//			this.bestParametersManager.TopBestParameters.Sort();
			this.runIsComplete = true;
		}
		#endregion Run

		/// <summary>
		/// executes the optimization
		/// </summary>
		public void Run( int numberOfAnalizedItemsForNewProgess ,
			int totalNumberOfItemsToBeAnalized )
		{
			this.numberOfAnalizedItemsForNewProgess =
				numberOfAnalizedItemsForNewProgess;
			this.totalNumberOfItemsToBeAnalized =
				totalNumberOfItemsToBeAnalized;
			this.Run();
		}
		#endregion
	}
}
