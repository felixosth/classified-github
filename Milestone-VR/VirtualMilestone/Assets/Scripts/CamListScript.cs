using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;


public class CamListScript : ScriptableObject {

    int logintype;

    string server, user, pass;

    // rtsp://onvif:onvif@192.168.2.150:554/live/507b804f-76c2-4163-9925-23873b78d372

    public List<MilestoneCamera> Cameras = new List<MilestoneCamera>();

    /// <summary>
    /// Refresh as default Windows user
    /// </summary>
    /// <param name="server"></param>
    //public CamListScript(string server)
    //{
    //    logintype = 2;
    //    this.server = server;
    //}

    public string GetStreamURL(int cameraIndex)
    {
        return string.Format("rtsp://{0}:{1}@{2}:554/live/{3}", onvifUser, onvifPass, server, Cameras[cameraIndex].GUID);
    }

    string onvifUser, onvifPass;

    /// <summary>
    /// Refresh as basic or windows user
    /// </summary>
    /// <param name="server">IP</param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="basicUser">True = Basic User, False = Windows User</param>
    public CamListScript(string server, string user, string password, bool basicUser, string onvifUser, string onvifPass)
    {
        if (basicUser)
        {
            logintype = 0;
        }
        else
            logintype = 1;
        this.onvifPass = onvifPass;
        this.onvifUser = onvifUser;

        this.user = user;
        this.pass = password;
        this.server = server;
    }

    public void RefreshCameraList(string filter)
    {
        refreshCameras(server, user, pass, logintype, filter);

        //foreach(MilestoneCamera cam in Cameras)
        //{
        //    Debug.Log(cam.GUID);
        
        //}
    }

    private void refreshCameras(string server, string user, string pass, int loginType, string filter) //0 = basic, 2 = windows default, 3 = windows
    {
        MilestoneSystemInfo sysInfo = new MilestoneSystemInfo();
        int rc = sysInfo.Connect(server, user, pass, (MilestoneSystemInfo.AuthenticationType)loginType);
        //Debug.Log(rc.ToString());

        XmlDocument sysDoc = sysInfo.GetSystemInfoXml(sysInfo.Token);
        XmlNodeList nodes = sysDoc.GetElementsByTagName("camera");
        foreach (XmlNode node in nodes)
        {
            string camName = "", camGUID = "";
            //Console.WriteLine(node.InnerText);
            foreach (XmlAttribute att in node.Attributes)
            {
                //string s = att.Name.ToLower();
                if (att.Name.ToLower() == "cameraid")
                {
                    camName = att.InnerText;
                    //Console.WriteLine(att.InnerText);
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                string s = childNode.Name.ToLower();

                if (s == "guid")
                    camGUID = childNode.InnerText;
                    //Console.WriteLine(childNode.InnerText);
            }
            if(filter != "" && !camName.Contains(filter))
                continue;
            Cameras.Add(new MilestoneCamera(camName, camGUID));
        }
    }



}

public class MilestoneCamera
{
    public string Name { get; set; }
    public string GUID { get; set; }
    public MilestoneCamera(string name, string guid)
    {
        this.Name = name;
        this.GUID = guid;
    }
}

public class MilestoneSystemInfo : ScriptableObject
{
	public delegate void OnTokenRefreshed(object p);
	private OnTokenRefreshed _onTokenRefreshed = null;

	public enum AuthenticationType { Basic, Windows, WindowsDefault };
	public enum SrvType { Enterprise = 0, Corporate, None };
	public enum ServiceType { Recorder, Server };

	private string _token = "";
	private string _errMsg = "";
	private string _server = "";
	private string _user = "";
	private string _domain = "";
	private string _pwd = "";
	private bool _isConnected = false;
	private SrvType _serverType = SrvType.None;
	private HttpStatusCode _err = HttpStatusCode.OK;
	private Timer _tokenExpireTimer;
	private bool _hasExpired = false;
	private AuthenticationType _autype = AuthenticationType.Windows;
	//private object _renderObject;
	private int _serverInternalVersion = -1;

