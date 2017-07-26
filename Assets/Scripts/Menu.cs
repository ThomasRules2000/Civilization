using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public int numCivs = 2;
    public int width = 80;
    public int height = 52;

    public int NumberOfCivilizations
    {
        get
        {
            return numCivs;
        }
        set
        {
            numCivs = value;
            numCivsText.text = value.ToString();
            numCivsSlider.value = value;
        }
    }

    public Slider numCivsSlider;
    public Dropdown mapSize;
    public Text numCivsText;

    public void updateNumCivs()
    {
        NumberOfCivilizations = (int)numCivsSlider.value;
    }

    public void updateMapSize()
    {
        switch (mapSize.value)
        {
            case 0:
                width = 40;
                height = 25;
                NumberOfCivilizations = 2;
                break;
            case 1:
                width = 56;
                height = 36;
                NumberOfCivilizations = 4;
                break;
            case 2:
                width = 66;
                height = 42;
                NumberOfCivilizations = 6;
                break;
            case 3:
                width = 80;
                height = 52;
                NumberOfCivilizations = 8;
                break;
            case 4:
                width = 104;
                height = 64;
                NumberOfCivilizations = 10;
                break;
            case 5:
                width = 128;
                height = 80;
                NumberOfCivilizations = 12;
                break;
        }
    }
}
