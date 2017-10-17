using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public Civilization currentOwner;
    public Civilization originalOwner;
    string name = "";
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    Queue<Production> production = new Queue<Production>();

    void Start()
    {
        Player player = GetComponentInParent<Player>();
        player.cities.Add(this);
    }

    public void NextTurn()
    {

    }

    public void AddProduction()
    {

    }
}
