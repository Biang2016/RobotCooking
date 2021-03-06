﻿using System.Collections.Generic;
using UdpKit.Platform.Photon;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    void Awake()
    {
        BoltManager.RefreshRoomListInUI = RefreshRoomList;
        VersionText.text = Application.version;
    }

    [SerializeField] private Transform RoomListContainer;

    public string CurrentFilter => RoomIDInputField.text;

    [SerializeField] private Text VersionText;
    [SerializeField] private Text RegionText;
    [SerializeField] private InputField RoomIDInputField;
    [SerializeField] private Text UserNameText;
    [SerializeField] private Button RenameButton;
    [SerializeField] private InputField SearchInputField;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button BackButton;

    private List<RoomButton> RoomButtons = new List<RoomButton>();

    private void RefreshRoomList(List<RoomInfoToken> roomInfos)
    {
        foreach (RoomButton rb in RoomButtons)
        {
            rb.PoolRecycle();
        }

        RoomButtons.Clear();

        foreach (RoomInfoToken ri in roomInfos)
        {
            RoomButton rb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RoomButton].AllocateGameObject<RoomButton>(RoomListContainer);
            rb.Button.interactable = Interactable;
            rb.Initialize(ri);
            RoomButtons.Add(rb);
        }
    }

    public void CreateRoom()
    {
        UIManager.Instance.ShowUIForms<CreateRoomPanel>();
    }

    void Start()
    {
        InvokeRepeating("RefreshButtonClick", 0f, 3f);
        RoomIDInputField.onValueChanged.AddListener(delegate { RefreshButtonClick(); });
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
        }
    }

    public void UpdateUserName()
    {
        UserNameText.text = PlayerPrefs.GetString("PlayerID");
    }

    public void Display()
    {
        gameObject.SetActive(true);
        UpdateUserName();
        RegionText.text = "Region: " + PhotonRegion.GetRegion(Regions.CurSelectedRegion).Name;
        RefreshButtonClick();
        RefreshRoomList(new List<RoomInfoToken>());
        if (!BoltNetwork.IsRunning)
        {
            BoltLauncher.StartClient();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (BoltNetwork.IsRunning)
        {
            BoltNetwork.ShutdownImmediate();
        }
    }

    public void OnRenameButtonClick()
    {
        UIManager.Instance.ShowUIForms<CreateNamePanel>().Display();
    }

    public void RefreshButtonClick()
    {
        if (GameManager.Instance.M_NetworkMode == GameManager.NetworkMode.Online)
        {
            BoltManager.UpdateRoomList(BoltNetwork.SessionList, CurrentFilter);
        }
    }

    public void OnBackButtonClick()
    {
        BoatMenuManager.Instance.FromLobbyBackToStartMenu();
    }

    private bool interactable = true;

    public bool Interactable
    {
        get { return interactable; }
        set
        {
            if (interactable != value)
            {
                interactable = value;
                RoomIDInputField.interactable = value;
                RenameButton.interactable = value;
                SearchInputField.interactable = value;
                CreateRoomButton.interactable = value;
                BackButton.interactable = value;

                foreach (RoomButton roomButton in RoomButtons)
                {
                    roomButton.Button.interactable = value;
                }
            }
        }
    }

    public void OnButtonHover()
    {
        AudioDuck.Instance.PlaySound(AudioDuck.Instance.Hover, GameManager.Instance.gameObject);
    }

    public void OnButtonClick()
    {
        AudioDuck.Instance.PlaySound(AudioDuck.Instance.Click, GameManager.Instance.gameObject);
    }
}