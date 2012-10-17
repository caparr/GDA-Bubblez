using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PuzzleBobble
{
    class BubbleSprite
    {
        public const int bubbleWidthAndHeight = 22;

        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public int colour { get; set; }        

        Rectangle sourceRect;
        Vector2 origin;

        public BubbleSprite (Texture2D newTexture, Vector2 newPosition, int newRow)
        {
            texture = newTexture;
            position = newPosition;
            colour = newRow;            
        }

        public void Draw (SpriteBatch spriteBatch)
        {            
            sourceRect = new Rectangle(0, colour * bubbleWidthAndHeight, bubbleWidthAndHeight, bubbleWidthAndHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            spriteBatch.Draw(texture, position, sourceRect, Color.White);//texture, position, sourceRect, Color.White, 0f, origin, SpriteEffects.None, 0f);
        }
    }
}
