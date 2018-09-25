using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PenthosBot.Penny
{
    class Penny
    {
        #region Private Members
        PennyGraphics m_Graphics;
        #endregion Private Members

        #region Ctor / Init
        public Penny()
        {
            m_Graphics = new PennyGraphics();
        }

        public void Init()
        {

        }
        #endregion Ctor / Init

        #region Public Methods
        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            m_Graphics.Draw(spriteBatch);
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
