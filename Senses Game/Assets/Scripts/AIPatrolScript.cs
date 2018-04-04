using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class AIPatrolScript : MonoBehaviour
{

    [SerializeField]
    List<Vector3> mPatrolLocations;

    [SerializeField]
    private float mWaitTime, mSeeDistance;

    private float mCurrentWait;
    private int mCurrentIndex;
    private bool mPathfinding;
    private GameObject mPlayer;
    // Use this for initialization
    void Start()
    {
        mCurrentIndex = 0;
        mCurrentWait = 0.0f;
        mPathfinding = false;

        mPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!mPathfinding && mCurrentWait <= 0.0f)
        {
            mCurrentIndex++;
            if (mCurrentIndex >= mPatrolLocations.Count)
                mCurrentIndex = 0;

            NavMeshPath path = new NavMeshPath();
            if (GetComponent<NavMeshAgent>().CalculatePath(mPatrolLocations[mCurrentIndex], path))
            {
                GetComponent<NavMeshAgent>().SetPath(path);
            }
            else
            {
                Debug.LogError(name + " Failed to find path");
            }

            mPathfinding = true;
        }
        else if (!mPathfinding)
            mCurrentWait -= Time.deltaTime;

        if (GetComponent<NavMeshAgent>().remainingDistance <= .025f && mPathfinding)
        {
            mPathfinding = false;
            mCurrentWait = mWaitTime;
        }

        if (SeesPlayer())
            GameObject.Find("Fade").GetComponent<FadeScript>().beginFade(.8f, 1, SceneManager.GetActiveScene().name);
    }

    private bool SeesPlayer()
    {
        Vector3 pos = GetComponent<Camera>().WorldToViewportPoint(mPlayer.transform.position);
        if (!(pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1 && pos.z > 0) || 
            Vector3.Distance(mPlayer.transform.position, transform.position) > mSeeDistance)
            return false;

        return true;
    }
}
