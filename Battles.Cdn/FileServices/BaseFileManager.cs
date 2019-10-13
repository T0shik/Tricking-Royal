using System;
using System.IO;

namespace Battles.Cdn.FileServices
{
    public abstract class BaseFileManager
    {
        protected static string CreateFileName() =>
            DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");

        protected static string GetFileMime(string fileName) =>
            fileName.Substring(fileName.LastIndexOf('.'));
        
        protected static bool TryRemoveFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            //todo: this doesn't look good
            catch
            {
                return false;
            }
        }
    }
}