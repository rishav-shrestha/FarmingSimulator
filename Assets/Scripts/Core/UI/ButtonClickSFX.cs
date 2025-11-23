using UnityEngine;

public class ButtonClickSFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playButtonSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
    }

    
}
