using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using WHS.DEVICE.AUDIO.Models;

namespace WHS.DEVICE.AUDIO.Until
{
    public  class SoundPlayers
    {
        public static void PlayMain(string json)
        {
            try
            {
                var player = JsonConvert.DeserializeObject<Player>(json);
                if (player != null)
                {
                    switch (player.Type)
                    {
                        case "PlayOK": { PlayOK(); } break;
                        case "PlayCancel": { PlayCancel(); } break;
                        case "PlayMsg": { PlayMsg(); } break;
                        case "PlayError": { PlayError(); } break;
                        case "ScanError": { ScanError(); } break;
                        case "PlaySucces": { PlaySucces(); } break;
                        case "PlayIndex": { PlayIndex(player.Index); } break;
                    }
                }
            }
            catch(Exception  ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 扫描完成
        /// </summary>
        private static void PlayOK()
        {
            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\ok.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.ok);
            }
           
        }
        /// <summary>
        /// 取消扫描
        /// </summary>
        private static void PlayCancel()
        {
            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\CE.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.CE);
            }
        }
        /// <summary>
        /// 扫描提示
        /// </summary>
        private static void PlayMsg()
        {
            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\Msg.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.Msg);
            }
        }
        /// <summary>
        /// 扫描错误
        /// </summary>
        private static void PlayError()
        {
            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\error.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.error);
            }
        }

        /// <summary>
        /// 扫描错误_人声音
        /// </summary>
        private static void ScanError()
        {
            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\ScanErr.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.ScanErr);
            }
        }

        /// <summary>
        /// 全部扫描成功
        /// </summary>
        private static void PlaySucces()
        {

            if (AudioData.LoadViceType == 1)
            {
                string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\WHS.DEVICE.AUDIO\\WAV\\Succes.wav");
                Paly(fileName);
            }
            else
            {
                Paly(AUDIO.Properties.Resource.Succes);
            }
        }
        /// <summary>
        /// 计数扫描
        /// </summary>
        private static void PlayIndex(int m)
        {
            if (AudioData.LoadViceType == 1)
            {
                if (m >= 0 && m <= 36)
                {
                    string str = string.Format(@"Plugins\\WHS.DEVICE.AUDIO\\WAV\\{0}.wav", m.ToString());
                    string fileName = fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                    Paly(fileName);
                }
                else
                {
                    PlayBySpeech(m.ToString());
                }
            }
            else
            {
                PlayBySpeech(m.ToString());
            }
        }
        private static void Paly(Stream stream)
        {
            if (stream != null)
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                try
                {
                    soundPlayer.Stream = stream;
                    if (AudioData.PlayType == 0)
                    {
                        soundPlayer.Play();
                    }
                    else
                    {
                        soundPlayer.PlaySync();
                    }
                }
                catch
                {
                    SystemSounds.Asterisk.Play();
                }
                finally
                {
                    soundPlayer.Dispose();
                }
            }
        }

        private static void Paly(string fileName)
        {
            if (File.Exists(fileName))
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                try
                {
                    soundPlayer.SoundLocation = fileName;
                    if (AudioData.PlayType == 0)
                    {
                        soundPlayer.Play();
                    }
                    else
                    {
                        soundPlayer.PlaySync();
                    }
                }
                catch
                {
                    SystemSounds.Asterisk.Play();
                }
                finally
                {
                    soundPlayer.Dispose();
                }
            }
        }
        private static void PlayBySpeech(string msg)
        {
            SpeechSynthesizer speechSyn = new SpeechSynthesizer();
            try
            {
                if (AudioData.PlayType == 0)
                {
                    speechSyn.SpeakAsync(msg);
                }
                else
                {
                    speechSyn.Speak(msg);
                }
            }
            catch
            {
                speechSyn.Dispose();
            }
            finally
            {
            }
        }
    }
}
