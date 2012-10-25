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
        public bool isHanging { get; set; }

        Rectangle sourceRect;
        Vector2 origin;

        public BubbleSprite (Texture2D newTexture, Vector2 newPosition, int newColour)//, float leftBoundary, float rightBoundary)
        {
            texture = newTexture;
            position = newPosition;
            colour = newColour;
            size = new Vector2(22.0f, 22.0f);
            //horizontalBoundaries = new Vector2(leftBoundary, rightBoundary);
            isHanging = true;
        }

        public bool Collides(BubbleSprite otherSprite)
        {
            // check if two sprites intersect
            if (this.position.X + this.size.X > otherSprite.position.X &&
                    this.position.X < otherSprite.position.X + otherSprite.size.X &&
                    this.position.Y + this.size.Y > otherSprite.position.Y &&
                    this.position.Y < otherSprite.position.Y + otherSprite.size.Y)
                return true;
            else
                return false;
        }

        public void Move()
        {            
            //  checking right boundary
            if (position.X + size.X + velocity.X > horizontalBoundaries.Y)
                velocity = new Vector2(-velocity.X, velocity.Y);
            //  checking bottom boundary
            //  TODO
            //if (position.Y + size.Y + velocity.Y > horizontalBoundaries.Y)
            //    velocity = new Vector2(velocity.X, -velocity.Y); // need to add some sort of invisible condition when it reaches here
            //  checking left boundary
            if (position.X + velocity.X < horizontalBoundaries.X)
                velocity = new Vector2(-velocity.X, velocity.Y);
            //  checking top boundary
            if (position.Y + velocity.Y < 0)
                velocity = new Vector2(0.0f, 0.0f);
            
            position += velocity;
        }

        // To make something invisible, multiple Color.White by 0.0f
        public void Draw (SpriteBatch spriteBatch)
        {            
            sourceRect = new Rectangle(0, colour * (int)size.X, (int)size.X, (int)size.Y);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            spriteBatch.Draw(texture, position, sourceRect, Color.White);//texture, position, sourceRect, Color.White, 0f, origin, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, position, sourceRect, Color.White, 0f, new Vector2(size.X / 2, size.Y / 2), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
