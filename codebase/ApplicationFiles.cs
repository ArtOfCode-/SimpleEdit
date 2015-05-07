using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEdit
{
    static class ApplicationFiles
    {
        public static string UserConfig = "{WorkingPath}\\user.cfg";

        public static string GetFilePath(string workingPath, string file)
        {
            return file.Replace("{WorkingPath}", workingPath);
        }
    }
}
