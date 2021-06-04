using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using UnityEngine.SceneManagement;
public class UpdatePlayerTimer : PunBehaviour
{
    private float playerTime;
    public GameGUIController gUIController; //tanmay
    public GameObject timerObject;
    private Image timer;
    private bool timeSoundsStarted;
    public AudioSource[] audioSources;
    public GameObject GUIController;
    public bool myTimer;
    public bool paused = false;
    
    // Use this for initialization
    private int counterT = 0;
    //to check if time is end or not
    void Start()
    {
        timer = gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        timer = gameObject.GetComponent<Image>();
    }

    public void Pause()
    {
        paused = true;
        audioSources[0].Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
            updateClock();
    }

    public void restartTimer()
    {
        paused = false;
        timer.fillAmount = 1.0f;
    }


    void OnDisable()
    {
        if (timer != null)
        {
            timer.fillAmount = 1.0f;
            paused = false;
            audioSources[0].Stop();
        }
    }

    private void updateClock()
    {
        float minus;

        playerTime = GameManager.Instance.playerTime;
        if (GameManager.Instance.offlineMode)
            playerTime = GameManager.Instance.playerTime + GameManager.Instance.cueTime;
        minus = 1.0f / playerTime * Time.deltaTime;

        timer.fillAmount -= minus;

        if (timer.fillAmount < 0.25f && !timeSoundsStarted)
        {
            audioSources[0].Play();
            timeSoundsStarted = true;
        }

        if (timer.fillAmount == 0)
        {

            Debug.Log("TIME 0");
            counterT = counterT+1;
            audioSources[0].Stop();
            GameManager.Instance.stopTimer = true;
            if (!GameManager.Instance.offlineMode)
            {
                if (myTimer)
                {   
                    if(counterT >= 3){
                        Debug.Log("Timer call finish turn" + counterT.ToString());
                    SceneManager.LoadScene("MenuScene");
                    PhotonNetwork.LeaveRoom();
                    }
                    
                GUIController.GetComponent<GameGUIController>().SendFinishTurn();
                    
                    
                    
                }
                //PhotonNetwork.RaiseEvent(9, null, true, null);
            }
            else
            {
                GameManager.Instance.wasFault = true;
                GameManager.Instance.cueController.setTurnOffline(true);
            }




            //showMessage("You " + StaticStrings.runOutOfTime);

            /*if (!GameManager.Instance.offlineMode)
            {
                GameManager.Instance.cueController.setOpponentTurn();
            }*/

        }


    }
}