	public OnTokenRefreshed OnTokenRefreshedMethod
	{
		set
		{
			_onTokenRefreshed = value;
		}
		get
		{
			return _onTokenRefreshed;
		}
	}

	//public object RenderObject
	//{
	//	set
	//	{
	//		_renderObject = value;
	//	}
	//}

	public AuthenticationType AuthType
	{
		get
		{
			return _autype;
		}
		set
		{
			_autype = value;
		}
	}

	public bool HasExpired
	{
		get
		{
			return _hasExpired;
		}
	}

	public SrvType ServerType
	{
		get
		{
			return _serverType;
		}
		set
		{
			_serverType = value;
		}
	}

	public bool IsConnected
	{
		get
		{
			return _isConnected;
		}
	}

	public string Server
	{
		get
		{
			return _server;
		}
		set
		{
			_server = value;
		}
	}

	public string Token
	{
		get
		{
			return _token;
		}
		set
		{
			_token = value;
		}
	}

	public string Message
	{
		get
		{
			return _errMsg;
		}
	}

	public string User
	{
		get
		{
			return _user;
		}
		set
		{
			_user = value;
		}
	}

	public string Domain
	{
		get
		{
			return _domain;
		}
		set
		{
			_domain = value;
		}
	}

	public string Password
	{
		get
		{
			return _pwd;
		}
		set
		{
			_pwd = value;
		}
	}

	public int Disconnect()
	{
		_tokenExpireTimer.Dispose();
		_tokenExpireTimer = null;

		_server = "";
		_user = "";
		_errMsg = "";
		_isConnected = false;
		_token = "";

		// There is no way to cancel the token with the server, sorry.
		return 200;
	}

	public int Connect(string server, string user, string pwd, AuthenticationType autype)
	{
		try
		{
			bool atSep = false;
			string dom = "";
			string usr = "";
			string[] ud = user.Split('\\');
			if (ud.Length == 1)
			{
				ud = user.Split('@');
				atSep = true;
			}
			if (ud.Length == 1)
			{
				dom = "";
				usr = user;
			}
			if (ud.Length > 1)
			{
				if (atSep)
				{
					usr = ud[0];
					dom = ud[1];
				}
				else
				{
					usr = ud[1];
					dom = ud[0];
				}
			}

			if (IsConnected)
			{
				if (usr == _user && server == _server && dom == _domain)
				{
					_errMsg = "Already connected to " + user;
					return 304; // HTTP 304: "Not Modified". This is no error.
				}
				else
				{
					_errMsg = "Connected to " + server + " " + user;
					return 401; // HTTP 401: "Unauthorized".
				}
			}

			_server = server;
			_user = usr;
			_domain = dom;
			_pwd = pwd;

			string url = "http://" + server;
			if (_serverType == SrvType.None)
			{
				_serverType = GetServerType(url, autype);
				if (_serverType == SrvType.None)
				{
					return (int)_err;
				}
			}

			XmlDocument doc = GetLogin(url, "", autype);
			XmlNodeList nodes = doc.GetElementsByTagName("Token");
			_token = "";
			foreach (XmlNode n in nodes)
			{
				_token = n.InnerXml;
				break;
			}

			if (_token == "")
				throw new Exception("Blank token");

			nodes = doc.GetElementsByTagName("MicroSeconds");
			int mi = 3600000;
			foreach (XmlNode n in nodes)
			{
				try
				{
					double d = double.Parse(n.InnerXml) / 1000.0;
					mi = (int)d;
				}
				catch (Exception e)
				{
					string s = e.Message;
				}
				break;
			}

			// React 30 seconds before token expires. (Never faster than 30 seconds after last renewal, but that ought not occur).
			// Enterprise's default timeout is 4 minutes, Corporate's is 1 hour.
			mi = mi > 60000 ? mi - 30000 : mi;
			_tokenExpireTimer = new Timer(_tokenExpireTimer_Callback, null, mi, Timeout.Infinite);

			_errMsg = "OK";
			_isConnected = true;
			_autype = autype;
			return 200;
		}
		catch (Exception e)
		{
			int rc = 502;
			if (e.Message == "Blank token")
				rc = 401;
			_errMsg = e.Message;
			_isConnected = false;
			return rc;
		}
	}

