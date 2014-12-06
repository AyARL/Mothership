using UnityEngine;
using System.Collections;
using MothershipOS;

public class ClientDataOnServer
{
    public User User { get; private set; }
    public Profile Profile { get; private set; }

    public enum Team { RedTeam, BlueTeam }
    public Team ClientTeam { get; private set; }

    public ClientDataOnServer(User user, Profile profile, Team team)
    {
        User = user;
        Profile = profile;
        ClientTeam = team;
    }
}
