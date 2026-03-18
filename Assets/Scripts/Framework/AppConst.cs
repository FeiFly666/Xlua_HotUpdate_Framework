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
public enum GameEvent
{
    StartLua,
    GameInit
}
public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static GameLoadMode GameMode = GameLoadMode.Editor;

    public static bool OpenLog = true;

    //热更地址
    public const string ResourceUrl = "http://175.163.9.205:5500/AssetBundles";
}
