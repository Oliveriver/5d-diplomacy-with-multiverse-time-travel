import java.util.ArrayList;
import java.util.Scanner;

public class Game
{
    public static void main(String[] args)
    {
        Scanner input = new Scanner(System.in);

        Board board = new Board();
        board.display();

        System.out.println("\nENTER ORDERS");
        String order = input.nextLine();
        ArrayList<String> orders = new ArrayList<>();
        while (!order.equals("r"))
        {
            orders.add(order);
            order = input.nextLine();
        }
        board.advanceTurn(orders);
        board.display();
    }
}
