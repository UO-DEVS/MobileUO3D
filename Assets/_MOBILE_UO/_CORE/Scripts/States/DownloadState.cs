using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;

public class DownloadState : IState
{
    public List<string> FilesToDownload;
    public string ResourcePathForFilesToDownload;
    
    public static readonly List<string> NeededUoFileExtensions = new() {".def", ".mul", ".idx", ".uop", ".enu", ".rle", ".txt"};
    public const string DefaultFileDownloadPort = "8080";
    
    private readonly DownloadPresenter downloadPresenter;
    
    private ServerConfiguration serverConfiguration;
    private DownloaderBase downloader;
    private const string H_REF_PATTERN = @"<a\shref=[^>]*>([^<]*)<\/a>";

    public DownloadState(DownloadPresenter downloadPresenter)
    {
        this.downloadPresenter = downloadPresenter;
        downloadPresenter.BackButtonPressed += OnBackButtonPressed;
        downloadPresenter.CellularWarningYesButtonPressed += OnCellularWarningYesButtonPressed;
        downloadPresenter.CellularWarningNoButtonPressed += OnCellularWarningNoButtonPressed;
    }

    private void OnCellularWarningYesButtonPressed()
    {
        downloadPresenter.ToggleCellularWarning(false);
        StartDirectoryDownloader();
    }
    
    private void OnCellularWarningNoButtonPressed()
    {
        downloadPresenter.ToggleCellularWarning(false);
        StateManager.GoToState<ServerConfigurationState>();
    }

    private void OnBackButtonPressed()
    {
        StateManager.GoToState<ServerConfigurationState>();
    }

    public void Enter()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        serverConfiguration = ServerConfigurationModel.ActiveConfiguration;
	    string configPath = serverConfiguration.GetPathToSaveFiles(); //ADDED DX4D
        Debug.Log($"Downloading files to {configPath}");
	    var port = int.Parse(serverConfiguration.FileDownloadServerPort);
        
	    //ADDED DX4D
	    bool updateRequired = !serverConfiguration.AllFilesDownloaded;
	    DirectoryInfo configurationDirectory = new DirectoryInfo(configPath);
	    if (configurationDirectory.Exists)
	    {
		    FileInfo[] files = configurationDirectory.GetFiles();
		    if (files == null || files.Length <= 0)
		    {
		    	Log("UPDATE REQUIRED: No files found in directory " + configurationDirectory.ToString());
		    	updateRequired = true;
		    }
	    }
	    else
	    {
	    	Log("UPDATE REQUIRED: Directory not found " + configurationDirectory.ToString());
	    	updateRequired = true;
	    }
	    //END ADDED
	    
