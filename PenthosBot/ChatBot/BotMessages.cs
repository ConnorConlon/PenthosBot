using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace PenthosBot.ChatBot
{
    public abstract class BotMessageBase
    {
        public abstract void Execute();
        public abstract void Setup(ContentManager content);
    }

    public class PlayMusicMessage : BotMessageBase
    {
        private Song m_song;
        private string m_songName;
        private float m_fVolume;
        private TimeSpan m_startPosition;

        public PlayMusicMessage(String songName, float fVolume, TimeSpan startPosition)
        {
            m_songName = songName;
            m_fVolume = fVolume;
            m_startPosition = startPosition;
        }

        public override void Setup(ContentManager content)
        {
            m_song = content.Load<Song>(m_songName);
        }

        public override void Execute()
        {
            MediaPlayer.Volume = m_fVolume;
            MediaPlayer.Play(m_song);
        }
    }

    public class StopMusicMessage : BotMessageBase
    {
        public override void Setup(ContentManager content)
        {
            // Do Nothing
        }

        public override void Execute()
        {
            MediaPlayer.Stop();
        }
    }


    public class PlaySoundMessage : BotMessageBase
    {
        private SoundEffect m_sfx;
        private String m_sfxName;
        private float m_fVolume;

        public PlaySoundMessage(String sfxName, float fVolume)
        {
            m_sfxName = sfxName;
            m_fVolume = fVolume;
        }

        public override void Setup(ContentManager content)
        {
            m_sfx = content.Load<SoundEffect>(m_sfxName);
        }

        public override void Execute()
        {
            m_sfx.Play();
        }
    }
}