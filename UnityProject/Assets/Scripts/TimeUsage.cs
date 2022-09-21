using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TimeUsage : MonoBehaviour
{
    public PathfindingDebugStepVisual ptf;
    bool stopWatchActive = false;
    float currentTime;
    public Text currentTimeText;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(ptf.autoShowSnapshots)
        {
            currentTime = currentTime + Time.deltaTime;
        }
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            currentTimeText.text = time.Seconds.ToString() + "." + time.Milliseconds.ToString() + " seconds";
    }
}