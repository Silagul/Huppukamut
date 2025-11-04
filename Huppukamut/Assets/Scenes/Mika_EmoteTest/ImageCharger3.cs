using UnityEngine;
using UnityEngine.UI;

public class UIImageChanger : MonoBehaviour
{
    public Sprite image1;
    public Sprite image2;
    public Sprite image3;
    public Sprite image4;

    public Image uiImage;
    public UIImageChanger changer;
    private int currentIndex = 0;

    void Start()
    {
        uiImage = GetComponent<Image>();
        uiImage.sprite = image1;
    }

    public void ChangeImage()
    {


        currentIndex++;

        if (currentIndex == 1)
            uiImage.sprite = image2;
        else if (currentIndex == 2)
            uiImage.sprite = image3;
        else if (currentIndex == 3)
            uiImage.sprite = image4;
        else
        {
            uiImage.sprite = image1;
            currentIndex = 0;
        }
    }
}
