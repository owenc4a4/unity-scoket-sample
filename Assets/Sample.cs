using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{

    public TMP_Text _loginTxt;
    public TMP_Text _lauunchTxt;

    void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

    private void HandleLog(string condition, string stackTrace, LogType type)
	{
		switch( type )
		{
			case LogType.Error:
			case LogType.Exception:
				//SLog(condition + "\n" + stackTrace);

                _lauunchTxt.text = _lauunchTxt.text + "\n---\n" + condition + "\n" + stackTrace;

				break;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_2022_3_OR_NEWER || !UNITY_2019_1_OR_NEWER
        Debug.LogWarning("yes");
#endif

        var args = System.Environment.GetCommandLineArgs();
        var str = string.Join(" ", args);

        //SLog(str);
        Debug.Log(str);
        _lauunchTxt.text = str;

        ConnectToServer("127.0.0.1", 40712);


        var count = GetLoginCount();
        _loginTxt.text = "login count: " + count;
        var path = Application.persistentDataPath + "/loginCount";
        File.WriteAllText(path, (++count).ToString());

    }

    int GetLoginCount()
    {
        var path = Application.persistentDataPath + "/loginCount";
        if (File.Exists(path)) {
            return int.Parse(File.ReadAllText(path));
        }

        return 0;
    }

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];
    private byte[] sendBuffer = new byte[1024];

    private void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);
            stream = client.GetStream();

            // Start asynchronous reading from the server
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);


        }
        catch (Exception e)
        {
            SLog("Error connecting to server: " + e.Message);
        }
    }

    byte _clientId;

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int bytesRead = stream.EndRead(result);
            if (bytesRead > 0)
            {

                var clientId = receiveBuffer[0];
                _clientId = clientId;
                var eventType = receiveBuffer[1];

                int event_length = 256 * receiveBuffer[2];
                event_length += receiveBuffer[3];

                SLog("----- recieve ----");
                SLog("cliendID " + _clientId);
                SLog("bytesRead " + bytesRead);
                SLog("eventType " + eventType);
                SLog("msgl 0 " + receiveBuffer[2]);
                SLog("msgl 1 " + receiveBuffer[3]);
                SLog("event_length " + event_length);

                if (eventType == 0) {
                    SLog("Connected");
                    SendMsgToClient("from unity connected");
                } else if (eventType == 1) {
                    SLog("Disconnected");
                } else if (eventType == 2) {
                    string receivedData = Encoding.UTF8.GetString(receiveBuffer, 4, event_length);
                    SLog("Received from server: " + receivedData);
                } else {
                    Debug.LogError("Unkonuwn");
                }






                // Continue asynchronous reading from the server
                stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
            }
        }
        catch (Exception e)
        {
            SLog("Error receiving data: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        stream?.Close();

        client?.Close();
    }

    private string _logmsg;


    public TMP_Text _logText;

    private void SLog(string msg)
    {
        Debug.Log(msg);
        _logmsg = _logmsg + msg + "\n";

    }

    public void OnBtnSendMsg()
    {
        throw new NotImplementedException();

        // Send initial data to the server
        string initialMessage = "Hello, server! I Client...";
        SendMsgToClient(initialMessage);
    }

    void SendMsgToClient(string msg)
    {
        SLog("----- send new msg -----");
        byte[] initialMessageBytes = Encoding.UTF8.GetBytes(msg);
        var message_length = initialMessageBytes.Length;

        SLog("_clientId " + _clientId);
        SLog("message_length " + message_length);
        SLog("message_length 0 " + (message_length / 256));
        SLog("message_length 1 " + (message_length % 256));

        // stream.WriteByte(_clientId);
        // stream.WriteByte((byte)2);
        // stream.WriteByte();
        // stream.WriteByte();

        sendBuffer[0] = _clientId;
        sendBuffer[1] = 2;
        sendBuffer[2] = (byte)(message_length / 256);
        sendBuffer[3] = (byte)(message_length % 256);

        Buffer.BlockCopy(initialMessageBytes, 0, sendBuffer, 4, message_length);

        //stream.Write(initialMessageBytes, 0, initialMessageBytes.Length);
        stream.Write(sendBuffer, 0, message_length + 4);
        stream.Flush();
    }


    // Update is called once per frame
    void Update()
    {
        _logText.text = _logmsg;
    }

    string persistentDataPath;
    string temporaryCachePath;

    void aa()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
				using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
    				using (AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
                    {
    					persistentDataPath = filesDir.Call<string>("getCanonicalPath");
    				}
    				using (AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getCacheDir"))
                    {
    					temporaryCachePath = filesDir.Call<string>("getCanonicalPath");
    				}
    			}
            }


            SaveText(
                persistentDataPath,
                "hoge",
                "cloud and"
            );
#else
        temporaryCachePath = Application.temporaryCachePath;
        persistentDataPath = Application.persistentDataPath;
#endif

        // ubitusResourcePath = Path.Combine(Application.dataPath, "..", UbitusResourceDirectory);
        // dataPathUrl = ToUrl(dataPath);
        // streamingAssetsPathUrl = ToUrl(streamingAssetsPath);
        // persistentDataPathUrl = ToUrl(persistentDataPath);
        // temporaryCachePathUrl = ToUrl(temporaryCachePath);

        // /data/data/{アプリのパッケージ名}/files

        // /data/data/jp.co.hoge.hoge/shared_prefs

        // /storage/emulated/0/Android/data/<packagename>/files
        Debug.Log("Application.persistentDataPath " + Application.persistentDataPath);
        Debug.Log("persistentDataPath " + persistentDataPath);
    }

    private void SaveText(string filePath, string fileName, string textToSave)
    {
        var combinedPath = Path.Combine(filePath, fileName);
        using (var streamWriter = new StreamWriter(combinedPath))
        {
            streamWriter.WriteLine(textToSave);
        }
    }

    public void OnBtn()
    {
        PlayerPrefs.SetInt("cloud01", 100);

        Debug.Log("#####");
        Debug.Log("Application.persistentDataPath: " + Application.persistentDataPath);

        SaveText(
            Application.persistentDataPath,
            "hoge",
            "cloud"
        );

        aa();
        //StartCoroutine(download());
    }


    int DownloadMaxConnection = 12;
    string FileCryptoSalt = "herensuge";
    public TMPro.TMP_Text _text;

    int DownloadFileBufferSize = 8 * 1024;

    private IEnumerator WaitForEnd(UnityWebRequest request, Action complete, Action error)
    {
        //Debug.Log("WaitForEnd");
        yield return request.SendWebRequest();
        //Debug.Log("WaitForEnd 2");

        if (request.result == UnityWebRequest.Result.Success)
        {
            complete();
            Debug.Log("end " + request.url);

        }
        else
        {
            error();
            Debug.LogError(request.error);
        }
    }




}
