using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VideoOS.Mobile.Portable.MetaChannel;
using VideoOS.Mobile.Portable.Utilities;
using VideoOS.Mobile.Portable.VideoChannel.Params;
using VideoOS.Mobile.Portable.ViewGroupItem;
using VideoOS.Mobile.SDK.Portable.Server.Base.CommandResults;
using VideoOS.Mobile.SDK.Portable.Server.Base.Connection;
using VideoOS.Mobile.SDK.Portable.Server.Base.Video;
using VideoOS.Mobile.SDK.Portable.Server.ViewGroups;

public class MilestoneConnection : MonoBehaviour
{
    private Connection Connection { get; set; }
    MilestoneSettings milestoneSettings;


    public List<ViewGroupTree> Cameras { get; private set; } = new List<ViewGroupTree>();

    public string[] CameraNames;


    // Start is called before the first frame update
    private void Awake()
    {
        VideoOS.Mobile.SDK.Environment.Instance.Initialize();

        milestoneSettings = MilestoneSettings.Load();
    }

    void Start()
    {

        var uri = new Uri(milestoneSettings.Server);
        var channelType = 0 == string.Compare(uri.Scheme, "http", StringComparison.InvariantCultureIgnoreCase)
            ? ChannelTypes.HTTP
            : ChannelTypes.HTTPSecure;
        Connection = new Connection(channelType, uri.Host, (uint)uri.Port);
        Connection.CommandsQueueing = CommandsQueueing.SingleThread;

        var connectResponse = Connection.Connect(null, TimeSpan.FromSeconds(15));
        if (connectResponse.ErrorCode != ErrorCodes.Ok)
            throw new Exception("Not connected to surveillance server");

        var loginResponse = Connection.LogIn(milestoneSettings.Username, milestoneSettings.Password, ClientTypes.MobileClient, TimeSpan.FromMinutes(2));
        if (loginResponse.ErrorCode != ErrorCodes.Ok)
            throw new Exception("Not loged in to the surveillance server");

        Connection.RunHeartBeat = true;

        var allCamerasViews = ViewGroupsHelper.GetAllCamerasViews(Connection.Views, TimeSpan.FromSeconds(15));
        ProcessViewItem(allCamerasViews);
        CameraNames = Cameras.Select(c => c.ItemName).ToArray();
        //string cameras = "";
        //Cameras.ForEach(c => cameras += c.ItemName + (Cameras.IndexOf(c) != Cameras.Count - 1 ? ", " : ""));
        //Debug.Log("Found cameras: " + cameras);
    }




    private void ProcessViewItem(ViewGroupTree item)
    {
        if (item.ItemType == ViewItemType.Camera)
            Cameras.Add(item);

        foreach (var subItem in item.GetMembersList())
        {
            ProcessViewItem((ViewGroupTree)subItem);
        }

    }

    public LiveVideo StartVideo(Guid cameraId, int width, int height, int fps)
    {
        LiveVideo _liveVideo = null;

        var cam = Cameras.First();
        var videoParams = new VideoParams()
        {
            CameraId = cameraId,
            DestWidth = width,
            DestHeight = height,
            CompressionLvl = 83,
            FPS = fps,
            MethodType = StreamParamsHelper.MethodType.Push,
            SignalType = StreamParamsHelper.SignalType.Live,
            StreamType = StreamParamsHelper.StreamType.Transcoded,
        };
        var response = Connection.Video.RequestStream(videoParams, null, TimeSpan.FromSeconds(30));

        if (response.ErrorCode != ErrorCodes.Ok)
        {
            throw new Exception("Error requesting stream from camera: " + response.ErrorCode.ToString());
        }

        _liveVideo = Connection.VideoFactory.CreateLiveVideo(new RequestStreamResponseLive(response));
        return _liveVideo;
        //_liveVideo.NewFrame = OnNewFrame;
        //_liveVideo.Start();
    }


    private void OnApplicationQuit()
    {
        Connection.Dispose();
    }
}

[Serializable]
public class MilestoneSettings
{
    [SerializeField]
    public string Server;
    [SerializeField]
    public string Username;
    [SerializeField]
    public string Password;

    public MilestoneSettings()
    {
        Server = "https://milestone:8082";
        Username = "username";
        Password = "password";
    }

    public static MilestoneSettings Load()
    {
        var path = Path.Combine(Application.dataPath, "milestone.cfg");
        if (File.Exists(path))
        {
            return JsonUtility.FromJson<MilestoneSettings>(File.ReadAllText(path));
        }
        else
        {
            var settings = new MilestoneSettings();

            File.WriteAllText(path, JsonUtility.ToJson(settings, true));
            return settings;
        }
    }
}
