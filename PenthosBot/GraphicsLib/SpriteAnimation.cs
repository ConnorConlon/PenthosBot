using System;

namespace PenthosBot.Graphics
{
    public class SpriteAnimation
    {
        #region Private Members
        private string m_assetname;
        private int m_iFrameWidth;
        private int m_iFrameHeight;
        private int m_iFrameCount;
        private int m_iStartingRow;
        private float m_fFrameTime;
        private bool m_bLooping;
        private bool m_bIsSet;
        #endregion Private Members

        #region Properties
        public string AssetName
        {
            get { return m_assetname; }
        }

        public int FrameWidth
        {
            get { return m_iFrameWidth; }
        }

        public int FrameHeight
        {
            get { return m_iFrameHeight; }
        }

        public int FrameCount
        {
            get { return m_iFrameCount; }
        }

        public int StartingRow
        {
            get { return m_iStartingRow; }
        }

        public float FrameTime
        {
            get { return m_fFrameTime; }
        }

        public bool Looping
        {
            get { return m_bLooping; }
        }

        public bool IsSet
        {
            get { return m_bIsSet; }
        }
        #endregion Properties

        #region Ctor / Init
        public SpriteAnimation()
        {
            ResetAnimationData();
        }
        #endregion Ctor / Init

        #region Public Methods
        public void Set(string assetName, int iWidth, int iHeight, int iCount, int iStartingRow, float fFrameTime, bool bLooping)
        {
            m_assetname = assetName;
            m_iFrameWidth = iWidth;
            m_iFrameHeight = iHeight;
            m_iFrameCount = iCount;
            m_iStartingRow = iStartingRow;
            m_fFrameTime = fFrameTime;
            m_bLooping = bLooping;
            m_bIsSet = true;
        }

        public void Destroy()
        {
            ResetAnimationData();
        }
        #endregion Public Methods

        #region Private Methods
        private void ResetAnimationData()
        {
            m_assetname = string.Empty;
            m_iFrameWidth = 0;
            m_iFrameHeight = 0;
            m_iFrameCount = 0;
            m_iStartingRow = 0;
            m_fFrameTime = 0.0f;
            m_bLooping = false;
            m_bIsSet = false;
        }
        #endregion Private Methods
    }
}
