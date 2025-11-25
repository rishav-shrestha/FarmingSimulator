using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CropDatabase : MonoBehaviour
{
    public List<Crop> crops;

    void OnValidate()
    {
        string[] guids = AssetDatabase.FindAssets("t:Crop", new[] { "Assets/Crops" });
        crops = new List<Crop>(guids.Length);

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            crops.Add(AssetDatabase.LoadAssetAtPath<Crop>(path));
        }
        Debug.Log("Loaded " + crops.Count + " crops from Assets/Crops/");
        
    }
}
