using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string Name;
    public Branch[] Branches;
}

[Serializable]
public class Branch
{
    public string Id;
    public Condition[] Conditions;
    public Line[] Lines;
}

[Serializable]
public class Condition
{
    public string Flag;
    public bool MustBe;
}

[Serializable]
public class Line
{
    public string Text;

}

public class Choice
{
    public string Text;
}

