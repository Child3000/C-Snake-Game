using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using WMPLib;

namespace Snake
{
    public partial class Form1 : Form
    {
        // Enemy AI parts
        private List<Circle> openList = new List<Circle>();
        private List<Circle> closedList = new List<Circle>();

        private Circle enemyAI = new Circle();
        // Enemy AI parts


        private List<Circle> Snake = new List<Circle>();

        private Circle food = new Circle();
        private Circle specialFood = new Circle();
        private Circle slowFood = new Circle();
        private Circle fastFood = new Circle();
        private Circle HealthFood = new Circle();

        private Circle Portal_1 = new Circle();
        private Circle Portal_2 = new Circle();

        // Obstacles
        private Circle[] fourBrickObs = new Circle[4];

        private int RandomNum;
        private static bool runOnce = true;     // Slow
        private static bool runOnceFast = true;
        private static bool runOnceHP = true;
        private static bool runSpecial = true;
        private static bool brickDamage;


        public Form1()
        {
            InitializeComponent();

            //Set settings to default
            new Settings();

            //Set game speed and start timer

            gameTimer.Interval = 3000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Start New game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            //Set settings to default
            new Settings();
            gameTimer.Interval = 3000 / Settings.Speed;

            //Create new player object
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            //Create enemy AI
            enemyAI = new Circle { X = 15, Y = 15 };

            // Set portals position
            Portal_1 = new Circle { X = 5, Y = 5 };
            Portal_2 = new Circle { X = 25, Y = 20 };

            // Set Bricks position
            fourBrickObs[0] = new Circle { X = 12, Y = 14 };
            fourBrickObs[1] = new Circle { X = fourBrickObs[0].X + 1, Y = fourBrickObs[0].Y };
            fourBrickObs[2] = new Circle { X = fourBrickObs[0].X, Y = fourBrickObs[0].Y + 1 };
            fourBrickObs[3] = new Circle { X = fourBrickObs[0].X + 1, Y = fourBrickObs[0].Y + 1 };

            lblScore.Text = Settings.Score.ToString();
            aLabelHP.Text = Settings.Lives.ToString();
            aLabelMoveLeft.Text = Settings.MoveLeft.ToString();
            GenerateFood();
        }

