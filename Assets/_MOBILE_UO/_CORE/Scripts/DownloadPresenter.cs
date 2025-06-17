using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPresenter : MonoBehaviour
{
    const string BACK_LABEL = "<";
    const string CANCEL_LABEL = "X";

    [SerializeField]
    private Text counterText;
    
    [SerializeField]
    private Text errorText;

    [SerializeField]
    private GameObject filesScrollView;
    
    [SerializeField]
    private FileNameView fileNameViewInstance;
    
    [SerializeField]
    private Button backOrCancelButton;
    
    [SerializeField]
    private Text backOrCancelButtonText;

    [SerializeField]
    private GameObject cellularWarningParent;

    [SerializeField]
    private Button cellularWarningYesButton;

    [SerializeField]
    private Button cellularWarningNoButton;

    public Action BackButtonPressed;
    public Action CellularWarningYesButtonPressed;
    public Action CellularWarningNoButtonPressed;

    private readonly Dictionary<string, FileNameView> fileNameToFileNameView = new Dictionary<string, FileNameView>();

    private void OnEnable()
    {
        backOrCancelButton.onClick.AddListener(() => BackButtonPressed?.Invoke());
        cellularWarningYesButton.onClick.AddListener(() => CellularWarningYesButtonPressed?.Invoke());
        cellularWarningNoButton.onClick.AddListener(() => CellularWarningNoButtonPressed?.Invoke());
        
        cellularWarningParent.gameObject.SetActive(false);
        backOrCancelButton.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        fileNameViewInstance.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        backOrCancelButton.onClick.RemoveAllListeners();
        counterText.text = "Getting list of files to download...";
    }

    public void ShowError(string error)
    {
        backOrCancelButton.gameObject.SetActive(true);
	    backOrCancelButtonText.text = BACK_LABEL;
        errorText.text = error;
        errorText.gameObject.SetActive(true);
    }

    public void UpdateView(int numberOfFilesDownloaded, int numberOfFilesToDownload)
    {
        if (counterText != null)
        {
            counterText.text = $"Files downloaded: {numberOfFilesDownloaded}/{numberOfFilesToDownload}";
        }
    }

    public void SetFileDownloaded(string file)
    {
        var fileNameView = fileNameToFileNameView[file];
        if (fileNameView != null)
        {
            fileNameView.SetDownloaded();
        }
    }

    public void SetFileList(List<string> filesList)
    {
        var filenameTextInstanceGameObject = fileNameViewInstance.gameObject;
        var filenameTextInstanceTransformParent = fileNameViewInstance.transform.parent;
        filesList.ForEach(file =>
        {
            var newFileNameText = Instantiate(filenameTextInstanceGameObject, filenameTextInstanceTransformParent).GetComponent<FileNameView>();
	        newFileNameText.SetText(file);
	        
	        if (fileNameToFileNameView.ContainsKey(file)) fileNameToFileNameView[file] = newFileNameText; //ADDED DX4D
	        else fileNameToFileNameView.Add(file, newFileNameText); //ADDED DX4D
	        //fileNameToFileNameView.Add(file, newFileNameText);
            
	        newFileNameText.gameObject.SetActive(true);
        });
        filesScrollView.SetActive(true);
        backOrCancelButton.gameObject.SetActive(true);
        backOrCancelButtonText.text = CANCEL_LABEL; //ADDED DX4D
        //backOrCancelButtonText.text = "Cancel"; //REMOVED DX4D
    }

    public void ClearFileList()
    {
        foreach (var keyValuePair in fileNameToFileNameView)
        {
            if (keyValuePair.Value != null)
            {
                Destroy(keyValuePair.Value.gameObject);
            }
        }
        
        fileNameToFileNameView.Clear();
        filesScrollView.SetActive(false);
    }

    public void SetDownloadProgress(string file, float progress)
    {
        if (fileNameToFileNameView.TryGetValue(file, out var fileNameView))
        {
            fileNameView?.SetProgress(progress);
        }
    }

    public void ToggleCellularWarning(bool enabled)
    {
        cellularWarningParent.SetActive(enabled);
    }
}