﻿using UnityEngine;
using System.Collections;
using Mothership;
using System.Linq;

namespace MothershipStateMachine
{
    public class ServerGameSetupState : ServerGameState
    {
        public ServerGameSetupState(ServerManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            ClientLoadedLevel loaded = message as ClientLoadedLevel;
            if(loaded != null)
            {
                ClientDataOnServer client = serverManager.RegisteredClients.FirstOrDefault(c => c.NetworkPlayer == loaded.Player);
                if (client != null)
                {
                    client.LoadedLevel = true;
                    if (serverManager.RegisteredClients.All(c => c.LoadedLevel == true))
                    {
                        serverManager.CountdownToMatch();
                        serverManager.networkManager.StartMatchCountdown();
                    }
                }
                return;
            }

            MatchCountdownEnded countdownEnded = message as MatchCountdownEnded;
            if(countdownEnded != null)
            {
                serverManager.ChangeState(serverManager.ServerGamePlayState);
                return;
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {

        }
    }
}
