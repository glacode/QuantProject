using System;
using System.Collections;
using System.Drawing;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Chart used for (record by record) visual validation
	/// </summary>
	public abstract class VisualValidationChart : Chart
	{
		private DateTime suspiciousDateTime;
		private int precedingDays;
//		protected ArrayList histories;

		protected DateTime startDateTime = DateTime.MinValue;
		protected DateTime endDateTime = DateTime.MinValue;
		
		public DateTime SuspiciousDateTime
		{
			get { return this.suspiciousDateTime; }
			set
			{
				this.suspiciousDateTime = value;
				this.Invalidate();
			}
		}
		
		public int PrecedingDays
		{
			get { return this.precedingDays; }
			set	{	this.precedingDays = value;	}
		}
		
		public VisualValidationChart()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected void add( History history , Color color )
		{
//			if ( this.startDateTime == DateTime.MinValue )
				// the startDateTime is not computed yet
				this.onPaint_setTimeInterval( history );
			this.Add( history , color , this.startDateTime , this.endDateTime );
		}

		protected abstract void addHistories();

		#region OnPaint
		private void onPaint_setTimeInterval( History history )
		{
//			this.startDateTime = (DateTime) this.history.GetKey( Math.Max( 0 ,
//				this.history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) - 20 ) );
//			this.endDateTime = (DateTime) this.history.GetKey( Math.Min( this.history.Count - 1 ,
//				this.history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) ) + 20 );
			this.startDateTime = (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) -
				ConstantsProvider.PrecedingDaysForVisualValidation ) );
			this.endDateTime = (DateTime) history.GetKey( Math.Min( history.Count - 1 ,
				history.IndexOfKeyOrPrevious( this.suspiciousDateTime ) ) +
				ConstantsProvider.PrecedingDaysForVisualValidation );
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			Console.WriteLine( "VisualValidationChart.OnPaint()" );
			this.Clear();
			this.addHistories();
//			this.history = DataProvider.GetCloseHistory( ((QuotesEditor)this.FindForm()).Ticker );
//			this.onPaint_setTimeInterval();
//			this.Add( history , Color.Red , this.startDateTime , this.endDateTime );
			base.OnPaint( e );
		}
		#endregion
	}
}
