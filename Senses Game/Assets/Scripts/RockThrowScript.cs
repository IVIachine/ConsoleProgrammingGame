using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowScript : MonoBehaviour {

    [SerializeField]
    private GameObject mRockPrefab;

    [SerializeField]
    private float mThrowCooldown, mThrowForce;

    private bool mHolding;
    private float mCurrentCooldown;
    private GameObject mRock;

	// Use this for initialization
	void Start () {
        mHolding = false;
        mCurrentCooldown = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(!mHolding)
        {
            mCurrentCooldown -= Time.deltaTime;
            if(Input.GetMouseButtonDown(0) && mCurrentCooldown <= 0)
            {
                mRock = GameObject.Instantiate(mRockPrefab, transform.position, Quaternion.identity);
                mRock.transform.parent = transform;
                mHolding = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                mRock.GetComponent<Rigidbody>().isKinematic = false;
                mRock.GetComponent<SphereCollider>().enabled = true;
                mRock.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * mThrowForce * Time.deltaTime);
                mRock.transform.parent = null;
                mCurrentCooldown = mThrowCooldown;
                mHolding = false;
            }
        }
    }
}
