using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    /// <summary>
    /// Wrapper for Voice Audio. Inspired by Google's Speech API.
    /// </summary>
    public class Voice : Audio
    {
        public enum VoiceAudioType
        {
            Unknown,
            OggOpus,
            Flac,
            Amr,
            Amrwb,
            Linear16,
            Speex,
            Mulaw
        }

        public VoiceAudioType type { get; private set; }
        public int sampleRate { get; private set; }
        public int channels { get; private set; }
        public int bitDepth { get; private set; }

        public Voice(VoiceAudioType _type = VoiceAudioType.Unknown)
        {
            type = _type;
            sampleRate = 16000;
            channels = 1;
            bitDepth = 16;
        }

        public Voice(VoiceAudioType _type, int _channels, int _sampleRate, int _bitDepth)
        {
            this.type = type;
            sampleRate = _sampleRate;
            channels = _channels;
            bitDepth = _bitDepth;
        }
    }
}
