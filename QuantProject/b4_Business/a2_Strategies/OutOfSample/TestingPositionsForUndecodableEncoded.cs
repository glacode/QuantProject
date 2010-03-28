/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 15/03/2010
 * Time: 16.46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuantProject.Business.Strategies.OutOfSample
{
	/// <summary>
	/// Object used to assign a dummy meaning to those objects (genomes or
	/// bruteForceOptimizableParameters) that
	/// cannot be decoded to a meaningful TestingPositions. It will be returned
	/// by an IDecoder when the Decode method fails
	/// </summary>
	[Serializable]
	public class TestingPositionsForUndecodableEncoded : TestingPositions , IGeneticallyOptimizable
	{
		private int generation;

		public int Generation
		{
			get { return this.generation; }
			set { this.generation = value; }
		}

		public TestingPositionsForUndecodableEncoded()
		{
		}
	}
}
