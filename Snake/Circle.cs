namespace Snake
{
    class Circle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Gvalue { get; set; }
        public int Hvalue { get; set; }
        public int Fvalue { get; set; }
        public Circle parent;       // class is reference type

        public Circle ()
        {
            X = 0;
            Y = 0;
            Gvalue = 0;
            Hvalue = 0;
            Fvalue = 0;
        }
    }
}
