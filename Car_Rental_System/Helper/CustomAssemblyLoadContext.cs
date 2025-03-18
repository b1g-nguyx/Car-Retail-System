using System;
using System.IO;
using System.Runtime.InteropServices;
namespace Car_Rental_System.Helpers;
public class CustomAssemblyLoadContext
{
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    public void LoadUnmanagedLibrary(string path)
    {
        if (LoadLibrary(path) == IntPtr.Zero)
        {
            throw new Exception($"Không thể tải thư viện: {path}");
        }
    }
}
