using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Google.Cloud.Speech.V1;

namespace Matbot.Handlers.Structure
{
    public abstract class SpeechHandler : VoiceHandler
    {
        public static RecognitionConfig.Types.AudioEncoding VoiceTypeToGoogleType(Voice.VoiceAudioType type)
        {
            switch(type)
            {
                case Voice.VoiceAudioType.Amr:
                    return RecognitionConfig.Types.AudioEncoding.Amr;
                case Voice.VoiceAudioType.Amrwb:
                    return RecognitionConfig.Types.AudioEncoding.AmrWb;
                case Voice.VoiceAudioType.Flac:
                    return RecognitionConfig.Types.AudioEncoding.Flac;
                case Voice.VoiceAudioType.Linear16:
                    return RecognitionConfig.Types.AudioEncoding.Linear16;
                case Voice.VoiceAudioType.Mulaw:
                    return RecognitionConfig.Types.AudioEncoding.Mulaw;
                case Voice.VoiceAudioType.OggOpus:
                    return RecognitionConfig.Types.AudioEncoding.OggOpus;
                case Voice.VoiceAudioType.Speex:
                    return RecognitionConfig.Types.AudioEncoding.SpeexWithHeaderByte;
                default:
                    return RecognitionConfig.Types.AudioEncoding.EncodingUnspecified;
            }
        }

        public string languageCode = "en-GB";

        public int maxDur = 20;

        public bool isAsync = true;

        public override void Handle(Message m)
        {
            if (isAsync)
            {
                Task.Run(() =>
                {
                    TranscribeSpeech(m);
                });
            }
            else
            {
                TranscribeSpeech(m);
            }
        }

        private void TranscribeSpeech(Message m)
        {
            if (m.voice == null) throw new Exception.EmptyVoiceMessageException(m);
            if (m.voice.Duration > maxDur)
            {
                MaxDurationExceeded(m);
                return;
            }

            SpeechClient speech = SpeechClient.Create();

            RecognitionConfig config = new RecognitionConfig();
            config.Encoding = SpeechHandler.VoiceTypeToGoogleType(m.voice.type);
            config.SampleRateHertz = m.voice.sampleRate;
            config.LanguageCode = languageCode;
            config.ProfanityFilter = false;


            RecognizeResponse resp = speech.Recognize(config, RecognitionAudio.FromStream(m.voice.AudioStream));
            foreach (var result in resp.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    HandleSpeech(m, alternative.Transcript);
                }
            }
        }

        void MaxDurationExceeded(Message m)
        {
            m.Reply("Max voice duration exceeded! (" + maxDur + " seconds).");
        }

        public abstract void HandleSpeech(Message m, string text);
    }
}
