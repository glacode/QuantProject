/*
QuantProject - Quantitative Finance Library

GenomeManipulator.cs
Copyright (C) 2003 
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
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Class providing static methods for manipulating Genomes
	/// (used by IGenomeManagerForEfficientPortfolio)
	/// </summary>
	[Serializable]
  public sealed class GenomeManipulator 
	{
		
    public static Random RandomGenerator; 
    private static int genomeSize;
		private static Genome[] childs;
		private static int[,] maskForChilds;
    
    static GenomeManipulator()
    {
	  	RandomGenerator = new Random(ConstantsProvider.SeedForRandomGenerator);
		  childs = new Genome[2];  	
		}
    
    private static void initializeStaticMembers(Genome parent1, Genome parent2)
    {
      genomeSize = parent1.Size;
      childs[0]=parent1.Clone();
      childs[1]=parent2.Clone();
      maskForChilds = new int[childs.Length, genomeSize];
      for(int i = 0; i<genomeSize; i++)
      {
      	maskForChilds[0,i]=1;
      	maskForChilds[1,i]=2;
      }
      //maskForChilds has been set in order to re-create
      //a copy of parents by using setChildsUsingMaskForChilds()
    }

    private static int[] genePositionsOfParent1NotPresentInParent2(Genome parent1,
                                                                   Genome parent2)
    {
      int[] returnValue = new int[parent1.Size];
      for(int i = 0; i < returnValue.Length; i++)
      {
        returnValue[i] = - 1;
        int geneValue = parent1.GetGeneValue(i);
        if(geneValue >= 0)
        {
          if(!parent2.HasGene(geneValue) &&
            !parent2.HasGene(-Math.Abs(geneValue) - 1))
            returnValue[i] = i;
        }
        else
        {
          if(!parent2.HasGene(geneValue) &&
            !parent2.HasGene(Math.Abs(geneValue) - 1))
            returnValue[i] = i;
        }
      }
      return returnValue;
    }
    /*old
    private static void setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		                                                           
    {
      int[] genePlacesOfParent1NotPresentInParent2 = 
        genePositionsOfParent1NotPresentInParent2(parent1, parent2);
      int[] genePlacesOfParent2NotPresentInParent1 = 
        genePositionsOfParent1NotPresentInParent2(parent2, parent1);
      bool justExchangedAtPreviousPosition = false;
      for(int i = 0;i<parent1.Size;i++)
      {
        if(!justExchangedAtPreviousPosition)
        //exchanges between genes of parents in childs
        //must follow an alternate pattern, in order to
        //avoid plain copy of parents in childs
        {  
        	if(genePlacesOfParent2NotPresentInParent1[i]!= - 1)
        	{
        		maskForChilds[0, i] = 2;//the change between genes
        		maskForChilds[1, i] = 1;//creates a child different from parents
            justExchangedAtPreviousPosition = true;
        	}
        	else if(genePlacesOfParent1NotPresentInParent2[i]!= - 1) //see 1st if
        	{
        		maskForChilds[0, i] = 1;
        		maskForChilds[1, i] = 2;
            justExchangedAtPreviousPosition = true;
        	}
        }
        else
          justExchangedAtPreviousPosition = false;
      }
    }
		*/
    private static void setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		                                                           
    {
      int[] genePlacesOfParent1NotPresentInParent2 = 
        genePositionsOfParent1NotPresentInParent2(parent1, parent2);
      int[] genePlacesOfParent2NotPresentInParent1 = 
        genePositionsOfParent1NotPresentInParent2(parent2, parent1);
      bool justExchangedAtPreviousPosition = false;
      for(int i = 0;i<parent1.Size;i++)
      {
        if(!justExchangedAtPreviousPosition)
          //exchanges between genes of parents in childs
          //must follow an alternate pattern, in order to
          //avoid plain copy of parents in childs when all genes
          //of the first parent are not in the second one (and viceversa)
        {  
          if(genePlacesOfParent2NotPresentInParent1[i]!= - 1)
          {
            maskForChilds[0, i] = 2;
            justExchangedAtPreviousPosition = true;
          }
          if(genePlacesOfParent1NotPresentInParent2[i]!= - 1)
          {
            maskForChilds[1, i] = 1;
            justExchangedAtPreviousPosition = true;
          }
        }
        else
          justExchangedAtPreviousPosition = false;
      }
    }
    private static void setChildsUsingMaskForChilds(Genome parent1,
		                                                Genome parent2)
		{
      for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int genePos = 0 ; genePos < GenomeManipulator.genomeSize ; genePos++)
      	{
  	      if(maskForChilds[childIndex,genePos]==1)
           	childs[childIndex].SetGeneValue(parent1.GetGeneValue(genePos), genePos);
       		else//maskForChilds[childIndex,genePos]==2
           	childs[childIndex].SetGeneValue(parent2.GetGeneValue(genePos), genePos);
        }
      }
			
		}
		
    private static bool hasSomeDuplicateGenes(Genome genome)
    {
      bool returnValue = false;
      for(int i = 0; i < genome.Size ; i++)
      {
        for(int j = i + 1; j < genome.Size ; j++)
        {
          if(genome.Genes()[i] == genome.Genes()[j] ||
            ((genome.Genes()[i]<0 && genome.Genes()[j]>0) && Math.Abs(genome.Genes()[i]) - 1 == genome.Genes()[j]) ||
            ((genome.Genes()[i]>0 && genome.Genes()[j]<0) && -Math.Abs(genome.Genes()[i]) - 1 == genome.Genes()[j]))
            returnValue = true;
        }
      }
      return returnValue;
    }
    //just for debugging purposes
    private static void throwExcIfAChildHasDuplicateGenes()
		{
      foreach(Genome gen in childs)
      {
      	if(GenomeManipulator.hasSomeDuplicateGenes(gen))
      		throw new Exception("A child with duplicate genes has been generated!");
      }
			
		}
		/// <summary>
		/// This method returns an array of genomes based on 
		/// a mix of the genes of parents, such that the 2 childs,
		/// if possible, are different from parents and, at 
		/// the same time, childs' genes are not duplicated  
		/// </summary> 
		/// <param name="parent1">First genome parent from which genes are to be mixed in offspring</param>
    /// <param name="parents">Second genome parent from which genes are to be mixed in offspring</param>
    /// <param name="constToDiscoverGenesDuplicates">Gene y is a duplicate of gene x iff y = |x| - 1
    ///  or y = -|x| -1</param>
    public static Genome[] MixGenesWithoutDuplicates(Genome parent1, Genome parent2)
    {
      initializeStaticMembers(parent1, parent2);
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      
      setMaskForChildsForMixingWithoutDuplicates(parent1, parent2);
      setChildsUsingMaskForChilds(parent1, parent2);
      //throwExcIfAChildHasDuplicateGenes(); //just for debugging purposes
      return childs;
    }
		
    /// <summary>
		/// Returns true if a given gene, when decoded by the
		/// GenomeManagerForEfficientPortfolio, refers to a
		/// ticker already contained in a given genome 
		/// </summary> 
		/// <param name="geneCorrespondingToATicker">Gene, corresponding to a certain ticker, that has to be checked</param>
    /// <param name="genome">Genome containing or not the ticker geneCorrespondingToATicker refers to</param>
    public static bool IsTickerContainedInGenome(int geneCorrespondingToATicker,
                                                 Genome genome)
    {
      return(
    		genome.HasGene(geneCorrespondingToATicker) ||
        	(geneCorrespondingToATicker<0 && 
          	genome.HasGene(Math.Abs(geneCorrespondingToATicker)-1)) ||
        	(geneCorrespondingToATicker>=0 && genome.HasGene(- Math.Abs(geneCorrespondingToATicker)- 1))
      );
      
    }
    
    /// <summary>
		/// Returns true if a given gene, when decoded by the
		/// GenomeManagerForEfficientPortfolio, refers to a
		/// ticker already contained in a given genome 
		/// </summary> 
		/// <param name="geneCorrespondingToATicker">Gene, corresponding to a certain ticker, that has to be checked</param>
    /// <param name="genome">Genome containing or not the ticker geneCorrespondingToATicker refers to</param>
    /// <param name="fromGenePosition">First gene position from which the checking
    /// 		has to be done, inside the given genome</param>
    /// <param name="toGenePosition">Last gene position to which the checking
    /// 		has to be done, inside the given genome</param>
    public static bool IsTickerContainedInGenome(int geneCorrespondingToATicker,
                                                 Genome genome, int fromGenePosition,
                                                int toGenePosition)
    {
      return(
    		genome.HasGene(geneCorrespondingToATicker, 
    	                 fromGenePosition, toGenePosition) ||
        ( geneCorrespondingToATicker < 0 && 
          genome.HasGene(
    		     Math.Abs(geneCorrespondingToATicker)-1,
    		     fromGenePosition, toGenePosition) )         ||
        ( geneCorrespondingToATicker >= 0 && 
    	    genome.HasGene(- Math.Abs(geneCorrespondingToATicker)- 1,
    	                       fromGenePosition, toGenePosition))
      );
      
    }
    
    /// <summary>
		/// Returns true if a given gene, when decoded by the
		/// GenomeManagerForEfficientPortfolio, refers to a
		/// ticker already contained in a given genome 
		/// </summary> 
		/// <param name="geneValueCorrespondingToATicker">Gene, corresponding to a certain ticker, that has to be checked</param>
    /// <param name="genome">Genome containing or not the ticker geneCorrespondingToATicker refers to</param>
    /// <param name="genesPointingToTickers">The int array that contains positions of genes pointing
    /// to tickers inside genome</param>
    public static bool IsTickerContainedInGenome(int geneValueCorrespondingToATicker,
                                                 Genome genome, int[] genePositionsPointingToTickers)
    {
      bool returnValue = false;
      for(int i = 0; i<genePositionsPointingToTickers.Length; i++)
      {
				returnValue = (genome.HasGene(geneValueCorrespondingToATicker, 
      	                              genePositionsPointingToTickers[i]) ||
        							(geneValueCorrespondingToATicker < 0 && 
          						 genome.HasGene(Math.Abs(geneValueCorrespondingToATicker)-1,
    		     													genePositionsPointingToTickers[i])  )   ||
        							(geneValueCorrespondingToATicker >= 0 && 
    	    							genome.HasGene(- Math.Abs(geneValueCorrespondingToATicker)- 1,
    	                       genePositionsPointingToTickers[i]))   );
      		if(returnValue)
      			i = genePositionsPointingToTickers.Length;//exit from FOR
      }
      return returnValue;
    }
