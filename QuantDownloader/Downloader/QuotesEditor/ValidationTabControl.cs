using System;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// TabControl to contain a TabPage for each kind of validation to
	/// be performed
	/// </summary>
	public class ValidationTabControl : TabControl
	{
		private OHLCtabPage oHLCtabPage = new OHLCtabPage();
		private CloseToCloseTabPage closeToCloseTabPage = new CloseToCloseTabPage();
		private RangeToRangeTabPage rangeToRangeTabPage = new RangeToRangeTabPage();
		public ValidationTabControl()
		{
      this.Controls.Add( oHLCtabPage );
			this.Controls.Add( closeToCloseTabPage );
			this.Controls.Add( rangeToRangeTabPage );
			this.Anchor = AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
//      this.SetStyle( ControlStyles.UserPaint , true );
		}
		/// <summary>
		///  clears all the contained tab pages and removes them
		/// </summary>
		public void Clear()
		{
			this.oHLCtabPage.Clear();
			this.closeToCloseTabPage.Clear();
			this.rangeToRangeTabPage.Clear();
//			this.RemoveAll();
		}
		public void Rebuild()
		{
			int i=0;
			if ( oHLCtabPage.SetGrid() != 0 )
				i++;
//				this.Controls.Add( oHLCtabPage );
			if ( closeToCloseTabPage.SetGrid() != 0 )
				i++;
			//				this.Controls.Add( closeToCloseTabPage );
			if ( rangeToRangeTabPage.SetGrid() != 0 )
				i++;
			//				this.Controls.Add( rangeToRangeTabPage );
		}
	}
}
