using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace V3UnityFontReader
{
    public class MyComparer : IComparer<string>
    {
#pragma warning disable CS8604
#pragma warning disable CS8767
        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
#pragma warning restore CS8767
#pragma warning restore CS8604

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);
    }
}