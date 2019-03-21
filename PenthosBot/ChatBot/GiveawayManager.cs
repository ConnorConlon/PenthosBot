using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot.ChatBot
{
    public class GiveawayManager
    {
        Dictionary<string, int> m_participants;
        bool m_bRunningGiveaway;

        public GiveawayManager()
        {
            m_participants = new Dictionary<string, int>();
            m_bRunningGiveaway = false;
        }

        public bool RunningGiveaway
        {
            get { return m_bRunningGiveaway; }
            set { m_bRunningGiveaway = value; }
        }

        public Dictionary<string, int> Participants
        {
            get { return m_participants; }
        }

        public bool DoesPlayerHaveMaxEntries(string participantName)
        {
            bool bResult = false;

            if(m_participants.ContainsKey(participantName))
            {
                bResult = m_participants[participantName] < 3;
            }

            return bResult;
        }

        public void EnterGiveaway(string participantName)
        {
            if(m_participants.ContainsKey(participantName))
            {
                m_participants[participantName]++;
            }
            else
            {
                m_participants.Add(participantName, 1);
            }
        }

        public void ClearParticipants()
        {
            m_participants.Clear();
        }

        public string PickWinner()
        {
            string winnerName = "INVALID";
            List<string> drawingPool = new List<string>();

            foreach(KeyValuePair<string, int> participant in m_participants)
            {
                for(int i = 0; i < participant.Value; ++i)
                {
                    drawingPool.Add(participant.Key);
                }
            }

            Random RNGesus = new Random(System.DateTime.Now.Millisecond);
            int winningIndex = RNGesus.Next(0, drawingPool.Count - 1);

            winnerName = drawingPool[winningIndex];

            return winnerName;
        }
    }
}
