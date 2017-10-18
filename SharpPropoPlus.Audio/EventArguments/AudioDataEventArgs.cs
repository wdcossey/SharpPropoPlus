using System;
using SharpPropoPlus.Audio.Enums;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class AudioDataEventArgs : EventArgs
    {

        public int SampleRate { get; }

        public int Channels { get; }

        public int BytesRecorded { get; }

        public byte[] Buffer { get; }

        public int BitsPerSample { get; }

        public AudioChannel PreferedChannel { get; }

        public AudioBitrate PreferedBitrate { get; }


        private AudioDataEventArgs()
        {

        }

        public AudioDataEventArgs(
            int sampleRate, 
            int bitsPerSample, 
            int channels, 
            int bytesRecorded, 
            byte[] buffer,
            AudioChannel audioChannel,
            AudioBitrate audioBitrate)
            : this()
        {
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            Channels = channels;
            BytesRecorded = bytesRecorded;
            Buffer = buffer;
            PreferedChannel = audioChannel;
            PreferedBitrate = audioBitrate;
        }

    }
}