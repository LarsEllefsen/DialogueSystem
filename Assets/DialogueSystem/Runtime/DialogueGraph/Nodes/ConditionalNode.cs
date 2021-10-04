using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalNode : BaseNode
{
    public override string NodeType => "ConditionalNode";

    [Input] public float input;
    
    [Output] public float trueOutput;
    [Output] public float falseOutput;

    public List<DialogueConditional> condition = new List<DialogueConditional>() { new DialogueConditional() };

    protected override void Init()
    {
        nodeName = "Condition";
        base.Init();
    }

    public override string GetID()
    {
        return id;
    }
}
