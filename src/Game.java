public class Game
{
    public static void main(String[] args)
    {
        Board board = new Board();
        board.display();

        for (int i = 0; i < 10; i++)
        {
            board = board.generateNextBoard();
            board.display();
        }
    }
}
