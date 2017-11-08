using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    Canvas nameCanvas;
    Text nameText;
    Image nameBg;

    public Civilization currentOwner;
    public Civilization originalOwner;
    string cityName = "";
    public string CityName
    {
        get
        {
            return cityName;
        }
        set
        {
            cityName = value;
            name = value + " (" + currentOwner.ToString() + ")";
            nameText.text = cityName;
            int textSizeX = 0;

            Font myFont = nameText.font;  //chatText is my Text component
            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = value.ToCharArray();

            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, nameText.fontSize);
                textSizeX += characterInfo.advance;
            }

            Debug.Log(textSizeX);

            nameBg.rectTransform.sizeDelta = new Vector2(textSizeX + 10, nameBg.rectTransform.sizeDelta.y);
        }
    }

    public int productionPerTurn
    {
        get
        {
            return 1; //replace with calculation based on worked tiles etc.
        }
    } 

    Queue<Production> productionQueue = new Queue<Production>();

    void Awake()
    {
        Player player = GetComponentInParent<Player>();
        player.cities.Add(this);

        nameCanvas = GetComponentInChildren<Canvas>();
        nameText = nameCanvas.GetComponentInChildren<Text>();
        nameBg = nameCanvas.GetComponentInChildren<Image>();
    }

    public void NextTurn()
    {
        productionQueue.Peek().containedProduction += productionPerTurn;
        if(productionQueue.Peek().ProductionRemaining <= 0)
        {
            productionQueue.Dequeue().Produce();
        }
    }

    public void AddProduction(Production prod)
    {
        productionQueue.Enqueue(prod);
    }

    public void SetNameRotation(float angle)
    {
        nameCanvas.transform.eulerAngles = new Vector3(90 + angle, 0, 0);
    }
}
