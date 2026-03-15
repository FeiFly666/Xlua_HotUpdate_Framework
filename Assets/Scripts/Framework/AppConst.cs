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
}
