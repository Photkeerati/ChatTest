using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;
using UnityEngine.UI;


public partial class MBXChat : MonoBehaviour  , IChatClientListener {

	ChatClient chatClient;
	public string UserName = "";
	public string ChannelName = "MBXRoom";
	public int HistoryLengthToFetch;

	public InputField InputChat;

	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		Connect ();
	}
	
	// Update is called once per frame
	void Update () {
		if (chatClient != null)
			chatClient.Service ();
	}
		

	public void Connect() {
		var connectedProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
		chatClient = new ChatClient (this, connectedProtocol);
		chatClient.ChatRegion = "US";

		var authValues =  new ExitGames.Client.Photon.Chat.AuthenticationValues ();
		authValues.UserId = UserName;
		authValues.AuthType = ExitGames.Client.Photon.Chat.CustomAuthenticationType.None;
		chatClient.Connect (ChatSettings.Instance.AppId, "1.0",authValues);
	}

	public void OnSendEnter() {
		if (Input.GetKey (KeyCode.KeypadEnter) ||  Input.GetKey(KeyCode.Return)) {
			SendChatMessage (InputChat.text);
			InputChat.text = "";
		}
	}

	public void OnClickSend() {
		SendChatMessage (InputChat.text);
		InputChat.text = "";
	}

	void SendChatMessage(string text) {
		chatClient.PublishMessage (ChannelName, text);

	}



}

public partial class MBXChat {

	void OnApplicationQuit() {
		if (chatClient != null) { chatClient.Disconnect(); }
	}

	#region IChatClientListener implementation

	void IChatClientListener.DebugReturn (ExitGames.Client.Photon.DebugLevel level, string message)
	{
		Debug.Log ("DebugReturn  : " + message);
	}

	void IChatClientListener.OnDisconnected ()
	{
		Debug.Log ("OnDisconnected");
	}

	void IChatClientListener.OnConnected ()
	{
		Debug.Log ("OnConnected");
		chatClient.Subscribe (new string[] { ChannelName } ,HistoryLengthToFetch);
	}

	void IChatClientListener.OnChatStateChange (ChatState state)
	{
		Debug.Log ("OnChatStateChange : " + state.ToString ());
	}

	void IChatClientListener.OnGetMessages (string channelName, string[] senders, object[] messages)
	{
		
		string msgs = "";
		for ( int i = 0; i < senders.Length; i++ )
		{
			msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
		}
		Debug.Log (string.Format("OnGetMessages: {0} ({1}) > {2}", channelName, senders.Length, msgs) );

	}

	void IChatClientListener.OnPrivateMessage (string sender, object message, string channelName)
	{
		Debug.Log ("OnPrivateMessage : " + sender);
	}

	void IChatClientListener.OnSubscribed (string[] channels, bool[] results)
	{
		Debug.Log ("OnSubscribed");
	}

	void IChatClientListener.OnUnsubscribed (string[] channels)
	{
		Debug.Log ("OnUnsubscribed");
	}

	void IChatClientListener.OnStatusUpdate (string user, int status, bool gotMessage, object message)
	{
		Debug.Log ("OnStateUpdate");
	}

	#endregion
}