	private void _tokenExpireTimer_Callback(System.Object state)
	{
		try
		{
			string url = "http://" + _server;
			XmlDocument doc = GetLogin(url, _token, _autype);
			XmlNodeList nodes = doc.GetElementsByTagName("Token");
			_token = "";
			foreach (XmlNode n in nodes)
			{
				_token = n.InnerXml;
				break;
			}

			if (_token == "")
				throw new Exception("Got blank token when trying to refresh");

			// Now the token is refreshed, so if anyone asks for the Token property, they will always get a valid one
			// That make us run OK with Enterprise and with versions Corporate before 4.0, because they only require a new token at image server CONNECT
			// Corporate 4.0 and later requires running imageserver TCP connections to be actively renewed.
			if (_onTokenRefreshed != null/* && this.ServerType == SrvType.Corporate && _serverInternalVersion > 4*/)
			{

				//System.Windows.Controls.Control pi = (System.Windows.Controls.Control)_renderObject;
				//pi.Dispatcher.Invoke(_onTokenRefreshed, new System.Object[] { _token });
			}

			// The timeout interval may have changed since last time
			nodes = doc.GetElementsByTagName("MicroSeconds");
			int mi = 3600000;
			foreach (XmlNode n in nodes)
			{
				try
				{
					double d = double.Parse(n.InnerXml) / 1000.0;
					mi = (int)d;
				}
				catch (Exception e)
				{
					string s = e.Message;
				}
				break;
			}

			mi = mi > 60000 ? mi - 30000 : mi;
			_tokenExpireTimer.Change(mi, Timeout.Infinite);
		}
		catch (Exception e)
		{
			Disconnect();
			throw new Exception("Error refreshing token: " + e.Message);
		}
	}

	public SrvType GetServerType(string url, AuthenticationType autype)
	{
		try
		{
			string content_str =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
				"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
				"<soap:Body>" +
				"<GetVersion xmlns=\"http://videoos.net/2/XProtectCSServerCommand\">" +
				"</GetVersion></soap:Body></soap:Envelope>";

			_serverType = SrvType.Corporate;
			XmlDocument doc = GetSoap(url, content_str, ServiceType.Server, "GetVersion", autype);
			if (_err == HttpStatusCode.OK)
			{
				_serverInternalVersion = 4;
				XmlNodeList nl = doc.GetElementsByTagName("GetVersionResult");
				string vr = "";
				foreach (XmlNode n in nl)
				{
					vr = n.InnerText;
				}
				int ivr;
				if (vr.Length > 0 && int.TryParse(vr, out ivr))
				{
					_serverInternalVersion = ivr;
				}
				return _serverType;
			}
			if (_err == HttpStatusCode.NotFound)
			{
				_serverType = SrvType.Enterprise;
				GetSoap(url, content_str, ServiceType.Server, "GetVersion", autype);
				if (_err == HttpStatusCode.OK)
				{
					_serverInternalVersion = 3;
					return _serverType;
				}
			}
		}
		catch (Exception e)
		{
			string s = e.Message;
		}

		_serverType = SrvType.None;
		return _serverType;
	}

	public XmlDocument GetConfiguration(string token)
	{
		try
		{
			string url = "http://" + _server;
			if (_serverType == SrvType.Corporate)
			{
				string templ =
					"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
					"<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
					"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
					"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
					"<soap:Body>" +
					"<GetConfiguration xmlns=\"http://videoos.net/2/XProtectCSServerCommand\">" +
					"<token>{0}</token>" +
					"</GetConfiguration></soap:Body></soap:Envelope>";
				string content_str = string.Format(templ, token);

				// This only works with XPCO - Not implemented on XPE
				return GetSoap(url, content_str, ServiceType.Server, "GetConfiguration", AuthenticationType.Windows);
			}

			if (_serverType == SrvType.Enterprise)
			{
				return GetFile(url + "/systeminfo.xml");
			}
		}
		catch (Exception e)
		{
			string s = e.Message;
		}

		return new XmlDocument();
	}

