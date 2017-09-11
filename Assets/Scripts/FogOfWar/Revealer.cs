using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revealer : MonoBehaviour
{
    public int radius;

    private void Start()
    {
        FOWManager.Instance.RegisterRevealer(this);
    }
}
