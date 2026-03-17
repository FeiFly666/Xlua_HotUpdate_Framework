using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

using UnityObject = UnityEngine.Object;

public class HotUpdate : MonoBehaviour
{
    internal class DownloadFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;

        public DownloadFileInfo(string url, string fileName)
        {
            this.url = url;
            this.fileName = fileName;
            //this.fileData = fileData;
        }
    }

    byte[] _ReadOnlyFileListData;
    byte[] _ServerFileListData;

    [SerializeField]private int maxRetry = 5;
    [SerializeField] int maxDownloadCount = 5;
    IEnumerator DownloadFile(DownloadFileInfo info, Action<DownloadFileInfo> Complete)
    {
        int retryTime = 0;

        while(retryTime < maxRetry)
        {
            UnityWebRequest request = UnityWebRequest.Get(info.url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                info.fileData = request.downloadHandler;
                Complete?.Invoke(info);
                request.Dispose();
                yield break;
            }
            Debug.LogWarning($"下载失败 {info.url} Retry:{retryTime + 1}/{maxRetry}");
            request.Dispose();

            yield return new WaitForSeconds(Mathf.Pow(2,retryTime));
            retryTime++;
        }

        Debug.LogError($"下载文件失败：{info.url}");
        yield break;
    }

    IEnumerator DownloadFileList(List<DownloadFileInfo> files, Action<DownloadFileInfo> Complete, Action AllComplete)
    {
        /*  foreach(DownloadFileInfo file in files)
          {
              yield return DownloadFile(file, Complete);
          }
          AllComplete?.Invoke();*/
        int index = 0;
        int runningCount = 0;
        int finishedCount = 0;
        //下载未完成
        while(finishedCount < files.Count)
        {
            //是否可以下载
            while (runningCount < maxDownloadCount && index < files.Count)
            {
                DownloadFileInfo file = files[index];
                index++;
                runningCount++;
                StartCoroutine(DownloadFile(file,(file) =>
                {
                    finishedCount++;
                    Debug.Log($"下载进度 {finishedCount}/{files.Count}");
                    runningCount--;
                    Complete?.Invoke(file);
                }));
            }
            yield return null;
        }
        AllComplete?.Invoke();
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="fileData"></param>
    /// <returns></returns>
    private List<DownloadFileInfo> GetFileList(string fileData,string path)
    {
        string content = fileData.Trim().Replace("\r", "");

        string[] files = content.Split('\n');

        List<DownloadFileInfo > result = new List<DownloadFileInfo>(files.Length);
        foreach (string file in files)
        {
            string[] splitInfos = file.Split("|");
            string fileName = splitInfos[1];
            string url = $"{path}/{splitInfos[1]}";

            result.Add(new DownloadFileInfo(url, fileName));
        }
        return result;
    }

    private void Start()
    {
        if(IsFirstEnter())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstEnter()
    {
        bool isExistsInReadPath = FileUtil.IsFileExists($"{PathUtil.ReadPath}/{AppConst.FileListName}");
        bool isExistsInWritablePath = FileUtil.IsFileExists($"{PathUtil.WritablePath}/{AppConst.FileListName}");

        return isExistsInReadPath && !isExistsInWritablePath;
    }
    #region 释放资源
    private void ReleaseResources()
    {
        string url = $"{PathUtil.ReadPath}/{AppConst.FileListName}";

        DownloadFileInfo info = new DownloadFileInfo(url, null);

        StartCoroutine(DownloadFile(info, OnDownloadReadPathFileComplete));
    }

    private void OnDownloadReadPathFileComplete(DownloadFileInfo file)
    {
        Debug.Log($"已经从只读区加载{file.fileName}");

        _ReadOnlyFileListData = file.fileData.data;
        List<DownloadFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownloadFileList(fileInfos, OnReleaseFileComplete, OnAllReleaseFileComplete));

    }
    private void OnReleaseFileComplete(DownloadFileInfo file)
    {
        Debug.Log($"资源{file.fileName}已释放完成");

        string writeFile = $"{PathUtil.WritablePath}/{file.fileName}";

        FileUtil.WriteFile(writeFile, file.fileData.data);
    }
    private void OnAllReleaseFileComplete()
    {
        Debug.Log($"所有资源已释放完成");
        string writeFile = $"{PathUtil.WritablePath}/{AppConst.FileListName}";
        FileUtil.WriteFile(writeFile, _ReadOnlyFileListData);
        CheckUpdate();
    }
    #endregion
    private void CheckUpdate()
    {
        string url = $"{AppConst.ResourceUrl}/{AppConst.FileListName}";
        DownloadFileInfo fileInfo = new DownloadFileInfo(url, null);
        StartCoroutine(DownloadFile(fileInfo, OnDownloadServerFilelistComplete));
    }

    private void OnDownloadServerFilelistComplete(DownloadFileInfo info)
    {
        Debug.Log($"{info.url}已下载完成");

        _ServerFileListData = info.fileData.data;
        List<DownloadFileInfo> fileInfos = GetFileList(info.fileData.text, AppConst.ResourceUrl);
        List<DownloadFileInfo> downloadFiles = new List<DownloadFileInfo>();

        foreach(var file in fileInfos)
        {
            string localFile = $"{PathUtil.WritablePath}/{file.fileName}";

            if(!FileUtil.IsFileExists(localFile))
            {
                file.url = $"{AppConst.ResourceUrl}/{file.fileName}";
                downloadFiles.Add(file);
            }
        }
        if(downloadFiles.Count > 0)
        {
            StartCoroutine(DownloadFileList(downloadFiles, OnUpdateFileComplete, OnUpdateAllFileComplete));
        }
        else
        {
            EnterGame();
        }
    }
    private void OnUpdateFileComplete(DownloadFileInfo file)
    {
        Debug.Log($"资源{file.fileName}已更新完成");

        string writeFile = $"{PathUtil.WritablePath}/{file.fileName}";
        FileUtil.WriteFile(writeFile, file.fileData.data);
    }
    private void OnUpdateAllFileComplete()
    {
        Debug.Log($"所有资源已更新完成");

        string writeFile = $"{PathUtil.WritablePath}/{AppConst.FileListName}";
        FileUtil.WriteFile(writeFile, _ServerFileListData);
        EnterGame();
    }
    private void EnterGame()
    {
        Manager.Resource.ParseFileText();
        Manager.Resource.LoadUI("UITest", OnComplete);
    }
    private void OnComplete(UnityObject obj)
    { 
        GameObject go = Instantiate(obj) as GameObject;

        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero; 
    }
}
