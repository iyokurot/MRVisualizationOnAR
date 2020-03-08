using System.Collections;
using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Client;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SelectController : MonoBehaviour {
    private string serverURL = "http://192.168.1.4:5000/";
    public static string ServerURL { get; private set; }
    IMqttClient mqttClient;
    private string mqttURL = "192.168.1.4";
    public static string MqttURL { get; private set; }
    private Subject<string> subject = new Subject<string> ();
    public static TopicData topicData = new TopicData ();
    [SerializeField]
    GameObject scrollContent;
    [SerializeField]
    GameObject contentObject;
    private List<TopicData> topicList = new List<TopicData> ();
    [SerializeField]
    GameObject errorPanel; //server error panel
    [SerializeField]
    Text responseText; //server res
    [SerializeField]
    InputField serverUrlText; //server url inputfield
    [SerializeField]
    Text responseTextMqtt; //mqtt res
    [SerializeField]
    InputField mqttUrlText; //mqtt url inputfield
    [SerializeField]
    Sprite itoyuSprite;
    [SerializeField]
    Sprite dummySprite;

    void Start () {
        if (ServerURL == null) {
            ServerURL = serverURL;
        }
        if (MqttURL == null) {
            MqttURL = mqttURL;
        }
        responseText.text = "ResponseCode:0";
        serverUrlText.text = ServerURL;
        //サーバからTopicリスト取得
        StartCoroutine (GetTopicData ());
        //サーバから取得完了
        subject.Subscribe (res => {
            //リストを成型後、Content追加
            foreach (TopicData topic in topicList) {
                GameObject topicObject = Instantiate (contentObject, scrollContent.transform);
                if ("itoyuNineAxis" == topic.topic) {
                    Debug.Log ("itoyu");
                    topicObject.GetComponent<Image> ().sprite = itoyuSprite;
                    topic.image = itoyuSprite;
                } else {
                    topicObject.GetComponent<Image> ().sprite = dummySprite;
                    topic.image = dummySprite;
                }
                topicObject.GetComponent<ContentPropaty> ().Data = topic;

            }
        });
        responseTextMqtt.text = "未接続";
        mqttUrlText.text = MqttURL;
        //MQTT
        ConnectMqtt ();
    }
    IEnumerator GetTopicData () {
        errorPanel.SetActive (false);
        UnityWebRequest req = UnityWebRequest.Get (ServerURL + "topics");
        req.SetRequestHeader ("key", "KEY");
        yield return req.SendWebRequest ();
        if (req.isNetworkError) {
            errorPanel.SetActive (true);
            responseText.text = "No Sever";
            Debug.Log (req.error);
        } else {
            if (req.responseCode == 200) {
                //OK
                string jsonText = req.downloadHandler.text;
                topicList = JsonUtility.FromJson<Serialize<TopicData>> (jsonText).ToList ();
                subject.OnNext ("load");
                responseText.text = "ResponseCode:200";
            } else {
                //error
                errorPanel.SetActive (true);
                responseText.text = "ResponseCode:" + req.responseCode;
            }
        }
    }
    async void ConnectMqtt () {
        //
        var factory = new MqttFactory ();
        mqttClient = factory.CreateMqttClient ();

        var options = new MqttClientOptionsBuilder ()
            .WithTcpServer (MqttURL, 1883)
            .WithClientId ("Unity.client.subscriber") //Guid.NewGuid ().ToString ())
            //.WithCredentials ("your_MQTT_username", "your_MQTT_password")
            //.WithTls ()
            .Build ();

        mqttClient.Connected += async (s, e) => {
            //Debug.Log ("MQTTブローカに接続しました");
            await mqttClient.SubscribeAsync (
                new TopicFilterBuilder ()
                .WithTopic ("itoyuNineAxis")
                .Build ());
            //Debug.Log ("指定したトピックをSubscribeしました");
            responseTextMqtt.text = "接続成功";
        };
        await mqttClient.ConnectAsync (options);
    }
    public void SetTopicData (TopicData data) {
        topicData = data;
    }
    public void OnClickVisualizationScene () {
        SceneManager.LoadScene ("VisualizeScene");
    }
    public void OnCLickArScene () {
        SceneManager.LoadScene ("ArScene");
    }
    public void OnClickReConnectServer () {
        //
        ServerURL = serverUrlText.text;
        StartCoroutine (GetTopicData ());
    }
    public async void OnClickReConectMqtt () {
        //
        MqttURL = mqttUrlText.text;
        //responseTextMqtt.text = "接続中";
        await mqttClient.DisconnectAsync ();
        ConnectMqtt ();
    }

}