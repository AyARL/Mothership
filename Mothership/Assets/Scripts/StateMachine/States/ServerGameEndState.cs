using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;
using MothershipUtility;
using MothershipOS;

namespace MothershipStateMachine
{
    public class ServerGameEndState : ServerGameState
    {
        public ServerGameEndState(ServerManager manager) : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                serverManager.networkManager.MatchExpired();

                MsgScoreUpdate score = enter.Message as MsgScoreUpdate;
                if(score != null)
                {
                    serverManager.StartCoroutine(RecordGameHistory(score.BlueScore, score.RedScore));
                    GameResult result = new GameResult() { BlueScore = score.BlueScore, RedScore = score.RedScore, PlayerResults = serverManager.GetPlayerResults() };
                    serverManager.networkManager.SendGameResult(result);
                }

                serverManager.networkManager.StartCoroutine(serverManager.networkManager.DisconnectClients());

                return;
            }
        }

        // Sends the details for the game and all players to the database
        private IEnumerator RecordGameHistory(int blueScore, int redScore)
        {
            string winner = "Draw";
            if(blueScore > redScore)
            {
                winner = "Blue";
            }
            else if(redScore > blueScore)
            {
                winner = "Red";
            }

            // create player array
            GamePlayerRecord[] playerRecords = new GamePlayerRecord[serverManager.ClientCount];
            int i = 0;
            foreach(ClientDataOnServer client in serverManager.RegisteredClients)
            {
                string clientTeam = client.ClientTeam == IAIBase.ETeam.TEAM_RED ? "Red" : "Blue";
                GamePlayerRecord record = new GamePlayerRecord() { UserID = client.User.UserID, Team = clientTeam, EXP = client.EXP };
                playerRecords[i] = record;
                ++i;
            }

            WWWForm gameForm = new WWWForm();
            gameForm.AddField("Winner", winner);
            gameForm.AddField("Players", JsonUtility.SerializeToJson<GamePlayerRecord[]>(playerRecords));
            gameForm.AddField("Hash", HashUtility.GetMD5Hash(winner + AppKey.appKey));

            WWW response = new WWW(WWWFormUtility.recordGameScoresURL, gameForm);
            yield return response;

            if(response.error == null)
            {
                Debug.Log(response.text);
            }
            else
            {
                Debug.Log(response.error);
            }
        }

        private class GamePlayerRecord
        {
            public int UserID { get; set; }
            public string Team { get; set; }
            public int EXP { get; set; }
        }
    }
    

}