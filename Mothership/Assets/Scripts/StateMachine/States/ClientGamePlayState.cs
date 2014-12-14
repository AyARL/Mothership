using UnityEngine;
using System.Collections;
using Mothership;
using MothershipOS;

namespace MothershipStateMachine
{
    public class ClientGamePlayState : ClientGameState
    {
        public ClientGamePlayState(ClientManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            MsgClientStatsUpdate statsUpdate = message as MsgClientStatsUpdate;
            if (statsUpdate != null)
            {
                clientManager.ClientStats.UpdateStats(statsUpdate);
                if (clientManager.OnStatsChanged != null)
                {
                    clientManager.OnStatsChanged(clientManager.ClientStats);
                }

                // If player is dead
                if(statsUpdate.CurrentHealth <= 0)
                {
                    clientManager.Die();
                }
                return;
            }

            MsgPlayerDied playerDead = message as MsgPlayerDied;
            if (playerDead != null)
            {
                if(clientManager.OnKillEvent != null)
                {
                    clientManager.OnKillEvent(playerDead.KillerName, playerDead.KillerTeam, playerDead.PlayerName, playerDead.PlayerTeam);
                }

                if(playerDead.PlayerName == UserDataManager.userData.Profile.DisplayName)
                {
                    if(clientManager.OnPlayerDied != null)
                    {
                        clientManager.OnPlayerDied(playerDead.KillerName, playerDead.KillerTeam);
                    }
                }
                return;
            }

            MsgFlagPickedUp flagPickUp = message as MsgFlagPickedUp;
            if (flagPickUp != null)
            {
                if(clientManager.OnPlayerDrivenEvent != null)
                {
                    clientManager.OnPlayerDrivenEvent(flagPickUp.PlayerName, flagPickUp.PlayerTeam, LogEventMessages.EVENT_PLAYER_FLAG_PICKUP);
                }

                return;
            }

            MsgFlagDelivered flagDelivered = message as MsgFlagDelivered;
            if (flagDelivered != null)
            {
                if(clientManager.OnPlayerDrivenEvent != null)
                {
                    clientManager.OnPlayerDrivenEvent(flagDelivered.PlayerName, flagDelivered.PlayerTeam, LogEventMessages.EVENT_PLAYER_FLAG_DROP_OFF);
                }
                return;
            }

            MsgScoreUpdate updateScore = message as MsgScoreUpdate;
            if(updateScore != null)
            {
                if(clientManager.OnTeamScoreChanged != null)
                {
                    clientManager.OnTeamScoreChanged(IAIBase.ETeam.TEAM_BLUE, updateScore.BlueScore);
                    clientManager.OnTeamScoreChanged(IAIBase.ETeam.TEAM_RED, updateScore.RedScore);
                }
            }

            PlayerRespawn respawn = message as PlayerRespawn;
            if(respawn != null)
            {
                if(clientManager.OnPlayerRespawn != null)
                {
                    clientManager.OnPlayerRespawn();
                }
                clientManager.Spawn();
            }

            MatchExpired expired = message as MatchExpired;
            if (expired != null)
            {
                if(clientManager.OnGameDrivenEvent != null)
                {
                    clientManager.OnGameDrivenEvent(LogEventMessages.EVENT_MATCH_ENDED);
                }

                clientManager.ChangeState(clientManager.ClientGameEndState);
                return;
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if (enter != null)
            {
                GamePlayStarted started = enter.Message as GamePlayStarted;
                if (started != null && clientManager.OnMatchStarted != null)
                {
                    if (clientManager.OnGameDrivenEvent != null)
                    {
                        clientManager.OnGameDrivenEvent(LogEventMessages.EVENT_MATCH_STARTED);
                    }
                    clientManager.OnMatchStarted(started.Delay);
                }
                return;
            }
        }
    }
}
