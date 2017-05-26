using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Sharing.Tests
{
    public class TextureLoader : MonoBehaviour
    {
        Texture2D selfTexture;
        // Use this for initialization
        void Start()
        {
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Texture] = UpdateTexture;


        }

        
        // Update is called once per frame
        void Update()
        {

        }

        private void UpdateTexture(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            string texture = msg.ReadString();
            byte[] b64_bytes = System.Convert.FromBase64String(texture);
            selfTexture = new Texture2D(1, 1);
            selfTexture.LoadImage(b64_bytes);

            GetComponent<Renderer>().material.mainTexture = selfTexture;

        }
    }
}
