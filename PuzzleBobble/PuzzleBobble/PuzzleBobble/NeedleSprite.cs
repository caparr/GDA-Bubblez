﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace PuzzleBobble
{
    class NeedleSprite
    {
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 center { get { return size / 2; } }
        public Vector2 screenSize { get; set; }

        public float rotation { get; set; }
        public float angle { get; set; }
        public NeedleSprite(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, Vector2 newScreenSize)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = newScreenSize;
        }

        public void TiltNeedle(float newRotation)
        {
            float oldRotation = rotation;
            float limit = MathHelper.Pi / 3;
            rotation += newRotation;
            if (rotation > limit || rotation < -limit) // prevents the needle from pointing down
            {
                rotation = oldRotation; // positions the needle at an angle                
            }

            // Need to actually have some sort of direction that corresponds to the movement.
            // this direction will be where the ball will be launched
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 compensatedPosition = position + center;

            //            Vector2 compensatedPosition = new Vector2(screenSize.X / 2 - size.X/2, screenSize.Y - size.Y);
            //graphics.PreferredBackBufferWidth / 2 - needleFrameWidth / 2, graphics.PreferredBackBufferHeight - needleFrameHeight)

            //spriteBatch.Draw(texture, compensatedPosition, null, Color.White, rotation, center, 1.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, compensatedPosition, null, Color.White, rotation, center, 1.5f, SpriteEffects.None, 0.0f);
        }
    }
}