	public XmlDocument GetSystemInfoXml(string token)
	{
		try
		{
			string url = "http://" + _server;
			if (_serverType == SrvType.Corporate)
			{
				return GetFile(url + "/rcserver/systeminfo.xml");
			}

			if (_serverType == SrvType.Enterprise)
			{
				return GetFile(url + "/systeminfo.xml");
			}
		}
		catch (Exception e)
		{
			string s = e.Message;
		}

		return new XmlDocument();
	}

	public XmlDocument GetFile(string url)
	{
		HttpWebResponse response = null;
		HttpWebRequest req = null;

		try
		{
			_err = HttpStatusCode.OK;
			req = (HttpWebRequest)WebRequest.Create(url);
			CredentialCache cr = new CredentialCache();
			string auname = "NTLM";
			if (_autype == AuthenticationType.Basic)
				auname = "Basic";
			if (_autype == AuthenticationType.WindowsDefault)
			{
				req.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
			}
			else
			{
				if (_domain == "")
				{
					cr.Add(new Uri(url), auname, new NetworkCredential(_user, _pwd));
				}
				else
				{
					cr.Add(new Uri(url), auname, new NetworkCredential(_user, _pwd, _domain));
				}
				req.Credentials = cr;
			}
			req.PreAuthenticate = true;
			req.Method = "GET";
			req.Accept = "text/xml";
			req.AllowWriteStreamBuffering = true;
			req.Timeout = 20000;

			response = (HttpWebResponse)req.GetResponse();
			_err = ((HttpWebResponse)response).StatusCode;
			long respLen = response.ContentLength;
			Stream stream = response.GetResponseStream();

			int got = 0;
			int bytes = 0;
			int get = 1;
			int maxb = (int)respLen;
			int miss = maxb;
			byte[] buffer = new byte[respLen];
			int retry = 3;

			do
			{
				get = miss > maxb ? maxb : miss;
				bytes = stream.Read(buffer, got, get);
				if (bytes == 0)
				{
					retry--;
				}
				got += bytes;
				miss -= bytes;
			}
			while (got < maxb && retry > 0);

			int off = (buffer[3] == 60) ? 3 : 0; // Skip XML indicator bytes
			string page = Encoding.UTF8.GetString(buffer, off, got - off);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);

			return doc;
		}
		catch (WebException we)
		{
			HttpWebResponse r = (HttpWebResponse)we.Response;
			_err = r.StatusCode;
			string s = we.Message;
			return new XmlDocument();
		}
		catch (Exception e)
		{
			_err = HttpStatusCode.Unused;
			string s = e.Message;
			return new XmlDocument();
		}
	}

	public XmlDocument GetLogin(string url, string token, AuthenticationType autype)
	{
		try
		{
			string templ =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
				"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
				"<soapenv:Body><Login xmlns=\"http://videoos.net/2/XProtectCSServerCommand\">" +
				"<instanceId>{0}</instanceId><currentToken>{1}</currentToken></Login>" +
				"</soapenv:Body></soapenv:Envelope>";
			Guid guid = Guid.NewGuid();
			string content_str = string.Format(templ, guid.ToString(), token);
			return GetSoap(url, content_str, ServiceType.Server, "Login", autype);
		}
		catch (Exception e)
		{
			string s = e.Message;
			return new XmlDocument();
		}
	}

