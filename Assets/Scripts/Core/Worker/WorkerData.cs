using UnityEngine;

public class WorkerData : MonoBehaviour
{
    public string workername = "Worker";
    [SerializeField] public int maxEnergy ;
    [SerializeField] public int energy ;
    [SerializeField] public int plantingskills;
    [SerializeField] public int harvestingskills;
    [SerializeField] public int wateringskills;

    [SerializeField] public int maxSkillLevel;

    public Crop selectedcrop;
    
    public bool hovered;

    void Update()
    {
        GameObject selectedCharacter = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().selectedCharacter;
            if (selectedCharacter ==
                this.gameObject)
            {
                UpdateOutline(true,0.5f,Color.yellow);
            }
            else if (hovered)
            {
                UpdateOutline(true,0.5f,Color.white);
            }
            else
            {
                UpdateOutline(false);
            }  
    }

    public void UpdateOutline(bool selected,float thickness,Color color)
    {
            this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", thickness);
            this.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", color);
    }
    public void UpdateOutline(bool selected)
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
