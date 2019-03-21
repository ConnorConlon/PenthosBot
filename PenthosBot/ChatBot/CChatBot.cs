using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Users;
using TwitchLib.Api.Models.v5.Channels;
using TwitchLib.Api.Models.v5.Subscriptions;

namespace PenthosBot.ChatBot
{
    public class CChatBot
    {
        #region Private Members
        private PrivateTwitchInfo TwitchInfo;
        private readonly ConnectionCredentials m_credentials;
        private TwitchClient m_client;
        private TwitchAPI API;
        private string m_channelId;

        private PennyManager m_pennyMgr;
        private PennyGambler m_gambler;
        private GiveawayManager m_giveaway;
        private TextCommands m_textCommands;

        private List<BotMessageBase> m_Messages;

        private List<string> m_followers;
        private List<string> m_subs;

        // Stuff to keep track of
        private int m_iCrashCount;
        private int m_iDeaths;

        // Intervals
        TimeSpan m_saveInterval;
        TimeSpan m_lastSave;

        TimeSpan m_twitterInterval;
        TimeSpan m_lastTwitter;

        TimeSpan m_penniesInterval;
        TimeSpan m_lastPennies;
        #endregion Private Members

        #region Ctor / Init
        public CChatBot(PrivateTwitchInfo _TwitchInfo)
        {
            TwitchInfo = _TwitchInfo;

            m_credentials = new ConnectionCredentials(TwitchInfo.BotName, TwitchInfo.BotAccessToken);

            m_Messages = new List<BotMessageBase>();

            m_pennyMgr = new PennyManager();
            m_gambler = new PennyGambler();
            m_giveaway = new GiveawayManager();

            m_textCommands = new TextCommands();

            m_followers = new List<string>();
            m_subs = new List<string>();

            Console.WriteLine("Connecting API...");
            API = new TwitchAPI();
            API.Settings.ClientId = TwitchInfo.ClientID;
            API.Settings.AccessToken = TwitchInfo.BotAccessToken;

            Console.WriteLine("API Connected!");

            m_saveInterval = new TimeSpan(0, 0, 5);
            m_twitterInterval = new TimeSpan(0, 45, 0);
            m_penniesInterval = new TimeSpan(0, 5, 0);

            LoadVariables();
            LoadTextCommands();
        }
        #endregion Ctor / Init

        #region Public Methods
        public TwitchClient Client
        {
            get { return m_client; }
        }

        public BotMessageBase ConsumeBotMessage()
        {
            if(m_Messages.Count > 0)
            {
                BotMessageBase msg = m_Messages[0];
                m_Messages.RemoveAt(0);
                return msg;
            }

            return null;
        }

        public void Connect()
        {
            Console.WriteLine("Connecting...");
            m_client = new TwitchClient();
            m_client.Initialize(m_credentials, TwitchInfo.ChannelName);

            m_client.OnConnected += Client_OnConnect;
            m_client.OnJoinedChannel += OnJoinChannel;
            m_client.OnMessageReceived += OnMessageRecieved;
            m_client.OnChatCommandReceived += OnCmdRecieved;

            m_client.OnLog += Client_OnLog;
            m_client.OnConnectionError += Client_OnConnectionError;

            m_client.Connect();
        }

        public void Disconnect()
        {
            m_client.SendMessage(TwitchInfo.ChannelName, "Later Nerds CoolCat");
            m_client.Disconnect();
            SaveVariables();
        }

        public void Update(TimeSpan elapsedTime)
        {
            m_lastSave += elapsedTime;
            if(m_lastSave > m_saveInterval)
            {
                SaveVariables();
                m_lastSave = new TimeSpan();
            }

            m_lastTwitter += elapsedTime;
            if(m_lastTwitter > m_twitterInterval)
            {
                if(m_client.IsConnected)
                {
                    m_client.SendMessage(TwitchInfo.ChannelName, "Check out my twitter! https://twitter.com/PenthosGaming");
                }
                m_lastTwitter = new TimeSpan();
            }

            m_lastPennies += elapsedTime;
            if(m_lastPennies > m_penniesInterval)
            {
                if(m_client.IsConnected)
                {
                    WebClient jsonSite = new WebClient();
                    string jsonString;
                    jsonString = jsonSite.DownloadString("https://tmi.twitch.tv/group/user/penthosinfinitum/chatters");
                    
                    ChatterData jsonData = JsonConvert.DeserializeObject<ChatterData>(jsonString);
                    if(jsonData != null)
                    {
                        List<string> allChatters = new List<string>();
                        allChatters.AddRange(jsonData.chatters.admins);
                        allChatters.AddRange(jsonData.chatters.global_mods);
                        allChatters.AddRange(jsonData.chatters.moderators);
                        allChatters.AddRange(jsonData.chatters.staff);
                        allChatters.AddRange(jsonData.chatters.viewers);

                        foreach(string chatter in allChatters)
                        {
                            int iPenniesForChatter = 1;
                            if(m_followers.Contains(chatter))
                            {
                                iPenniesForChatter += 2;
                            }

                            if(m_subs.Contains(chatter))
                            {
                                iPenniesForChatter += 3;
                            }

                            m_pennyMgr.AddPennies(chatter, iPenniesForChatter);
                        }
                    }
                }
                m_lastPennies = new TimeSpan();
            }
        }

