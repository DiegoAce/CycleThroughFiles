using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleThroughFiles.Helpers
{
  public static class DocumentHelpers
  {

    /// <summary>
    /// Return the name of the file without extension 
    /// </summary>
    /// <param name="fileName">complete file name</param>
    /// <returns>string: file's name</returns>
    /// <remarks>supports multiple extensions : ex. myFile.cshtml.cs => myFile</remarks>
    public static string FileNameWithoutExtension(string fileName)
    {
      return String.Join(".", System.IO.Path.GetFileName(fileName)
                .Split('.')
                .Take(1));
    }

    /// <summary>
    /// Return the complete extension of the filename
    /// </summary>
    /// <param name="fileName">complete file name</param>
    /// <returns>string: file's extension</returns>
    /// <remarks>supports multiple extensions : ex. myFile.cshtml.cs => .cshtml.cs</remarks>
    public static string ExtensionOfFileName(string fileName)
    {
      return String.Join(".", System.IO.Path.GetFileName(fileName)
                 .Split('.')
                 .Skip(1));
    }


    public static string[] Siblings(string fileName)
    {


      string curntFileName = fileName;
      string curntExtension = DocumentHelpers.ExtensionOfFileName(curntFileName);

      string curntPath = System.IO.Path.GetDirectoryName(curntFileName);

      string FileNameWOExt = DocumentHelpers.FileNameWithoutExtension(curntFileName);

      var candidates = System.IO.Directory.GetFiles(curntPath, FileNameWOExt + ".*")
                            .OrderBy(n=>n)               
                          .Select(n => System.IO.Path.GetFileName(n));
                       
      return candidates.ToArray();

    }

    public static string NextSibling( string[] siblings, string actualFileName)
    {

      int curntPos = Array.IndexOf(siblings, actualFileName);

      curntPos += 1;
      if (curntPos == siblings.Count()) curntPos = 0;

      return siblings[curntPos];

    }
    public static string PreviousSibling(string[] siblings, string actualFileName)
    {

      int curntPos = Array.IndexOf(siblings, actualFileName);

      curntPos -= 1;
      if (curntPos < 0) curntPos = siblings.Count()-1;

      return siblings[curntPos];

    }
  }
}
