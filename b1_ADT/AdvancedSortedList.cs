using System;
using System.Collections;

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for AdvancedSortedList.
	/// </summary>
  [Serializable]
  public class AdvancedSortedList : SortedList
	{
		public AdvancedSortedList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    #region "IndexOfKeyOrPrevious"
    public int previousIndexOfKey_dicotomicSearch(
      Object key , int startIndex , int endIndex )
    {
      if ( startIndex == endIndex - 1 )
        return startIndex;
      else
      {
        int middle = ( startIndex + endIndex )/2;
        if ( ((IComparable)this.GetKey( middle )).CompareTo( key ) > 0 )
          // this.GetKey( middle ) > key
          endIndex = middle;
        else
          // this.GetKey( middle ) < key
          startIndex = middle;
      }
      return previousIndexOfKey_dicotomicSearch(
        key , startIndex , endIndex );
    }

    public int previousIndexOfKey( Object key )
    {
      if ( ((IComparable)this.GetKey( this.Count - 1 )).CompareTo( key ) < 0 )
        // the last element key is less then the key to search
        return this.Count - 1;
      else
        // ((IComparable)this.GetKey( this.Count - 1 )).CompareTo( key ) > 0
        return 
          previousIndexOfKey_dicotomicSearch( key , 0 , this.Count - 1 );
    }

    public int IndexOfKeyOrPrevious( Object key )
    {
      int indexOfKey = this.IndexOfKey( key );
      if ( indexOfKey >= 0 )
        return indexOfKey;
      else
        return previousIndexOfKey( key );
    }
    #endregion
    public Object GetKeyOrPrevious( Object key )
    {
      return this.GetKey( this.IndexOfKey( key ) );
    }
    public bool IsLastKey( Object key )
    {
      return ( this.IndexOfKey( key ) == ( this.Count - 1 ) );
    }
	}
}
