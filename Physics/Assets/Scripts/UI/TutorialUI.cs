using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public Text titleTutorial;
    public Text descriptionTutorial;

    public void ShowTutorial(string title, string description)
    {
        titleTutorial.text = title;
        descriptionTutorial.text = description;
    }
}
