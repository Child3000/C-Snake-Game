using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using WMPLib;
using System.Windows.Threading;

namespace Snake
{
    public partial class Form1 : Form
    {
        #region Global Declaration

        WindowsMediaPlayer player = new WindowsMediaPlayer();
        WindowsMediaPlayer player2 = new WindowsMediaPlayer();
        string url = "D:/Users/Desktop/Sound Effect/";

        #region EnemyAI

        private List<Circle> openList = new List<Circle>();
        private List<Circle> closedList = new List<Circle>();

        private Circle enemyAI = new Circle();

        #endregion


        private List<Circle> Snake = new List<Circle>();

        private Circle food = new Circle();
        private Circle specialFood = new Circle();
        private Circle slowFood = new Circle();
        private Circle fastFood = new Circle();
        private Circle HealthFood = new Circle();

        private Circle Portal_1 = new Circle();
        private Circle Portal_2 = new Circle();

        //  Obstacles   //
        private Circle[] fourBrickObs = new Circle[4];

        private int RandomNum;
        private static bool runOnce = true;     // Slow
        private static bool runOnceFast = true;
        private static bool runOnceHP = true;
        private static bool runSpecial = true;
        private static bool brickDamage;
        private int maxXPos;
        private int maxYPos;

        #endregion

        public Form1()
        {
            InitializeComponent();

            //  Set settings to default //
            new Settings();

            //  Setting game speed  //
            gameTimer.Interval = 3000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            // Setting enemyAl  //
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Tick += delegate (object s, EventArgs args)
            {
                PathFinding();
            };

            dispatcherTimer.Start();

            //  Start New Game  //
            StartGame();
        }

        private void StartGame()
        {
            maxXPos = pbCanvas.Size.Width / Settings.Width;
            maxYPos = pbCanvas.Size.Height / Settings.Height;

            lblGameOver.Visible = false;

            PlayBackgroundMusic();

            //  Setting to default  //
            new Settings();
            gameTimer.Interval = 3000 / Settings.Speed;

            //  Create New Player Object    //
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            //  Create Enemy AI //
            enemyAI = new Circle { X = 15, Y = 15 };

            // Setting Portals' Position    //
            Portal_1 = new Circle { X = 5, Y = 5 };
            Portal_2 = new Circle { X = 25, Y = 20 };

            // Setting Bricks Position  //
            fourBrickObs[0] = new Circle { X = 12, Y = 14 };
            fourBrickObs[1] = new Circle { X = fourBrickObs[0].X + 1, Y = fourBrickObs[0].Y };
            fourBrickObs[2] = new Circle { X = fourBrickObs[0].X, Y = fourBrickObs[0].Y + 1 };
            fourBrickObs[3] = new Circle { X = fourBrickObs[0].X + 1, Y = fourBrickObs[0].Y + 1 };

            //  Initialize Score and HP //
            lblScore.Text = Settings.Score.ToString();
            aLabelHP.Text = Settings.Lives.ToString();
            aLabelMoveLeft.Text = Settings.MoveLeft.ToString();

            //  Initialize Food Position    //
            GenerateFood();
        }

        /* Function Description : Generate Food at Random Position */
        private void GenerateFood()
        {
            Random random = new Random();
            food = new Circle { X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos) };
        }

        /* Function Description : Generate Special Food at Random Position */
        private void GenerateSpecialFood()
        {
            Random random2 = new Random();
            specialFood = new Circle { X = random2.Next(0, maxXPos), Y = random2.Next(0, maxYPos) };
        }

        /* Function Description : Generate Slow Food at Random Position */
        private void GenerateSlowFood()
        {
            Random random3 = new Random();
            slowFood = new Circle { X = random3.Next(0, maxXPos), Y = random3.Next(0, maxYPos) };

        }

        /* Function Description : Generate Fast Food at Random Position */
        private void GenerateFastFood()
        {
            Random random4 = new Random();
            fastFood = new Circle { X = random4.Next(0, maxXPos), Y = random4.Next(0, maxYPos) };

        }

