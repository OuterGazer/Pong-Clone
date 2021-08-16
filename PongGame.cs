/*My version of a pong clone. Players vs. Player. They can enter their names and play to the best of 5 scores
 Game can be paused and it loops for a new game every time someone wins
Version 1.5: fixed a bug where when the ball would hit the top or the bottom of a paddle it would hit the paddle multiple times
             and show a weird, unintended behaviour
Created by OuterGazer*/

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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PongGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PongGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Allows for input from the keyboard for pausing the game and releasing the ball in service state
        KeyboardState keyboard, lastKeyPress;

        //Allows for entering names from the player. Class finds itself at the bottom of this file
        KeyboardInput enterPlayerName = new KeyboardInput(); 

        // Game Data

        /// <summary>
        /// The different states of the game.
        /// </summary>
        enum GameState
        {
            MainMenu,
            EnterNamePlayer1,
            EnterNamePlayer2,
            Service,
            Playing,
            AfterScore,
            Paused,
            End
        }
        GameState state;

        enum BallHitLast
        {
            RightPlayer,
            LeftPlayer
        }
        BallHitLast whoHitLast;

        /// <summary>
        /// To keep track of which player scored the last to give the ball when resuming the game
        /// </summary>
        enum LastScore
        {
            LeftPlayer,
            RightPlayer
        }
        LastScore whoScored;

        //Message Font
        SpriteFont messageFont;
        SpriteFont instructionsFont;
        
        // Textures for the ball, paddles and background
        Texture2D ballTexture;
        Texture2D lPaddleTexture;
        Texture2D rPaddleTexture;
        Texture2D backgroundTexture;

        // Rectangles for the ball, paddles and background
        Rectangle rPaddleRectangle;
        Rectangle lPaddleRectangle;
        Rectangle ballRectangle;
        Rectangle backgroundRectangle;

        // Speed of the ball
        float ballXSpeed;
        float ballYSpeed;

        //Speed of the paddles
        int rPaddleSpeed;
        int lPaddleSpeed;

        //amount of times the ball has bounced against a paddle
        int ballBounce;

        //Score of the players
        int leftScore;
        int rightScore;

        // Distance of the paddle from the edge of the screen
        int margin;

        //names of the players
        string player_1 = "";
        string player_2 = "";

        public PongGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
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
        }

        /// <summary>
        /// After the end of a game, this method triggers if the players want to pla again.
        /// sets all values and positions to standard.
        /// </summary>
        protected void ResetGame()
        {
            //Set the game to the playing state
            this.state = GameState.Service;

            //Sets the score to zero
            this.leftScore = 0;
            this.rightScore = 0;

            this.StandardPlacement();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Sets the game first into the main menu
            this.state = GameState.MainMenu;

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.messageFont = Content.Load<SpriteFont>("MessageFont");
            this.instructionsFont = Content.Load<SpriteFont>("InstructionsFont");
            this.ballTexture = Content.Load<Texture2D>("ball");
            this.lPaddleTexture = Content.Load<Texture2D>("lpaddle");
            this.rPaddleTexture = Content.Load<Texture2D>("rpaddle");
            this.backgroundTexture = Content.Load<Texture2D>("SpaceBackground");

            this.margin = Window.ClientBounds.Width / 100;            

            this.lPaddleRectangle = new Rectangle(
              0, 0,
              Window.ClientBounds.Width / 30,
              Window.ClientBounds.Height / 6);

            this.rPaddleRectangle = new Rectangle(
              0, 0,
              Window.ClientBounds.Width / 30,
              Window.ClientBounds.Height / 6);

            this.ballRectangle = new Rectangle(
              0, 0,
              Window.ClientBounds.Width / 30,
              Window.ClientBounds.Width / 30);

            this.backgroundRectangle = new Rectangle(
              0, 0, //top left hand corner
              Window.ClientBounds.Width,
              Window.ClientBounds.Height); //size of the client screen display
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void StandardPlacement()
        {
            this.ballBounce = 0;

            this.ballXSpeed = 5.0f;
            this.ballYSpeed = 5.0f;

            this.lPaddleSpeed = 5;
            this.rPaddleSpeed = 5;

            this.lPaddleRectangle.Location = new Point(
              this.margin,
              Window.ClientBounds.Height / 2 - Window.ClientBounds.Height / 16);

            this.rPaddleRectangle.Location = new Point(
              Window.ClientBounds.Width - lPaddleRectangle.Width - this.margin,
              Window.ClientBounds.Height / 2 - Window.ClientBounds.Height / 16);

            switch (this.whoScored)
            {
                case LastScore.RightPlayer:
                    this.ballRectangle.Location = new Point(
                        this.margin + this.lPaddleRectangle.Width,
                        Window.ClientBounds.Height / 2 - this.ballRectangle.Height / 2);
                    //this.whoHitLast = BallHitLast.LeftPlayer;
                    break;

                case LastScore.LeftPlayer:
                    this.ballRectangle.Location = new Point(
                        Window.ClientBounds.Width - this.rPaddleRectangle.Width - this.margin - this.ballRectangle.Width,
                        Window.ClientBounds.Height / 2 - this.ballRectangle.Height / 2);
                    this.whoHitLast = BallHitLast.LeftPlayer;
                    break;
            }
            
        }

        /// <summary>
        /// Standard movement for paddles
        /// </summary>
        protected void PaddleMovement()
        {
            //Movement for left paddle
            if (this.keyboard.IsKeyDown(Keys.W))
                this.lPaddleRectangle.Y -= this.lPaddleSpeed;
            if (this.keyboard.IsKeyDown(Keys.S))
                this.lPaddleRectangle.Y += this.lPaddleSpeed;
            //The game window limits are set as boundaries so the paddle won't leave the screen
            if (this.lPaddleRectangle.Bottom >= GraphicsDevice.Viewport.Height)
                this.lPaddleRectangle.Y = GraphicsDevice.Viewport.Height - this.lPaddleRectangle.Height;
            if (this.lPaddleRectangle.Top <= 0)
                this.lPaddleRectangle.Y = 0;

            //Movement for right paddle
            if (this.keyboard.IsKeyDown(Keys.Up))
                this.rPaddleRectangle.Y -= this.rPaddleSpeed;
            if (this.keyboard.IsKeyDown(Keys.Down))
                this.rPaddleRectangle.Y += this.rPaddleSpeed;
            //The game window limits are set as boundaries so the paddle won't leave the screen
            if (this.rPaddleRectangle.Bottom >= GraphicsDevice.Viewport.Height)
                this.rPaddleRectangle.Y = GraphicsDevice.Viewport.Height - this.rPaddleRectangle.Height;
            if (this.rPaddleRectangle.Top <= 0)
                this.rPaddleRectangle.Y = 0;
        }

        /// <summary>
        /// Changes the direction of the ball accordingly when the ball hits the top or bottom of a paddle
        /// It also sums up one bounce and increases the speed of the ball and paddles
        /// </summary>
        protected void ChangeBallDirectionWhenTopOrBottomHit()
        {
            if(this.ballBounce < 8)
            {
                this.ballBounce++;
                this.ballXSpeed = -this.ballXSpeed * 1.125f; //We increase the speed of the ball every bounce against a paddle
                this.ballYSpeed = -this.ballYSpeed * 1.125f;
                this.lPaddleSpeed += 1; //we also increase the speed of the paddles bit by bit
                this.rPaddleSpeed += 1;
            }
            else
            {
                this.ballXSpeed = -this.ballXSpeed;
                this.ballYSpeed = -this.ballYSpeed;
            }
            
        }
        /// <summary>
        /// When the ball hits the top or bottom of a paddle a nasty bug occurs where the ball gets inside the paddle's rectangle
        /// This method gets rid if that by making the ball bounce in the same direction it came from
        /// </summary>
        /// <returns>True if the ball hits a paddle top or bottom, false if hits a side</returns>
        protected bool CheckIfBallHitPaddleTopOrBottom()
        {
            if (this.ballRectangle.Bottom >= this.lPaddleRectangle.Bottom &&
                this.ballRectangle.Left <= this.lPaddleRectangle.Right &&
                this.whoHitLast == BallHitLast.RightPlayer)                                
            {
                this.ChangeBallDirectionWhenTopOrBottomHit();
                this.whoHitLast = BallHitLast.LeftPlayer;
                return true;
            }

            if(this.ballRectangle.Top <= this.lPaddleRectangle.Top &&
               this.ballRectangle.Left <= this.lPaddleRectangle.Right &&
               this.whoHitLast == BallHitLast.RightPlayer)
            {
                this.ChangeBallDirectionWhenTopOrBottomHit();
                this.whoHitLast = BallHitLast.LeftPlayer;
                return true;
            }

            if (this.ballRectangle.Bottom >= this.rPaddleRectangle.Bottom &&
                this.ballRectangle.Right >= this.rPaddleRectangle.Left &&
                this.whoHitLast == BallHitLast.LeftPlayer)
            {
                this.ChangeBallDirectionWhenTopOrBottomHit();
                this.whoHitLast = BallHitLast.RightPlayer;
                return true;
            }

            if (this.ballRectangle.Top <= this.rPaddleRectangle.Top &&
                this.ballRectangle.Right >= this.rPaddleRectangle.Left &&
                this.whoHitLast == BallHitLast.LeftPlayer)
            {
                this.ChangeBallDirectionWhenTopOrBottomHit();
                this.whoHitLast = BallHitLast.RightPlayer;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Standard behaviour when the ball intersects any of the paddles
        /// </summary>
        protected void BallIntersectsPaddle(Rectangle paddleRectangle)
        {
            if((this.ballYSpeed < 0 &&
                this.ballRectangle.Center.Y < paddleRectangle.Center.Y) ||
                (this.ballYSpeed > 0 &&
                this.ballRectangle.Center.Y > paddleRectangle.Center.Y))
            {
                //do nothing, the rest of the code takes care
                //this is the situation where the ball and the paddle go in the same direction and the ball hits the bottom or top
                //it should keep going in the same Y direction and only change the X, which will happen in the code below
                //otherwise, for example, the ball going down hits the bottom of a paddle also going down and ballwould go up again
                //when it should keep going down
            }
            else
            {
                //this code triggers if ball and paddle are moving in opposite directions in order to send the ball straight in the
                //direction it came from
                if (this.CheckIfBallHitPaddleTopOrBottom())
                    return;
            }

            if (this.ballBounce < 8)
            {
                this.ballBounce++;
                this.ballXSpeed = -this.ballXSpeed * 1.125f; //We increase the speed of the ball every bounce against a paddle
                this.ballYSpeed = this.ballYSpeed * 1.125f;
                this.lPaddleSpeed += 1; //we also increase the speed of the paddles bit by bit
                this.rPaddleSpeed += 1;
            }
            else
            {
                this.ballXSpeed = -this.ballXSpeed;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Allows for input from the keyboard
            this.lastKeyPress = this.keyboard;
            this.keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (this.keyboard.IsKeyDown(Keys.Escape))
                this.Exit();            

            switch (this.state)
            {
                case GameState.MainMenu:
                    if ((this.keyboard.IsKeyUp(Keys.Space)) && (this.lastKeyPress.IsKeyDown(Keys.Space)))
                        this.state = GameState.EnterNamePlayer1;                    
                    break;

                case GameState.EnterNamePlayer1:                    
                    this.enterPlayerName.Update();
                    this.player_1 = this.enterPlayerName.playername;
                    if ((this.keyboard.IsKeyUp(Keys.Enter)) && (this.lastKeyPress.IsKeyDown(Keys.Enter)))
                    {
                        this.enterPlayerName.playername = "";
                        this.state = GameState.EnterNamePlayer2;
                    }                        
                    break;

                case GameState.EnterNamePlayer2:
                    this.enterPlayerName.Update();
                    this.player_2 = this.enterPlayerName.playername;
                    if ((this.keyboard.IsKeyUp(Keys.Enter)) && (this.lastKeyPress.IsKeyDown(Keys.Enter)))
                        this.ResetGame();
                    break;

                case GameState.Service:

                    this.PaddleMovement();

                    if(this.whoScored == LastScore.LeftPlayer)
                    {
                        this.ballRectangle.Location = new Point(
                        Window.ClientBounds.Width - rPaddleRectangle.Width - this.margin - this.ballRectangle.Width,
                        this.rPaddleRectangle.Y + this.rPaddleRectangle.Height / 2 - this.ballRectangle.Height / 2);
                        //this.whoHitLast = BallHitLast.RightPlayer;
                    }
                    else
                    {
                        this.ballRectangle.Location = new Point(
                        this.margin + this.lPaddleRectangle.Width,
                        this.lPaddleRectangle.Y + this.lPaddleRectangle.Height / 2 - this.ballRectangle.Height / 2);
                        this.whoHitLast = BallHitLast.LeftPlayer;
                    }

                    if ((this.keyboard.IsKeyUp(Keys.Space)) && (this.lastKeyPress.IsKeyDown(Keys.Space)))
                        this.state = GameState.Playing;
                    break;

                case GameState.Playing:
                    //Allows the game to pause
                    if ((this.keyboard.IsKeyUp(Keys.P)) && (this.lastKeyPress.IsKeyDown(Keys.P)))
                        this.state = GameState.Paused;                    

                    //Start of normal gameplay
                    this.ballRectangle.X += (int)this.ballXSpeed; //Ball movement for the horizontal component

                    if (this.ballRectangle.X < 0 ||
                        this.ballRectangle.X + this.ballRectangle.Width > GraphicsDevice.Viewport.Width)
                    {
                        this.ballXSpeed = -this.ballXSpeed; //if encounter left or right bound, change direction

                        if (this.ballRectangle.X < 0) //right player scores when ball bounces off the left margin
                        {
                            this.rightScore++;
                            this.whoScored = LastScore.RightPlayer;
                            this.state = GameState.AfterScore;
                        }

                        if (this.ballRectangle.X + this.ballRectangle.Width > GraphicsDevice.Viewport.Width)
                        {
                            this.leftScore++; //left player scores when ball bounces off the right margin
                            this.whoScored = LastScore.LeftPlayer;
                            this.state = GameState.AfterScore;
                        }
                    }

                    this.ballRectangle.Y += (int)this.ballYSpeed; //Ball movement for the vertical component

                    if (this.ballRectangle.Y < 0 ||
                        this.ballRectangle.Y + this.ballRectangle.Height > GraphicsDevice.Viewport.Height)
                    {
                        this.ballYSpeed = -this.ballYSpeed; //if encounter upper or lower bound, change direction
                    }

                    this.PaddleMovement();

                    //Colision detection between ball and paddles. It changes the X direction of the ball
                    //if it colides with the top or bottom of a paddle a nasty bug appears, to avoid that we change bot the x and y direction
                    
                    if (this.ballRectangle.Intersects(this.lPaddleRectangle) && this.whoHitLast == BallHitLast.RightPlayer)                        
                    {
                        this.BallIntersectsPaddle(this.lPaddleRectangle);                        

                        this.whoHitLast = BallHitLast.LeftPlayer;
                    }

                    if (this.ballRectangle.Intersects(this.rPaddleRectangle) && this.whoHitLast == BallHitLast.LeftPlayer)                        
                    {                        
                        this.BallIntersectsPaddle(this.rPaddleRectangle);

                        this.whoHitLast = BallHitLast.RightPlayer;
                    }


                    //Game finishes when one of the players reaches a score of 5
                    if ((this.leftScore == 5) || (this.rightScore == 5))
                        this.state = GameState.End;
                    break;

                case GameState.AfterScore:
                    if ((this.keyboard.IsKeyUp(Keys.Space)) && (this.lastKeyPress.IsKeyDown(Keys.Space)))
                    {
                        this.StandardPlacement();
                        this.state = GameState.Service;
                    }                        
                    break;

                case GameState.Paused:
                    if ((this.keyboard.IsKeyUp(Keys.P)) && (this.lastKeyPress.IsKeyDown(Keys.P)))
                        this.state = GameState.Playing;
                    break;

                case GameState.End:
                    if ((this.keyboard.IsKeyUp(Keys.Space)) && (this.lastKeyPress.IsKeyDown(Keys.Space)))
                        this.ResetGame();
                    break;               
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Contains all the drawing calls for the playing state of the game
        /// </summary>
        protected void DrawPlayingState()
        {
            string playerNames = this.player_1 + " vs " + this.player_2;
            string gameScore = this.leftScore + ":" + this.rightScore;

            Vector2 centeredPlayerNames = this.instructionsFont.MeasureString(playerNames);
            Vector2 centeredScore = this.messageFont.MeasureString(gameScore);

            this.spriteBatch.Draw(this.backgroundTexture, this.backgroundRectangle, Color.White);

            this.spriteBatch.DrawString(this.instructionsFont,
                                   playerNames,
                                   new Vector2((this.graphics.GraphicsDevice.Viewport.Width / 2) - 
                                               (centeredPlayerNames.Length() / 2), 15),
                                               Color.White);
            this.spriteBatch.DrawString(this.messageFont,
                                   gameScore,
                                   new Vector2((this.graphics.GraphicsDevice.Viewport.Width / 2) -
                                               (centeredScore.Length() / 2), 30), Color.White);

            this.spriteBatch.Draw(this.ballTexture, this.ballRectangle, Color.White);
            this.spriteBatch.Draw(this.lPaddleTexture, this.lPaddleRectangle, Color.White);
            this.spriteBatch.Draw(this.rPaddleTexture, this.rPaddleRectangle, Color.White);
        }

        /// <summary>
        /// Draws the pertinent string messages to prompt each player to enter their names
        /// </summary>
        /// <param name="playerNumber">The player number, either Player 1 or Player 2</param>
        /// <param name="playerName">The name of the player</param>
        /// <param name="gameTime">The current time snapshot of the game</param>
        protected void DrawPlayerName(string playerNumber, ref string playerName, GameTime gameTime)
        {
            this.spriteBatch.Draw(this.backgroundTexture, this.backgroundRectangle, Color.White);
            this.spriteBatch.DrawString(this.instructionsFont, "Enter the name of " + playerNumber + ", press Enter when done:",
                                   new Vector2(Window.ClientBounds.Width / 8,
                                               Window.ClientBounds.Height / 4),
                                   Color.White);

            //this if else statement allows a blinking underscore to let the player know the game
            //is waiting for input
            if (gameTime.TotalGameTime.Duration().Seconds % 2 == 0)
            {
                this.spriteBatch.DrawString(this.instructionsFont, playerName,
                                   new Vector2(Window.ClientBounds.Width / 8,
                                               Window.ClientBounds.Height / 4 + 50),
                                   Color.White);
            }
            else
            {
                this.spriteBatch.DrawString(this.instructionsFont, playerName + "_",
                                   new Vector2(Window.ClientBounds.Width / 8,
                                               Window.ClientBounds.Height / 4 + 50),
                                   Color.White);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();
            switch (this.state)
            {
                case GameState.MainMenu:
                    this.spriteBatch.Draw(this.backgroundTexture, this.backgroundRectangle, Color.White);
                    this.spriteBatch.DrawString(this.instructionsFont,
                                           "Welcome to Pong! An exciting game about bouncing a ball!\n\n" +
                                           "Game Controls:\n" +
                                           "Player 1 (on the left) moves up or down with W and S keys\n" +
                                           "Player 2 (on the right) moves up or down with the Up and Down arrowkeys\n" +
                                           "Both players release the ball by pressing the spacebar\n" +
                                           "Game can be paused/unpaused at any given time by pressing P\n" +
                                           "To exit the game press Esc at any given time\n\n" +
                                           "The first player to reach a score of 5 wins the game!\n\n" +
                                           "Press the spacebar to begin the game!",
                                           new Vector2(Window.ClientBounds.Width / 8,
                                                       Window.ClientBounds.Height / 4 - 30),
                                           Color.White);
                    break;

                case GameState.EnterNamePlayer1:
                    this.DrawPlayerName("Player 1", ref this.player_1, gameTime);                                        
                    break;

                case GameState.EnterNamePlayer2:
                    this.DrawPlayerName("Player 2", ref this.player_2, gameTime);
                    break;

                case GameState.Service:
                    this.DrawPlayingState();
                    break;

                case GameState.Playing:
                    this.DrawPlayingState();
                    break;

                case GameState.AfterScore:
                    this.DrawPlayingState();
                    if(this.whoScored == LastScore.LeftPlayer)
                    {
                        this.spriteBatch.DrawString(this.instructionsFont,
                                           this.player_1 + " scored!\nPress the spacebar to continue.",
                                           new Vector2(Window.ClientBounds.Width / 3,
                                                       Window.ClientBounds.Height / 2 - Window.ClientBounds.Height / 16),
                                           Color.White);
                    }
                    else
                    {
                        this.spriteBatch.DrawString(this.instructionsFont,
                                           this.player_2 + " scored!\nPress the spacebar to continue.",
                                           new Vector2(Window.ClientBounds.Width / 3,
                                                       Window.ClientBounds.Height / 2 - Window.ClientBounds.Height / 16),
                                           Color.White);
                    }                    
                    break;

                case GameState.Paused:
                    this.DrawPlayingState();
                    this.spriteBatch.DrawString(this.messageFont, "Game paused. Press P to resume.", 
                                           new Vector2(Window.ClientBounds.Width / 8 ,
                                                       Window.ClientBounds.Height / 2 - Window.ClientBounds.Height / 16),
                                           Color.White);
                    break;

                case GameState.End:
                    this.spriteBatch.Draw(this.backgroundTexture, this.backgroundRectangle, Color.White);
                    if(this.leftScore == 5)
                    {
                        this.spriteBatch.DrawString(this.instructionsFont,
                                           this.player_1 + " has reached " + this.leftScore + "\n" + this.player_1 + " wins!",
                                           new Vector2(Window.ClientBounds.Width / 3,
                                                       Window.ClientBounds.Height / 2 - Window.ClientBounds.Width / 16),
                                           Color.White);
                    }
                    else
                    {
                        this.spriteBatch.DrawString(this.instructionsFont,
                                           this.player_2 + " has reached " + this.rightScore + "\n" + this.player_2 + " wins!",
                                           new Vector2(Window.ClientBounds.Width / 3,
                                                       Window.ClientBounds.Height / 2 - Window.ClientBounds.Width / 16),
                                           Color.White);
                    }

                    this.spriteBatch.DrawString(this.instructionsFont, "Press the spacebar to play again\nor Esc to exit.",
                                           new Vector2(Window.ClientBounds.Width / 3,
                                                       Window.ClientBounds.Height / 2),
                                           Color.White);
                    break;
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class KeyboardInput
    {
        private Keys[] previousFrameKey;

        public KeyboardInput()
        {
            this.previousFrameKey = new Keys[0];
        }

        public void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();
            Keys[] currentPressedKeys = keyboard.GetPressedKeys();

            //check if any of the previous update's keys are no longer pressed
            foreach (Keys key in this.previousFrameKey)
            {
                if (!currentPressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            //check if the currently pressed keys were already pressed
            foreach (Keys key in currentPressedKeys)
            {
                if (!this.previousFrameKey.Contains(key))
                    OnKeyDown(key);
            }

            //save the currently pressed keys so we can compare on the next update
            this.previousFrameKey = currentPressedKeys;
        }

        private void OnKeyDown(Keys key)
        {
            //because a simple key stroke goes on for several frames, we avoid getting single letters multiple times repeated
            return;
        }

        public string playername = "";
        private void OnKeyUp(Keys key)
        {
            //Letters get added to the name only when the key is released
            if(key.ToString() != "Enter")
                this.playername += key.ToString();
        }
    }
}
