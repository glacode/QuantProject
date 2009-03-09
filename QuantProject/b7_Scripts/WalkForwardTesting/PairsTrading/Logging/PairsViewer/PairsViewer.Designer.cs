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
			this.dateTimePickerForFirstDateTime = new System.Windows.Forms.DateTimePicker();
			this.buttonShow = new System.Windows.Forms.Button();
			this.labelFirstDateTime = new System.Windows.Forms.Label();
			this.labelLastDateTime = new System.Windows.Forms.Label();
			this.dateTimePickerForLastDateTime = new System.Windows.Forms.DateTimePicker();
			this.panelGreenTicker = new System.Windows.Forms.Panel();
			this.textBoxGreenTicker = new System.Windows.Forms.TextBox();
			this.textBoxRedTicker = new System.Windows.Forms.TextBox();
			this.panelRedTicker = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// dateTimePickerForFirstDateTime
			// 
			this.dateTimePickerForFirstDateTime.CustomFormat = "yyyy-MM-dd HH:mm";
			this.dateTimePickerForFirstDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePickerForFirstDateTime.Location = new System.Drawing.Point(137, 27);
			this.dateTimePickerForFirstDateTime.Name = "dateTimePickerForFirstDateTime";
			this.dateTimePickerForFirstDateTime.ShowUpDown = true;
			this.dateTimePickerForFirstDateTime.Size = new System.Drawing.Size(143, 20);
			this.dateTimePickerForFirstDateTime.TabIndex = 0;
			// 
			// buttonShow
			// 
			this.buttonShow.Location = new System.Drawing.Point(102, 197);
			this.buttonShow.Name = "buttonShow";
			this.buttonShow.Size = new System.Drawing.Size(75, 23);
			this.buttonShow.TabIndex = 1;
			this.buttonShow.Text = "Show";
			this.buttonShow.UseVisualStyleBackColor = true;
			this.buttonShow.Click += new System.EventHandler(this.ButtonShowClick);
			// 
			// labelFirstDateTime
			// 
			this.labelFirstDateTime.Location = new System.Drawing.Point(31, 27);
			this.labelFirstDateTime.Name = "labelFirstDateTime";
			this.labelFirstDateTime.Size = new System.Drawing.Size(100, 23);
			this.labelFirstDateTime.TabIndex = 2;
			this.labelFirstDateTime.Text = "First Date Time:";
			this.labelFirstDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelLastDateTime
			// 
			this.labelLastDateTime.Location = new System.Drawing.Point(31, 66);
			this.labelLastDateTime.Name = "labelLastDateTime";
			this.labelLastDateTime.Size = new System.Drawing.Size(100, 23);
			this.labelLastDateTime.TabIndex = 4;
			this.labelLastDateTime.Text = "Last Date Time:";
			this.labelLastDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateTimePickerForLastDateTime
			// 
			this.dateTimePickerForLastDateTime.CustomFormat = "yyyy-MM-dd HH:mm";
			this.dateTimePickerForLastDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePickerForLastDateTime.Location = new System.Drawing.Point(137, 66);
			this.dateTimePickerForLastDateTime.Name = "dateTimePickerForLastDateTime";
			this.dateTimePickerForLastDateTime.ShowUpDown = true;
			this.dateTimePickerForLastDateTime.Size = new System.Drawing.Size(143, 20);
			this.dateTimePickerForLastDateTime.TabIndex = 3;
			// 
			// panelGreenTicker
			// 
			this.panelGreenTicker.BackColor = System.Drawing.Color.Green;
			this.panelGreenTicker.Location = new System.Drawing.Point(121, 113);
			this.panelGreenTicker.Name = "panelGreenTicker";
			this.panelGreenTicker.Size = new System.Drawing.Size(10, 14);
			this.panelGreenTicker.TabIndex = 5;
			// 
			// textBoxGreenTicker
			// 
			this.textBoxGreenTicker.Location = new System.Drawing.Point(137, 110);
			this.textBoxGreenTicker.Name = "textBoxGreenTicker";
			this.textBoxGreenTicker.Size = new System.Drawing.Size(66, 20);
			this.textBoxGreenTicker.TabIndex = 6;
			// 
			// textBoxRedTicker
			// 
			this.textBoxRedTicker.Location = new System.Drawing.Point(137, 146);
			this.textBoxRedTicker.Name = "textBoxRedTicker";
			this.textBoxRedTicker.Size = new System.Drawing.Size(66, 20);
			this.textBoxRedTicker.TabIndex = 8;
			// 
			// panelRedTicker
			// 
			this.panelRedTicker.BackColor = System.Drawing.Color.Red;
			this.panelRedTicker.Location = new System.Drawing.Point(121, 149);
			this.panelRedTicker.Name = "panelRedTicker";
			this.panelRedTicker.Size = new System.Drawing.Size(10, 14);
			this.panelRedTicker.TabIndex = 7;
			// 
			// PairsViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 262);
			this.Controls.Add(this.textBoxRedTicker);
			this.Controls.Add(this.panelRedTicker);
			this.Controls.Add(this.textBoxGreenTicker);
			this.Controls.Add(this.panelGreenTicker);
			this.Controls.Add(this.labelLastDateTime);
			this.Controls.Add(this.dateTimePickerForLastDateTime);
			this.Controls.Add(this.labelFirstDateTime);
			this.Controls.Add(this.buttonShow);
			this.Controls.Add(this.dateTimePickerForFirstDateTime);
			this.Name = "PairsViewer";
			this.Text = "PairsViewer";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Panel panelRedTicker;
		private System.Windows.Forms.TextBox textBoxRedTicker;
		private System.Windows.Forms.DateTimePicker dateTimePickerForLastDateTime;
		private System.Windows.Forms.TextBox textBoxGreenTicker;
		private System.Windows.Forms.Panel panelGreenTicker;
		private System.Windows.Forms.Label labelLastDateTime;
		private System.Windows.Forms.Label labelFirstDateTime;
		private System.Windows.Forms.DateTimePicker dateTimePickerForFirstDateTime;
		private System.Windows.Forms.Button buttonShow;
	}
}
