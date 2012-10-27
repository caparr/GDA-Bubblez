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
    class BubbleSprite
    {
        public static Vector2 horizontalBoundaries { get; set; } // X is left boundary, Y is right boundary

        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 size { get; set; }        
        public int colour { get; set; }
        public bool isFalling { get; set; }
        public int currentFrame { get; set; }
        Rectangle sourceRect;

        public BubbleSprite (Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int newColour)//, float leftBoundary, float rightBoundary)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            colour = newColour;
            isFalling = false;
        }

        public bool Collides(BubbleSprite otherSprite)
        {
            // check if two sprites intersect
            if (this.position.X + this.size.X > otherSprite.position.X &&
                    this.position.X < otherSprite.position.X + otherSprite.size.X &&
                    this.position.Y + this.size.Y > otherSprite.position.Y &&
                    this.position.Y < otherSprite.position.Y + otherSprite.size.Y &&
                    !otherSprite.isFalling)
                return true;
            else
                return false;
        }

        public void Move()
        {            
            //  checking right boundary
            if (position.X + size.X + velocity.X > horizontalBoundaries.Y)
                velocity = new Vector2(-velocity.X, velocity.Y);            
            //  checking left boundary
            if (position.X + velocity.X < horizontalBoundaries.X)
                velocity = new Vector2(-velocity.X, velocity.Y);
            //  checking top boundary
            if (position.Y + velocity.Y < Game1.ceiling * size.Y)
                velocity = new Vector2(0.0f, 0.0f);
            
            position += velocity;
        }

        // To make something invisible, multiple Color.White by 0.0f
        public void Draw (SpriteBatch spriteBatch)
        {            
            sourceRect = new Rectangle((int)(currentFrame*size.X), colour * 16, 16, 16); // Will always be 16 since that's the dimension in the spritesheet                        
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0f, new Vector2(0f, 0f), 2.0f, SpriteEffects.None, 0.0f);
        }
    }
}
