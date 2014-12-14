using UnityEngine;
using System.Collections;
using System.Linq;
using Mothership;

namespace MothershipStateMachine
{
    public class ServerGamePlayState : ServerGameState
    {
        private int m_iBlueScore = 0;

        private int m_iRedScore = 0;

        public ServerGamePlayState(ServerManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            // Please not that the messages have been segmented intentionally as both
            //  clients and AI characters are sending messages and the serverManager
            //  does not differentiate between messages sent by AI or clients.
            //  
            //  For example, score should always be updated when an AI character has scored
            //  a point, but as the AI has no ClientDataOnServer object there's no point
            //  in updating client stats.
            //  
            //  Additionally, sometimes we may not want to forward certain messages to the clients,
            //  therefore message forwarding is a separate RPC.
            MatchExpired expired = message as MatchExpired;
            if(expired != null)
            {
                if(serverManager.OnMatchEnded != null)
                {
                    serverManager.OnMatchEnded();
                }
                serverManager.ChangeState(serverManager.ServerGameEndState, new MsgScoreUpdate() { RedScore = m_iRedScore, BlueScore = m_iBlueScore });
                return;
            }

            //MsgDamageClient msgDamageClient = message as MsgDamageClient;
            //if ( null != msgDamageClient )
            //{
            //    // A client has been hit, we want to damage the client which the 
            //    //  message specifies.
            //    foreach ( ClientDataOnServer clientData in serverManager.RegisteredClients )
            //    {
            //        if ( clientData.Profile.DisplayName == msgDamageClient.UserName )
            //        {
            //            clientData.CurrentHealth -= msgDamageClient.Damage;

            //            serverManager.networkManager.UpdateClientStats( clientData );

            //            break;
            //        }
            //    }
            //}

            MsgFlagPickedUp msgFlagPickedUp = message as MsgFlagPickedUp;
            if ( null != msgFlagPickedUp )
            {
                // Forward this message to the clients so they log the event.
                serverManager.networkManager.ForwardFlagPickedUp( msgFlagPickedUp );
            }

            PlayerTakenDamage playerDamage = message as PlayerTakenDamage;
            if(playerDamage != null)
            {
                ClientDataOnServer clientData = serverManager.RegisteredClients.First(c => c.NetworkPlayer == playerDamage.Player);
                clientData.CurrentHealth -= playerDamage.Damage;

                // If client is dead
                if(clientData.CurrentHealth <= 0)
                {
                    serverManager.CountdownToPlayerRespawn(clientData.NetworkPlayer);
                    serverManager.SendGameMessage(new MsgPlayerDied() { PlayerName = clientData.Profile.DisplayName, PlayerTeam = clientData.ClientTeam, KillerName = playerDamage.Attacker, KillerTeam = playerDamage.AttackerTeam });
                }
                else // otherwise just send the updated stats
                {
                    serverManager.networkManager.UpdateClientStats(clientData);
                }
            }

            MsgPlayerDied msgPlayerDied = message as MsgPlayerDied;
            if ( null != msgPlayerDied )
            {

                // Try to find the client data object for the scoring player and update his stats.
                foreach ( ClientDataOnServer clientData in serverManager.RegisteredClients )
                {
                    if ( clientData.Profile.DisplayName == msgPlayerDied.PlayerName )
                    {
                        clientData.DeathCount += 1;
                        clientData.CurrentHealth = 0;
                        serverManager.networkManager.UpdateClientStats( clientData );
                    }

                    if ( clientData.Profile.DisplayName == msgPlayerDied.KillerName )
                    {
                        clientData.KillCount += 1;
                        clientData.EXP += Constants.POINTS_ON_KILL;
                        serverManager.networkManager.UpdateClientStats( clientData );
                    }
                }

                // Forward this message to the clients so they log the event.
                serverManager.networkManager.ForwardCharacterDied( msgPlayerDied );
            }

            PlayerRespawn playerRespawn = message as PlayerRespawn;
            if(playerRespawn != null)
            {
                serverManager.networkManager.RespawnPlayer(playerRespawn.Player);
            }

            MsgFlagDelivered msgFlagDelivered = message as MsgFlagDelivered;
            if ( null != msgFlagDelivered )
            {
                // Increase score depending on the player's team.
                switch ( msgFlagDelivered.PlayerTeam )
                {
                    case IAIBase.ETeam.TEAM_BLUE:

                        m_iBlueScore += 1;

                        break;
                    case IAIBase.ETeam.TEAM_RED:

                        m_iRedScore += 1;

                        break;
                }

                // Update the score on the network.
                serverManager.networkManager.UpdateScore( m_iRedScore, m_iBlueScore );

                // Try to find the client data object for the scoring player and update his stats.
                foreach ( ClientDataOnServer clientData in serverManager.RegisteredClients )
                {
                    if ( clientData.Profile.DisplayName == msgFlagDelivered.PlayerName )
                    {
                        clientData.CaptureCount += 1;
                        clientData.EXP += Constants.POINTS_ON_CAPTURE;

                        serverManager.networkManager.UpdateClientStats( clientData );

                        break;
                    }
                }

                // Forward this message to the clients so they log the event.
                serverManager.networkManager.ForwardFlagCaptured( msgFlagDelivered );
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                serverManager.StartGameTimer();
                serverManager.networkManager.GamePlayStarted();
            }
        }
    }
}
