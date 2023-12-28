using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyGestureManager : MonoBehaviour {
    public class kinnectBody
    {
        public int bodyID;
        public Vector3 headPos;
        public Vector3 bodyPos;
        public Vector3 leftPos;
        public Vector3 rightPos;
        public Vector3 lShoulderPos;
        public Vector3 rShoulderPos;
        public Vector3 SpineShoulderPos;
        public Vector3 lKneePos;
        public Vector3 rKneePos;
        public Vector3 lFootPos;
        public Vector3 rFootPos;


        public Vector3 intLeftPos;
        public Vector3 intRightPos;
        public float holdTimer =0;
        public int holdcount;
        public Vector3 intcloseRightPos;
        public int closecount = 0;
        public float closeTimer = 0;
        public float cutGestureTimer = 0;
        public Vector3 prvlCutPos = Vector3.zero;
        public Vector3 prvRCutPos = Vector3.zero;
    }

    public Dictionary<int,kinnectBody> bodyMap = new Dictionary<int, kinnectBody>();

    public motionconfig mConfig;
    public GameObject[] DebugObj;
    static bodyGestureManager _instance;
    public static bodyGestureManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        mConfig = new motionconfig();

        if (System.IO.File.Exists(Application.dataPath + "/../config.json"))
            mConfig = JsonUtility.FromJson<motionconfig>(System.IO.File.ReadAllText(Application.dataPath + "/../config.json"));
    }
    // Use this for initialization
    void Start () {
        
        if (!mConfig.debug)
            foreach (GameObject obj in DebugObj)
                obj.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
        foreach(kinnectBody body in bodyMap.Values)
        {
            updateHoldGesture(body.bodyID);
            updateCloseGesture(body.bodyID);
            cutGestureUpdate(body.bodyID);
        }
        
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Debug.Log("Config path:"+ Application.dataPath + "/../config1.json");
        //    System.IO.File.WriteAllText(Application.dataPath + "/../config1.json", JsonUtility.ToJson(mConfig));
        //}
        if(Input.GetKeyDown(KeyCode.D))
        {
            foreach (GameObject obj in DebugObj)
                obj.SetActive(!obj.activeSelf);
            mConfig.debug = !mConfig.debug;
            Cursor.visible = mConfig.debug;
        }
        if (info)
        {
            info.text = "";
            foreach (kinnectBody body in bodyMap.Values)
            {
                info.text += string.Format("id:{0}\ncenter:{1}\nleft hand:{2}\nright hand:{3}\n"+/*gesture:{4}\nHold count:{5}\n*/"L shoulder:{6}\nR shoulder:{7}\nHead:{8}\nleft knee:{9}\nright knee:{10}\nDirection:{11}\n\n",
                body.bodyID, body.bodyPos.ToString(),  body.leftPos, body.rightPos, isbodyGesture(body.bodyID), 0, Vector3.Dot((body.lShoulderPos - body.leftPos), Vector3.up), Vector3.Dot((body.rShoulderPos - body.rightPos), Vector3.up), body.headPos,body.lKneePos,body.rKneePos, bodyHeading(body));
            }

        }
    }

    public TextMesh info;

   float bodyHeading(kinnectBody body)
    {
        Vector3 bleftDir = body.SpineShoulderPos - body.rShoulderPos;
        bleftDir =  Quaternion.Euler(0,-90,0)* bleftDir;
        float ang = Vector3.Angle(bleftDir,Vector3.back);
        
        return ang*(Vector3.Cross(bleftDir, Vector3.back).y<0?-1:1);
    }

    public int numberOfBody() {
        return bodyMap.Count;
    }

    public int GetClosestBody() {
        float zPos = float.MaxValue;
        int closestBodyId = 0;
        foreach (int key in bodyMap.Keys) {
            Vector3 bodyPos = this.getBodyPos(key);
            if (bodyPos.z < zPos) {
                zPos = bodyPos.z;
                closestBodyId = key;
            }
        }
        return closestBodyId;
    }

    public void AddBody(int id)
    {
        if (!bodyMap.ContainsKey(id))
        {
            kinnectBody body = new kinnectBody();
            body.bodyID = id;
            bodyMap.Add(id, body);
        }   
    }

    public void removeBody(int id)
    {
        if (bodyMap.ContainsKey(id))
            bodyMap.Remove(id);
    }

    public void updateLastKnowBodyCenter(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].bodyPos = pos;
        }
    }
    public void updateSpineShoulder(int id,Vector3 pos)
    {
        if(bodyMap.ContainsKey(id))
        {
            bodyMap[id].SpineShoulderPos = pos;
        }
    }
    public void updateLastKnowBodyLeftHand(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].leftPos = pos;
        }
    }
    public void updateLastKnowBodyRightHand(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].rightPos = pos;
        }
    }
    public void updateLastKnowBodyLeftShoulder(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].lShoulderPos = pos;
        }
    }
    public void updateLastKnowBodyRightShoulder(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].rShoulderPos = pos;
        }
    }
    public void updateLastKnowBodyHead(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].headPos = pos;
        }
    }

    public void updateLastKnowLeftKnee(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].lKneePos = pos;
        }
    }
    public void updateLastKnowRightKnee(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].rKneePos = pos;
        }
    }
    public void updateLastKnowLeftFoot(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].lFootPos = pos;
        }
    }
    public void updateLastKnowRightFoot(int id, Vector3 pos)
    {
        if (bodyMap.ContainsKey(id))
        {
            bodyMap[id].rFootPos = pos;
        }
    }

    public Vector3 getheadPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].headPos;
        return Vector3.zero;
    }
    public Vector3 getBodyPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].bodyPos;
        return Vector3.zero;
    }
    public Vector3 getleftPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].leftPos;
        return Vector3.zero;
    }
    public Vector3 getrightPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].rightPos;
        return Vector3.zero;
    }
    public Vector3 getCenterPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].bodyPos;
        return Vector3.zero;
    }

    public Vector3 getSpineShoulderPos(int id)
    {
        if (bodyMap.ContainsKey(id))
            return bodyMap[id].SpineShoulderPos;
        return Vector3.zero;
    }

    public float holdProgess(int id)
    {
        return Mathf.Clamp01(bodyMap[id].holdTimer / mConfig.holdThreshold);
    }

    void updateHoldGesture(int id)
    {
        if (bodyMap[id].holdTimer == 0)
        {
            if (inRangeLeft(id) || inRangeRight(id))
            {
                bodyMap[id].intLeftPos = bodyMap[id].leftPos;
                bodyMap[id].intRightPos = bodyMap[id].rightPos;
            }
            bodyMap[id].holdTimer += Time.deltaTime;
        }
        else
        {
            if (inRangeLeft(id) && Vector3.Distance(bodyMap[id].intLeftPos, bodyMap[id].leftPos) < mConfig.holdDistlimit)
            {
                bodyMap[id].holdTimer += Time.deltaTime;
            }else if (inRangeRight(id) && Vector3.Distance(bodyMap[id].intRightPos, bodyMap[id].rightPos) < mConfig.holdDistlimit)
            {
                bodyMap[id].holdTimer += Time.deltaTime;
            }
            
            else
                bodyMap[id].holdTimer = 0;
        }
    }
    public bool isHoldGesture(int id)
    {
        if (mConfig.holdThreshold <= bodyMap[id].holdTimer)
        {
            bodyMap[id].holdcount++;
            return true;
        }
        return false;
    }

    public bool isDetected()
    {
        return bodyMap.Count > 0;
    }

    public bool isRasehandGesture(int id)
    {
        if(bodyMap.ContainsKey(id))
        {
            float sign = Vector3.Dot((bodyMap[id].lShoulderPos - bodyMap[id].leftPos), Vector3.up);
            if (sign < -3)
            {
                return true;
            }
            sign = Vector3.Dot((bodyMap[id].rShoulderPos - bodyMap[id].rightPos), Vector3.up);
            if (sign < -3)
            {
                return true;
            }
        }
        return Input.GetMouseButton(1);
    }
   
    public float closeProgess(int id)
    {
        return Mathf.Clamp01(bodyMap[id].closeTimer / mConfig.holdThreshold);
    }
    void updateCloseGesture(int id)
    {
        if (bodyMap[id].closeTimer == 0)
        {
            if ( inRangeRight(id))
            {
                bodyMap[id].intcloseRightPos = bodyMap[id].rightPos;
            }

            bodyMap[id].closeTimer += Time.deltaTime;
        }
        else
        {
            if (inRangeRight(id) && Vector3.Distance(bodyMap[id].intRightPos, bodyMap[id].rightPos) < mConfig.holdDistlimit)
            {
                bodyMap[id].closeTimer += Time.deltaTime;
            }
            else
                bodyMap[id].closeTimer = 0;
        }
    }
    public bool isCloseGesture(int id)
    {
        if (mConfig.holdThreshold <= bodyMap[id].closeTimer)
        {
            bodyMap[id].closecount++;
            return true;
        }
        return false;
    }

    //swap gesture: body z22,hand z19
    bool inRangeLeft(int id)
    {
        //return bodyPos.z > 21 && leftPos.z < 20;
        return bodyMap[id].bodyPos.z > mConfig.bodyDist && bodyMap[id].leftPos.z < mConfig.handDist;
    }
    bool inRangeRight(int id)
    {
        //return bodyPos.z > 21 && leftPos.z < 20;
        return bodyMap[id].bodyPos.z > mConfig.bodyDist && bodyMap[id].rightPos.z < mConfig.handDist;
    }
    public float getGestureX(int id)
    {
        if (Input.GetMouseButton(0)) return Input.mousePosition.x/10.0f;
        if (inRangeLeft(id))
        {
            return bodyMap[id].leftPos.x;
        };
        return bodyMap[id].rightPos.x;
    }
    public float getGestureY(int id)
    {
        if (Input.GetMouseButton(0)) return Input.mousePosition.y / 10.0f;
        if (inRangeLeft(id))
        {
            return bodyMap[id].leftPos.y;
        };
        return bodyMap[id].rightPos.y;
    }
    public Vector3 getGestureVec(int id)
    {
        if (inRangeLeft(id))
        {
            return bodyMap[id].leftPos;
        };
        return bodyMap[id].rightPos;
    }
    public bool isbodyGesture(int id)
    {
        if (Input.GetMouseButton(0)) return true;
        if(bodyMap.ContainsKey(id))
        {
            if (inRangeLeft(id))
                return true;
            if (inRangeRight(id))
                return true;
        }
        return false;
    }
    public bool isCutGesture(int id)
    {
        if (Input.GetMouseButton(0)) return true;
        if (bodyMap[id].cutGestureTimer > 0.1f) return true;
        
        return false;
    }
    void cutGestureUpdate(int id)
    {
 
        if (bodyMap[id].prvlCutPos == Vector3.zero)
            bodyMap[id].prvlCutPos = bodyMap[id].leftPos;
        if (bodyMap[id].prvRCutPos == Vector3.zero)
            bodyMap[id].prvRCutPos = bodyMap[id].rightPos;

        if ((bodyMap[id].prvlCutPos - bodyMap[id].leftPos).y > 2 || (bodyMap[id].prvRCutPos - bodyMap[id].rightPos).y > 2 )
        {
            bodyMap[id].cutGestureTimer = 1;
        }
        else
        {
            bodyMap[id].cutGestureTimer = 0;
        }
        bodyMap[id].prvlCutPos = bodyMap[id].leftPos;
        bodyMap[id].prvRCutPos = bodyMap[id].rightPos;
    }
}
