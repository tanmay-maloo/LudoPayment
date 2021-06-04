using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameConfigrationController : MonoBehaviour
{

    public GameObject TitleText;
    public GameObject bidText;
    public GameObject MinusButton;
    public GameObject PlusButton;
    public GameObject[] Toggles;
    private int currentBidIndex = 0;

    private MyGameMode[] modes = new MyGameMode[] { MyGameMode.Classic, MyGameMode.Quick, MyGameMode.Master };
    public GameObject privateRoomJoin;

    public GameObject failedDialog;
    // Use this for initialization

    public bool inRoomActive = false;
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            Toggles[i].GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    ChangeGameMode(value, modes[index]);
                }
            );
        }

        currentBidIndex = 0;
        UpdateBid(true);

        Toggles[0].GetComponent<Toggle>().isOn = true;
        GameManager.Instance.mode = MyGameMode.Classic;

        switch (GameManager.Instance.type)
        {
            case MyGameType.TwoPlayer:
                TitleText.GetComponent<Text>().text = "Two Players";
                break;
            case MyGameType.FourPlayer:
                TitleText.GetComponent<Text>().text = "Four Players";
                break;
            case MyGameType.Private:
                TitleText.GetComponent<Text>().text = "Private Room";
                privateRoomJoin.SetActive(true);
                break;
            case MyGameType.Online:
                TitleText.GetComponent<Text>().text = "Online Room";
                break;
        }

    }

    void OnDisable()
    {
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        }

        privateRoomJoin.SetActive(false);
        currentBidIndex = 0;
        UpdateBid(false);
        Toggles[0].GetComponent<Toggle>().isOn = true;
        Toggles[1].GetComponent<Toggle>().isOn = false;
        Toggles[2].GetComponent<Toggle>().isOn = false;
    }

    public void setCreatedProvateRoom()
    {
        GameManager.Instance.JoinedByID = false;
    }

    public void startGame()
    {
        inRoomActive = false;
        if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
        {
            if (GameManager.Instance.type != MyGameType.Private && GameManager.Instance.type != MyGameType.Online)
            {
                GameManager.Instance.payoutCoins= 0;
                GameManager.Instance.facebookManager.startRandomGame();
            }
            else if(GameManager.Instance.type == MyGameType.Private)
            {
                if (GameManager.Instance.JoinedByID)
                {
                    Debug.Log("Joined by id!");
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }
                else
                {
                    Debug.Log("Joined and created");
                    GameManager.Instance.playfabManager.CreatePrivateRoom();
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }

            }
            else{
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsVisible = true; //check
                roomOptions.MaxPlayers = 2;
                //PhotonNetwork.JoinRandomRoom();
                RoomInfo[] rooms = PhotonNetwork.GetRoomList();
                OnJointrandom();
                if (rooms.Length == 0 || inRoomActive== false) //|| PhotonNetwork.inRoom == false
                {
                    Debug.Log("no rooms! need to create one");
                    GameManager.Instance.playfabManager.CreatePrivateRoom2();
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                    // CreateOnlineRoom();
                    // GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }
            }
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }

    public void CreateOnlineRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        string roomName = Random.Range(0, 100000).ToString() + "A";

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "pc" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "pc", GameManager.Instance.payoutCoins}
         };
        Debug.Log("Online room name: " + roomName);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void OnJointrandom()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        
            for (int i = 0; i < rooms.Length; i++){
                Debug.Log("Name of each room: " + rooms[i].Name);
                if (rooms[i].CustomProperties.ContainsKey("pc") && rooms[i].Name[0] != 'a')
                {
                    GameManager.Instance.payoutCoins = int.Parse(rooms[i].CustomProperties["pc"].ToString());

                    if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins && StaticStrings.bidValues[currentBidIndex] == GameManager.Instance.payoutCoins) //
                    {
                        Debug.Log("room bid amount:" + GameManager.Instance.payoutCoins.ToString());
                        Debug.Log("player total cash available:"+GameManager.Instance.myPlayerData.GetCoins().ToString());
                        Debug.Log("MY bid:"+ StaticStrings.bidValues[currentBidIndex].ToString());
                        PhotonNetwork.JoinRoom(rooms[i].Name);
                        Debug.Log("Room Status" + inRoomActive.ToString());
                        inRoomActive = true;
                        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                    }
                    
                }
            }
    }




















    private void ChangeGameMode(bool isActive, MyGameMode mode)
    {
        if (isActive)
        {
            GameManager.Instance.mode = mode;
        }
    }



    public void IncreaseBid()
    {
        // if(GameManager.Instance.type == MyGameType.TwoPlayer || GameManager.Instance.type == MyGameType.FourPlayer){
        //     MinusButton.GetComponent<Button>().interactable = false;
        //     PlusButton.GetComponent<Button>().interactable = false;
        // }
        if (currentBidIndex < StaticStrings.bidValues.Length - 1)
        {
            currentBidIndex++;
            UpdateBid(true);
        }
    }

    public void DecreaseBid()
    {
        if (currentBidIndex > 0)
        {
            currentBidIndex--;
            UpdateBid(true);
        }
    }

    private void UpdateBid(bool changeBidInGM)
    {
        bidText.GetComponent<Text>().text = StaticStrings.bidValuesStrings[currentBidIndex];
        if (changeBidInGM)
            GameManager.Instance.payoutCoins = StaticStrings.bidValues[currentBidIndex];

        if (currentBidIndex == 0) MinusButton.GetComponent<Button>().interactable = false;
        else MinusButton.GetComponent<Button>().interactable = true;

        if (currentBidIndex == StaticStrings.bidValues.Length - 1) PlusButton.GetComponent<Button>().interactable = false;
        else PlusButton.GetComponent<Button>().interactable = true;
    }

    public void HideThisScreen()
    {
        gameObject.SetActive(false);
    }
}
