using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventFlag
{
    public string name;
    public bool active;
    public GameEventFlag(string name, bool active)
    {
        this.name = name;
        this.active = active;
    }
}
