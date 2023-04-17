using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    int avgFrameRate;
    [SerializeField]
    Text display_Text;
    string display = "{0} FPS";

    public void Update()
    {
        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        display_Text.text = string.Format(display, avgFrameRate.ToString());
    }
}