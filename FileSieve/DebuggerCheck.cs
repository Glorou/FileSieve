using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FileSieve;

public static class DebuggerCheck
{
    private static Process? process;
    private static bool isDebuggerAttached;

    static DebuggerCheck()
    {
        process = Process.GetProcessesByName("ffxiv_dx11").FirstOrDefault();
    }

    public static bool IsDebuggerAttached()
    {
        if(!CheckRemoteDebuggerPresent(process?.SafeHandle, ref isDebuggerAttached)){
            
        }

        return isDebuggerAttached;
    }
    
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CheckRemoteDebuggerPresent(
        SafeHandle hProcess,
        [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);
}
