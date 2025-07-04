﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

public class GameState : IState
{
    private readonly ClientRunner clientRunner;
    private readonly ErrorPresenter errorPresenter;

    public GameState(ClientRunner clientRunner, ErrorPresenter errorPresenter)
    {
        this.clientRunner = clientRunner;
        this.errorPresenter = errorPresenter;
    }
    public void Enter()
    {
        errorPresenter.BackButtonClicked += GoBackToServerConfigurationState;
        clientRunner.OnExiting += GoBackToServerConfigurationState;
        clientRunner.OnError += OnError;

        var config = ServerConfigurationModel.ActiveConfiguration;
        
        //Check that some of the essential UO files exist
        if (Application.isMobilePlatform || string.IsNullOrEmpty(config.ClientPathForUnityEditor))
        {
            var configPath = config.GetPathToSaveFiles();
            var configurationDirectory = new DirectoryInfo(configPath);
	        var files = configurationDirectory.GetFiles().Select(x => x.Name).ToList();
            
            
        	//ADDED DX4D
	        //string pathToSaveFiles = config.GetPathToSaveFiles();
        	foreach (var file in files)
        	{
        		if (file.ToLowerInvariant().Contains(".zip"))
        		{
        			UnZip(Path.Combine(configPath, file), configPath);	
        		}
	        	//string filePath = Path.Combine(pathToSaveFiles, file);
        	}
        	//END ADDED
        	
            var hasAnimationFiles = UtilityMethods.EssentialUoFilesExist(files);
            if (hasAnimationFiles == false)
            {
                var error = $"Server configuration directory does not contain UO files such as anim.mul or animationFrame1.uop. Make sure that the UO files have been downloaded or transferred properly.\nPath: {configPath}";
                OnError(error);
                return;
            }
        }

        clientRunner.enabled = true;
        clientRunner.StartGame(config);
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

    private void GoBackToServerConfigurationState()
    {
        StateManager.GoToState<ServerConfigurationState>();
    }

    private void OnError(string error)
    {
        clientRunner.enabled = false;
        errorPresenter.gameObject.SetActive(true);
        errorPresenter.SetErrorText(error);
    }

    public void Exit()
    {
        clientRunner.enabled = false;
        errorPresenter.gameObject.SetActive(false);
        errorPresenter.BackButtonClicked -= GoBackToServerConfigurationState;
        clientRunner.OnExiting -= GoBackToServerConfigurationState;
        clientRunner.OnError -= OnError;
    }
}