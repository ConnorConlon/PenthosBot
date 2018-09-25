using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PenthosBot.Graphics;

namespace PenthosBot.Penny
{
    class PennyGraphics
    {
        #region Private Members
        SpriteAnimationPlayer m_SpriteAnimator;
        #endregion Private Members

        #region Ctor / Init
        public PennyGraphics()
        {

        }

        public void Init()
        {

        }
        #endregion Ctor / Init

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            m_SpriteAnimator.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
        }
        #endregion Public Mehtods

        #region Private Methods
        #endregion Private Methods
    }
}
