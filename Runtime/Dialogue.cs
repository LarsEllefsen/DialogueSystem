using System;

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
    public DialogueConditional.BoolCondition mustBe = DialogueConditional.BoolCondition.TRUE;
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