        public void Death()
        {
            m_iDeaths++;
            String word = m_iDeaths > 1 ? " times" : " time";
            m_client.SendMessage(TwitchInfo.ChannelName, "Penthos has died " + m_iDeaths + word);
            m_gambler.FailedAttempt();
        }

        public void BeatBoss()
        {
            m_client.SendMessage(TwitchInfo.ChannelName, "Penthos beat the boss! FeelsGoodMan");
            m_Messages.Add(new PlayMusicMessage("airhorns", 0.2f, new TimeSpan(0, 2, 32)));

            Dictionary<string, int> attemptWinners = m_gambler.BeatAttempt();
            foreach(KeyValuePair<string, int> winner in attemptWinners)
            {
                m_pennyMgr.AddPennies(winner.Key, winner.Value * 2);
            }
        }

        public void Victory()
        {
            OutputVictoryMessage();
        }

        public void Crash()
        {
            m_iCrashCount++;
            m_client.SendMessage(TwitchInfo.ChannelName, "Sekiro has crashed " + m_iCrashCount.ToString() + " times FeelsBadMan");
            m_Messages.Add(new PlaySoundMessage("YouDied", 0.2f));
        }
        #endregion Public Methods

        #region Private Methods
        private void Client_OnConnect(object sender, OnConnectedArgs args)
        {
            Console.WriteLine("Connected!");

            m_channelId = API.Users.v5.GetUserByNameAsync(TwitchInfo.ChannelName).Result.Matches[0].Id;

            UpdateFollowers();
            UpdateSubs();
        }

        private void Client_OnLog(object sender, OnLogArgs args)
        {
            Console.WriteLine(args.DateTime.ToShortTimeString() + ": " + args.Data.ToString());
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs args)
        {
            Console.WriteLine(args.Error.ToString());
        }

        private void OnJoinChannel(object sender, OnJoinedChannelArgs args)
        {
            Console.WriteLine("PenthosBot Online!");
            m_client.SendMessage(args.Channel, "Hey All! VoHiYo");
        }

        private void OnMessageRecieved(object sender, OnMessageReceivedArgs args)
        {
            
        }

