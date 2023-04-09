using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Common;
using Assets.Common.Messages;
using Assets.Common.Serialization;

using Assets.Common.Replays;
using Zenject;
using Assets.Client.Replays;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LineWar Replay and Summary Reader by Tim Baler");

        readReplayFile(@"C:\temp\94c63125-d368-4d4c-9713-174ae2803df6.zlwreplay");

        //readSummaryFile(@"C:\temp\20230408_102005_FFA_TimBaler,_RK_big_[+1]_on_465019_36_1.lwsummary");

    }

    private void readSummaryFile(string filename)
    {

        Serializer serializer = new Serializer();

        List<RegularUpdate> regularUpdates = new List<RegularUpdate>();

        using (FileStream fileStream = File.OpenRead(filename))
        {
            while (fileStream.Position < fileStream.Length)
            {
                RegularUpdate message = serializer.DeserializeForDataSummary(fileStream);

                var matchTime = TimeSpan.FromSeconds(message.MatchTime).ToString("h\\:mm\\:ss");

                Debug.Log("matchTime: " + matchTime + " Player: " + message.PlayerRecipientId + " EmpireValue: " + message.EconomyReport.EmpireValue + " CapitalTreasury: " + message.EconomyReport.CapitalTreasury);

                regularUpdates.Add(message);
            }
        }


    }

    /// <summary>
    /// Read Replay files v36.1
    /// </summary>
    /// <param name="filename"></param>
    private static void readReplayFile(string filename)
    {
        ReplayPlayback playback = new ReplayPlayback(filename, new Serializer());
        if (playback.TryInitialize())
        {
            bool isMatchEnd = false;

            while (!isMatchEnd)
            {
                Message next = playback.ReadNext();


                if (next.ToString().Contains("CreatePresenter"))
                {
                    CreatePresenter message = (CreatePresenter)next;
                    EntityType PlayerType = message.Type;
                    Vector3 position = message.Position;

                    //Debug.Log("Elapsed: " + next.Elapsed + " CreatePresenter " + PlayerType + " at " + position.ToString());
                }

                if (next.ToString().Equals("MatchEnded"))
                {
                    isMatchEnd = true;


                    MatchEnded message = (MatchEnded)next;
                    IEnumerable<PlayerData> players = message.MatchInfo.GetAll();
                    foreach (var playerData in players)
                    {

                        Debug.Log("Player: " + playerData.ToString());
                        Debug.Log("Id: " + playerData.Id);

                        Debug.Log("TotalEnergyConsumed: " + playerData.TotalEnergyConsumed);
                        Debug.Log("TerritoryConquests: " + playerData.TerritoryConquests);
                        Debug.Log("CitiesConstructed: " + playerData.CitiesConstructed);
                        Debug.Log("StructuresLostWorth: " + playerData.StructuresLostWorth);
                        Debug.Log("StructuresLost: " + playerData.StructuresLost);
                        Debug.Log("StructuresConstructed: " + playerData.StructuresConstructed);
                        Debug.Log("UnitsLostWorth: " + playerData.UnitsLostWorth);
                        Debug.Log("UnitsLost: " + playerData.UnitsLost);
                        Debug.Log("UnitsProduced: " + playerData.UnitsProduced);
                        Debug.Log("TotalEnergyProduced: " + playerData.TotalEnergyProduced);
                        Debug.Log("TotalCapitalUpkeep: " + playerData.TotalCapitalUpkeep);
                        Debug.Log("TotalCapitalGenerated: " + playerData.TotalCapitalGenerated);
                        Debug.Log("Status: " + playerData.Status.ToString());
                        Debug.Log("TerritoryLosses: " + playerData.TerritoryLosses);


                    }
                    IEnumerable<PlayerData> all = message.MatchInfo.GetAll();
                    string ResolveEndReason = string.Join(", ", all.Select((Func<PlayerData, string>)(data => "'" + message.MatchInfo.ResolveEndReason(data.Id) + "'")).Distinct<string>());

                    Debug.Log("ResolveEndReason: " + ResolveEndReason);


                }

            }

            playback.Dispose();
        }
    }
}
