/*
QuantProject - Quantitative Finance Library

LinearRegressionNew.cs
Copyright (C) 2010
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

namespace QuantProject.ADT.LinearAlgebra
{
	/// <summary>
	/// Solves a linear system using the Gauss elimination algorithm
	/// </summary>
	public class LinearSystemSolver
	{
		public LinearSystemSolver()
		{
		}
		
		#region FindSolution
		private static void findSolution_checkParameters( double[,] A , double[] b )
		{
			if ( A.GetLength( 0 ) != b.Length )
				throw new Exception(
					"A and b don't have the same number of rows!" );
		}
		
		private static void createACopyOfTheParameters(
				double[,] originalA , double[] original_b , out double[,] A , out double[] b )
		{
			int n = original_b.Length;
			A = new double[ n , n ];
			b = new double[ n ];
			for( int i = 0 ; i < n ; i++ )
			{
				for( int j = 0 ; j < n ; j++ )
					A[ i , j ] = originalA[ i , j ];
				b[ i ] = original_b[ i ];
			}
		}
		
		#region findSolution
		
		#region forwardElimination
		private static int findTheRowWithTheLargestFirstValue(
			int currentRowIndex , double[,] A , double[] b )
		{
			int rowWithTheLargestFirstValue = currentRowIndex;
			for ( int i = currentRowIndex + 1 ; i < b.Length ; i++ )
				if ( Math.Abs( A[ i , currentRowIndex ] ) >
				    Math.Abs( A[ rowWithTheLargestFirstValue , currentRowIndex ] ) )
				rowWithTheLargestFirstValue = i;
			return rowWithTheLargestFirstValue;
		}
		private static void checkIfSingular( int currentRowIndex , double[,] A )
		{
			if ( Math.Abs( A[ currentRowIndex , currentRowIndex ] ) < 0.000001 )
				throw new Exception(
					"The system doesn't have a single solution!" );
		}
		private static void swapRows(
			int index1 , int index2 , double[,] A , double[] b )
		{
			for( int j = 0 ; j < b.Length ; j++ )
			{
				double temp = A[ index1 , j ];
				A[ index1 , j ] = A[ index2 , j ];
				A[ index2 , j ] = temp;
				temp = b[ index1 ];
				b[ index1 ] = b[ index2 ];
				b[ index2 ] = temp;
			}
		}
		private static void normalizeTheCurrentRow( int currentRowIndex , double[,] A , double[] b )
		{
			double pivotValue = A[ currentRowIndex , currentRowIndex ];
			for( int j = 0 ; j < b.Length ; j++ )
				A[ currentRowIndex , j ] = A[ currentRowIndex , j ] / pivotValue;
			b[ currentRowIndex ] = b[ currentRowIndex ] / pivotValue;
		}
		
		#region reduceTheFollowingRows
		private static void reduceTheFollowingRow(
			int rowToBeReduced , int indexOfThePivotRow , double[,] A , double[] b )
		{
			double pivotValue = A[ rowToBeReduced , indexOfThePivotRow ];
			for( int j = indexOfThePivotRow ; j < b.Length ; j++ )
				A[ rowToBeReduced , j ] -= pivotValue * A[ indexOfThePivotRow , j ];
			b[ rowToBeReduced ] -= pivotValue * b[ indexOfThePivotRow ];
		}
		private static void reduceTheFollowingRows( int currentRowIndex , double[,] A , double[] b )
		{
			for( int i = currentRowIndex + 1 ; i < b.Length ; i++ )
				LinearSystemSolver.reduceTheFollowingRow( i , currentRowIndex , A , b );
		}
		
		#endregion reduceTheFollowingRows
		private static void forwardElimination(
			int currentRowIndex , double[,] A , double[] b )
		{
			int indexOfTheRowWithLargestFirstValue =
				LinearSystemSolver.findTheRowWithTheLargestFirstValue( currentRowIndex , A , b );
			LinearSystemSolver.swapRows(
				currentRowIndex , indexOfTheRowWithLargestFirstValue , A , b );
			LinearSystemSolver.checkIfSingular( currentRowIndex , A );
			LinearSystemSolver.normalizeTheCurrentRow( currentRowIndex , A , b );
			LinearSystemSolver.reduceTheFollowingRows( currentRowIndex , A , b );
		}
		private static void forwardElimination( double[,] A , double[] b )
		{
			for( int i = 0 ; i < b.Length ; i++ )
				LinearSystemSolver.forwardElimination( i , A , b );
		}
		#endregion forwardElimination

		#region backSubstitution
		private static double computeValueToBeSubtracted( double[,] A , double[] b , int i , double[] solution )
		{
			double valueToBeSubtracted = 0;
			for( int j = i+1 ; j < b.Length ; j++ )
				valueToBeSubtracted += A[ i , j ] *  solution[ j ];
			return valueToBeSubtracted;
		}
		private static void backSubstitution( double[,] A , double[] b , int i , double[] solution )
		{
			double valueToBeSubtracted =
				LinearSystemSolver.computeValueToBeSubtracted( A , b , i , solution );
			solution[ i ] = b[ i ] - valueToBeSubtracted;
		}
		private static double[] backSubstitution( double[,] A , double[] b )
		{
			double[] solution = new double[ b.Length ];
			for( int i = b.Length - 1 ; i >= 0 ; i-- )
				LinearSystemSolver.backSubstitution( A , b , i , solution );
			return solution;
		}
		#endregion backSubstitution
		
		private static double[] findSolution( double[,] A , double[] b )
		{			
			LinearSystemSolver.forwardElimination( A , b );
			double[] solution = LinearSystemSolver.backSubstitution( A , b );
			return solution;
		}
		#endregion findSolution

		/// <summary>
		/// Computes the solution of the linear system Ax=b
		/// </summary>
		/// <param name="A"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double[] FindSolution( double[,] originalA , double[] original_b )
		{
			double[,] A; double[] b;
			LinearSystemSolver.findSolution_checkParameters( originalA , original_b );
			LinearSystemSolver.createACopyOfTheParameters(
				originalA , original_b , out A , out b );
			double[] solution = LinearSystemSolver.findSolution( A , b );
			return solution;
		}
		#endregion FindSolution
	}
}