        /* Function Description : Generate Health Food at Random Position */
        private void GenerateHealthFood()
        {
            Random random4 = new Random();
            HealthFood = new Circle { X = random4.Next(0, maxXPos), Y = random4.Next(0, maxYPos) };
        }

        /* Function Description : Invoke the function each every time intervals */
        private void UpdateScreen(object sender, EventArgs e)
        {
            //  Handling GameOver   //
            if (Settings.GameOver)
            {
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MoveBrick();
                MovePlayer();
            }

            pbCanvas.Invalidate();
        }

        /* Function Description : Update the graphic each every time intervals  */
        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;     // Draw head
                    else
                        snakeColour = Brushes.Green;    //  Draw rest of body

                    //  Draw snake
                    canvas.FillRectangle(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    // Draw EnemyAI
                    canvas.FillRectangle(Brushes.GhostWhite,
                                         enemyAI.X * Settings.Width,
                                         enemyAI.Y * Settings.Height,
                                         Settings.Width, Settings.Height);

                    // Draw Special Food, Fast Food and Slow Food
                    bool specialFoodAppear = Settings.Score % 500 == 0 && Settings.Score != 0;

                    if (specialFoodAppear)
                    {
                        switch (RandomNum)
                        {
                            case 0:
                                //  Special Food
                                canvas.FillRectangle(Brushes.Yellow,
                                 new Rectangle(specialFood.X * Settings.Width,
                                 specialFood.Y * Settings.Height,
                                 Settings.Width, Settings.Height));

                                break;
                            case 1:
                                if (runOnce)
                                {
                                    //  Slow Food
                                    canvas.FillRectangle(Brushes.Purple,
                                    new Rectangle(slowFood.X * Settings.Width,
                                    slowFood.Y * Settings.Height,
                                    Settings.Width, Settings.Height));

                                    // runOnce = false;
                                }

                                break;
                            case 2:
                                if (runOnceFast)
                                {
                                    //  Fast Food
                                    canvas.FillRectangle(Brushes.Cyan,
                                    new Rectangle(fastFood.X * Settings.Width,
                                    fastFood.Y * Settings.Height,
                                    Settings.Width, Settings.Height));

                                    // runOnce = false;
                                }

                                break;

                            case 3:
                                if (runOnceHP)
                                {
                                    //  Health Food
                                    canvas.FillRectangle(Brushes.Orange,
                                    new Rectangle(HealthFood.X * Settings.Width,
                                    HealthFood.Y * Settings.Height,
                                    Settings.Width, Settings.Height));
                                }
                                break;
                        }
                    }

                    //  Draw ordinary food
                    canvas.FillRectangle(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                             food.Y * Settings.Height,
                             Settings.Width, Settings.Height));


                    // Draw both portals
                    canvas.FillEllipse(Brushes.Blue, new Rectangle(Portal_1.X * Settings.Width,
                             Portal_1.Y * Settings.Height,
                             Settings.Width, Settings.Height));

                    canvas.FillEllipse(Brushes.Blue, new Rectangle(Portal_2.X * Settings.Width,
                             Portal_2.Y * Settings.Height,
                             Settings.Width, Settings.Height));

