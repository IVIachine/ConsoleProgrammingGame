using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockColScript : MonoBehaviour {
    [SerializeField]
    private float mSoundScalar, mDestroyTime;

    [SerializeField]
    private GameObject mPulseGenPrefab;

    private bool mCollided = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!mCollided)
        {
            GameObject pulse = GameObject.Instantiate(mPulseGenPrefab, transform.position, Quaternion.identity);
            pulse.transform.parent = transform;
            pulse.SetActive(true);
            Destroy(gameObject, mDestroyTime);
            mCollided = true;
        }
    }
}
