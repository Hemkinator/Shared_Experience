using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using UnityEngine.Windows.Speech;
using System.IO;
using System.Collections;

namespace HoloToolkit.Sharing.Tests
{
    public class PictureManager : MonoBehaviour
    {

        PhotoCapture photoCaptureObject = null;
        KeywordRecognizer keywordRecognizer;
        delegate void KeywordAction(PhraseRecognizedEventArgs args);
        Dictionary<string, KeywordAction> keywordCollection;

        void Start()
        {
            keywordCollection = new Dictionary<string, KeywordAction>();
            keywordCollection.Add("Take Picture", TakePicture);
            keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }
        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            KeywordAction keywordAction;
            if (keywordCollection.TryGetValue(args.text, out keywordAction))
                keywordAction.Invoke(args);
        }
        void TakePicture(PhraseRecognizedEventArgs prea)
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }
        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 0.0f;
            c.cameraResolutionWidth = cameraResolution.width;
            c.cameraResolutionHeight = cameraResolution.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }
        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
            else Debug.LogError("Unable to start photo mode!");
        }


        void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            if (result.success)
            {
                // Create our Texture2D for use and set the correct resolution
                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
                // Copy the raw image data into our target texture
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);
                // Do as we wish with the texture such as apply it to a material, etc.
                byte[] bytes;

                bytes = targetTexture.EncodeToPNG();
                string encodedImage = System.Convert.ToBase64String(bytes);
                CustomMessages.Instance.SendTexture(encodedImage);
            }
            // Clean up
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }
}