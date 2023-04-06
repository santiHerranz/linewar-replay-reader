
using Assets.Common.Messages;
using Assets.Common.Serialization;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Client.Replays
{
    public class ReplayPlayback : IDisposable
    {
        private readonly string fileName;
        private readonly Serializer serializer;
        private MemoryStream stream;
        private MatchStarted entry;

        public ReplayPlayback(string fileName, Serializer serializer)
        {
            this.fileName = fileName;
            this.serializer = serializer;
        }

        public MatchStarted Entry
        {
            get
            {
                if (this.entry == null)
                    this.Initialize();
                return this.entry;
            }
        }

        private Stream Stream
        {
            get
            {
                if (this.stream == null)
                    this.Initialize();
                return (Stream)this.stream;
            }
        }

        public bool TryInitialize()
        {
            try
            {
                this.Initialize();
                return this.Entry != null;
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);

                return false;
            }
        }

        public Message ReadNext()
        {
            return this.serializer.DeserializeForReplay(this.Stream);
        }

        public void Dispose()
        {
            if (this.stream == null)
                return;
            this.stream.Close();
            this.stream.Dispose();
            this.stream = (MemoryStream)null;
        }

        private void Initialize()
        {
            this.stream = new MemoryStream(File.ReadAllBytes(this.fileName));
            this.entry = (MatchStarted)this.serializer.DeserializeForReplay(this.Stream);
        }
    }
}