        //Place random food object
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle { X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos) };
        }

        private void GenerateSpecialFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random2 = new Random();
            specialFood = new Circle { X = random2.Next(0, maxXPos), Y = random2.Next(0, maxYPos) };
        }

        private void GenerateSlowFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random3 = new Random();
            slowFood = new Circle { X = random3.Next(0, maxXPos), Y = random3.Next(0, maxYPos) };

        }

        private void GenerateFastFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random4 = new Random();
            fastFood = new Circle { X = random4.Next(0, maxXPos), Y = random4.Next(0, maxYPos) };

        }

        private void GenerateHealthFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random4 = new Random();
            HealthFood = new Circle { X = random4.Next(0, maxXPos), Y = random4.Next(0, maxYPos) };
        }


        private void UpdateScreen(object sender, EventArgs e)
        {

            //Check for Game Over
            if (Settings.GameOver)
            {
                //Check if Enter is pressed
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

                //if (Math.Abs(Snake[0].X - enemyAI.X) >= 5)
                //    PathFinding();
                MoveBrick();
                MovePlayer();


            }

            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                //Set colour of snake

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;     //Draw head
                    else
                        snakeColour = Brushes.Green;    //Rest of body

                    //Draw snake
                    canvas.FillRectangle(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    bool specialFoodAppear = Settings.Score % 1000 == 0 && Settings.Score != 0;

                    // Draw EnemyAI
                    canvas.FillRectangle(Brushes.GhostWhite,
                                         enemyAI.X * Settings.Width,
                                         enemyAI.Y * Settings.Height,
                                         Settings.Width, Settings.Height);

                    // Draw Special Food / Fast Food / Slow Food
                    if (specialFoodAppear)
                    {
                        switch (RandomNum)
                        {
                            case 0:
                                canvas.FillRectangle(Brushes.Yellow,
                                 new Rectangle(specialFood.X * Settings.Width,
                                 specialFood.Y * Settings.Height,
                                 Settings.Width, Settings.Height));

                                break;
                            case 1:
                                if (runOnce)
                                {
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
                                    canvas.FillRectangle(Brushes.Orange,
                                    new Rectangle(HealthFood.X * Settings.Width,
                                    HealthFood.Y * Settings.Height,
                                    Settings.Width, Settings.Height));

                                }

                                break;
                        }
                    }

                    //Draw Food
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

            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

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
                //Move head
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


                    //Get maximum X and Y Pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

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

                    // Detect collision with Portals
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

                    if (Settings.Score % 1000 == 0 && Settings.Score != 0)
                    {
                        // Detect collision with Special Food
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

                            Settings.Speed += 7;
                            gameTimer.Interval = 3000 / Settings.Speed;
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

                    //Detect collision with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            ReduceLive();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else
                {
                    //Move body
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

            player[0] = new SoundPlayer("D:/Users/Desktop/Sound Effect/wEat01.wav");
            player[1] = new SoundPlayer("D:/Users/Desktop/Sound Effect/wEat02.wav");
            player[2] = new SoundPlayer("D:/Users/Desktop/Sound Effect/wEat03.wav");

            //player[ranSoundNum] = new SoundPlayer();
            player[ranSoundNum].Play();

            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update Score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            // Generate food before special Food generated
            do
            {
                GenerateFood();

            } while ((food.X == Portal_1.X && food.Y == Portal_2.Y) ||
                     food.X == Portal_2.X && food.Y == Portal_2.Y);


            if (Settings.Score != 0)
            {
                if (Settings.Score % 500 == 0)
                {
                    Settings.Speed += 10;
                    gameTimer.Interval = 3000 / Settings.Speed;
                }

                if (Settings.Score % 1000 == 0)
                {
                    // Make sure the coordinate of special food is not overlapped with food's.
                    Random ran_food = new Random();
                    RandomNum = ran_food.Next(0, 3);
                    switch (RandomNum)
                    {
                        case 0:
                            do
                            {
                                GenerateSpecialFood();

                            } while (food.X == specialFood.X && food.Y == specialFood.Y);

                            break;
                        case 1:

                            do
                            {
                                GenerateSlowFood();

                            } while (food.X == slowFood.X && food.Y == slowFood.Y);

                            break;
                        case 2:

                            do
                            {
                                GenerateFastFood();

                            } while (food.X == fastFood.X && food.Y == fastFood.Y);

                            break;

                        case 3:

                            do
                            {
                                GenerateHealthFood();

                            } while (food.X == HealthFood.X && food.Y == HealthFood.Y);

                            break;
                    }
                }
            }
        }

        private void EatSpecial()
        {
            if (Settings.Score % 1000 == 0 && Settings.Score != 0)
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
            WindowsMediaPlayer player = new WindowsMediaPlayer();
            player.URL = "D:/Users/Desktop/Sound Effect/mYum01.mp3";

            // Adjust Window Media Player's Volume
            while (player.settings.volume > 55)
                player.settings.volume -= 2;

            while (player.settings.volume < 40)
                player.settings.volume += 2;
            player.controls.play();
        }


        /* Ignore bottom part, not yet finished */
        /* This is the part of Enemy AI */
        /* It is messy now, just ignore it */
        
        //private void PathFinding()
        //{
        //    // Starting point
        //    Circle startNode = new Circle { X = enemyAI.X, Y = enemyAI.Y };
        //    openList.Add(startNode);

          
        //    // Target point
        //    Circle targetNode = FindTargetNode(startNode);

        //    int i = 0; // Debugging purpose


        //    //// Loop
        //    //while (openList.Count > 0 && i != 1000)
        //    //{
        //    // Get node with lowest z value
        //    Circle node = FindNodeLowestF();
        //    if (node.X == startNode.X)
        //    {
        //        GetPath(startNode, startNode);
        //        return;
        //    }
        //    // Check if the node is target's node
        //    if (node.X == targetNode.X &&
        //       node.Y == targetNode.Y)
        //    {
        //        GetPath(node, startNode);
        //        //break;
        //    }

        //    // remove node from openList
        //    // then add it in closedList
        //    openList.Remove(node);
        //    closedList.Add(node);


        //    List<Circle> neighbours = GetNeighbours(node);

        //    foreach (Circle n in neighbours)
        //    {
        //        if (!((n.X == Portal_1.X && n.Y == Portal_1.Y) ||
        //             (n.X == Portal_2.X && n.Y == Portal_2.Y) ||
        //              closedList.Contains(n)))
        //        {
        //            if (!openList.Contains(n))
        //            {
        //                openList.Add(n);
        //                n.parent = node;

        //                // Calculate G, H, and F
        //                n.Gvalue = node.Gvalue + 10;
        //                n.Hvalue = CalculateManhattanDistance(n, targetNode);
        //                n.Fvalue = n.Gvalue + n.Hvalue;
        //            }
        //            else
        //            {
        //                if (node.Gvalue + 10 < n.Gvalue)
        //                {
        //                    n.parent = node;

        //                    // Recalculate G & F
        //                    n.Gvalue = node.Gvalue + 10;
        //                    n.Fvalue = n.Gvalue + n.Hvalue;
        //                }
        //            }
        //        }
        //    }
        //    //    i++;
        //    //}
        //    // If it is not on the open list, add it to open list. 
        //    // Make the currentNode parent to this square
        //    // Record F, G, and H costs of square

        //    // It if it on open list, check to see if this path to that square is better
        //    // If it has lower G cost in total, change parent to the current square
        //    // Recalculate G & F

        //    // Loop stop when
        //    // Target square is in closed list
        //    // Or open list is empty
        //}

        //private void GetPath (Circle targetNode, Circle startNode)
        //{
        //    //Circle nextNode = targetNode;

        //    //while(nextNode.parent != startNode)
        //    //{
        //    //    nextNode = nextNode.parent;
        //    //}

        //    //// Move enemyAI one step forward
        //    //enemyAI.X = nextNode.X;
        //    //enemyAI.Y = nextNode.Y;

        //    enemyAI.X++;
        //    enemyAI.Y++;
        //}

        //private int CalculateManhattanDistance(Circle currentNode, Circle targetNode)
        //{
        //    int currentToTargetDistanceX = Math.Abs(targetNode.X - currentNode.X);
        //    int currentToTargetDistanceY = Math.Abs(targetNode.Y - currentNode.Y);
        //    int sumCurrentToTargetDistance = currentToTargetDistanceX + currentToTargetDistanceY;

        //    return sumCurrentToTargetDistance;
        //}


        ///* Get lowest Z value and return back */
        //private Circle FindNodeLowestF()
        //{
        //    Circle lowestNode = new Circle();
        //    lowestNode.Fvalue = int.MaxValue;

        //    foreach(Circle element in openList)
        //    {
        //        if(element.Fvalue < lowestNode.Fvalue )
        //        {
        //            lowestNode = element;
        //        }
        //    }
        //    return lowestNode;
        //}


        ///* Get all the four square position from its parents */
        //private List<Circle> GetNeighbours(Circle currentNode)
        //{
        //    List<Circle> neighbours = new List<Circle>();

        //    Circle neighbour = new Circle { X = currentNode.X + 1, Y = currentNode.Y };
        //    neighbours.Add(neighbour);

        //    neighbour = new Circle { X = currentNode.X - 1, Y = currentNode.Y };
        //    neighbours.Add(neighbour);

        //    neighbour = new Circle { X = currentNode.X, Y = currentNode.Y + 1 };
        //    neighbours.Add(neighbour);

        //    neighbour = new Circle { X = currentNode.X, Y = currentNode.Y - 1 };
        //    neighbours.Add(neighbour);

        //    return neighbours;
        //}


        ///* Find the closet Target Node.
        // * This function will decide either track snake's head or snake's tail.
        // */
        //private Circle FindTargetNode(Circle currentNode)
        //{
        //    Circle targetNode = new Circle() { X = Portal_1.X, Y = Portal_1.Y };

        //    //// Find number of square needed to go to head of snake
        //    //int distanceToHeadX = Math.Abs(currentNode.X - Snake[0].X);
        //    //int distanceToHeadY = Math.Abs(currentNode.Y - Snake[0].Y);
        //    //int sumDistanceToHead = distanceToHeadX + distanceToHeadY;

        //    //// Find number of square needed to go to tail of snake
        //    //int distanceToTailX = Math.Abs(currentNode.X - Snake[Snake.Count - 1].X);
        //    //int distanceToTailY = Math.Abs(currentNode.Y - Snake[Snake.Count - 1].Y);
        //    //int sumDistanceToTail = distanceToTailX + distanceToTailY;

        //    //if (sumDistanceToHead < sumDistanceToTail)
        //    //    targetNode = Snake[0];
        //    //else
        //    //    targetNode = Snake[Snake.Count - 1];

        //    return targetNode;
        //}

        
    }
}
