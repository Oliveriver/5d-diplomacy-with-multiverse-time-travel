import java.util.ArrayList;
import java.util.Scanner;

public class GameRunner {
    public static void main(String[] args)
    {
        Scanner input = new Scanner(System.in);

        Game game = new Game();
        game.display();

        while (true)
        {
            System.out.println("\nEnter orders:");
            String order = input.nextLine();
            ArrayList<String> orders = new ArrayList<>();
            while (!order.equals("r"))
            {
                orders.add(order);
                order = input.nextLine();
            }
            ArrayList<Order> parsedOrders = game.parseOrders(orders);
            game.resolveOrders(parsedOrders, false);
            game.display();
            if (game.isRetreatNeeded())
            {
                System.out.println("\nEnter retreats:");
                String retreat = input.nextLine();
                ArrayList<String> retreats = new ArrayList<>();
                while (!retreat.equals("r"))
                {
                    retreats.add(retreat);
                    retreat = input.nextLine();
                }
                ArrayList<Retreat> parsedRetreats = game.parseRetreats(retreats);
                game.resolveRetreats(parsedRetreats);
                game.display();
            }
        }
    }
}
