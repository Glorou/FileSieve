using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;

namespace FileSieve;

public unsafe class GetResourceDetours
{
    public List<FileHashInfo> Watching = new List<FileHashInfo>();
    private delegate IntPtr GetResourceSyncDelegate(IntPtr a1, IntPtr a2, IntPtr a3, int* hash, byte* path, IntPtr a4, IntPtr a5, uint line);
    
    [Signature("E8 ?? ?? ?? ?? 48 8B C8 8B C3 F0 0F C0 81", DetourName = nameof(GetResourceSyncDetour))]
    private Hook<GetResourceSyncDelegate>? _getResourceSync = null!;

    private IntPtr GetResourceSyncDetour(IntPtr a1, IntPtr a2, IntPtr a3, int* hash, byte* path, IntPtr a4, IntPtr a5, uint line)
    {
        if (Watching.Any(p => p.FullHash == *hash) && DebuggerCheck.IsDebuggerAttached())
        {
            Debugger.Break();
        }

        return _getResourceSync!.Original.Invoke(a1, a2, a3, hash, path, a4, a5, line);
    }
    
    private delegate IntPtr GetResourceAsyncDelegate(IntPtr a1, IntPtr a2, IntPtr a3, int* hash, byte* path, IntPtr a4, byte hasHandleLock, IntPtr a5, uint line);
    
    [Signature("E8 ?? ?? ?? ?? 48 8B C8 8B C3 F0 0F C0 81", DetourName = nameof(GetResourceAsyncDetour))]
    private Hook<GetResourceAsyncDelegate>? _getResourceAsync = null!;

    private IntPtr GetResourceAsyncDetour(IntPtr a1, IntPtr a2, IntPtr a3, int* hash, byte* path, IntPtr a4, byte hasHandleLock, IntPtr a5, uint line)
    {
        if (Watching.Any(p => p.FullHash == *hash) && DebuggerCheck.IsDebuggerAttached())
        {
            Debugger.Break();
        }
        return _getResourceAsync!.Original.Invoke(a1, a2, a3, hash, path, a4, hasHandleLock, a5, line);
    }
    public GetResourceDetours()
    {
        Plugin.InteropProvider.InitializeFromAttributes(this);
            
        _getResourceSync?.Enable();
        _getResourceAsync?.Enable();
    }

    public void Dispose()
    {
        _getResourceSync?.Dispose();
        _getResourceAsync?.Dispose();
    }
}
// reference: https://github.com/xivdev/Penumbra/blob/testing/Penumbra/Interop/Hooks/ResourceLoading/PapRewriter.cs#L10
/*public class GetResourceDetours(PeSigScanner sigScanner) : AsmHookBase(sigScanner)
{
    
}*/
