using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseNoiseGen : MonoBehaviour
{

    [SerializeField]
    private float mFrequency, mSoundScalar;

    [SerializeField]
    private GameObject mSenseSwitchObj;

    private bool mHasPlayed;
    // Use this for initialization
    void Start()
    {
        mSenseSwitchObj = GameObject.Find("SenseManager");
        PlaySound();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mHasPlayed)
        {
            Invoke("PlaySound", mFrequency);
            mHasPlayed = true;
        }
    }

    private void PlaySound()
    {
        GetComponent<AudioSource>().Play();
        mSenseSwitchObj.GetComponent<SenseSwitcher>().AddNewSound(GetComponent<AudioSource>().volume * mSoundScalar, transform.position);
        mHasPlayed = false;
    }
}