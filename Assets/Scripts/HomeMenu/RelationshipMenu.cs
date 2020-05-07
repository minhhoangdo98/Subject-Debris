using Relationship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipMenu : MonoBehaviour
{
    private GameObject[] relationshipObject;
    private Slider[] relationshipLevel;
    private Text[] npcName;
    private Image[] npcFace;

    private void OnEnable()
    {
        InitValue();
        LoadRelationship();
    }

    private void InitValue()
    {
        relationshipObject = new GameObject[RelationshipSystem.npcCount];
        npcFace = new Image[relationshipObject.Length];
        npcName = new Text[relationshipObject.Length];
        relationshipLevel = new Slider[relationshipObject.Length];
        for (int i = 0; i < RelationshipSystem.npcCount; i++)
        {
            GameObject npcRelaDetail = Instantiate(Resources.Load<GameObject>("Prefabs/UIPrefabs/NpcRelaDetail"), gameObject.transform);
            relationshipObject[i] = npcRelaDetail;
        }
    }

    private void LoadRelationship()
    {
        for (int i = 0; i < relationshipObject.Length; i++)
        {
            //-------------Set Index Number------------------
            relationshipObject[i].transform.Find("index").gameObject.name = i.ToString();
            //-------------Load Name-----------------
            npcName[i] = relationshipObject[i].transform.Find("NpcName").GetComponent<Text>();
            npcName[i].text = RelationshipSystem.npcName[i];
            //-------------Load Face-----------------
            npcFace[i]= relationshipObject[i].transform.Find("NpcFace").GetComponent<Image>();
            npcFace[i].sprite = Resources.Load<Sprite>("Content/Faces/" + npcName[i].text.ToString() + "/" + npcName[i].text.ToString());
            //-------------Load Relationship slider-----------------
            relationshipLevel[i] = relationshipObject[i].transform.Find("NpcRelationship").GetComponent<Slider>();
            relationshipLevel[i].minValue = 0;
            relationshipLevel[i].maxValue = 10;
            relationshipLevel[i].value = PlayerPrefs.GetInt(npcName[i].ToString() + "Relationship");
            if (relationshipLevel[i].value > relationshipLevel[i].maxValue)
                relationshipLevel[i].value = relationshipLevel[i].maxValue;        
        }
    }

    private void OnDisable()
    {
        ClearMenu();
    }

    private void ClearMenu()
    {
        for (int i = 0; i < relationshipObject.Length; i++)
        {
            Destroy(relationshipObject[i]);
        }
    }
}
