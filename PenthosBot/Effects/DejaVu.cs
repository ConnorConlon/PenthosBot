using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TwitchLib.Client;
using PenthosBot.ChatBot;

namespace PenthosBot.Effects
{
    public class DejaVu
    {
        enum RunningState
        {
            Off,
            RevingUp,
            Running
        }

        RunningState m_state;
        VideoPlayer m_videoPlayer;
        Video m_video;
        Song m_music;

        TwitchClient m_client;

        TimeSpan m_effectDelay;
        TimeSpan m_ellapsedTime;
        TimeSpan m_effectDuration;
        TimeSpan m_timespanHack;

        public DejaVu(ContentManager contentMgr, GraphicsDevice graphicsDevice, TwitchClient client)
        {
            m_state = RunningState.Off;

            m_videoPlayer = new VideoPlayer();

            m_music = contentMgr.Load<Song>("running90s");
            m_video = contentMgr.Load<Video>("speedeffect");

            m_client = client;

            m_effectDelay = new TimeSpan(0, 0, 22);
            m_effectDuration = new TimeSpan(0, 4, 37);
            m_ellapsedTime = new TimeSpan();
            m_timespanHack = new TimeSpan(0, 0, 1);
        }

        public Texture2D GetEffect()
        {
            if(m_videoPlayer.State != MediaState.Stopped)
            {
                return m_videoPlayer.GetTexture();
            }

            return null;
        }

        public bool IsRunning()
        {
            return m_state == RunningState.Running;
        }

        public void RunInThe90s()
        {
            if(m_state == RunningState.Off)
            {
                try
                {
                    MediaPlayer.Volume = 0.3f;
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(m_music);

                    Console.WriteLine("Trying To Play DajaVu");
                    m_videoPlayer.Play(m_video);

                    m_state = RunningState.RevingUp;
                }
                catch
                {
                    Halt();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if(m_state != RunningState.Off)
            {
                m_ellapsedTime += gameTime.ElapsedGameTime;
            }

            if (m_ellapsedTime >= m_effectDuration)
            {
                Console.WriteLine("Halting DajaVu");
                Halt();
            }

            if (m_state == RunningState.RevingUp)
            {
                if (m_ellapsedTime >= m_effectDelay)
                {
                    m_state = RunningState.Running;
                }
            }

            if(m_state == RunningState.Running)
            {
                if(m_videoPlayer.PlayPosition >= (m_video.Duration - m_timespanHack) ||
                    m_videoPlayer.State == MediaState.Stopped)
                {
                    try
                    {
                        Console.WriteLine("Looping DajaVu");
                        m_videoPlayer.Stop();
                        m_videoPlayer.Play(m_video);
                    }
                    catch
                    {
                        Halt();
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            if(m_state == RunningState.Running && m_videoPlayer.GetTexture() != null)
            {
                spriteBatch.Draw(m_videoPlayer.GetTexture(), rect, Color.White);
            }
        }

        public void Halt()
        {
            m_state = RunningState.Off;

            m_videoPlayer.Stop();
            MediaPlayer.Stop();

            m_ellapsedTime = new TimeSpan();
        }
    }
}
