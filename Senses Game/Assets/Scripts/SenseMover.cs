using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SenseMover : MonoBehaviour
{

    private int mIndex;
    private GameObject mSenseSwitcherRef;
    private GameObject mPlayerRef;
    private float mCurrentUpdate, mUpdateFrequency, mRadius;
    private int mLastCol;

    public void beginMovement(int index, GameObject reference, GameObject player, float frequency, float radius)
    {
        mIndex = index;
        mSenseSwitcherRef = reference;
        mPlayerRef = player;

        mUpdateFrequency = frequency;
        mCurrentUpdate = mUpdateFrequency;
        mRadius = radius;
        mLastCol = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NavMeshAgent>().remainingDistance <= .05f)
            mSenseSwitcherRef.GetComponent<SenseSwitcher>().RemoveSound(gameObject);

        AIPatrolScript[] patrolAI = GameObject.FindObjectsOfType<AIPatrolScript>();
        int col = 0;
        foreach(AIPatrolScript ai in patrolAI)
        {
            if(Vector3.Distance(transform.position, ai.transform.position) <= mRadius)
            {
                col = 1;
            }
        }

        if(col != mLastCol)
        {
            mSenseSwitcherRef.GetComponent<SenseSwitcher>().setCol(gameObject, col);
        }

        mLastCol = col;
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
