using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileUtil
{
    public static bool IsFileExists(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }
    public static void WriteFile(string path, byte[] content)
    {
        path = PathUtil.GetStandardPath(path);

        //文件夹路径
        string dir = path.Substring(0,path.LastIndexOf('/'));
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        try
        {
            File.WriteAllBytes(path, content);
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);
        }

    }


}
