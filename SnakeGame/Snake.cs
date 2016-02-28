﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class GameWindow : Form
    {
        private List<Square> snake = new List<Square>();
        private Square food = new Square();
        public GameWindow()
        {
            InitializeComponent();

            //set settings to default
            new Settings();

            lblScore.Text = "Score: " + Settings.Score; 

            //Set game speed and start the timer
            gameTimer.Interval = 1000 / Settings.GameSpeed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            StartGame();
        }
        private void StartGame()
        {
            new Settings();
            snake.Clear(); // deletes old snake
            lblGameOver.Visible = false;
            btnStartGame.Enabled = false;

            Square head = new Square(); // creates new snake
            head.xCoord = 10;
            head.yCoord = 5;
            snake.Add(head);

            GenerateFood();

            lblScore.Text = "Score: " + Settings.Score;


        }
        private void UpdateScreen(object sender, EventArgs e)
        {
            if(Settings.GameOver == true) // checks if the game has ended and starts a new one if enter is pressed
            {
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                // makes sure it doesn't do impossible moves
                if (Input.KeyPressed(Keys.Right) && Settings.Direction != Direction.Left)
                    Settings.Direction = Direction.Right;

                else if (Input.KeyPressed(Keys.Left) && Settings.Direction != Direction.Right)
                    Settings.Direction = Direction.Left;

                else if (Input.KeyPressed(Keys.Up) && Settings.Direction != Direction.Down)
                    Settings.Direction = Direction.Up;

                else if (Input.KeyPressed(Keys.Down) && Settings.Direction != Direction.Up)
                    Settings.Direction = Direction.Down;

                MovePlayer();
            }
         
            pbCanvas.Invalidate(); //Updates the screen
        }

        private void MovePlayer()
        {
            for (int i = snake.Count - 1; i >= 0; i--)
            {
                if(i != 0) // moves the body
                {
                    snake[i].xCoord = snake[i - 1].xCoord;
                    snake[i].yCoord = snake[i - 1].yCoord;
                }
                else // moves the head
                {
                    switch (Settings.Direction)
                    {                            
                        case Direction.Up:
                            snake[i].yCoord--;
                            break;
                        case Direction.Down:
                            snake[i].yCoord++;
                            break;
                        case Direction.Right:
                            snake[i].xCoord++;
                            break;
                        case Direction.Left:
                            snake[i].xCoord--;
                            break;
                    }

                    if (DetectCollision() != 0) // checks if it collides
                        Die(DetectCollision());

                    if (AteFood()) // checks if it collides with a food object
                        Eat();
                }
            }
        }
        private void GenerateFood()
        {
            ushort maxXPosition = (ushort)(pbCanvas.Size.Width / Settings.Width);
            ushort maxYPosition = (ushort)(pbCanvas.Size.Height / Settings.Height);
            bool collides = false;

            Random getPos = new Random();
            food = new Square();

            while (true) // makes sure the food doesn't spawn on top of the snake
            {
                collides = false;
                food.xCoord = getPos.Next(0, maxXPosition);
                food.yCoord = getPos.Next(0, maxYPosition);

                foreach (var piece in snake)
                {
                    if (piece.xCoord == food.xCoord || piece.yCoord == food.yCoord)
                        collides = true;
                }

                if (!collides)
                    break;
            }
        }
        public void Eat()
        {
            //Add piece to body
            Square food = new Square();
            food.xCoord = snake[snake.Count - 1].xCoord;
            food.yCoord = snake[snake.Count - 1].yCoord;

            snake.Add(food);
            GenerateFood();
            Settings.Score += Settings.Points;
            lblScore.Text = "Score: " + Settings.Score;
        }
        private void Die(int death)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("You died by ");
            if (death == 1)
                sb.AppendLine("going out of the world :(");
            else
                sb.AppendLine("eating your own body :(");
            sb.AppendLine("Your score was " + Settings.Score + "!");
            sb.AppendLine("Press ENTER or click the button to play again.");

            Settings.GameOver = true;
            lblGameOver.Text = sb.ToString();
            lblGameOver.Visible = true;
            btnStartGame.Enabled = true;
            Settings.Score = 0;

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                Brush snakeColor;

                //Draw Snake piece by piece
                for (int i = 0; i < snake.Count; i++)
                {
                    if (i == 0)
                        snakeColor = Brushes.Black; // color of the head
                    else
                        snakeColor = Brushes.DarkGreen; // color of the body

                    // Draw Snake Piece
                    canvas.FillRectangle(snakeColor, new Rectangle(
                        snake[i].xCoord * Settings.Width,
                        snake[i].yCoord * Settings.Height,
                        Settings.Width, Settings.Height));
                }

                // Draw Food
                canvas.FillEllipse(Brushes.Red, new Rectangle(
                    food.xCoord * Settings.Width,
                    food.yCoord * Settings.Height,
                    Settings.Width, Settings.Height));
            }
            else
            {
                // Add Game Over message
            }
        }
        private int DetectCollision()
        {
            // 0 - has not collided / 1 - has collided with borders / 2 - has collided with body
            int collided = 0;
            ushort maxXPosition = (ushort)(pbCanvas.Size.Width / Settings.Width);
            ushort maxYPosition = (ushort)(pbCanvas.Size.Height / Settings.Height);

            //Checks collision with game borders
            if (snake[0].xCoord < 0 || snake[0].yCoord < 0 || snake[0].xCoord >= maxXPosition || snake[0].yCoord >= maxYPosition)
                collided = 1;

            //Checks collision with body
            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[0].xCoord == snake[i].xCoord && snake[0].yCoord == snake[i].yCoord)
                    collided = 2;
            }

            return collided;
        }
        private bool AteFood()
        {
            bool ate = false;

            if (snake[0].xCoord == food.xCoord && snake[0].yCoord == food.yCoord)
                ate = true;

            return ate;
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }
        private void btnStartGame_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void GameWindow_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
    }
}
