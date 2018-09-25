using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenthosBot.ChatBot
{
    public class ChatterData
    {
        public _Links _links { get; set; }
        public int chatter_count { get; set; }
        public Chatters chatters { get; set; }
    }

    public class _Links
    {
    }

    public class Chatters
    {
        public string[] moderators { get; set; }
        public string[] staff { get; set; }
        public string[] admins { get; set; }
        public string[] global_mods { get; set; }
        public string[] viewers { get; set; }
    }


    class PennyManager
    {
        private Dictionary<string, int> PenthosPennies { get; }

        public PennyManager()
        {
            PenthosPennies = new Dictionary<string, int>();
        }

        public int GetPennies(string username)
        {
            if (PenthosPennies.ContainsKey(username))
            {
                return PenthosPennies[username];
            }

            return -1;
        }

        public void AddPennies(string user, int iAmount)
        {
            int iPennies = GetPennies(user);
            if (iPennies != -1)
            {
                PenthosPennies[user] += iAmount;
            }
            else
            {
                PenthosPennies.Add(user, iAmount);
            }
        }

        public void RemovePennies(string user, int iAmount)
        {
            int iPennies = GetPennies(user);
            if (iPennies != -1)
            {
                PenthosPennies[user] -= iAmount;
            }
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("penthospennies");
            foreach (KeyValuePair<string, int> userPennies in PenthosPennies)
            {
                writer.WriteStartElement("userPennies");
                writer.WriteAttributeString("name", userPennies.Key);
                writer.WriteAttributeString("pennies", userPennies.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void Load(XmlDocument doc)
        {
            XmlNode userPennies = doc.DocumentElement.SelectSingleNode("/BotData/penthospennies");
            foreach (XmlNode child in userPennies.ChildNodes)
            {
                int iPennies = 0;
                int.TryParse(child.Attributes["pennies"].Value, out iPennies);
                PenthosPennies.Add(child.Attributes["name"].Value, iPennies);
            }
        }
    }
}
