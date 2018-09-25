using System;
using Microsoft.Xna.Framework;

namespace PenthosBot.Graphics
{
    class SpriteAnimationPlayer
    {
        #region Private Members
        private SpriteAnimation m_CurrentAnimation;
        private Rectangle m_SourceRectangle;
        private int m_iCurrentFrame;
        private float m_fTime;
        #endregion Private Members

        #region Properties
        public int CurrentFrame
        {
            get { return m_iCurrentFrame; }
        }

        public Rectangle SourceRectangle
        {
            get { return m_SourceRectangle; }
        }
        #endregion Properties

        #region Ctor / Init
        public SpriteAnimationPlayer()
        {
            m_CurrentAnimation = null;
            m_SourceRectangle = new Rectangle();
            m_iCurrentFrame = 0;
            m_fTime = 0.0f;
        }
        #endregion Ctor / Init

        #region Public Methods
        public void PlayAnimation(SpriteAnimation animation)
        {
            if (m_CurrentAnimation == animation)
                return;

            m_CurrentAnimation = animation;
            m_SourceRectangle = new Rectangle(0, animation.StartingRow, animation.FrameWidth, animation.FrameHeight);
            m_iCurrentFrame = 0;
            m_fTime = 0.0f;
        }

        public void Update(GameTime gameTime)
        {
            if (m_CurrentAnimation == null)
                return;

            m_fTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while(m_fTime > m_CurrentAnimation.FrameTime)
            {
                m_fTime -= m_CurrentAnimation.FrameTime;
                if (m_CurrentAnimation.Looping)
                {
                    m_iCurrentFrame = (m_iCurrentFrame + 1) % m_CurrentAnimation.FrameCount;
                }
                else
                {
                    m_iCurrentFrame = Math.Min(m_iCurrentFrame + 1, m_CurrentAnimation.FrameCount - 1);
                }
            }

            // NOTE: ONLY SUPPORTS SINGLE LINE ANIMATIONS
            m_SourceRectangle = new Rectangle(
                m_iCurrentFrame * m_CurrentAnimation.FrameWidth,
                m_CurrentAnimation.StartingRow * m_CurrentAnimation.FrameHeight,
                m_CurrentAnimation.FrameWidth,
                m_CurrentAnimation.FrameHeight);
        }
        #endregion Public Methods
    }
}
