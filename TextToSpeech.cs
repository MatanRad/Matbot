using System.Speech.Synthesis;
using System.IO;
using Concentus.Structs;

namespace Matbot
{
    class TextToSpeech
    {

        public static Stream Speak(string text)
        {
            SpeechSynthesizer s = new SpeechSynthesizer();
            MemoryStream stream = new MemoryStream();
            s.SetOutputToAudioStream(stream,new System.Speech.AudioFormat.SpeechAudioFormatInfo(24000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono));
            s.Speak(text);
            s.SetOutputToNull();
            return stream;
        }

        private static short[] BytesToShorts(byte[] input, int offset, int length)
        {
            short[] processedValues = new short[length / 2];
            for (int c = 0; c < processedValues.Length; c++)
            {
                processedValues[c] = (short)(((int)input[(c * 2) + offset]) << 0);
                processedValues[c] += (short)(((int)input[(c * 2) + 1 + offset]) << 8);
            }

            return processedValues;
        }

        public static Stream SpeakOgg(string text)
        {
            MemoryStream orgstream = Speak(text) as MemoryStream;
            orgstream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = orgstream.ToArray();
            short[] shorts = BytesToShorts(bytes, 0, bytes.Length);

            OpusEncoder encoder = OpusEncoder.Create(24000, 1, Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);

            MemoryStream finish = new MemoryStream();
            Concentus.Oggfile.OpusOggWriteStream s = new Concentus.Oggfile.OpusOggWriteStream(encoder, finish);
            s.WriteSamples(shorts, 0, shorts.Length);
            s.Finish();

            finish.Seek(0, SeekOrigin.Begin);
            return finish;
        }

        
    }


}