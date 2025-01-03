using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Runtime.Player.Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool isOnline = false;
    private bool GameIsPaused = false;
    private List<PlayerInputHandler> playerInputHandlers = new List<PlayerInputHandler>();
    [SerializeField] private TextMeshProUGUI disconnectText;
    [SerializeField] private Button DisconnectFirstSelectedButton;
    [SerializeField] private Button OptionsFirstSelectedButton;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject disconnectPanel;
    [SerializeField] private GameObject settingsPanel;
    public GameObject pauseMenuUI;




    public bool EscButton()
    {
        if (GameIsPaused)
        {
            Resume();
            return false;
        }
        
        Pause();
        return true;
        
    }

    public void setIsOnline(bool isOnline)
    {
        this.isOnline = isOnline;
        if (isOnline)
        {
           
            disconnectText.text = "Deseja desconectar da sala?";
        }

    }
    public void Resume()
    {
        foreach (var player in playerInputHandlers)
        {
            player.SetGameIsPaused(false);
        }
        GameIsPaused = false;
        if(settingsMenu.gameObject.activeSelf)
            settingsMenu.gameObject.SetActive(false);
        if (disconnectPanel.activeSelf)
            disconnectPanel.SetActive(false);
        pauseMenuUI.SetActive(false);
        if(!isOnline)
            Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void settingsOptions()
    {
        settingsPanel.SetActive(true);
        settingsMenu.gameObject.SetActive(true);
        settingsMenu.focusOnFirstButton();
        pauseMenuUI.SetActive(false);
    }

    public void closeSettingsOption()
    {
        settingsPanel.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void disconnectOption()
    {
        disconnectPanel.SetActive(true);
        DisconnectFirstSelectedButton.Select();
        pauseMenuUI.SetActive(false);
    }
    
    public void closeDisconnectOption()
    {
        disconnectPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    public void Pause()
    {
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        OptionsFirstSelectedButton.Select();
        if(!isOnline)
            Time.timeScale = 0f;
    }

    public void addPlayer(PlayerInputHandler player)
    {
        playerInputHandlers.Add(player);
    }
    

    public void LoadMenu()
    {
        if (isOnline)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Time.timeScale = 1f;
            Destroy(PlayerConfigurationManager.Instance.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

}
