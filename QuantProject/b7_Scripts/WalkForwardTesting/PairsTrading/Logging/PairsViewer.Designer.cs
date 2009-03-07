/*
QuantProject - Quantitative Finance Library

PairsViewer.cs
Copyright (C) 2009
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
 
 
namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	partial class PairsViewer
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.buttonShow = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker.Location = new System.Drawing.Point(24, 47);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.ShowUpDown = true;
			this.dateTimePicker.Size = new System.Drawing.Size(256, 20);
			this.dateTimePicker.TabIndex = 0;
			// 
			// buttonShow
			// 
			this.buttonShow.Location = new System.Drawing.Point(104, 131);
			this.buttonShow.Name = "buttonShow";
			this.buttonShow.Size = new System.Drawing.Size(75, 23);
			this.buttonShow.TabIndex = 1;
			this.buttonShow.Text = "Show";
			this.buttonShow.UseVisualStyleBackColor = true;
			this.buttonShow.Click += new System.EventHandler(this.ButtonShowClick);
			// 
			// PairsViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 262);
			this.Controls.Add(this.buttonShow);
			this.Controls.Add(this.dateTimePicker);
			this.Name = "PairsViewer";
			this.Text = "PairsViewer";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button buttonShow;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
	}
}
