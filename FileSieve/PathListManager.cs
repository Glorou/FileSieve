using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileSieve;

public struct FileHashInfo
{
    public uint IndexId;
    public uint FolderHash;
    public uint FileHash;
    public uint FullHash;
    public string Path;
}

public static class PathListManager
{
    public static List<FileHashInfo> Files = new List<FileHashInfo>();
    
    public static async Task GetPathList()
    {
        var url = @"https://rl2.perchbird.dev/download/CurrentPathListWithHashes.gz";
        using var httpClient = new HttpClient();
        var stream = new GZipStream(httpClient.GetStreamAsync(url).Result, CompressionMode.Decompress);
        var pathList = new StreamReader(stream).ReadToEnd();
        var lines = pathList.Split("\n");
        linef: foreach (var line in lines)
        {
            if(line == lines[0])
                continue;
            var curLine = line.Split(",");
            uint[] numbers = [0u,0u,0u,0u];


            for(var i = 0; i < curLine.Length; i++)
            {
                if (!uint.TryParse(curLine[i], out numbers[i]))
                {
                    goto linef;
                }
            }
            
            
            var hash = new FileHashInfo()
            {
                IndexId = numbers[0],
                FolderHash = numbers[1],
                FileHash = numbers[2],
                FullHash = numbers[3],
                Path = curLine[4]
            };
            Files.Add(hash);
        }
    }
}
