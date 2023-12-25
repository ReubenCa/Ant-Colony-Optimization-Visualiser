using UnityEngine;

public class FadingPanel : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup;   // Reference to the CanvasGroup of your panel
   // public float fadeSpeed = 5f;           // Speed of the fade in/out animation

    private bool isPanelVisible = true;

    void Start()
    {
        // Ensure that the panel is initially visible
        panelCanvasGroup.alpha = 1f;
    }

    void Update()
    {
        // Check for user input to toggle panel state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePanel();
        }
    }

    void TogglePanel()
    {
        // Toggle the panel state
        isPanelVisible = !isPanelVisible;

        // Set the target alpha based on the panel state
        float targetAlpha = isPanelVisible ? 1f : 0f;

        // Smoothly interpolate between the current alpha and the target alpha
        panelCanvasGroup.alpha = targetAlpha;
    }
}
