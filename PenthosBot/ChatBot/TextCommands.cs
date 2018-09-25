using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot.ChatBot
{
    class TextCommands
    {
        #region Private Members
        Dictionary<string, string> m_textCommands;
        #endregion Private Members

        #region Ctor / Init
        public TextCommands()
        {
            m_textCommands = new Dictionary<string, string>();
        }
        #endregion Ctor / Init

        #region Public Methods
        public void LoadTextCommands(string filePath)
        {    
            if (System.IO.File.Exists(filePath))
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                for(int i = 0; i < lines.Length; ++i)
                {
                    if(i % 2 == 0)
                    {
                        m_textCommands.Add(lines[i], "");
                    }
                    else
                    {
                        m_textCommands[lines[i - 1]] = lines[i];
                    }
                }
            }
        }

        public string GetCommand(string cmd)
        {
            if(m_textCommands.ContainsKey(cmd))
            {
                return m_textCommands[cmd];
            }
            return string.Empty;
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