// // 
//    /// <summary>
//		/// Returns true if a given gene value,
//		/// for which the meaning is a ticker code, has the same sign
//		/// of all the other genes that point to tickers
//		/// contained in a given genome.
//		/// </summary> 
//		/// <param name="newGeneValuePointingToTicker">Gene's value for which sign has to be checked</param>
//		/// <param name="newGeneValuePosition">Position for the new gene value inside the genome to be checked</param>
//    /// <param name="positionsForTickersInsideGenomeToBeChecked">Positions for genes pointing to tickers, inside genome, to be checked</param>
//    /// <param name="genomeToBeModified">The genome inside which check has to be done, before
//    /// 																		a mutation or a new gene assignment
//    /// 																		through the parameter newGeneValuePointingToTicker</param>
//    public static bool WillGenomeHaveTickersOfSameSign(int newGeneValuePointingToTicker,
//                                                       int newGeneValuePosition,
//                                                 			 int[] positionsForTickersInsideGenomeToBeChecked,
//                                                 			 Genome genomeToBeModified)
//    {
//      bool returnValue = true;
//			foreach(int position in positionsForTickersInsideGenome)
//			{
//				if( (position != newGeneValuePosition) &&
//				    ( (newGeneValuePointingToTicker < 0 && genomeToBeModified.GetGeneValue(position) >= 0 ) ||
//				    	(newGeneValuePointingToTicker >= 0 && genomeToBeModified.GetGeneValue(position) < 0 )   )   )
//				// if the current position is different from the newGeneValuePosition
//				// and
//				// newGeneValuePointingToTicker points to a short (long) position and
//			  // a the gene in the current position points to a long (short) one 
//							  returnValue = false;
//			}
//			return returnValue;
//    }
//    
//		/// <summary>
//		/// Returns true if a given gene value,
//		/// for which the meaning is a ticker code, has the same sign
//		/// of all the other genes that point to tickers
//		/// contained in a given genome.
//		/// </summary> 
//		/// <param name="newGeneValuePointingToTicker">Gene's value for which sign has to be checked</param>
//		/// <param name="newGeneValuePosition">Position for the new gene value inside the genome to be checked</param>
//		/// <param name="genomeToBeModified">The genome to be checked, before
//    /// 																 a mutation or a new gene assignment 
//    /// 																 through the parameter newGeneValuePointingToTicker</param>
//		public static bool WillGenomeHaveTickersOfSameSign(int newGeneValuePointingToTicker,
//                                                       int newGeneValuePosition,
//																											 Genome genomeToBeModified)
//		{
//			bool returnValue = true;
//			for( int position = 0; position < genomeToBeModified.Size; position++ )
//			{
//				if( (position != newGeneValuePosition) && 
//				    (	(newGeneValuePointingToTicker < 0 && genomeToBeModified.GetGeneValue(position) >= 0 ) ||
//				    	(newGeneValuePointingToTicker >= 0 && genomeToBeModified.GetGeneValue(position) < 0 )		)  )
//				// if the current position is different from the newGeneValuePosition
//				// and
//				// newGeneValuePointingToTicker points to a short (long) position and
//			  // a the gene in the current position points to a long (short) one
//							returnValue = false;
//			}
//			return returnValue; 
//		}

	}
}
