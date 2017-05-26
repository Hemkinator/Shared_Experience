using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing.Tests
{
    public class ObjectSync : MonoBehaviour
    {
        protected Vector3 Position;
        protected Quaternion Rotation;
        protected Vector3 Scale;
        // Use this for initialization
        void Start()
        {
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ObjTransform] = UpdateObjTransform;

            // SharingStage should be valid at this point.
            SharingStage.Instance.SharingManagerConnected += Connected;
        }
        private void Connected(object sender, EventArgs e)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;


        }
        // Update is called once per frame
        void Update()
        {
            Position = transform.localPosition;
            Rotation = transform.localRotation;
            CustomMessages.Instance.SendObjTransform(Position, Rotation);
        }

        private void UpdateObjTransform(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

            Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

            transform.localPosition = headPos;
            transform.localRotation = headRot;
        }
    }
}
