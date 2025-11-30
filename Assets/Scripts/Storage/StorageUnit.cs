    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class StorageUnit : MonoBehaviour
{
    public Crop crop;
    public int maxCapacity; 
    [SerializeField] public int cropStored;
    [SerializeField] Sprite emptyStorageUnitSprite;
    public bool hovered;
    
    public State state;

    private void Update()
    {
        UpdateState();
        UpdateRender();
        if(cropStored > maxCapacity) 
            cropStored = maxCapacity;
        maxCapacity = crop.maxStorageCapacity;
    }

    public void AddCrop(int amount)
    {
        cropStored += amount;
    }
    public void RemoveCrop(int amount)
    {
        cropStored -= amount;
    }
    private void UpdateState()
    {
        if(cropStored >= maxCapacity) state = State.Full;
        else if(cropStored >= maxCapacity/2) state = State.HalfFull;
        else if(cropStored == 0) state = State.Empty;
    }

    private void UpdateRender()
    {
        SpriteRenderer renderer= GetComponent<SpriteRenderer>();
        switch (state)
        {
            case State.Empty:
                renderer.sprite = emptyStorageUnitSprite;
                break;
            case State.Partial:
                renderer.sprite = crop.storageunitsprite[0];
                break;
            case State.HalfFull:
                renderer.sprite = crop.storageunitsprite[1];
                break;
            case State.Full:
                renderer.sprite = crop.storageunitsprite[2];
                break;
        }
    }
    public enum State { Empty, Partial, HalfFull, Full }
    
}
