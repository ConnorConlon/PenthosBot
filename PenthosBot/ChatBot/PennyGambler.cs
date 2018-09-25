using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot.ChatBot
{
    public class PennyGambler
    {
        Dictionary<string, int> m_attemptBetters;
        bool m_bAttemptsEnabled;

        public PennyGambler()
        {
            m_attemptBetters = new Dictionary<string, int>();
            m_bAttemptsEnabled = false;
        }

        internal Dictionary<string, int> BeatAttempt()
        {
            m_bAttemptsEnabled = false;
            return m_attemptBetters;
        }

        internal void FailedAttempt()
        {
            m_attemptBetters.Clear();
        }

        internal void EnableAttempBetting()
        {
            m_bAttemptsEnabled = true;
        }

        internal void AttemptBet(string name, int iAmount)
        {
            if(m_bAttemptsEnabled)
            {
                m_attemptBetters.Add(name, iAmount);
            }
        }

        internal string Roulette(ref int iAmount, string bet)
        {
            Random rouletteWheel = new Random(System.DateTime.Now.Millisecond);
            int iWinner = rouletteWheel.Next(0, 36); // 0-36
            int iPayout = 0;
            int iBet;
            if(!int.TryParse(bet, out iBet))
            {
                iBet = -1;
            }

            // Odd/Even
            if(String.Compare(bet, "even", true) == 0)
            {
                if(iWinner % 2 == 0 && iWinner != 0)
                {
                    iPayout = iAmount;
                }
            }
            if(String.Compare(bet, "odd", true) == 0)
            {
                if(iWinner % 2 != 0 && iWinner != 0)
                {
                    iPayout = iAmount;
                }
            }

            // Number
            if(iBet != -1)
            {
                if(iBet == iWinner)
                {
                    iPayout = iAmount * 34;
                }
            }

            // Halfs
            if(String.Compare(bet, "1to18", true) == 0)
            {
                if(iWinner >= 1 && iWinner <= 18)
                {
                    iPayout = iAmount;
                }
            }
            if(String.Compare(bet, "19to36", true) == 0)
            {
                if(iWinner >= 19 && iWinner <= 36)
                {
                    iPayout = iAmount;
                }
            }

            // Red/Black
            if (String.Compare(bet, "red", true) == 0)
            {
                int[] reds = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36};
                if(reds.Contains(iWinner))
                {
                    iPayout = iAmount;
                }
            }
            if (String.Compare(bet, "black", true) == 0)
            {
                int[] blacks = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35};
                if (blacks.Contains(iWinner))
                {
                    iPayout = iAmount;
                }
            }

            // Cols
            if (String.Compare(bet, "1to12", true) == 0)
            {
                if(iWinner >= 1 && iWinner <= 12)
                {
                    iPayout = iAmount * 2;
                }
            }
            if (String.Compare(bet, "13to24", true) == 0)
            {
                if (iWinner >= 13 && iWinner <= 24)
                {
                    iPayout = iAmount * 2;
                }
            }
            if (String.Compare(bet, "25to36", true) == 0)
            {
                if (iWinner >= 25 && iWinner <= 36)
                {
                    iPayout = iAmount * 2;
                }
            }

            // Rows
            if (String.Compare(bet, "row1", true) == 0)
            {
                int[] row1 = { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 };
                if(row1.Contains(iWinner))
                {
                    iPayout = iAmount * 2;
                }
            }
            if (String.Compare(bet, "row2", true) == 0)
            {
                int[] row2 = { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 };
                if (row2.Contains(iWinner))
                {
                    iPayout = iAmount * 2;
                }
            }
            if (String.Compare(bet, "row3", true) == 0)
            {
                int[] row3 = { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
                if (row3.Contains(iWinner))
                {
                    iPayout = iAmount * 2;
                }
            }


            if (iPayout > 0)
            {
                iAmount += iPayout;
            }
            else
            {
                iAmount = 0 - iAmount;
            }

            string result = iPayout > 0 ? " WIN " : " LOSE ";
            string emote = iPayout > 0 ? " FeelsGoodMan " : " NotLikeThis ";
            string payout = iPayout > 0 ? iPayout.ToString() : Math.Abs(iAmount).ToString();
            return "Winning Number: " + iWinner + ". You" + result + " " + payout + " Pennies " + emote;
        }
    }
}
