namespace testUniveralApp
{
    public class GameData
    {
        public int xMove { get; set; }
        public int yMove { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int player1Pos { get; set; }
        public int player2Pos { get; set; }

        public GameData()
        {
            xMove = 1;
            yMove = 1;
            xPos = 50;
            yPos = 50;
            player1Pos = 50;
            player2Pos = 50;
        }
    }
}