	    // || serverConfiguration.AllFilesDownloaded //REMOVED DX4D
	    if (!updateRequired || (Application.isEditor && string.IsNullOrEmpty(serverConfiguration.ClientPathForUnityEditor) == false))
        {
            StateManager.GoToState<GameState>();
        }
        else
        {
            downloadPresenter.gameObject.SetActive(true);

            //Figure out what kind of downloader we should use
            if (serverConfiguration.FileDownloadServerUrl.ToLowerInvariant().Contains(".zip"))
            {
                downloader = new ZipDownloader();
                downloader.Initialize(this, serverConfiguration, downloadPresenter);
            }
            else if (serverConfiguration.FileDownloadServerUrl.ToLowerInvariant().Contains("uooutlands.com"))
            {
                downloader = new OutlandsDownloader();
                downloader.Initialize(this, serverConfiguration, downloadPresenter);
            }
            else if (serverConfiguration.FileDownloadServerUrl.ToLowerInvariant().Contains("uorenaissance.com"))
            {
                downloader = new RenaissanceDownloader();
                downloader.Initialize(this, serverConfiguration, downloadPresenter);
            }
            else
            {
                //Get list of files to download from server
                var uri = GetUri(serverConfiguration.FileDownloadServerUrl, port);
                var request = UnityWebRequest.Get(uri);
                //This request should not take more than 5 seconds, the amount of data being received is very small
                request.timeout = 5;
                request.SendWebRequest().completed += operation =>
                {
                    //if (request.isHttpError || request.isNetworkError) //REMOVED DX4D
                    //ADDED DX4D
                    if (request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        StopAndShowError($"Error while making http request to server: {request.error}");
	                    return;
                    }
                    else if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        StopAndShowError($"Could not connect to server: {request.error}");
	                    return;
                    }
                    //END ADDED

                    var headers = request.GetResponseHeaders();

                    if (headers.TryGetValue("Content-Type", out var contentType))
                    {
                        if (contentType.Contains("application/json"))
                        {
                            //Parse json response to get list of files
                            Debug.Log($"Json response: {request.downloadHandler.text}");
                            FilesToDownload = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(request.downloadHandler.text);
                        }
                        else if (contentType.Contains("text/html"))
                        {
                            FilesToDownload = new List<string>(Regex
                                .Matches(request.downloadHandler.text, H_REF_PATTERN, RegexOptions.IgnoreCase)
                                .Cast<Match>()
                                .Select(match => match.Groups[1].Value));
                        }
                    }

                    if (FilesToDownload != null)
                    {
	                    //ADDED DX4D
	                    string pathToSaveFiles = serverConfiguration.GetPathToSaveFiles();
	                    foreach (var file in FilesToDownload)
	                    {
		                    if (file.ToLowerInvariant().Contains(".zip"))
		                    {
			                    UnZip(Path.Combine(pathToSaveFiles, file), pathToSaveFiles);	
		                    }
		                    //string filePath = Path.Combine(pathToSaveFiles, file);
	                    }
	                    //END ADDED
                    	
                        FilesToDownload.RemoveAll(file => NeededUoFileExtensions.Any(file.Contains) == false);
                        SetFileListAndDownload(FilesToDownload);
                    }
                    else
                    {
                        StopAndShowError("Could not determine file list to download");
                    }
                };
            }
        }
    }

    public void SetFileListAndDownload(List<string> filesList, string resourcePathForFilesToDownload = null)
    {
        FilesToDownload = filesList;
        ResourcePathForFilesToDownload = resourcePathForFilesToDownload;

        //Check that some of the essential UO files exist
        var hasAnimationFiles = UtilityMethods.EssentialUoFilesExist(FilesToDownload);
                    
        if (FilesToDownload.Count == 0 || hasAnimationFiles == false)
        {
            var error = "Download directory does not contain UO files such as anim.mul or animationFrame1.uop";
            StopAndShowError(error);
            return;
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            ShowCellularWarning();
        }
        else
        {
            StartDirectoryDownloader();
        }
    }
    
	//ADDED DX4D
	private void UnZip(string source, string destination, bool deleteZipFile = false)
	{
		try
		{
			ZipFile.ExtractToDirectory(source, destination, true);
			if (deleteZipFile) File.Delete(source);
		}
		catch (Exception e)
		{
			var error = $"Error while extracting {source}: {e}";
			Debug.Log(error);
			//downloadState.StopAndShowError(error);
			//yield break;
		}
		finally
		{
			//downloadCoroutine = null;
			//if (deleteAfterDownload) 
		}
	}
	//END ADDED

    private void StartDirectoryDownloader()
    {
        downloader = new DirectoryDownloader();
        downloader.Initialize(this, serverConfiguration, downloadPresenter);
    }

    private void ShowCellularWarning()
    {
        downloadPresenter.ToggleCellularWarning(true);
    }

    public static Uri GetUri(string serverUrl, int port, string fileName = null)
    {
        var httpPort = port == 80;
        var httpsPort = port == 443;
        var defaultPort = httpPort || httpsPort;
        var scheme = httpsPort ? "https" : "http";
        var serverUrlWithoutHttp = serverUrl.Replace("http://", "");
        serverUrlWithoutHttp = serverUrlWithoutHttp.Replace("https://", "");
        var uriBuilder = new UriBuilder(scheme, serverUrlWithoutHttp, defaultPort ? - 1 : port, fileName);
        return uriBuilder.Uri;
    }

    public void StopAndShowError(string error)
    {
	    //Debug.LogError(error); //REMOVED DX4D
	    LogIssue(error); //ADDED DX4D
        //Stop downloads
        downloadPresenter.ShowError(error);
        downloadPresenter.ClearFileList();
    }

    public void Exit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        
        downloadPresenter.ClearFileList();
        downloadPresenter.gameObject.SetActive(false);
        downloader?.Dispose();
        
        FilesToDownload = null;
        serverConfiguration = null;
    }
    
	//LOG
	[SerializeField] bool _consoleLogging = true;
	[SerializeField] bool _onscreenLogging = true;
	void Log(string message, bool warning = false)
	{
		if (_consoleLogging) 
		{
			if (warning) Debug.LogWarning(message);
			else Debug.Log(message);
		}
		if (_onscreenLogging) downloadPresenter.ShowError(message);
	}
	void LogIssue(string message, string title = "ISSUE", string color = "red")
	{
		Log("<color=" + color + ">" + title + "</color>: " + message, true);
	}
}