namespace TestEmbedLibrary;

using EmbedResourceCSharp;
using System;
using System.Runtime.InteropServices;

public static unsafe partial class MyEmbedlibrary
{
    [FileEmbed("data/TestFromLibrary.png")]
    private static partial ReadOnlySpan<byte> GetFGetTestFromLibrary();

    [UnmanagedCallersOnly(EntryPoint ="FromLibrary")]
    public static byte* DataFromLibrary()
    {
        var data = GetFGetTestFromLibrary();
        fixed (byte* data_ptr = data)
        {
            return data_ptr;
        }
    }

    [UnmanagedCallersOnly(EntryPoint ="FromLibraryLength")]
    public static int DataFromLibraryLength()
    {
        var data = GetFGetTestFromLibrary();
        return data.ToArray().Length;
    }
}