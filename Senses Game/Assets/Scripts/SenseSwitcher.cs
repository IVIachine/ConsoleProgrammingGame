using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SenseSwitcher : MonoBehaviour {

    [SerializeField]
    private Material mSwitchMat;

    [SerializeField]
    private int mMaxSounds;

    [SerializeField]
    private GameObject mSoundPrefab;

    [SerializeField]
    private GameObject mPlayerRef;

    [SerializeField]
    private float mUpdateFrequency;

    private List<GameObject> mObjectRefs;
    private List<List<Material>> mOriginalMats;

    private bool mIsSight;
    private Material mActualMat, mOldSkybox;

    private ComputeBuffer mSoundPositions;
    private ComputeBuffer mSoundRadii;
    private ComputeBuffer mReds;

    private int mNumSounds;
    private GameObject[] mCurrentSounds;

	// Use this for initialization
	void Start () {
        mCurrentSounds = new GameObject[mMaxSounds];
        mIsSight = true;
        mNumSounds = 0;
        mActualMat = Material.Instantiate(mSwitchMat);
        mOldSkybox = RenderSettings.skybox;

        mObjectRefs = new List<GameObject>();
        mOriginalMats = new List<List<Material>>();
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        foreach(MeshRenderer rend in renderers)
        {
            mObjectRefs.Add(rend.gameObject);

            List<Material> oldMats = new List<Material>();
            foreach(Material mat in rend.materials)
                oldMats.Add(mat);

            mOriginalMats.Add(oldMats);
        }

        SkinnedMeshRenderer[] skinnedRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer rend in skinnedRenderers)
        {
            mObjectRefs.Add(rend.gameObject);

            List<Material> oldMats = new List<Material>();
            foreach (Material mat in rend.materials)
                oldMats.Add(mat);

            mOriginalMats.Add(oldMats);
        }


        Vector3[] soundInit = new Vector3[mMaxSounds];
        mSoundPositions = new ComputeBuffer(mMaxSounds, System.Runtime.InteropServices.Marshal.SizeOf(Vector3.zero), ComputeBufferType.Default);
        mSoundPositions.SetData(soundInit);

        float temp = 0;
        float[] soundRadiiInit = new float[mMaxSounds];
        mSoundRadii = new ComputeBuffer(mMaxSounds, System.Runtime.InteropServices.Marshal.SizeOf(temp), ComputeBufferType.Default);
        mSoundRadii.SetData(soundRadiiInit);

        int temp2 = 0;
        int[] mRedsInitBuff = new int[mMaxSounds];
        mReds = new ComputeBuffer(mMaxSounds, System.Runtime.InteropServices.Marshal.SizeOf(temp2), ComputeBufferType.Default);
        mReds.SetData(mRedsInitBuff);

        mActualMat.SetBuffer("soundPositions", mSoundPositions);
        mActualMat.SetBuffer("soundRadii", mSoundRadii);
        mActualMat.SetBuffer("redList", mReds);

        mActualMat.SetInt("numPositions", mNumSounds);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            mIsSight = !mIsSight;

            if(mIsSight)
            {
                int index = 0;
                foreach(GameObject obj in mObjectRefs)
                {
                    if (obj.GetComponent<MeshRenderer>())
                    {
                        Material[] mats = obj.GetComponent<MeshRenderer>().materials;
                        for (int i = 0; i < mOriginalMats[index].Count; i++)
                        {
                            mats[i] = mOriginalMats[index][i];
                        }

                        obj.GetComponent<MeshRenderer>().materials = mats;
                    }
                    else if (obj.GetComponent<SkinnedMeshRenderer>())
                    {
                        Material[] mats = obj.GetComponent<SkinnedMeshRenderer>().materials;
                        for (int i = 0; i < mOriginalMats[index].Count; i++)
                        {
                            mats[i] = mOriginalMats[index][i];
                        }

                        obj.GetComponent<SkinnedMeshRenderer>().materials = mats;
                    }
                    index++;
                }

                RenderSettings.skybox = mOldSkybox;
            }
            else
            {
                int index = 0;
                foreach (GameObject obj in mObjectRefs)
                {
                    if (obj.GetComponent<MeshRenderer>())
                    {
                        Material[] mats = obj.GetComponent<MeshRenderer>().materials;
                        for (int i = 0; i < mOriginalMats[index].Count; i++)
                        {
                            mats[i] = mActualMat;
                        }

                        obj.GetComponent<MeshRenderer>().materials = mats;
                    }
                    else if (obj.GetComponent<SkinnedMeshRenderer>())
                    {
                        Material[] mats = obj.GetComponent<SkinnedMeshRenderer>().materials;
                        for (int i = 0; i < mOriginalMats[index].Count; i++)
                        {
                            mats[i] = mActualMat;
                        }

                        obj.GetComponent<SkinnedMeshRenderer>().materials = mats;
                    }

                    index++;
                }

                RenderSettings.skybox = null;
            }
        }

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        for (int i = 0; i < mNumSounds; i++)
        {
            soundPos[i] = mCurrentSounds[i].transform.position;
        }

        mSoundPositions.SetData(soundPos);
        mActualMat.SetInt("numPositions", mNumSounds);
    }

    public  void RemoveSound(GameObject reference)
    {
        int index = -1;
        for (int i = 0; i < mCurrentSounds.Length; i++)
        {
            if(mCurrentSounds[i] == reference)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return;

        float[] soundRadii = new float[mMaxSounds];
        mSoundRadii.GetData(soundRadii);

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        int[] reds = new int[mMaxSounds];
        mReds.GetData(reds);

        soundPos[index] = Vector3.zero;
        soundRadii[index] = 0;
        reds[index] = 0;

        mSoundRadii.SetData(soundRadii);
        mSoundPositions.SetData(soundPos);
        mReds.SetData(reds);

        Destroy(mCurrentSounds[index]);

        float[] radiiTemp = new float[mMaxSounds];
        Array.Copy(soundRadii, radiiTemp, mMaxSounds);
        Array.Sort(radiiTemp, soundPos);

        float[] radiiTemp2 = new float[mMaxSounds];
        Array.Copy(soundRadii, radiiTemp2, mMaxSounds);

        float[] radiiTemp3 = new float[mMaxSounds];
        Array.Copy(soundRadii, radiiTemp3, mMaxSounds);

        Array.Sort(radiiTemp3, reds);
        Array.Sort(radiiTemp2, mCurrentSounds);

        Array.Sort(soundRadii);

        Array.Reverse(reds);
        Array.Reverse(soundRadii);
        Array.Reverse(soundPos);
        Array.Reverse(mCurrentSounds);

        mSoundPositions.SetData(soundPos);
        mSoundRadii.SetData(soundRadii);
        mReds.SetData(reds);

        mNumSounds--;
    }

    public int AddNewSound(float radii, Vector3 pos)
    {
        if (mNumSounds >= mMaxSounds)
            return -1;

        float[] soundRadii = new float[mMaxSounds];
        mSoundRadii.GetData(soundRadii);

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        int[] reds = new int[mMaxSounds];
        mReds.GetData(reds);

        int index = -1;
        for (int i = 0; i < mMaxSounds; i++)
        {
            if (soundRadii[i] == 0)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return -1;

        soundRadii[index] = radii;
        soundPos[index] = pos;
        reds[index] = 0;
        mNumSounds++;

        mSoundRadii.SetData(soundRadii);
        mSoundPositions.SetData(soundPos);
        mReds.SetData(reds);

        mCurrentSounds[index] = GameObject.Instantiate(mSoundPrefab, pos, Quaternion.identity);
        NavMeshPath path = new NavMeshPath();
        if (mCurrentSounds[index].GetComponent<NavMeshAgent>().CalculatePath(mPlayerRef.transform.position, path))
        {
            mCurrentSounds[index].GetComponent<NavMeshAgent>().SetPath(path);
            mCurrentSounds[index].AddComponent<SenseMover>().beginMovement(index, this.gameObject, mPlayerRef, mUpdateFrequency, radii);
        }

        return index;
    }

    public void setCol(GameObject objRef, int isRed)
    {
        int index = -1;
        int[] reds = new int[mMaxSounds];
        mReds.GetData(reds);

        for (int i = 0; i < mCurrentSounds.Length; i++)
        {
            if (mCurrentSounds[i] == objRef)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return;

        reds[index] = isRed;
        mReds.SetData(reds);
    }

    private void OnDestroy()
    {
        mSoundPositions.Dispose();
        mSoundRadii.Dispose();
    }
}
