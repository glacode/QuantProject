using System;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts.MSFTsimpleTest
{
  /// <summary>
  /// Summary description for AsMSFTsimpleTest.
  /// </summary>
  public class AsMSFTsimpleTest : AccountStrategy
  {
    public AsMSFTsimpleTest( Account account )
    {
      //
      // TODO: Add constructor logic here
      //
    }

//    public virtual Orders GetOrders( Signal signal )
//    {
//      Orders orders = new Orders();
//      foreach ( Order virtualOrder in signal )
//      {
//        ArrayList ordersForCurrentVirtualOrder =
//          this.getOrdersForCurrentVirtualOrder( virtualOrder , dateTime );
//        foreach( Order order in ordersForCurrentVirtualOrder )
//          orders.Add( order );
//      }
//      return orders;
//    }
  }
}
