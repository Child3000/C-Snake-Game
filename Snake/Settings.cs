namespace Snake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    public enum BrickDirection
    {
        Horizontal,
        Vertical
    };

    public class Settings
    {
        public const int MAX_SPEED = 50;
        public const int MAX_MOVE = 800;
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int Speed { get; set; }
        public static int Score { get; set; }
        public static int Points { get; set; }
        public static int BonusPoints { get; set; }
        public static bool GameOver { get; set; }
        public static Direction direction { get; set; }
        public static int Lives { get; set; }
        public static int MoveLeft { get; set; }
        public static BrickDirection Bdirection { get; set; }

        public Settings()
        {
            Width = 16;
            Height = 16;
            Speed = 16;
            Score = 0;
            Points = 100;
            BonusPoints = 300;
            GameOver = false;
            direction = Direction.Down;

            // Set lives to 3
            Lives = 3;
            MoveLeft = MAX_MOVE;
        }
    }


}