                    // Draw block bricks
                    for (int j = 0; j < fourBrickObs.Length; j++)
                    {
                        canvas.FillEllipse(Brushes.Chocolate, new Rectangle(fourBrickObs[j].X * Settings.Width,
                        fourBrickObs[j].Y * Settings.Height,
                        Settings.Width, Settings.Height));
                    }
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }

        private void MoveBrick()
        {
            if (Settings.Bdirection == BrickDirection.Horizontal)
                fourBrickObs[0].X++;

            else if (Settings.Bdirection == BrickDirection.Vertical)
                fourBrickObs[0].Y++;

            fourBrickObs[1].X = fourBrickObs[0].X + 1;
            fourBrickObs[1].Y = fourBrickObs[0].Y;

            fourBrickObs[2].X = fourBrickObs[0].X;
            fourBrickObs[2].Y = fourBrickObs[0].Y + 1;

            fourBrickObs[3].X = fourBrickObs[0].X + 1;
            fourBrickObs[3].Y = fourBrickObs[0].Y + 1;

            for (int i = 0; i < 4; i++)
            {
                if (fourBrickObs[i].X < 0)
                {
                    fourBrickObs[i].X = maxXPos;
                }
                else if (fourBrickObs[i].Y < 0)
                {
                    fourBrickObs[i].Y = maxYPos;
                }
                else if (fourBrickObs[i].X >= maxXPos)
                {
                    fourBrickObs[i].X = 0;
                }
                else if (fourBrickObs[i].Y >= maxYPos)
                {
                    fourBrickObs[i].Y = 0;
                }
            }
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //  Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxXPos;
                    }
                    else if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxYPos;
                    }
                    else if (Snake[i].X >= maxXPos)
                    {
                        Snake[i].X = 0;
                    }
                    else if (Snake[i].Y >= maxYPos)
                    {
                        Snake[i].Y = 0;
                    }

                    //  Detect collision with Portals
                    if (Snake[i].X == Portal_1.X &&
                       Snake[i].Y == Portal_1.Y)
                    {
                        Snake[i].X = Portal_2.X;
                        Snake[i].Y = Portal_2.Y;
                    }

                    else if (Snake[i].X == Portal_2.X &&
                       Snake[i].Y == Portal_2.Y)
                    {
                        Snake[i].X = Portal_1.X;
                        Snake[i].Y = Portal_1.Y;
                    }

                    if (Settings.Score % 500 == 0 && Settings.Score != 0)
                    {
                        //  Detect collision with Special Food
                        if (Snake[i].X == specialFood.X &&
                           Snake[i].Y == specialFood.Y &&
                           runSpecial)
                        {
                            PlayYummy();
                            EatSpecial();

                            runSpecial = false;
                        }

                        // Detect collision with Slow Food
                        else if (Snake[i].X == slowFood.X &&
                                Snake[i].Y == slowFood.Y &&
                                runOnce)
                        {
                            PlayYummy();

                            Settings.Speed -= 10;
                            gameTimer.Interval = 3000 / Settings.Speed;
                            runOnce = false;
                        }

                        // Detect collision with Fast Food
                        else if (Snake[i].X == fastFood.X &&
                                Snake[i].Y == fastFood.Y &&
                                 runOnceFast)
                        {
                            PlayYummy();

                            // Set limitation of speed
                            if(Settings.Speed <= Settings.MAX_SPEED)
                            {
                                Settings.Speed += 7;
                                gameTimer.Interval = 3000 / Settings.Speed;
                            }
                            runOnceFast = false;
                        }

                        // Detect collision with Health Food
                        else if (Snake[i].X == HealthFood.X &&
                                Snake[i].Y == HealthFood.Y &&
                                runOnceHP)
                        {
                            PlayYummy();

                            if (Settings.Lives < 3)
                            {
                                Settings.Lives++;
                                aLabelHP.Text = Settings.Lives.ToString();
                            }

                            runOnceHP = false;
                        }
                    }


                    // Detect collision with brick blocks
                    for (int k = 0; k < fourBrickObs.Length; k++)
                    {
                        if (Snake[i].X == fourBrickObs[k].X &&
                            Snake[i].Y == fourBrickObs[k].Y)
                        {
                            brickDamage = true;

                            if (brickDamage)
                            {
                                ReduceLive();

                                if (Settings.Bdirection == BrickDirection.Horizontal)
                                    Settings.Bdirection = BrickDirection.Vertical;
                                else
                                    Settings.Bdirection = BrickDirection.Horizontal;
                                brickDamage = false;
                            }
                        }
                    }

                    //  Detect collision with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            ReduceLive();
                        }
                    }

                    //  Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else
                {
                    //  Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
            Settings.MoveLeft--;
            aLabelMoveLeft.Text = Settings.MoveLeft.ToString();
            if (Settings.MoveLeft == 0)
            {
                ReduceLive();
                Settings.MoveLeft = Settings.MAX_MOVE;
            }
        }

        private void ReduceLive()
        {
            Settings.Lives--;
            aLabelHP.Text = Settings.Lives.ToString();
            if (Settings.Lives < 1)
            {
                Die();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            runOnce = true;
            runOnceFast = true;
            runOnceHP = true;
            runSpecial = true;

            Settings.MoveLeft = Settings.MAX_MOVE;
            SoundPlayer[] player = new SoundPlayer[3];
            Random ranSound = new Random();
            int ranSoundNum = ranSound.Next(0, 2);

            player[0] = new SoundPlayer( url + "wEat01.wav");
            player[1] = new SoundPlayer(url + "wEat02.wav");
            player[2] = new SoundPlayer(url + "wEat03.wav");
            
            // Play Sound
            player[ranSoundNum].Play();

            //  Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //  Update Score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            // Generate Food
            do
            {
                GenerateFood();

            } while ((food.X == Portal_1.X && food.Y == Portal_1.Y) ||
                     food.X == Portal_2.X && food.Y == Portal_2.Y);


            if (Settings.Score != 0 && Settings.Score % 500 == 0)
            {
                if (Settings.Speed < Settings.MAX_SPEED)
                {
                    Settings.Speed += 10;
                    gameTimer.Interval = 3000 / Settings.Speed;

                }
                    // Make sure the coordinate of special food is not overlapped with food's.
                    Random ran_food = new Random();
                    RandomNum = ran_food.Next(0, 3);
                switch (RandomNum)
                {
                    case 0:
                        do
                        {
                            GenerateSpecialFood();

                        } while ((food.X == specialFood.X && food.Y == specialFood.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_1.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_2.Y));

                        break;
                    case 1:

                        do
                        {
                            GenerateSlowFood();

                        } while ((food.X == slowFood.X && food.Y == slowFood.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_1.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_2.Y));

                        break;
                    case 2:

                        do
                        {
                            GenerateFastFood();

                        } while ((food.X == fastFood.X && food.Y == fastFood.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_1.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_2.Y));

                        break;

                    case 3:

                        do
                        {
                            GenerateHealthFood();

                        } while ((food.X == HealthFood.X && food.Y == HealthFood.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_1.Y) ||
                                 (specialFood.X == Portal_1.X && specialFood.Y == Portal_2.Y));

                        break;
                }
                
            }
        }

        private void EatSpecial()
        {
            if (Settings.Score % 500 == 0 && Settings.Score != 0)
            {
                // Gain 500 bonus scores
                Settings.Score += Settings.BonusPoints;
                lblScore.Text = Settings.Score.ToString();
            }
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void PlayYummy()
        {
            player2.URL = url + "mYum01.mp3";

            // Adjust Window Media Player's Volume
            player.settings.volume = 55;
            player2.controls.play();
        }

        private void PlayBackgroundMusic()
        {
            player.URL = "D:/Users/Desktop/hk/hk.mp3";
            while (player.settings.volume > 40)
                player.settings.volume -= 2;

            while (player.settings.volume < 35)
                player.settings.volume += 2;

            player.controls.play();
        }

        
        private void PathFinding()
        {
            int distanceX = Math.Abs(Snake[0].X - enemyAI.X);
            int distanceY = Math.Abs(Snake[0].Y - enemyAI.Y);

            if (distanceX > distanceY)
            {
                if (Snake[0].X < enemyAI.X)
                    enemyAI.X--;

                else if (Snake[0].X > enemyAI.X)
                    enemyAI.X++;
            }
            else
            {
                if (Snake[0].Y < enemyAI.Y)
                    enemyAI.Y--;

                else if (Snake[0].Y > enemyAI.Y)
                    enemyAI.Y++;
            }



            // Boardless 
            if (enemyAI.X < 0)
            {
                enemyAI.X = maxXPos;
            }
            else if (enemyAI.Y < 0)
            {
                enemyAI.Y = maxYPos;
            }
            else if (enemyAI.X >= maxXPos)
            {
                enemyAI.X = 0;
            }
            else if (enemyAI.Y >= maxYPos)
            {
                enemyAI.Y = 0;
            }
        }
    }
}
