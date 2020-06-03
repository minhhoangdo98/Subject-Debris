﻿using Relationship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipMenu : MonoBehaviour
{
    private GameObject[] relationshipObject;
    private Slider[] relationshipLevel;
    private Text[] npcName, npcRelaValue;
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
        npcRelaValue = new Text[relationshipObject.Length];
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
            npcFace[i].sprite = Resources.Load<Sprite>("Content/Faces/" + npcName[i].text + "/" + npcName[i].text.ToString());
            //-------------Load Relationship slider-----------------
            relationshipLevel[i] = relationshipObject[i].transform.Find("NpcRelationship").GetComponent<Slider>();
            relationshipLevel[i].minValue = RelationshipSystem.relaMinValue;
            relationshipLevel[i].maxValue = RelationshipSystem.relaMaxValue;
            relationshipLevel[i].value = PlayerPrefs.GetInt(npcName[i].text + "Relationship");
            if (relationshipLevel[i].value > relationshipLevel[i].maxValue)
                relationshipLevel[i].value = relationshipLevel[i].maxValue;
            //-------------Load Relationship value text-----------------
            npcRelaValue[i] = relationshipLevel[i].transform.Find("RelaValue").GetComponent<Text>();
            npcRelaValue[i].text = relationshipLevel[i].value.ToString();
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

    public void ButtonDecreaseRelationship(string npcName)
    {
        RelationshipSystem.DecreaseRelationship(npcName, 1);
    }
}
