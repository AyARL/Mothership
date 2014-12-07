using UnityEngine;
using System.Collections;

public class TeamList
{
    public IAIBase.ETeam TeamColour { get; private set; }
    public string[] TeamDisplayNames { get; private set; }

    public TeamList() { }

    public TeamList(IAIBase.ETeam colour, string[] playerNames)
    {
        TeamColour = colour;
        TeamDisplayNames = playerNames;
    }
}
