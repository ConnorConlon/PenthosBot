using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot
{
    class PrivateTwitchInfo
    {
        #region Private Members
        private Dictionary<string, string> m_TwitchInfo;
        #region Private Members

        #region Ctor / Init
        public PrivateTwitchInfo()
        {
            string filePath = "TwitchInfo.txt";
            if (System.IO.File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; ++i)
                {
                    if (i % 2 == 0)
                    {
                        m_TwitchInfo.Add(lines[i], "");
                    }
                    else
                    {
                        m_TwitchInfo[lines[i - 1]] = lines[i];
                    }
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
