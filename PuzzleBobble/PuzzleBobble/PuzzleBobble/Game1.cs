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

        enum GameState
        {
            MainMenu,
            Level1,
            Level2,
            End,
        }

        GameState currentGameState = GameState.Level1;

        List<BubbleSprite> bubbleList;      // Contains all the bubbles that need to be knocked down
        List<BubbleSprite> droppingBubbles; // Gives the update enough time to drop the balls before making them disappear


        //  Textures
        Texture2D border;
        Texture2D bubbles;      // Contains all the bubble textures
        Texture2D needleFrame;  // Frame for the needle texture
        Texture2D background;   // different backgrounds for different levels
        
        BubbleSprite loadShot;     // Bubble that will be shot
        BubbleSprite nextShot;
        NeedleSprite needle;    // The needle that determines the direction for shooting

        SpriteFont font;
        int score;
        int numberOfShots;
        int ceiling;
        float timer; //10 second timer

        // Dimensions
        const float needleWidth = 22f;
        const float needleHeight = 55f;

        const float needleFrameWidth = 74f * 1.2f;
        const float needleFrameHeight = 46f * 1.2f;

        const float bubbleHeight = 16f * 2.0f;
        const float bubbleWidth = 16f * 2.0f;

        int numOfBubbles;
        int numOfColours;
        float bubblePositionAdjustment;
        bool isShot = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = (int)(12.0f * bubbleHeight + needleHeight);//480; // 11 rows of balls + needle height + 1 ball to pass the line to lose the game

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
            droppingBubbles = new List<BubbleSprite>();

            //  Statistics
            timer = 10000.0f; //10 second timer
            score = 0;
            numberOfShots = 8;
            ceiling = 0;

            //  Bubble Conditions
            numOfBubbles = 8;
            numOfColours = 5;

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

            switch (currentGameState)
            {
                case GameState.Level1:
                    Level1();
                    break;
                case GameState.Level2:
                    Level2();
                    break;
            }

            CreateShootBubble();

        }

        private void Level1()
        {
            numOfColours = 4;       //  Set how many colours will be available in this level
            int rowsToClear = 4;    //  Number of Rows to clear            
            int colour;             //  The colour of the ball that will be generated

            Random randomNums = new Random();
            for (int row = 0; row < rowsToClear; row++)
            {
                colour = row / 2;
                if (row % 2 == 1)
                {
                    for (int col = 0; col < numOfBubbles - 1; col++)
                    {
                        if (col % 2 == 0)
                        {
                            colour = (colour + 1) % numOfColours;
                        }

                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + bubbleWidth / 2 + col * bubbleWidth, row * bubbleHeight), new Vector2(bubbleWidth, bubbleHeight), colour);
                        bubbleList.Add(bubble);
                    }
                }
                else
                {
                    for (int col = 0; col < numOfBubbles; col++)
                    {
                        if (col % 2 == 0)
                        {
                            colour = (colour + 1) % numOfColours;
                        }
                        BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + col * bubbleWidth, row * bubbleHeight), new Vector2(bubbleWidth, bubbleHeight), colour);
                        bubbleList.Add(bubble);
                    }
                }
            }
        }

        private void Level2()
        {
            numOfColours = 7;       //  Set how many colours will be available in this level
            int rowsToClear = 4;    //  Number of Rows to clear            
            int colour;             //  The colour of the ball that will be generated

            Random randomNums = new Random();
            for (int row = 0; row < rowsToClear; row++)
            {
                colour = randomNums.Next(numOfColours);
                if (row % 2 == 1)
                {

                    BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + bubbleWidth / 2 + (numOfBubbles / 2 - 1) * bubbleWidth, row * bubbleHeight), new Vector2(bubbleWidth, bubbleHeight), colour);
                    bubbleList.Add(bubble);

                }
                else
                {
                    if (row == 0)
                    {
                        BubbleSprite extraBubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + (numOfBubbles / 2) * bubbleWidth, row * bubbleHeight), new Vector2(bubbleWidth, bubbleHeight), colour);
                        bubbleList.Add(extraBubble);
                    }

                    BubbleSprite bubble = new BubbleSprite(bubbles, new Vector2(bubblePositionAdjustment + (numOfBubbles / 2 - 1) * bubbleWidth, row * bubbleHeight), new Vector2(bubbleWidth, bubbleHeight), colour);
                    bubbleList.Add(bubble);

                }
            }

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
            background = Content.Load<Texture2D>("level1");
            border = Content.Load<Texture2D>("SideBorder");
            bubbles = Content.Load<Texture2D>("ShinyBubbles16");
            needleFrame = Content.Load<Texture2D>("NeedleFrame");
            needle = new NeedleSprite(Content.Load<Texture2D>("Needle"), new Vector2(xPosNeedle, yPosNeedle), new Vector2(needleWidth, needleHeight), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));            
            font = Content.Load<SpriteFont>("Score");
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

            switch (currentGameState)
            {
                case GameState.Level1:
                    if (bubbleList.Count == 0)
                    {
                        currentGameState = GameState.Level2;
                        background = Content.Load<Texture2D>("level2");
                        Level2();
                    }
                    break;
                case GameState.Level2:
                    if (bubbleList.Count == 0)
                    {
                        currentGameState = GameState.End;
                        isShot = true;  // disabling shooting
                    }
                    break;
            }

            if (timer <= 0.0f)
            {
                Shoot();
            }

            if (isShot)
            {

                if (loadShot.velocity.Y == 0 && loadShot.velocity.X == 0)
                {
                    RepositionBubble();
                    CheckScore();
                    CreateShootBubble();
                }
                else
                {
                    foreach (BubbleSprite bs in bubbleList)
                    {
                        if (loadShot.Collides(bs))
                        {
                            RepositionBubble();
                            CheckScore();
                            CreateShootBubble();
                            break;
                        }
                    }
                }
            }
            else
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            foreach (BubbleSprite bs in bubbleList)
            {
                bs.Move();
            }

            foreach (BubbleSprite bs in droppingBubbles)
            {
                bs.Move();
            }



            loadShot.Move();
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
                        needle.TiltNeedle(-0.028f);
                        break;
                    case Keys.Right:
                        needle.TiltNeedle(0.028f);
                        break;
                    // Shoot
                    case Keys.Up:
                    case Keys.Space:
                        if (!isShot)
                        {
                            Shoot();
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

            float yPos = RepositionRowValue();
            float xPos = RepositionColumnValue(yPos, out xShift);

            if (yPos % 2 == 1)
            {
                loadShot.position = new Vector2(xPos * bubbleWidth + bubblePositionAdjustment + xShift, yPos * bubbleHeight);
            }
            else
            {
                loadShot.position = new Vector2(xPos * bubbleWidth + bubblePositionAdjustment, yPos * bubbleHeight);
            }

            loadShot.velocity = new Vector2(0.0f, 0.0f);
            BubbleSprite attachedShot = loadShot;
            bubbleList.Add(attachedShot);

        }

        private float RepositionRowValue()
        {
            float row = loadShot.position.Y / bubbleHeight;
            float yRemainder = row % 1;

            if (yRemainder >= 0.5)
            {
                row = (float)Math.Ceiling(row);
            }
            else
            {
                row = (float)Math.Floor(row);
            }
            return row;
        }

        private float RepositionColumnValue(float row, out float xShift)
        {
            float col;
            xShift = bubbleWidth / 2;

            if (row % 2 == 1)
                col = (loadShot.position.X - bubblePositionAdjustment - xShift) / bubbleWidth;
            else
                col = (loadShot.position.X - bubblePositionAdjustment) / bubbleWidth;

            float xRemainder = col % 1;

            if (col < 0)
            {
                col += 1;
            }

            if (col > numOfBubbles - 1)
            {
                xShift = -xShift;
            }

            if (xRemainder >= 0.5)
            {
                col = (float)Math.Ceiling(col);
            }
            else
            {
                col = (float)Math.Floor(col);
            }

            return col;
        }

        private void Shoot()
        {
            loadShot.velocity = new Vector2((float)Math.Cos(needle.rotation + MathHelper.Pi / 2) * -10f, (float)Math.Sin(needle.rotation + MathHelper.Pi / 2) * -10f);
            numberOfShots--;
            isShot = true;
            timer = 10000.0f;
        }


        private void CheckScore()
        {
            int poppedBubbles = LocatePoppedBubbles();
            int hangingBubbles = LocateHangingBubbles();

            if (poppedBubbles > 0)
            {
                score += poppedBubbles * 10;
            }

            if (hangingBubbles > 0)
            {
                score += (2 ^ (hangingBubbles) * 10);
            }
        }


        /// <summary>
        /// Checks to see if the shot ball is connected to three or more matching bubbles
        /// </summary>
        private int LocatePoppedBubbles()
        {
            //  When bubbles explode, the score is calculated as followed: Number of exploded balls * 10
            BubbleSprite[][] bubbleLayout = MapBubbleLayout();
            List<BubbleSprite> bubbleChain = new List<BubbleSprite>();

            FindMatchingBubbles(bubbleLayout, loadShot, bubbleChain);

            if (bubbleChain.Count > 2)
            {
                foreach (BubbleSprite bs in bubbleChain)
                {
                    bubbleList.Remove(bs);
                }
                return bubbleChain.Count;
            }

            return 0;
        }


        /// <summary>
        /// Drops all hanging bubbles. Score is calculated as follows:
        /// 2^(Number of Hanging balls) * 10
        /// </summary>
        private int LocateHangingBubbles()
        {
            List<BubbleSprite> hangingBubbles = new List<BubbleSprite>();
            if (bubbleList.Count > 0)
            {
                BubbleSprite[][] bubbleLayout = MapBubbleLayout();
                for (int i = bubbleLayout.GetLength(0) - 1; i > 0; i--)
                {
                    for (int j = 0; j < bubbleLayout[i].Length; j++)
                    {
                        bool isHanging = false;
                        if (bubbleLayout[i][j] != null)
                        {
                            List<BubbleSprite> bubblePath = new List<BubbleSprite>();
                            FindBubblePath(bubbleLayout, bubbleLayout[i][j], bubblePath, ref isHanging);
                            if (!isHanging)
                            {
                                hangingBubbles.Add(bubbleLayout[i][j]);
                            }
                        }

                    }
                }
                if (hangingBubbles.Count > 0)
                {
                    foreach (BubbleSprite bs in hangingBubbles)
                    {
                        bubbleList.Remove(bs);
                        bs.isFalling = true;
                        bs.velocity = new Vector2(0.0f, 10.0f);
                    }
                }
            }

            if (hangingBubbles.Count > 0)
                droppingBubbles.AddRange(hangingBubbles);

            return hangingBubbles.Count;
        }

        private void FindMatchingBubbles(BubbleSprite[][] bubbleLayout, BubbleSprite bs, List<BubbleSprite> bubbleChain)
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
                            }
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null)
                        {
                            if (bubbleLayout[row - 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col + 1]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col + 1]);
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col + 1], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
                        }
                    }

                    //  Check Bottom Right
                    if (bubbleLayout[row + 1][col + 1] != null)
                    {
                        if (bubbleLayout[row + 1][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row + 1][col + 1]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col + 1], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
                        }
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null)
                    {
                        if (bubbleLayout[row][col + 1].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row][col + 1]))
                        {
                            bubbleChain.Add(bubbleLayout[row][col + 1]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubbleChain);
                            }
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null)
                        {
                            if (bubbleLayout[row - 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row - 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row - 1][col]);
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubbleChain);
                            }
                        }
                        //  Check Bottom Right
                        if (bubbleLayout[row + 1][col] != null)
                        {
                            if (bubbleLayout[row + 1][col].colour == bs.colour && !bubbleChain.Contains(bubbleLayout[row + 1][col]))
                            {
                                bubbleChain.Add(bubbleLayout[row + 1][col]);
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col + 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubbleChain);
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
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row][col - 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row - 1][col - 1], bubbleChain);
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
                                FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubbleChain);
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
        private void FindBubblePath(BubbleSprite[][] bubbleLayout, BubbleSprite bs, List<BubbleSprite> bubblePath, ref bool isHanging)
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

                    if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col - 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                    }


                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col + 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                    }


                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col + 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                        }
                    }
                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 2))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col - 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                    }

                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                        }
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col + 1]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col + 1]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col + 1], bubblePath, ref isHanging);
                        }
                    }
                }

                // not the very bottom, therefore there might be some matching bubbles below
                if ((row + 1) != bubbleLayout.GetLength(0))
                {
                    //  Check Bottom Left
                    if (bubbleLayout[row + 1][col] != null && !bubblePath.Contains(bubbleLayout[row + 1][col]))
                    {
                        bubblePath.Add(bubbleLayout[row + 1][col]);
                        FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubblePath);
                    }

                    //  Check Bottom Right
                    if (bubbleLayout[row + 1][col + 1] != null && !bubblePath.Contains(bubbleLayout[row + 1][col + 1]))
                    {
                        bubblePath.Add(bubbleLayout[row + 1][col + 1]);
                        FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col + 1], bubblePath);
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
                    if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col - 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                    }

                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col + 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col - 1]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col - 1], bubblePath, ref isHanging);
                        }

                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                        }
                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        //  Check Bottom Left
                        if (bubbleLayout[row + 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row + 1][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row + 1][col - 1]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubblePath);
                        }

                        //  Check Bottom Right
                        if (bubbleLayout[row + 1][col] != null && !bubblePath.Contains(bubbleLayout[row + 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row + 1][col]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubblePath);
                        }
                    }
                }

                //  Check the leftmost ball
                else if (col == 0)
                {
                    //  Check Right
                    if (bubbleLayout[row][col + 1] != null && !bubblePath.Contains(bubbleLayout[row][col + 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col + 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col + 1], bubblePath, ref isHanging);
                    }


                    if (row != 0)   // the very top row, therefore there is nothing to look at top
                    {
                        //  Check Top Right
                        if (bubbleLayout[row - 1][col] != null && !bubblePath.Contains(bubbleLayout[row - 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col], bubblePath, ref isHanging);
                        }

                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        //  There is no Bottom Left to check for
                        //  Check Bottom Right
                        if (bubbleLayout[row + 1][col] != null && !bubblePath.Contains(bubbleLayout[row + 1][col]))
                        {
                            bubblePath.Add(bubbleLayout[row + 1][col]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col], bubblePath);
                        }

                    }
                }

                //  Check the rightmost ball
                else if (col == (numOfBubbles - 1))
                {
                    //  Check Left
                    if (bubbleLayout[row][col - 1] != null && !bubblePath.Contains(bubbleLayout[row][col - 1]))
                    {
                        bubblePath.Add(bubbleLayout[row][col - 1]);
                        FindBubblePath(bubbleLayout, bubbleLayout[row][col - 1], bubblePath, ref isHanging);
                    }

                    // the very top row, therefore there is nothing to look at top
                    if (row != 0)
                    {
                        //  Check Top Left
                        if (bubbleLayout[row - 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row - 1][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row - 1][col - 1]);
                            FindBubblePath(bubbleLayout, bubbleLayout[row - 1][col - 1], bubblePath, ref isHanging);
                        }

                    }

                    // not the very bottom, therefore there might be some matching bubbles below
                    if ((row + 1) != bubbleLayout.GetLength(0))
                    {
                        //  There is no Bottom Right to check for
                        //  Check Bottom Left
                        if (bubbleLayout[row + 1][col - 1] != null && !bubblePath.Contains(bubbleLayout[row + 1][col - 1]))
                        {
                            bubblePath.Add(bubbleLayout[row + 1][col - 1]);
                            FindMatchingBubbles(bubbleLayout, bubbleLayout[row + 1][col - 1], bubblePath);
                        }

                    }
                }
            }
        }


        ///// <summary>
        ///// Find all bubble sprites of the same colour and orders them in by greatest YPos followed by greatest XPos
        ///// </summary>
        ///// <returns></returns>
        //private List<BubbleSprite> LocateColouredBalls()
        //{
        //    IComparer<BubbleSprite> comparer = new ComparingBubbleSprites();

        //    List<BubbleSprite> sameColour = new List<BubbleSprite>();
        //    foreach (BubbleSprite bs in bubbleList)
        //    {
        //        if (bs.colour == loadShot.colour)
        //        {
        //            sameColour.Add(bs);
        //        }
        //    }
        //    sameColour.Sort(comparer);

        //    return sameColour;
        //}


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
            loadShot = new BubbleSprite(bubbles, new Vector2(graphics.PreferredBackBufferWidth / 2 - bubbleWidth / 2, graphics.PreferredBackBufferHeight - (needleHeight / 2) - bubbleHeight / 2),
               new Vector2(bubbleWidth, bubbleHeight), randomNums.Next(numOfColours));//, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);            
            isShot = false;
        }

        private int FindNumberOfRows()
        {
            int numberOfRows;
            float highestBubble = bubbleList.First<BubbleSprite>().position.Y;
            //float lowestBubble = bubbleList.First<BubbleSprite>().position.Y;

            foreach (BubbleSprite bs in bubbleList)
            {
                if (bs.position.Y > highestBubble)
                {
                    highestBubble = bs.position.Y;
                }
            }

            numberOfRows = (int)(highestBubble - ceiling * bubbleHeight) / 22 + 1; // ceiling drops down every 8 bubble shot, so that needs to be considered for the number of rows
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

            //  Drawing background depending on level
            Rectangle backgroundRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            switch (currentGameState)
            {
                case GameState.Level1:  //  http://www.dreamstime.com/abstract-geometric-background-imagefree3425491
                    spriteBatch.Draw(background, backgroundRectangle, Color.White * 0.5f);
                    break;
                case GameState.Level2:
                    spriteBatch.Draw(background, backgroundRectangle, Color.White * 0.5f);
                    break;
                case GameState.End:
                    spriteBatch.DrawString(font, "You beat the game!\n Please press ESC to exit", new Vector2(100, 100), Color.Black);
                    break;

            }
            
            //TODO
            //  Draw the borders
            Rectangle leftBorderRectangle = new Rectangle((int)(BubbleSprite.horizontalBoundaries.X - 10), 0, 10, graphics.PreferredBackBufferHeight);
            spriteBatch.Draw(border, leftBorderRectangle, Color.White);

            Rectangle rightBorderRectangle = new Rectangle((int)(BubbleSprite.horizontalBoundaries.Y), 0, 10, graphics.PreferredBackBufferHeight);
            spriteBatch.Draw(border, rightBorderRectangle, Color.White);
            //spriteBatch.Draw()
            //  Draw the boundary line

            //  Either make the font bold and white so the score is legible or... I don't know


            // Drawing the bubbles that need to be taken down
            foreach (BubbleSprite node in bubbleList)
            {
                node.Draw(spriteBatch);
            }

            //  Drawing the falling balls, they'll be removed once they pass the screen params
            foreach (BubbleSprite bs in droppingBubbles)
            {
                bs.Draw(spriteBatch);
            }

            //  Needle Frame            
            //spriteBatch.Draw(needleFrame, new Vector2(graphics.PreferredBackBufferWidth / 2 - needleFrameWidth / 2, graphics.PreferredBackBufferHeight - needleFrameHeight), Color.White);
            spriteBatch.Draw(needleFrame, new Vector2(graphics.PreferredBackBufferWidth / 2 - needleFrameWidth / 2, graphics.PreferredBackBufferHeight - needleFrameHeight), null, Color.White, 0f, new Vector2(0f, 0f), 1.2f, SpriteEffects.None, 0.0f);

            //  Needle
            needle.Draw(spriteBatch);

            // The ball to be shot
            loadShot.Draw(spriteBatch);

            //  Drawing the score
            spriteBatch.DrawString(font, String.Format("Score: {0}", score), new Vector2(10, 10), Color.Black);

            //  Drawing the time left for the next autoshot
            spriteBatch.DrawString(font, String.Format("{0:0.0}", timer / 1000.0f), new Vector2(10, 30), Color.Black);

            //  Drawing the number of shots left until ceiling falls
            spriteBatch.DrawString(font, String.Format("Shots Left: {0}", numberOfShots), new Vector2(10, 50), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
