using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NarratorLines", menuName = "Narrator/Narrator Lines")]
public class NarratorLines : ScriptableObject
{
    [TextArea]
    public List<string> lines = new List<string>();
    public float chanceOfSaying = 85f; //The lines aren't supposed to always be said, this will make sure of it
}
