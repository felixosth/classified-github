using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VideoOS.Mobile.Portable.MetaChannel;
using VideoOS.Mobile.Portable.Utilities;
using VideoOS.Mobile.Portable.VideoChannel.Params;
using VideoOS.Mobile.Portable.ViewGroupItem;
using VideoOS.Mobile.SDK.Portable.Server.Base.CommandResults;
using VideoOS.Mobile.SDK.Portable.Server.Base.Connection;
using VideoOS.Mobile.SDK.Portable.Server.Base.Video;
using VideoOS.Mobile.SDK.Portable.Server.ViewGroups;

namespace Assets
{
    public class MilestoneLiveVideo : MonoBehaviour
    {
        public MilestoneConnection Connection;

        private LiveVideo liveVideo;
        public Renderer RenderTo;
        private Texture2D renderTex;
        private RenderTexture MyRenderTexture;

        public string CameraId;
        public bool UseAnyCamera;
        public int CameraIndex = -1;

        public ViewGroupTree Camera { get; set; }

        private byte[] lastImgBytes;
        private int lastImgBytesHash = 0;

        public int Width = 1280;
        public int Height = 720;
        public int FPS = 5;

        public int RenderTextureSize = 512;
        public int RenderTextureDepth = 24;

        public bool Playing { get; set; } = false;
        public bool StartVideoOnStart = true;

        private void Start()
        {
            renderTex = new Texture2D(1, 1);
            if(RenderTo == null)
                RenderTo = GetComponent<Renderer>();

            MyRenderTexture = new RenderTexture(RenderTextureSize, RenderTextureSize, RenderTextureDepth, RenderTextureFormat.ARGB32);
            MyRenderTexture.antiAliasing = 2;
            //MyRenderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            MyRenderTexture.Create();
            RenderTo.material.mainTexture = MyRenderTexture;

            //var renderTexss = GetComponent<RenderTexture>();

            if (UseAnyCamera)
            {
                if (CameraIndex < 0)
                    Camera = Connection.Cameras.FirstOrDefault();
                else if(CameraIndex < Connection.Cameras.Count)
                    Camera = Connection.Cameras[CameraIndex];
                //Debug.Log("Starting feed to " + Camera?.ItemName);
                CameraId = Camera?.CameraId.ToString();
            }
            else
            {
                Camera = Connection.Cameras.FirstOrDefault(c => c.CameraId.ToString().ToLower() == CameraId.ToLower());
            }

            if (!string.IsNullOrEmpty(CameraId) && StartVideoOnStart)
            {
                StartVideo();
            }
        }

        public void StartVideo()
        {
            if (liveVideo != null)
                liveVideo.Stop();
            if (Playing)
                return;

            liveVideo = Connection.StartVideo(Guid.Parse(CameraId), Width, Height, FPS);
            liveVideo.NewFrame = OnNewFrame;
            liveVideo.Start();
            Playing = true;
        }

        public void StopVideo()
        {
            if (liveVideo == null || !Playing)
                return;

            liveVideo.Stop();
            Playing = false;
        }

        private void OnNewFrame(VideoFrame frame)
        {
            if ((frame.Data != null) && frame.Data.Any())
            {
                lastImgBytes = frame.Data;
            }
        }

        bool first = true;

        public void Update()
        {
            if (lastImgBytes != null && lastImgBytes.Any())
            {
                var imgBytesHashCode = lastImgBytes.GetHashCode();
                if (lastImgBytesHash != imgBytesHashCode)
                {
                    //if (first)
                    //{
                    //    first = false;
                    //    File.WriteAllBytes(@"D:\Dev\GitHub\InSupportVR\MilestoneVR\Assets\" + CameraIndex + ".jpg", lastImgBytes);
                    //}

                    lastImgBytesHash = imgBytesHashCode;
                    if(RenderTo.material.mainTexture is Texture2D)
                    {
                        ((Texture2D)RenderTo.material.mainTexture).LoadImage(lastImgBytes);
                    }
                    else if (renderTex.LoadImage(lastImgBytes))
                    {
                        //RenderTo.material.mainTexture = renderTex;
                        Graphics.Blit(renderTex, MyRenderTexture);
                    }
                }
            }
        }
    }
}
