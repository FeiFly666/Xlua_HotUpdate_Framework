using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum GameLoadMode
{
    Editor,
    PackageBundle,
    Update
}
public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static GameLoadMode GameMode = GameLoadMode.Editor;

    public static bool OpenLog = true;

    //热更地址
    public const string ResourceUrl = "http://127.0.0.1:5500/AssetBundles";
}
