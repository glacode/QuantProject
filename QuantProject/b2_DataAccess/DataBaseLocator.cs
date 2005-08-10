/*
QuantProject - Quantitative Finance Library

DataBaseLocator.cs
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
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Locate the database
	/// </summary>
	public class DataBaseLocator
	{
		private string dataBaseType;
		private StreamReader stream; 
		private XmlTextReader xmlTextReader;
		
		private string path;

		public DataBaseLocator(string fileExtension)
		{
			try
			{
				this.dataBaseType = fileExtension;
				//it looks for the file in the application directory
//				if (!File.Exists("DataBase.xml"))
				string xmlPath = Application.ExecutablePath.Substring(0,
					Application.ExecutablePath.LastIndexOf('\\') )
					+ @"\DataBase.xml";
				if (!File.Exists( xmlPath ))
					createXmlFile();
						
				this.stream = new StreamReader(Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\'))
											+ @"\DataBase.xml");
				this.xmlTextReader = new XmlTextReader(this.stream);
				
				while(xmlTextReader.Read())
				{
					if (xmlTextReader.LocalName.ToString() == this.dataBaseType)
					{
						//gets full path of the file that contains the database
						this.Path = xmlTextReader.GetAttribute(0);
            if(!File.Exists(this.path)) 
              throw new Exception("Specified file in DataBase.xml doesn't exist!\n" +
                                  "Delete Database.xml and retry!");
					}
				}
				xmlTextReader.Close();
				stream.Close();
      }
			
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				xmlTextReader.Close();
				stream.Close();
			}
		}
		
		public string Path
		{
			get
			{
				return path;
			}
			set
			{	
				path = value;
			}
		}

		/// <summary>
		/// create xmlFile in the application directory
		/// where to store name and path for the mdb file
		/// selected by the user
		/// </summary>
		private void createXmlFile()
		{
			string path;
			string xmlPath;
			string selectionByUser;
			selectionByUser = selectDataBase();
			if (selectionByUser == "")
				{
					MessageBox.Show("With no selection application won't run!");
					Application.Exit();
				}
			else
				{
					path = selectionByUser;
					
					xmlPath = 
							Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\'))
								+ @"\DataBase.xml";
					XmlTextWriter xmlTextWriter = new XmlTextWriter(xmlPath, null);
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.WriteStartDocument();
					xmlTextWriter.WriteStartElement("FILES");
					xmlTextWriter.WriteStartElement("MDB");
					xmlTextWriter.WriteAttributeString("fullpath",path);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndDocument();
					xmlTextWriter.Flush();
					xmlTextWriter.Close();
					
				}

		}
		private string selectDataBase()
		{
			string fileName = "";
			switch (this.dataBaseType)
			{
				case "MDB":
				fileName = "QuantProject.mdb";
				break;
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select " + fileName + " please ...";
			openFileDialog.Multiselect = false;
			openFileDialog.CheckFileExists = true;
			openFileDialog.Filter = fileName + "|" + fileName;
			openFileDialog.ShowDialog();
			return openFileDialog.FileName;
		}
		
	}
}
