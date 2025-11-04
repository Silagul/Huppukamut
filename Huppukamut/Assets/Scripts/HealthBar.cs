using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public UIImageChanger changer;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        changer.uiImage = fill;
        

        fill.color = gradient.Evaluate(1f);
    }
    
    public void SetHealth(int health)
    {
        slider.value = health;
        if(health == 65)
        {
            changer.ChangeImage();
        }
        else
        {
            if(health == 30)
            {
                changer.ChangeImage();
            }
            else
            {
                if(health == 0)
                {
                    changer.ChangeImage();
                }
            }
        }

            fill.color = gradient.Evaluate(slider.normalizedValue);
    }


}
