using UnityEngine;
using System.Collections;

public class TeamList
{
    public ClientDataOnServer.Team TeamColour { get; private set; }
    public string[] TeamDisplayNames { get; private set; }

    public TeamList() { }

    public TeamList(ClientDataOnServer.Team color, string[] playerNames)
    {
        TeamColour = color;
        TeamDisplayNames = playerNames;
    }
}
