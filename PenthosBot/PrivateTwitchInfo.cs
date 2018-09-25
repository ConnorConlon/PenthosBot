using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot
{
    public class PrivateTwitchInfo
    {
        #region Private Members
        private Dictionary<string, string> m_TwitchInfo;
        #endregion Private Members

        #region Properties
        public string BotAccessToken
        {
            get { return m_TwitchInfo["BotAccessToken"]; }
        }

        public string BotName
        {
            get { return m_TwitchInfo["BotName"]; }
        }

        public string BotRefreshToken
        {
            get { return m_TwitchInfo["BotRefreshToken"]; }
        }

        public string ChannelName
        {
            get { return m_TwitchInfo["ChannelName"]; }
        }

        public string ClientID
        {
            get { return m_TwitchInfo["ClientID"]; }
        }
        #endregion Properties

        #region Ctor / Init
        public PrivateTwitchInfo()
        {
            m_TwitchInfo = new Dictionary<string, string>();
            string filePath = "TwitchInfo.txt";
            if (System.IO.File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; ++i)
                {
                    if (lines[i] == string.Empty) continue;

                    m_TwitchInfo.Add(lines[i], lines[i+1]);
                    i++;
                }
            }

        }
        #endregion Ctor / Init

        #region Public Methods
        public string GetInfo(string property)
        {
            return m_TwitchInfo[property];
        }
        #endregion Public Methods
    }
}
