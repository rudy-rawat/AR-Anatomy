using UnityEngine;

[System.Serializable]
public class OrganVariant
{
    public string organName;              // e.g., "heart"
    public string organDescription;       // description of the organ
    public GameObject basicPrefab;        // basic outer heart
    public GameObject detailedPrefab;     // dissected inner heart
}
