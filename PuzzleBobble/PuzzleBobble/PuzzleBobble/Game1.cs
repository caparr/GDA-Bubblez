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
        Texture2D needleFrame;  // Frame for the needle texture
        BubbleSprite shoot;     // Bubble that will be sent to shoot        
        NeedleSprite needle;    // The needle that determines the direction for shooting


        // Dimensions
        const float needleWidth = 22f;
        const float needleHeight = 55f;

        const float needleFrameWidth = 74f;
        const float needleFrameHeight = 46f;

        const float bubbleHeight = 22f * 1.0f;
        const float bubbleWidth = 22f * 1.0f;

        int numOfBubbles;
        int numOfColours;
        float bubblePositionAdjustment;
        bool isShot = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 200;

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
            numOfBubbles = 9;
            numOfColours = 1;

            int rowsToClear = 1;
            int halfOfBubbles = numOfBubbles / 2;               //  subtracting one so that the first one is placed in the right position            
            float middle = graphics.PreferredBackBufferWidth / 2;
            float horizontalHalfLength = numOfBubbles * bubbleWidth / 2;
            BubbleSprite.horizontalBoundaries = new Vector2(middle - horizontalHalfLength, middle + horizontalHalfLength);

            if (numOfBubbles % 2 == 1)
            {
                bubblePositionAdjustment = middle - halfOfBubbles * bubbleWidth - bubbleWidth / 2;
            }
            else
            {
                bubblePositionAdjustment = middle - halfOfBubbles * bubbleWidth;
            }

            Random randomNums = new Random();
            for (int yPos = 0; yPos < rowsToClear; yPos++)
            {
                if (yPos % 2 == 1)
                {
                    for (int xPos = 0; xPos < numOfBubbles - 1; xPos++)
                    {
                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + bubbleWidth / 2 + xPos * bubbleWidth, yPos * bubbleHeight), randomNums.Next(numOfColours));//, screenWidth, graphics.PreferredBackBufferHeight);
                        bubbleList.Add(bubble);
                    }
                }
                else
                {
                    for (int xPos = 0; xPos < numOfBubbles; xPos++)
                    {
                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + xPos * bubbleWidth, yPos * bubbleHeight), randomNums.Next(numOfColours));//, screenWidth, graphics.PreferredBackBufferHeight);
                        bubbleList.Add(bubble);
                    }
                }
            }

            CreateShootBubble();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Needle Sprite is 22 x 55
            float xPosNeedle = graphics.PreferredBackBufferWidth / 2 - needleWidth / 2;
            float yPosNeedle = graphics.PreferredBackBufferHeight - needleHeight;

            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            bubbles = Content.Load<Texture2D>("Bubbles");
            needleFrame = Content.Load<Texture2D>("NeedleFrame");
            needle = new NeedleSprite(Content.Load<Texture2D>("Needle"), new Vector2(xPosNeedle, yPosNeedle), new Vector2(needleWidth, needleHeight));
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


            if (isShot && shoot.velocity.Y == 0 && shoot.velocity.X == 0)
            {
                RepositionBubble();
                ExplodeBubble();
                CreateShootBubble();
            }
            else
            {
                foreach (BubbleSprite bs in bubbleList)
                {
                    if (shoot.Collides(bs))
                    {
                        RepositionBubble();
                        ExplodeBubble();
                        test();
                        CreateShootBubble();
                        break;
                    }
                }
            }
            

            

            foreach (BubbleSprite bs in bubbleList)
            {
                bs.Move();
            }

            shoot.Move();
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
                        needle.TiltNeedle(-0.05f);
                        break;
                    case Keys.Right:
                        needle.TiltNeedle(0.05f);
                        break;
                    // Shoot
                    case Keys.Space:
                        // shoot ball                        
                        if (!isShot)
                        {
                            shoot.velocity = new Vector2((float)Math.Cos(needle.rotation + MathHelper.Pi / 2) * -7.5f, (float)Math.Sin(needle.rotation + MathHelper.Pi / 2) * -7.5f);
                            isShot = true;
                        }
                        break;
                    case Keys.Escape:
                        this.Exit();
                        break;
                }
            }
        }

        private void RepositionBubble()
        {
            float xShift;

            float yPos = RepositionYValue();
            float xPos = RepositionXValue(yPos, out xShift);

            if (yPos % 2 == 1)
            {
                shoot.position = new Vector2(xPos * bubbleWidth + bubblePositionAdjustment + xShift, yPos * bubbleHeight);
            }
            else
            {
                shoot.position = new Vector2(xPos * bubbleWidth + bubblePositionAdjustment, yPos * bubbleHeight);
            }

            shoot.velocity = new Vector2(0.0f, 0.0f);
            BubbleSprite attachedShot = shoot;
            bubbleList.Add(attachedShot);

        }

        private float RepositionXValue(float yPos, out float xShift)
        {
            float xPos;
            xShift = bubbleWidth / 2;

            if (yPos % 2 == 1)
                xPos = (shoot.position.X - bubblePositionAdjustment - xShift) / bubbleWidth;
            else
                xPos = (shoot.position.X - bubblePositionAdjustment) / bubbleWidth;

            float xRemainder = xPos % 1;

            if (xPos < 0)
            {
                xPos += 1;
            }

            if (xPos > numOfBubbles - 1)
            {
                xShift = -xShift;
            }

            if (xRemainder >= 0.5)
            {
                xPos = (float)Math.Ceiling(xPos);
            }
            else
            {
                xPos = (float)Math.Floor(xPos);
            }

            return xPos;
        }

        private float RepositionYValue()
        {
            float yPos = shoot.position.Y / bubbleHeight;
            float yRemainder = yPos % 1;

            if (yRemainder >= 0.5)
            {
                yPos = (float)Math.Ceiling(yPos);
            }
            else
            {
                yPos = (float)Math.Floor(yPos);
            }
            return yPos;
        }

        private void ExplodeBubble()
        {
            BubbleSprite[][] bubbleLayout = MapBubbleLayout();
            List<BubbleSprite> bubbleChain = new List<BubbleSprite>();

            CheckAdjacentBubbles(bubbleLayout, shoot, bubbleChain);

            if (bubbleChain.Count > 2)
            {
                foreach (BubbleSprite bs in bubbleChain)
                {
                    bubbleList.Remove(bs);
                }
            }


            //BubbleSprite[] sameColours = LocateColouredBalls().ToArray<BubbleSprite>();
            //// Check for these four vectors:


            //for (int i = 0; i < sameColours.Length; i++)
            //{
            //    sameColours[i].velocity = new Vector2(0.0f, 5.0f);
            //    sameColours[i].isPhysical = false;
            //}


        }


        private void test()
        {
            if (bubbleList.Count > 0)
            {
                BubbleSprite[][] bubbleLayout = MapBubbleLayout();
                List<BubbleSprite> fall = new List<BubbleSprite>();
                for (int i = bubbleLayout.GetLength(0) - 1; i > 0; i--)
                {
                    for (int j = 0; j < bubbleLayout[i].Length; j++)
                    {
                        if (bubbleLayout[i][j] != null)
                        {
                            List<BubbleSprite> bubblePath = new List<BubbleSprite>();
                            bool isHanging = false;
                            DropHangingBubbles(bubbleLayout, bubbleLayout[i][j], bubblePath, ref isHanging);
                            if (!isHanging)
                            {
                                fall.Add(bubbleLayout[i][j]);
                            }
                        }

                    }
                }

                foreach (BubbleSprite bs in fall)
                {
                    bs.velocity = new Vector2(0.0f, 5.0f);
                }
            }
        }

        private void CheckAdjacentBubbles(BubbleSprite[][] bubbleLayout, BubbleSprite bs, List<BubbleSprite> bubbleChain)
        {
            int row = (int)(bs.position.Y / bubbleHeight);
            int col;

            if (row % 2 == 1)
            {
                float adjustedXPos = bs.position.X - bubblePositionAdjustment - (bubbleWidth / 2);
                col = (int)(adjustedXPos / bubbleWidth);
            }
            else
            {
                float adjustedXPos = bs.position.X - bubblePositionAdjustment;
                col = (int)(adjustedXPos / bubbleWidth);
            }

            if (!bubbleChain.Contains(bs))
            {
                bubbleChain.Add(bs);
            }


            //  Checking rules for odd rows
            if (row % 2 == 1)
            {
                //  Check Middle Bubbles
                if (col > 0 && col < (numOfBubbles - 2))
                {
                    //  Check Left

                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col - 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
                        }
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
                            }
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
                            }
                        }
                    }
                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 2))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col - 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
                            }
                        }
                    }
                }

                // not the very bottom, therefore there might be some matching bubbles below
                if ((row + 1) != bubbleLayout.GetLength(0))
                {
                    //  Check Bottom Left
                    if (bubbleLayout[row + 1][col] != null)
                    {
                        if (bubbleLayout[row + 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col]))
                        {
                            bubbleChain.Add(bubbleLayout[row + 1][col]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
                        }
                    }

                    //  Check Bottom Right
                    if (bubbleLayout[row + 1][col + 1] != null)
                    {
                        if (bubbleLayout[row + 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row + 1][col + 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col + 1], bubbleChain);
                        }
                    }
                }
            }


            //  Checking rules for even rows
            else
            {
                //  Check middle balls
                if (col > 0 && col < (numOfBubbles - 1))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col - 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
                        }
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null)
                        {
                            if (bubbleLayout[row - 1][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col - 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col - 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubbleChain);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        //  Check Bottom Left
                        if (bubbleLayout[row + 1][col - 1] != null)
                        {
                            if (bubbleLayout[row + 1][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col - 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row + 1][col - 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubbleChain);
                            }
                        }
                        //  Check Bottom Right
                        if (bubbleLayout[row + 1][col] != null)
                        {
                            if (bubbleLayout[row + 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row + 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
                            }
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        // There is no Bottom Left to check for
                        //  Check Bottom Right
                        if (bubbleLayout[row + 1][col] != null)
                        {
                            if (bubbleLayout[row + 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row + 1][col]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
                            }
                        }
                    }

                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 1))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col - 1]);
                            CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null)
                        {
                            if (bubbleLayout[row - 1][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col - 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col - 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubbleChain);
                            }
                        }
                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        //  There is no Bottom Right to check for
                        //  Check Bottom Left
                        if (bubbleLayout[row + 1][col - 1] != null)
                        {
                            if (bubbleLayout[row + 1][col - 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col - 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row + 1][col - 1]);
                                CheckAdjacentBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubbleChain);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The way this will be implemented is that it will check each bubble: if there's a path where it reaches the top, it is still hanging.
        /// </summary>
        /// <param name="explodedBubbles"></param>
        /// <param name="bubbleLayout"></param>
        /// <param name="bs"></param>
        private void DropHangingBubbles(BubbleSprite[][] bubbleLayout, BubbleSprite bs, List<BubbleSprite> bubblePath, ref bool isHanging)
        {

            int row = (int)(bs.position.Y / bubbleHeight);
            int col;

            if (row % 2 == 1)
            {
                float adjustedXPos = bs.position.X - bubblePositionAdjustment - (bubbleWidth / 2);
                col = (int)(adjustedXPos / bubbleWidth);
            }
            else
            {
                float adjustedXPos = bs.position.X - bubblePositionAdjustment;
                col = (int)(adjustedXPos / bubbleWidth);
            }

            // Made it to the top row
            if (row == 0)
            {
                isHanging = true;
                return;
            }

            //  Checking rules for odd rows
            if (row % 2 == 1)
            {
                //  Check Middle Bubbles
                if (col > 0 && col < (numOfBubbles - 2))
                {
                    //  Check Left

                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col - 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col + 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                        }
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                            }
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col + 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                            }
                        }
                    }
                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 2))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col - 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                            }
                        }
                    }
                }
            }


            //  Checking rules for even rows
            else
            {
                //  Check middle balls
                if (col > 0 && col < (numOfBubbles - 1))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col - 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col + 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                        }
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null)
                        {
                            if (bubbleLayout[row - 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col - 1]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col - 1]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubblePath, ref isHanging);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                            }
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col + 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                            }
                        }
                    }
                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 1))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null)
                    {
                        if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row][col - 1]);
                            DropHangingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                        }
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null)
                        {
                            if (bubbleLayout[row - 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col - 1]))
                            {
                                bubblePath.Add(bubbleLayout[row - 1][col - 1]);
                                DropHangingBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubblePath, ref isHanging);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Find all bubble sprites of the same colour and orders them in by greatest YPos followed by greatest XPos
        /// </summary>
        /// <returns></returns>
        private List<BubbleSprite> LocateColouredBalls()
        {
            IComparer<BubbleSprite> comparer = new ComparingBubbleSprites();

            List<BubbleSprite> sameColour = new List<BubbleSprite>();
            foreach (BubbleSprite bs in bubbleList)
            {
                if (bs.colour == shoot.colour)
                {
                    sameColour.Add(bs);
                }
            }
            sameColour.Sort(comparer);

            return sameColour;
        }


        /// <summary>
        /// Maps the current bubble layout into a 2D array to detect matching bubbles
        /// </summary>
        /// <returns></returns>
        private BubbleSprite[][] MapBubbleLayout()
        {
            int rows = FindNumberOfRows();
            BubbleSprite[][] reconstruction = new BubbleSprite[rows][];
            for (int i = 0; i < rows; i++)
            {
                if (i % 2 == 1)
                {
                    reconstruction[i] = new BubbleSprite[numOfBubbles - 1];
                }
                else
                {
                    reconstruction[i] = new BubbleSprite[numOfBubbles];
                }
            }

            foreach (BubbleSprite bs in bubbleList)
            {
                int row = (int)(bs.position.Y / bubbleHeight);
                int col;

                if (row % 2 == 1)
                {
                    float adjustedXPos = bs.position.X - bubblePositionAdjustment - (bubbleWidth / 2);
                    col = (int)(adjustedXPos / bubbleWidth);
                }
                else
                {
                    float adjustedXPos = bs.position.X - bubblePositionAdjustment;
                    col = (int)(adjustedXPos / bubbleWidth);
                }

                reconstruction[row][col] = bs;
            }

            return reconstruction;

        }



        /// <summary>
        /// Creates a new bubble to shoot
        /// </summary>
        private void CreateShootBubble()
        {
            Random randomNums = new Random();
            shoot = new BubbleSprite(bubbles, new Vector2(graphics.PreferredBackBufferWidth / 2 - bubbleWidth / 2, graphics.PreferredBackBufferHeight - (needleHeight / 2) - bubbleHeight / 2),
                randomNums.Next(numOfColours));//, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            isShot = false;
        }

        private int FindNumberOfRows()
        {
            int numberOfRows;
            float highestBubble = bubbleList.First<BubbleSprite>().position.Y;
            float lowestBubble = bubbleList.First<BubbleSprite>().position.Y;

            foreach (BubbleSprite bs in bubbleList)
            {
                if (bs.position.Y > highestBubble)
                {
                    highestBubble = bs.position.Y;
                }
                if (bs.position.Y < lowestBubble)
                {
                    lowestBubble = bs.position.Y;
                }
            }

            numberOfRows = (int)(highestBubble - lowestBubble) / 22 + 1; // First row is 0, need to add one more into consideration
            return numberOfRows;


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

            // Drawing the bubbles that need to be taken down
            foreach (BubbleSprite node in bubbleList)
            {
                node.Draw(spriteBatch);

            }

            //  Needle Frame            
            spriteBatch.Draw(needleFrame, new Vector2(graphics.PreferredBackBufferWidth / 2 - needleFrameWidth / 2, graphics.PreferredBackBufferHeight - needleFrameHeight), Color.White);

            //  Needle
            needle.Draw(spriteBatch);

            // The ball to be shot
            shoot.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
