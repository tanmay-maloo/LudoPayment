using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopup : MonoBehaviour
{
    public Image Image;
    public GameObject Popup;
    // Start is called before the first frame update
    void Start()
    {
        Image.canvasRenderer.SetAlpha(1.0f);
        Image.CrossFadeAlpha(0,3,false);
        //fadeOut();
    }

    // Update is called once per frame
    void fadeOut()
    {
        
        //yield return new WaitForSeconds(3.0f);
        //Popup.SetActive(false);
    }
}
