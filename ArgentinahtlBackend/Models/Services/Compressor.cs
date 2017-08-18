using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using SevenZip;

namespace ArgentinahtlMVC.Models.Services
{
    public class Compressor
    {
        public static FileResult CompressFile(string fileName, FileContentResult file)
        {
            //SevenZipCompressor.SetLibraryPath(@".\7z64.dll");

            //var compressor = new SevenZipCompressor();
            //compressor.CompressionMethod = CompressionMethod.Copy;

            //try
            //{
            //    file.

            //    var originalFile = new FileInfo(fileName);
            //    originalFile.Delete();

            //}
            //catch (Exception ex)
            //{
            //}

            return null;
        }
    }
}