        private void OnCmdRecieved(object sender, OnChatCommandReceivedArgs args)
        {
            Console.WriteLine("Command Recieved!");
            String cmdText = args.Command.CommandText;

            string textResponse = m_textCommands.GetCommand(cmdText);
            if(textResponse != string.Empty)
            {
                m_client.SendMessage(TwitchInfo.ChannelName, textResponse);
            }
            else
            {
                // COMPLEX COMMANDS
                // Misc
                if (String.Compare(cmdText, "uptime", true) == 0)
                {
                    OutputUptime().Wait();
                }
                else if(String.Compare(cmdText, "title", true) == 0)
                {
                    if(DoesUserHaveModPriv(args.Command.ChatMessage))
                    {
                        API.Channels.v5.UpdateChannelAsync(m_channelId, args.Command.ArgumentsAsString);
                    }
                }
                else if (String.Compare(cmdText, "so", true) == 0)
                {
                    if (DoesUserHaveModPriv(args.Command.ChatMessage))
                    {
                        ShoutoutUser(args.Command.ArgumentsAsString);
                    }
                }

                // Pennies
                if (String.Compare(cmdText, "pennies", true) == 0)
                {
                    string username = args.Command.ChatMessage.Username;
                    int iPennies = m_pennyMgr.GetPennies(username);
                    if (iPennies != -1)
                    {
                        m_client.SendWhisper(args.Command.ChatMessage.DisplayName, "You have " + iPennies.ToString() + " Pennies.");
                    }
                    else
                    {
                        m_client.SendWhisper(args.Command.ChatMessage.DisplayName, "You don't have any Pennies yet FeelsBadMan");
                    }
                }

                // Giveaways
                if (String.Compare(cmdText, "entergiveaway", true) == 0)
                {
                    if(m_giveaway.RunningGiveaway)
                    {
                        string username = args.Command.ChatMessage.Username;

                        if (m_pennyMgr.GetPennies(username) > 30)
                        {
                            m_pennyMgr.RemovePennies(username, 30);
                            m_giveaway.EnterGiveaway(username);
                        }
                    }
                }

                if (IsUserBroadcaster(args.Command.ChatMessage))
                {
                    if (String.Compare(cmdText, "startgiveaway", true) == 0)
                    {
                        m_giveaway.RunningGiveaway = true;
                        m_client.SendMessage(TwitchInfo.ChannelName, "GIVEAWAY STARTED! Enter up to 3 times with !entergiveaway. 30 Pennies per entry.");
                    }

                    if (String.Compare(cmdText, "stopGiveAway", true) == 0)
                    {
                        if(m_giveaway.RunningGiveaway)
                        {
                            m_giveaway.RunningGiveaway = false;

                            Dictionary<string, int> refunds = m_giveaway.Participants;
                            foreach (KeyValuePair<string, int> participant in refunds)
                            {
                                for (int i = 0; i < participant.Value; ++i)
                                {
                                    m_pennyMgr.AddPennies(participant.Key, 30);
                                }
                            }

                            m_giveaway.ClearParticipants();
                            m_client.SendMessage(TwitchInfo.ChannelName, "GIVEAWAY CANCELLED! Pennies have been refunded.");
                        }
                    }

                    if (String.Compare(cmdText, "pickwinner", true) == 0)
                    {
                        if(m_giveaway.RunningGiveaway)
                        {
                            string winner = m_giveaway.PickWinner();
                            m_giveaway.RunningGiveaway = false;
                            m_giveaway.ClearParticipants();
                            m_client.SendMessage(TwitchInfo.ChannelName, "CONGRATULATIONS TO " + winner + "! YOU WIN THE GIVEAWAY!");
                        }
                    }
                }

                // Gambling
                if (String.Compare(cmdText, "enableattempts", true) == 0)
                {
                    if (DoesUserHaveModPriv(args.Command.ChatMessage))
                    {
                        m_gambler.AttemptBettingEnabled = true;
                        m_client.SendMessage(TwitchInfo.ChannelName, "Attempt betting has been enabled! Think that this will be the attempt? !gamble attempt <amount>");
                    }
                }

                if (String.Compare(cmdText, "gamble", true) == 0)
                {
                    List<string> gambleArgs = args.Command.ArgumentsAsList;
                    if (gambleArgs.Count == 0)
                    {
                        m_client.SendMessage(TwitchInfo.ChannelName, "MoneyRest Feel like putting your Pennies on the line? " +
                            "Type \"!gamble <game>\" to learn more. Games supported: attempt.");
                    }
                    else
                    {
                        /*
                        if (gambleArgs[0] == "roulette")
                        {
                            if (gambleArgs.Count != 3)
                            {
                                m_client.SendMessage(TwitchInfo.ChannelName, "Format: !gamble roulette <amount> <number or range>. Ex. !gamble roulette 200 row2");
                            }
                            else
                            {
                                string username = args.Command.ChatMessage.Username;
                                int iPennies = m_pennyMgr.GetPennies(username);
                                if (iPennies > 0)
                                {
                                    int iAmount = -1;
                                    int.TryParse(gambleArgs[1], out iAmount);

                                    if (iAmount != -1 && iAmount <= iPennies)
                                    {
                                        m_pennyMgr.RemovePennies(username, iAmount);
                                        string message = m_gambler.Roulette(ref iAmount, gambleArgs[2]);
                                        m_pennyMgr.AddPennies(username, iAmount);
                                        m_client.SendMessage(TwitchInfo.ChannelName, message);
                                    }
                                }
                            }
                        }
                        */
                        if(gambleArgs[0] == "attempt")
                        {
                            if(m_gambler.AttemptBettingEnabled)
                            {
                                if (gambleArgs.Count != 2)
                                {
                                    m_client.SendMessage(TwitchInfo.ChannelName, "Format: !gamble attempt <amount>. Ex. !gamble attempt 200");
                                }
                                else
                                {
                                    string username = args.Command.ChatMessage.Username;
                                    int iPennies = m_pennyMgr.GetPennies(username);
                                    if (iPennies > 0)
                                    {
                                        int iAmount = -1;
                                        int.TryParse(gambleArgs[1], out iAmount);

                                        if (iAmount != -1 && iAmount <= iPennies)
                                        {
                                            m_pennyMgr.RemovePennies(username, iAmount);
                                            m_gambler.AttemptBet(username, iAmount);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OutputVictoryMessage()
        {
            string status;
            if (m_iDeaths > 0)
            {
                string word1 = m_iDeaths > 1 ? " were " : " was ";
                string word2 = m_iDeaths > 1 ? " deaths " : " death ";
                status = " There" + word1 + m_iDeaths.ToString() + word2 + "this run";
            }
            else
            {
                status = " There were ZERO deaths this run";
            }

            string emote;
            if (m_iDeaths > 0)
            {
                if (m_iDeaths >= 1 && m_iDeaths <= 10)
                {
                    emote = " SeemsGood";
                }
                else if (m_iDeaths > 10 && m_iDeaths <= 20)
                {
                    emote = " FeelsBadMan";
                }
                else if (m_iDeaths > 20 && m_iDeaths <= 30)
                {
                    emote = " FailFish";
                }
                else
                {
                    emote = " NotLikeThis";
                }
            }
            else
            {
                emote = " PogChamp";
            }
            status += emote;

            m_client.SendMessage(TwitchInfo.ChannelName, "Penthos beat the run PogChamp" +
                status + " Death counter reset FeelsGoodMan");
            m_iDeaths = 0;
            m_Messages.Add(new PlayMusicMessage("victory", 0.3f, TimeSpan.Zero));
        }

        private async Task OutputUptime()
        {
            User[] users = API.Users.v5.GetUserByNameAsync(TwitchInfo.ChannelName).Result.Matches;
            TimeSpan? uptime = await API.Streams.v5.GetUptimeAsync(users[0].Id);
            if (uptime != null)
            {
                m_client.SendMessage(TwitchInfo.ChannelName, "Uptime: " + uptime.ToString());
            }
            else
            {
                m_client.SendMessage(TwitchInfo.ChannelName, "Channel Offline");
            }
        }

        private void ShoutoutUser(string user)
        {
            User[] users = API.Users.v5.GetUserByNameAsync(user).Result.Matches;
            if(users.Length > 0)
            {
                string userid = users[0].Id;
                string game = API.Channels.v5.GetChannelByIDAsync(userid).Result.Game;
                m_client.SendMessage(TwitchInfo.ChannelName, "imGlitch Go check out " + user + ", an amazing " +
                    game + " streamer at https://www.twitch.tv/" + user + " imGlitch");
            }
        }
        
        private bool DoesUserHaveModPriv(ChatMessage chatMsg)
        {
            return chatMsg.IsModerator || chatMsg.IsBroadcaster;
        }

        private bool IsUserBroadcaster(ChatMessage chatMsg)
        {
            return chatMsg.IsBroadcaster;
        }

        private bool DoesUserHaveSubPriv(ChatMessage chatMsg)
        {
            return chatMsg.IsSubscriber || chatMsg.IsBroadcaster;
        }

        private void UpdateFollowers()
        {
            m_followers.Clear();
            List<ChannelFollow> followers = API.Channels.v5.GetAllFollowersAsync(m_channelId).Result;
            foreach (ChannelFollow cf in followers)
            {
                m_followers.Add(cf.User.Name);
            }
        }

        private void UpdateSubs()
        {
            m_subs.Clear();
            try
            {
                List<Subscription> subs = API.Channels.v5.GetAllSubscribersAsync(m_channelId).Result;
                foreach (Subscription sub in subs)
                {
                    m_subs.Add(sub.User.Name);
                }
            }
            catch
            {

            }
        }

        private void LoadVariables()
        {
            string filePath = @"E:\Connor\Twitch\PennyBot\SaveData\botdata.xml";
            if (System.IO.File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                int.TryParse(doc.DocumentElement.SelectSingleNode("/BotData/deaths").InnerText, out m_iDeaths);
                int.TryParse(doc.DocumentElement.SelectSingleNode("/BotData/crashes").InnerText, out m_iCrashCount);

                m_pennyMgr.Load(doc);
            }
        }

        private void LoadTextCommands()
        {
            string filePath = @"E:\Connor\Twitch\PennyBot\SaveData\textcommands.txt";
            m_textCommands.LoadTextCommands(filePath);
        }

        private void SaveVariables()
        {
            XmlWriterSettings formatting = new XmlWriterSettings();
            formatting.Indent = true;
            XmlWriter writer = XmlWriter.Create(@"E:\Connor\Twitch\PennyBot\SaveData\botdata.xml", formatting);

            writer.WriteStartDocument();

            writer.WriteStartElement("BotData");

            writer.WriteStartElement("deaths");
            writer.WriteString(m_iDeaths.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("crashes");
            writer.WriteString(m_iCrashCount.ToString());
            writer.WriteEndElement();

            m_pennyMgr.Save(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        #endregion Private Methods
    }
}
