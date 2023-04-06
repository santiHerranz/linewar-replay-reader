using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Common;
using Assets.Common.Messages;
using Assets.Common.Serialization;

using Assets.Common.Replays;
using Zenject;
using Assets.Client.Replays;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LineWar Replay Reader by Tim Baler");

        string filename = @"C:\temp\94c63125-d368-4d4c-9713-174ae2803df6.zlwreplay";

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

                    Debug.Log("Elapsed: " + next.Elapsed + " CreatePresenter " + PlayerType + " at " + position.ToString());
                }

                if (next.ToString().Equals("MatchEnded"))
                {
                    isMatchEnd = true;
                }

            }

            playback.Dispose();
        }

    }

}
