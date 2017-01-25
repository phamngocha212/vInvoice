using ICSharpCode.SharpZipLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin
{
    public class CompressHelper
    {
        public static void unZip(byte[] dataZip, string path)
        {
            try
            {
                using (Stream zipFile = new MemoryStream(dataZip))
                {
                    using (ICSharpCode.SharpZipLib.Zip.ZipInputStream ZipStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(zipFile))
                    {
                        ICSharpCode.SharpZipLib.Zip.ZipEntry theEntry;
                        theEntry = ZipStream.GetNextEntry();
                        if (theEntry.IsFile)
                        {
                            if (theEntry.Name != "")
                            {
                                FileStream outputStream = new FileStream(path, FileMode.OpenOrCreate);
                                StreamUtils.Copy(ZipStream, outputStream, new byte[4096]);
                                ZipStream.Close();
                                outputStream.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        public static byte[] CompressFile(byte[] data, string filename)
        {
            Stream stream = new MemoryStream(data);
            // Compress
            using (MemoryStream fsOut = new MemoryStream())
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(3);
                    ICSharpCode.SharpZipLib.Zip.ZipEntry newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(filename);
                    newEntry.DateTime = DateTime.UtcNow;
                    zipStream.PutNextEntry(newEntry);
                    StreamUtils.Copy(stream, zipStream, new byte[2048]);
                    zipStream.Finish();
                    zipStream.Close();
                }
                return fsOut.ToArray();
            }
        }
    }
}