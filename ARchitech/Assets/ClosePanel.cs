using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class ClosePanel : MonoBehaviour
{
    private RawImage panelImage;
    private float timeCountDowm;

    // Update is called once per frame
    private void Start()
    {
        panelImage = this.gameObject.GetComponent<RawImage>();
        timeCountDowm = 5f;
    }

    private void Update()
    {
        if (panelImage.enabled) {
            
            timeCountDowm -= Time.deltaTime;
            if (timeCountDowm <= 0) {
                hideImage();
            }
        }
    }

    private void hideImage()
    {
        panelImage.enabled = false;
    }
}
