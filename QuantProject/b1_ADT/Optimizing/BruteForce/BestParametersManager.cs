/*
QuantProject - Quantitative Finance Library

BestParametersManager.cs
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

using QuantProject.ADT.Optimizing.Fitness;

namespace QuantProject.ADT.Optimizing.BruteForce
{
	/// <summary>
	/// Keeps and manages the array of BruteForceOptimizableParameters that
	/// have the highest fitness
	/// </summary>
	public class BestParametersManager
	{
		private int numberOfTopBestParametersToBeReturned;
		private IBruteForceOptimizableParametersManager
			bruteForceOptimizableParametersManager;

		private int numberOfNonNullItemsInTopBestParameters;
		private int indexForTheCurrentCandidateToBeSwappedOut;

		private BruteForceOptimizableParameters[] topBestParameters;

		public BruteForceOptimizableParameters[] TopBestParameters
		{
			get
			{
				return this.topBestParameters;
			}
		}

		/// <summary>
		/// Keeps and manages the array of BruteForceOptimizableParameters that
		/// have the highest fitness
		/// </summary>
		/// <param name="numberOfTopBestParametersToBeReturned">number of
		/// BruteForceOptimizableParameters that are going to be returned, as
		/// the ones with the highest fitness</param>
		public BestParametersManager(
			int numberOfTopBestParametersToBeReturned ,
			IBruteForceOptimizableParametersManager
			bruteForceOptimizableParametersManager )
		{
			this.numberOfTopBestParametersToBeReturned =
				numberOfTopBestParametersToBeReturned;
			this.bruteForceOptimizableParametersManager =
				bruteForceOptimizableParametersManager;

			this.topBestParameters =
				new BruteForceOptimizableParameters[ numberOfTopBestParametersToBeReturned ];
			this.numberOfNonNullItemsInTopBestParameters = 0;
			this.indexForTheCurrentCandidateToBeSwappedOut = 0;
		}

		#region Analize

		#region hasToBeInsertedIntoTheTopBestParameters
		private bool isBetterThanTheCurrentCandidateToBeSwappedOut(
			BruteForceOptimizableParameters bruteForceOptimizableParameters )
		{
			bool isBetter = true;
			BruteForceOptimizableParameters currentCandidateToBeSwappedOut =
				this.topBestParameters[ this.indexForTheCurrentCandidateToBeSwappedOut ];
			if ( currentCandidateToBeSwappedOut != null )
				// the initialization phase has been complete: the first
				// this.numberOfTopBestParametersToBeKept items has been added to
				// this.topBestParameters
				isBetter = ( bruteForceOptimizableParameters.Fitness >
					currentCandidateToBeSwappedOut.Fitness );
			return isBetter;
		}
		private bool isEquivalentAlreadyInTopBestParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters )
		{
			int indexForCurrentTopBestParameters = 0;
			while (
				indexForCurrentTopBestParameters < this.numberOfNonNullItemsInTopBestParameters &&
				!this.bruteForceOptimizableParametersManager.AreEquivalentAsTopBestParameters(
				bruteForceOptimizableParameters ,
				this.topBestParameters[ indexForCurrentTopBestParameters ] ) )
				indexForCurrentTopBestParameters++;
			bool isEquivalentAlreadyPresent =
				( indexForCurrentTopBestParameters < this.numberOfNonNullItemsInTopBestParameters );
			return isEquivalentAlreadyPresent;
		}
		private bool hasToBeInsertedIntoTheTopBestParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters )
		{
			bool hasToBeInserted =
				( this.isBetterThanTheCurrentCandidateToBeSwappedOut(
				bruteForceOptimizableParameters ) &&
				!this.isEquivalentAlreadyInTopBestParameters(
				bruteForceOptimizableParameters ) );
			return hasToBeInserted;
		}
		#endregion hasToBeInsertedIntoTheTopBestParameters

		// true iif we are still in the initialization phase, when the
		// first this.numberOfTopBestParametersToBeReturned items have already
		// been added to this.topBestParameters
		private bool isStillTheInitializationPhase()
		{
			bool isStillInitialization =
				( this.numberOfNonNullItemsInTopBestParameters <
				this.numberOfTopBestParametersToBeReturned );
			return isStillInitialization;
		}
		private void update_numberOfNonNullItemsInTopBestParameters()
		{
			if ( this.isStillTheInitializationPhase() )
				this.numberOfNonNullItemsInTopBestParameters++;
		}

		#region updateIndexForTheCurrentCandidateToBeSwappedOut
		private void updateIndexForTheCurrentCandidateToBeSwappedOut_ifTheCase(
			int indexForCurrentItemInTopBestParameters )
		{
			BruteForceOptimizableParameters currentItemInTopBestParametes =
				this.topBestParameters[ indexForCurrentItemInTopBestParameters ];
			if ( !this.isBetterThanTheCurrentCandidateToBeSwappedOut(
				currentItemInTopBestParametes ) )
				// the current candidate to be swapped out is better (or equally good)
				// than the current item in this.topBestParameters

				// the current item becomes the candidate to be swapped out
				this.indexForTheCurrentCandidateToBeSwappedOut =
					indexForCurrentItemInTopBestParameters;
		}
		private int getIndexForTheWorstAmongTheBestParameters()
		{
			int indexForTheWorstAmongTheBestParameters = 0;
			for ( int indexForTheCurrentItemInTopBestParameters = 1 ;
				indexForTheCurrentItemInTopBestParameters < this.topBestParameters.Length ;
				indexForTheCurrentItemInTopBestParameters++ )
			{
				BruteForceOptimizableParameters currentWorstAmongTheBestParameters =
					this.topBestParameters[ indexForTheWorstAmongTheBestParameters ];
				BruteForceOptimizableParameters currentItemInTopBestParametes =
					this.topBestParameters[ indexForTheCurrentItemInTopBestParameters ];
				if ( currentItemInTopBestParametes.Fitness <
					currentWorstAmongTheBestParameters.Fitness )
					indexForTheWorstAmongTheBestParameters =
						indexForTheCurrentItemInTopBestParameters;
			}
			return indexForTheWorstAmongTheBestParameters;
		}
		private void updateIndexForTheCurrentCandidateToBeSwappedOut_afterTheInitializationPhase()
		{			
			this.indexForTheCurrentCandidateToBeSwappedOut =
				this.getIndexForTheWorstAmongTheBestParameters();
		}
		private void updateIndexForTheCurrentCandidateToBeSwappedOut()
		{
//			this.indexForTheCurrentCandidateToBeSwappedOut = 0;
//			int indexForCurrentItemInTopBestParameters = 1;
//			int indexForTheFirstNullItemInTopBestParameters = -1;
			if ( this.isStillTheInitializationPhase() )
				this.indexForTheCurrentCandidateToBeSwappedOut =
					this.numberOfNonNullItemsInTopBestParameters;
			else
				// the initialization phase is complete: the first
				// this.numberOfTopBestParametersToBeReturned items have already
				// been added to this.topBestParameters
				this.updateIndexForTheCurrentCandidateToBeSwappedOut_afterTheInitializationPhase();
		}
		#endregion updateIndexForTheCurrentCandidateToBeSwappedOut

		/// <summary>
		/// replaces the current candidate to be swapped out with the given
		/// bruteForceOptimizableParameters
		/// </summary>
		/// <param name="bruteForceOptimizableParameters"></param>
		private void replaceTheCurrentCandidateToBeSwappedOut( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			this.topBestParameters[ this.indexForTheCurrentCandidateToBeSwappedOut ] =
				bruteForceOptimizableParameters;
//			this.topBestParameters[ 0 ] =
//				bruteForceOptimizableParameters;
			this.update_numberOfNonNullItemsInTopBestParameters();
			this.updateIndexForTheCurrentCandidateToBeSwappedOut();
		}
		/// <summary>
		/// Checks if bruteForceOptimizableParameters needs to be added to
		/// this.topBestParameters
		/// If it has to be added, then this.topBestParameters is managed
		/// accordingly
		/// </summary>
		/// <param name="bruteForceOptimizableParameters"></param>
		public void Analize( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			if ( this.hasToBeInsertedIntoTheTopBestParameters( bruteForceOptimizableParameters ) )
				this.replaceTheCurrentCandidateToBeSwappedOut(
					bruteForceOptimizableParameters );
		}
		#endregion Handle
		
		/// <summary>
		/// Sortes the top best parameters descending. To be invoked when
		/// all BruteForceOptimizableParameters have been analyzed
		/// </summary>
		public void SortTopBestParametersDescending()
		{
			// comment out the following three lines if you have an exception
			// in the Sort method and you want to break to the proper line
//				double fitness;
//				for ( int i = 0 ; i < this.topBestParameters.Length ; i++ )
//					fitness = ((BruteForceOptimizableParameters) this.topBestParameters[ i ]).Fitness;

			FitnessComparer fitnessComparer = new FitnessComparer();
			Array.Sort( this.topBestParameters , fitnessComparer );
			Array.Reverse( this.topBestParameters );
		}
	}
}
