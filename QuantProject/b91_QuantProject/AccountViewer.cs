using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Principale
{
	/// <summary>
	/// AccountViewer.
	/// </summary>
	public class AccountViewer : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Button buttonAddAccounts;
    private System.Windows.Forms.DataGrid dataGridAccounts;
		
		private System.ComponentModel.Container components = null;

		public AccountViewer()
		{
			
			//
			InitializeComponent();

			//
			// TODO: 
			//
		}

		/// <summary>
		/// Clean up
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		//do not modify with code editor
		private void InitializeComponent()
		{
      this.buttonAddAccounts = new System.Windows.Forms.Button();
      this.dataGridAccounts = new System.Windows.Forms.DataGrid();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridAccounts)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonAddAccounts
      // 
      this.buttonAddAccounts.Location = new System.Drawing.Point(320, 112);
      this.buttonAddAccounts.Name = "buttonAddAccounts";
      this.buttonAddAccounts.Size = new System.Drawing.Size(120, 32);
      this.buttonAddAccounts.TabIndex = 1;
      this.buttonAddAccounts.Text = "Add Accounts ...";
      this.buttonAddAccounts.Click += new System.EventHandler(this.buttonAddAccounts_Click);
      // 
      // dataGridAccounts
      // 
      this.dataGridAccounts.DataMember = "";
      this.dataGridAccounts.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGridAccounts.Location = new System.Drawing.Point(8, 0);
      this.dataGridAccounts.Name = "dataGridAccounts";
      this.dataGridAccounts.Size = new System.Drawing.Size(296, 264);
      this.dataGridAccounts.TabIndex = 2;
      // 
      // AccountViewer
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(456, 266);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.dataGridAccounts,
                                                                  this.buttonAddAccounts});
      this.Name = "AccountViewer";
      this.Text = "AccountViewer";
      ((System.ComponentModel.ISupportInitialize)(this.dataGridAccounts)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

    private void buttonAddAccounts_Click(object sender, System.EventArgs e)
    {
      //add code her for loading accounts
    }
	}
}
