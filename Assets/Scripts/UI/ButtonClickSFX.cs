using UnityEngine;

public class ButtonClickSFX : MonoBehaviour
{
    public void playButtonSound()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
    }

    
}
