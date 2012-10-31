using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PuzzleBobble
{
    class ExplodingBubbleSprite
    {
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 size { get; set; }
        public int colour { get; set; }
        public bool isFalling { get; set; }
        public int currentFrame { get; set; }
        Rectangle sourceRect;

        public ExplodingBubbleSprite(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int newColour)//, float leftBoundary, float rightBoundary)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            colour = newColour;
            
            isFalling = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sourceRect = new Rectangle((int)(currentFrame * size.X), colour * 32, 32, 32); // Will always be 32 since that's the dimension in the spritesheet
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0f, new Vector2(0f, 0f), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
