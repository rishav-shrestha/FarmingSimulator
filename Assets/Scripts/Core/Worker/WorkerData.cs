using System;
using UnityEngine;

public class WorkerData : MonoBehaviour
{
    public string workername = "Worker";
    [SerializeField] public int MaxEnergy = 0;
    [SerializeField] public int Energy = 0;
    [SerializeField] public int plantingskills = 0;
    [SerializeField] public int harvestingskills = 0;
    [SerializeField] public int wateringskills = 0;

    [SerializeField] public int maxSkillLevel = 0;

    public Crop selectedcrop;
    
    public bool hovered = false;

    private void Update()
    {
        GameObject selectedCharacter = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().selectedCharacter;
            if (selectedCharacter ==
                this.gameObject)
            {
                updateOutline(true,0.5f,Color.yellow);
            }
            else if (hovered)
            {
                updateOutline(true,0.5f,Color.white);
            }
            else
            {
                updateOutline(false);
            }  
    }

    public void updateOutline(bool selected,float thickness,Color color)
    {
            this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", thickness);
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", color);
    }
    public void updateOutline(bool selected)
    {
        if (selected)
        {
            this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0.5f);
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", Color.white);
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
        }
    
    }
}
