using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SelectController : MonoBehaviour {
    private string serverURL = "http://192.168.1.4:5000/";
    public static string ServerURL { get; private set; }
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

    void Start () {
        if (ServerURL == null) {
            ServerURL = serverURL;
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
                topicObject.GetComponent<ContentPropaty> ().Data = topic;
            }
        });
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

}