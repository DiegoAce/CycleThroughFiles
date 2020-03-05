using System;
using CycleThroughFiles.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
  [TestClass]
  public class DocumentHelpersTests
  {
    [TestMethod]
    public void FileNameWithoutExtension_return_fileName_single_ext()
    {
      Assert.AreEqual(DocumentHelpers.FileNameWithoutExtension("test.txt"), "test");

    }

    [TestMethod]
    public void FileNameWithoutExtension_return_fileName_multiple_ext()
    {
      Assert.AreEqual(DocumentHelpers.FileNameWithoutExtension("test.cshtml.cs"), "test");

    }


    [TestMethod]
    public void ExtensionOfFileName_return_single_ext()
    {
      Assert.AreEqual(DocumentHelpers.ExtensionOfFileName("test.txt"), "txt");

    }

    [TestMethod]
    public void ExtensionOfFileName_return_multiple_ext()
    {
      Assert.AreEqual(DocumentHelpers.ExtensionOfFileName("test.cshtml.cs"), "cshtml.cs");

    }


    [TestMethod]
    [Ignore]
    public void List_of_Siblings()
    {
      
      foreach( var item in DocumentHelpers.Siblings(@"c:\\temp\\test\\a.cshtml") )
      {
        Console.WriteLine(item);
      
      }


    }
    [TestMethod]
    [DataTestMethod]
    [DataRow("a.cshtml", "a.cshtml.cs")]
    [DataRow("a.cshtml.cs", "a.ts")]
    [DataRow("a.ts", "a.cshtml")]
    public void Next_Siblings_ReturnNext(string fileName, string expected)
    {
      var path = @"c:\\temp\\test\\";
      string[] sibl = DocumentHelpers.Siblings( path + fileName);

      Assert.AreEqual(  DocumentHelpers.NextSibling(sibl, fileName) , expected);


    }

  }
}
