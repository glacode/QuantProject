/*
QuantProject - Quantitative Finance Library

ObjectArchiver.cs
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace QuantProject.ADT.FileManaging
{
	
  /// <summary>
  /// Implements static methods
  /// to save and load objects through serialization / deserialization
  /// </summary>
  public class ObjectArchiver
  {
   	private static Stream stream;
  	private static BinaryFormatter formatter;
    private static Object archive;
  	private static string fileName;
  	private static string directoryPath;
  	
   	
    public static void Archive(Object objectToArchive, 
                        			 	string fileName,
                        				string directoryPath)
    {
        	
    	try
    	{
			  ObjectArchiver.archive = objectToArchive;
    		ObjectArchiver.setDiskPosition(fileName, directoryPath);
    		ObjectArchiver.setVariables();
				ObjectArchiver.formatter.Serialize(ObjectArchiver.stream,
				                                   objectToArchive);
    	}
    	catch(Exception ex)
    	{
    		System.Windows.Forms.MessageBox.Show(ex.ToString());
    	}
    	
    	finally
    	{
    		ObjectArchiver.stream.Close();
    	}
    	
    	      
    }
   
    private static void setDiskPosition(string fileName,
                        	 	     				string directoryPath)
    {
    	ObjectArchiver.fileName = fileName;
    	ObjectArchiver.directoryPath = directoryPath;
      
    }
    private static void setVariables()
    {
    	ObjectArchiver.stream = new FileStream(
    	                             ObjectArchiver.directoryPath +
    	                             ObjectArchiver.fileName,
    	                             FileMode.Create,
    	                             FileAccess.Write,
    	                             FileShare.None);
    	ObjectArchiver.formatter = new BinaryFormatter();
      
    }
   
    public static object Extract(string fileName,
                        				 string directoryPath)
    {
    	try
    	{
    		ObjectArchiver.setDiskPosition(fileName, directoryPath);
    		ObjectArchiver.setVariables();
      	return ObjectArchiver.formatter.Deserialize(ObjectArchiver.stream);
    	}
    	catch(Exception ex)
    	{
    		System.Windows.Forms.MessageBox.Show(ex.ToString());
    		return new Object(); //just to avoid compiling error
    		//throw new Exception("Extracting object failed!");
    	}
    	finally
    	{
    		ObjectArchiver.stream.Close();
    	}
    }
	
  }
  
}
