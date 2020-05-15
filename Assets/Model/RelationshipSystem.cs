using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Relationship
{
    public static class RelationshipSystem
    {
        public static string[] npcName = { "Lily", "Gildeon", "Mio", "Isaac", "Rosie" };
        public static int npcCount = npcName.Length, relaMinValue = 0, relaMaxValue = 10;

        public static void IncreaseRelationship(string npcName, int num)
        {
            int currentRelaPoint = PlayerPrefs.GetInt(npcName + "Relationship");
            currentRelaPoint += num;
            if (currentRelaPoint > relaMaxValue)
                currentRelaPoint = relaMaxValue;
            PlayerPrefs.SetInt(npcName + "Relationship", currentRelaPoint);
        }

        public static void DecreaseRelationship(string npcName, int num)
        {
            int currentRelaPoint = PlayerPrefs.GetInt(npcName + "Relationship");
            currentRelaPoint -= num;
            if (currentRelaPoint < 0)
                currentRelaPoint = 0;
            PlayerPrefs.SetInt(npcName + "Relationship", currentRelaPoint);
        }
    }
}
