using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
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
