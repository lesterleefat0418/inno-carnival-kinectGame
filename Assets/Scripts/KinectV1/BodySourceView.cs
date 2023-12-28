using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public string bodyLayerName;

    private BodySourceManager _BodyManager = null;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    

    void Update () 
    {
     
        _BodyManager = BodySourceManager.Instance;

        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                bodyGestureManager.Instance.removeBody((int)trackingId);
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        int layerIndex = LayerMask.NameToLayer(bodyLayerName);
        if (layerIndex >= 0) {
            body.layer = layerIndex;
        }
        bodyGestureManager.Instance.AddBody((int)id);
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (layerIndex >= 0)
            {
                jointObj.layer = layerIndex;
            }

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = BoneMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            
            if (jt == Kinect.JointType.HandLeft)
            {
                kv2LeftHandTracker lhtrack = jointObj.AddComponent<kv2LeftHandTracker>();
                lhtrack.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.HandRight)
            {
                kv2RightHandTracker rht= jointObj.AddComponent<kv2RightHandTracker>();
                rht.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.SpineBase)
            {
                kv2CenterTracker center = jointObj.AddComponent<kv2CenterTracker>();
                center.bodyID = (int)id;
            }
            if(jt == Kinect.JointType.ShoulderLeft)
            {
                kv2LeftShoulderTracker lShoulder = jointObj.AddComponent<kv2LeftShoulderTracker>();
                lShoulder.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.ShoulderRight)
            {
                kv2RightShoulderTracker rShoulder = jointObj.AddComponent<kv2RightShoulderTracker>();
                rShoulder.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.SpineShoulder)
            {
                kv2SpineShoulderTracker SpineShoulder = jointObj.AddComponent<kv2SpineShoulderTracker>();
                SpineShoulder.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.Head)
            {
                kv2HeadTracker mHead = jointObj.AddComponent<kv2HeadTracker>();
                mHead.bodyID = (int)id;
            }
            if(jt == Kinect.JointType.KneeLeft)
            {
                kv2LeftKnee lknee = jointObj.AddComponent<kv2LeftKnee>();
                lknee.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.KneeRight)
            {
                kv2RightKnee rknee = jointObj.AddComponent<kv2RightKnee>();
                rknee.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.AnkleLeft)
            {
                kv2LeftFootTracker lFoot = jointObj.AddComponent<kv2LeftFootTracker>();
                lFoot.bodyID = (int)id;
            }
            if (jt == Kinect.JointType.AnkleRight)
            {
                kv2RightFootTracker rFoot = jointObj.AddComponent<kv2RightFootTracker>();
                rFoot.bodyID = (int)id;
            }
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            

            Transform jointObj = bodyObject.transform.Find(jt.ToString());

            if(jointObj != null) { 
                jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                if(targetJoint.HasValue)
                {
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                    lr.startColor = GetColorForState(sourceJoint.TrackingState);
                    lr.endColor = GetColorForState(targetJoint.Value.TrackingState);
                }
                else
                {
                    lr.enabled = false;
                }
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
