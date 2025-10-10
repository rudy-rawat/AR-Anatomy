using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class OrganRegistry : MonoBehaviour
{
    public static OrganRegistry Instance; // this make this script a singelton

    public List<OrganVariant> OrganVariants = new List<OrganVariant>();
    private Dictionary<string, OrganVariant> organMap = new Dictionary<string, OrganVariant>(); //This dictionary maps an organ name like "heart" to the actual OrganVariant entry so it can be accessed quickly at runtime.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            foreach (var organ in OrganVariants)   // Loops through all items in the Inspector list and adds them to the dictionary (organMap) for easy access 
            {
                if (!organMap.ContainsKey(organ.organName.ToLower()))
                {
                    organMap.Add(organ.organName.ToLower(), organ);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("OrganRegistry initialized with " + OrganVariants.Count + " variants.");

    }

    public GameObject GetBasic(string organName) //Returns the basic prefab for a given organ name.
    {
        organName = organName.ToLower();

        if (organMap.ContainsKey(organName))
        {
            Debug.Log("Basic Loaded successfully of :- " + organName);
            return organMap[organName].basicPrefab;
            
        }
        Debug.LogWarning("Basic prefab is not found of : " +  organName);
        return null;
    }

    public GameObject GetDetailed(string organName) //Returns the detailed prefab for a given organ name.
    {
        organName = organName.ToLower();

        if (organMap.ContainsKey(organName))
        {
            Debug.Log("Detailed Loaded successfully of :- " + organName);
            return organMap[organName].detailedPrefab;
        }
        Debug.LogWarning("Detailed prefab is not found of : " + organName);
        return null;
    }
}
