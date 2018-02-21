using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SenseMover : MonoBehaviour {

    private int mIndex;
    private GameObject mSenseSwitcherRef;

	public void beginMovement(int index, GameObject reference)
    {
        mIndex = index;
        mSenseSwitcherRef = reference;
    }
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<NavMeshAgent>().remainingDistance <= 0)
            mSenseSwitcherRef.GetComponent<SenseSwitcher>().RemoveSound(mIndex);
	}
}
