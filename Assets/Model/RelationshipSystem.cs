using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Relationship
{
    public static class RelationshipSystem
    {
        public static string[] npcName = { "Lily", "Gideon", "Mio", "Isaac", "Rose" };
        public static int npcCount = npcName.Length;

        public static void IncreaseRelationship(string npcName, int num)
        {
            int currentRelaPoint = PlayerPrefs.GetInt(npcName + "Relationship");
            currentRelaPoint += num;
            PlayerPrefs.SetInt(npcName + "Relationship", currentRelaPoint);
        }

        public static void DecreaseRelationship(string npcName, int num)
        {
            int currentRelaPoint = PlayerPrefs.GetInt(npcName + "Relationship");
            currentRelaPoint -= num;
            PlayerPrefs.SetInt(npcName + "Relationship", currentRelaPoint);
        }
    }
}
