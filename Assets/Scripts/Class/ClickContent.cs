using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ClickContent : MonoBehaviour, IPointerClickHandler {
    //ScrollViewのContentにアタッチ、項目クリックEventTrigger
    [SerializeField]
    SelectController selectController;
    [SerializeField]
    GameObject selectPanel;
    [SerializeField]
    Text nameText;
    public void OnPointerClick (PointerEventData eventData) {
        //クリックされたオブジェクトからIDを取得
        //position
        //Debug.Log (eventData);
        //GameObject
        //Debug.Log (eventData.pointerEnter);
        TopicData data = eventData.pointerEnter.GetComponent<ContentPropaty> ().Data;
        //selectController.SetSelectId (5);
        selectController.SetTopicData (data);
        selectPanel.SetActive (true);
        nameText.text = data.name;
    }
}