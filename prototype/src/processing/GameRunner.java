package processing;

import javax.swing.*;
import java.util.ArrayList;
import java.util.Scanner;

public class GameRunner {
    public static void main(String[] args)
    {
        Scanner input = new Scanner(System.in);

        JFrame frame = new JFrame();
        frame.setSize(1920, 1080);
        frame.setTitle("5D Diplomacy With Multiverse Time Travel");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.setVisible(true);

        Game game = new Game();
        game.displayText();
        game.displayGraphics(frame);

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
            game.displayText();
            game.displayGraphics(frame);
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
                game.displayText();
                game.displayGraphics(frame);
            }
        }
    }
}
