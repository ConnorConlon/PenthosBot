using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using PenthosBot.ChatBot;
using PenthosBot.Effects;

namespace PenthosBot
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        PrivateTwitchInfo TwitchInfo;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CChatBot m_ChatBot;
        KeyboardState m_previousKeyboardState;

        public Game1()
        {
            FileStream filestream = new FileStream("log.txt", FileMode.Create);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            TwitchInfo = new PrivateTwitchInfo();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            IntPtr hWnd = this.Window.Handle;
            Control control = System.Windows.Forms.Control.FromHandle(hWnd);
            Form form = control.FindForm();
            //form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Location = new System.Drawing.Point((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) - (graphics.PreferredBackBufferWidth / 2), (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - (graphics.PreferredBackBufferHeight / 2));

            m_ChatBot = new CChatBot(TwitchInfo);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_ChatBot.Connect();
            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs e)
        {
            m_ChatBot.Disconnect();
            base.OnExiting(sender, e);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ProcessInput();
            
            m_ChatBot.Update(gameTime.ElapsedGameTime);

            BotMessageBase msg = m_ChatBot.ConsumeBotMessage();
            if(msg != null)
            {
                msg.Setup(Content);
                msg.Execute();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            base.Draw(gameTime);
        }

        private void ProcessInput()
        {
            if(!this.IsActive)
            {
                return;
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D))
            {
                m_ChatBot.Death();
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.B))
            {
                m_ChatBot.BeatBoss();
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
            {
                m_ChatBot.Victory();
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
            {
                m_ChatBot.Crash();
            }

            if(IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.S))
            {
                MediaPlayer.Stop();
            }

            m_previousKeyboardState = Keyboard.GetState();
        }

        private bool IsKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            return Keyboard.GetState().IsKeyUp(key) && m_previousKeyboardState.IsKeyDown(key);
        }
    }
}
