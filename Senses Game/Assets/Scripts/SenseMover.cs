using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SenseMover : MonoBehaviour
{

    private int mIndex;
    private GameObject mSenseSwitcherRef;
    private GameObject mPlayerRef;
    private float mCurrentUpdate, mUpdateFrequency;

    public void beginMovement(int index, GameObject reference, GameObject player, float frequency)
    {
        mIndex = index;
        mSenseSwitcherRef = reference;
        mPlayerRef = player;

        mUpdateFrequency = frequency;
        mCurrentUpdate = mUpdateFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NavMeshAgent>().remainingDistance <= .025f)
            mSenseSwitcherRef.GetComponent<SenseSwitcher>().RemoveSound(gameObject);

        mCurrentUpdate -= Time.deltaTime;

        if (mCurrentUpdate <= 0)
        {
            NavMeshPath path = new NavMeshPath();
            if (GetComponent<NavMeshAgent>().CalculatePath(mPlayerRef.transform.position, path))
            {
                GetComponent<NavMeshAgent>().SetPath(path);
            }

            mCurrentUpdate = mUpdateFrequency;
        }
    }
}
