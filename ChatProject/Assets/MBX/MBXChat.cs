using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;
using UnityEngine.UI;


public partial class MBXChat : MonoBehaviour  , IChatClientListener {

	ChatClient chatClient;
	public string UserName = "";
	public string FriendName = "";
	public string FriendList = "";
	public string ChannelName = "MBXRoom";
	public int HistoryLengthToFetch;
	public bool OnlineStatus = false;

	bool IsPrivateMessage = false;

	public InputField InputChat;
	public Text RoomStatusLabel;

	// Use this for initialization
	void Start () {
		initValue ();
		Connect ();
	}
	
	// Update is called once per frame
	void Update () {
		if (chatClient != null)
			chatClient.Service ();
	}

	void initValue() {
		Application.runInBackground = true;
		var connectedProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
		chatClient = new ChatClient (this, connectedProtocol);
		chatClient.ChatRegion = "US";
		Debug.LogError (IsPrivateMessage);
		if (IsPrivateMessage) {
			Debug.Log ("Private");
			RoomStatusLabel.text = "Room : Private";
//			string [] friendList = FriendList.Split (',');
//			chatClient.AddFriends (friendList);

		} else {
			RoomStatusLabel.text = "Room : Public";
		}

		if (OnlineStatus) {
			chatClient.SetOnlineStatus (ChatUserStatus.Online, "I'm here");
		} else {
			chatClient.SetOnlineStatus (ChatUserStatus.Offline, "I'm here");
		}

	}
		

	public void Connect() {
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
		if (IsPrivateMessage) {
			chatClient.SendPrivateMessage (FriendName, text);
		} else {
			chatClient.PublishMessage (ChannelName, text);
		}


	}


	public void OnConnect() {
		initValue ();
		Connect ();
	}

	public void OnDisConnect(){ 
		disConnect ();
	}

	public void OnSwitchRoom() {
		disConnect ();
		IsPrivateMessage = !IsPrivateMessage;
		OnConnect ();
	}

	void OnApplicationQuit() {
		disConnect ();
	}

	void disConnect() {
		if (chatClient != null) { chatClient.Disconnect(); }
	}

}

public partial class MBXChat {



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
		chatClient.Subscribe (new string[] { ChannelName },HistoryLengthToFetch);
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
			msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]) + "\n";
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
		Debug.Log (string.Format("Status change for : {0} to: {1}", user, status));
	}

	#endregion
}