	public XmlDocument GetCustomSettingDataUser(string url, string token, AuthenticationType autype)
	{
		try
		{
			string templ =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
				"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
				"<soapenv:Body><GetCustomSettingDataGlobal xmlns=\"http://videoos.net/2/XProtectCSServerCommand\">" +
				"<customSettingId>{0}</customSettingId><token>{1}</token></GetCustomSettingDataGlobal>" +
				"</soapenv:Body></soapenv:Envelope>";
			Guid guid = Guid.NewGuid();
			string content_str = string.Format(templ, "2d37e832-dffd-41e1-9cd5-9fe374aaa79e", token);
			XmlDocument doc = GetSoap(url, content_str, ServiceType.Server, "GetCustomSettingDataGlobal", autype);
			return doc;
		}
		catch (Exception e)
		{
			string s = e.Message;
			return new XmlDocument();
		}
	}

	public XmlDocument GetSoap(string url, string content_str, ServiceType sType, string cmd, AuthenticationType autype)
	{
		HttpWebResponse response = null;
		HttpWebRequest req = null;

		try
		{
			_err = HttpStatusCode.OK;
			byte[] content = Encoding.UTF8.GetBytes(content_str);

			string url1 = url;
			if (!url1.EndsWith("/"))
				url1 = url1 + "/";

			string soapAction = "SOAPAction: None";
			if (sType == ServiceType.Server)
			{
				soapAction = string.Format("SOAPAction: \"http://videoos.net/2/XProtectCSServerCommand/{0}\"", cmd);
				if (_serverType == SrvType.Corporate)
				{
					url1 = url1 + "ServerAPI/ServerCommandService.asmx";
				}
				else
				{
					url1 = url1 + "servercommandservice/servercommandservice.asmx";
				}
			}
			if (sType == ServiceType.Recorder)
			{
				soapAction = string.Format("SOAPAction: \"http://videoos.net/2/XProtectCSRecorderCommand/{0}\"", cmd);
				if (_serverType == SrvType.Corporate)
				{
					url1 = url1 + "RecorderAPI/ServerCommandService.asmx";
				}
				else
				{
					url1 = url1 + "recordercommandservice/recordercommandservice.asmx";
				}
			}

			req = (HttpWebRequest)WebRequest.Create(url1);
			CredentialCache cr = new CredentialCache();
			string auname = "NTLM";
			if (autype == AuthenticationType.Basic)
				auname = "Basic";

			if (autype == AuthenticationType.WindowsDefault)
			{
				req.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
			}
			else
			{
				if (_domain == "")
				{
					cr.Add(new Uri(url), auname, new NetworkCredential(_user, _pwd));
				}
				else
				{
					cr.Add(new Uri(url), auname, new NetworkCredential(_user, _pwd, _domain));
				}
				req.Credentials = cr;
			}
			req.PreAuthenticate = true;
			req.Method = "POST";
			req.ContentLength = content.Length;
			req.ContentType = "text/xml; charset=utf-8";
			req.Accept = "text/xml";
			req.Headers.Add(soapAction);
			req.AllowWriteStreamBuffering = true;
			req.Timeout = 20000;

			Stream req_stream = req.GetRequestStream();
			req_stream.Write(content, 0, content.Length);
			req_stream.Close();

			response = (HttpWebResponse)req.GetResponse();
			_err = ((HttpWebResponse)response).StatusCode;
			long respLen = response.ContentLength;
			Stream stream = response.GetResponseStream();

			int got = 0;
			int bytes = 0;
			int get = 1;
			int maxb = (int)respLen;
			int miss = maxb;
			byte[] buffer = new byte[respLen];
			int retry = 3;

			do
			{
				get = miss > maxb ? maxb : miss;
				bytes = stream.Read(buffer, got, get);
				if (bytes == 0)
				{
					retry--;
				}
				got += bytes;
				miss -= bytes;
			}
			while (got < maxb && retry > 0);

			string page = Encoding.UTF8.GetString(buffer, 0, got);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);

			return doc;
		}
		catch (WebException we)
		{
			HttpWebResponse r = (HttpWebResponse)we.Response;
			if (r != null)
				_err = r.StatusCode;
			else
				_err = HttpStatusCode.RequestTimeout;
			string s = we.Message;
			return new XmlDocument();
		}
		catch (Exception e)
		{
			_err = HttpStatusCode.Unused;
			string s = e.Message;
			return new XmlDocument();
		}
	}
}
