/**
 * This game was developed under academic purposes. No copyright infringement is intended.
 * No one may use this project without permission. That is all.
 * Sprites taken from http://spritedatabase.net/file/7313.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PuzzleBobble
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;     // necessary default stuff
        SpriteBatch spriteBatch;


        List<BubbleSprite> bubbleList;      // Contains all the bubbles that need to be knocked down
        

        Texture2D bubbles;      // Contains all the bubble textures
        BubbleSprite shoot;     // Bubble that will be sent to shoot
        NeedleSprite needle;    // The needle that determins the direction for shooting


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 200;
            graphics.PreferredBackBufferHeight = 600;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);
            bubbleList = new List<BubbleSprite>();

            int rowsToClear = 4;
            int numOfBubbles = 9;
            Random randomNums = new Random();
            for (int yPos = 0; yPos < rowsToClear; yPos++)
            {
                if (yPos % 2 == 1)
                {
                    for (int xPos = 0; xPos < numOfBubbles - 1; xPos++)
                    {
                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(11f + xPos * 22f, yPos * 22f), randomNums.Next(7));
                        bubbleList.Add(bubble);
                    }
                }
                else
                {
                    for (int xPos = 0; xPos < numOfBubbles; xPos++)
                    {
                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(xPos * 22f, yPos * 22f), randomNums.Next(7));
                        bubbleList.Add(bubble);
                    }
                }
            }

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            bubbles = Content.Load<Texture2D>("Bubbles");
            needle = new NeedleSprite(Content.Load<Texture2D>("Needle"), new Vector2(graphics.PreferredBackBufferWidth / 2 - 11, graphics.PreferredBackBufferHeight - 55), new Vector2(22f, 55f));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //bubble.texture.Dispose();
            //spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            // TODO: Add your update logic here
            KeyboardActions();

            base.Update(gameTime);
        }


        private void KeyboardActions()
        {
            KeyboardState ks = Keyboard.GetState();
            Keys[] keys = ks.GetPressedKeys();
            foreach (Keys key in keys)
            {
                switch (key)
                {
                    // Arrow Keys
                    case Keys.Left:
                        needle.TiltNeedle(-0.04f);
                        break;
                    case Keys.Right:
                        needle.TiltNeedle(0.04f);
                        break;
                    // Shoot
                    case Keys.Space:
                        // shoot ball
                        break;
                    case Keys.Escape:
                        this.Exit();
                        break;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (BubbleSprite node in bubbleList)
            {
                node.Draw(spriteBatch);
            }
            needle.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
