package old;

import java.util.ArrayList;

public class Board
{
    private int turn;
    private ArrayList<Province> provinces = new ArrayList<>();
    private int centreX;
    private int centreY;

    public Board(int centreX, int centreY, int turn)
    {
        this.centreX = centreX;
        this.centreY = centreY;
        this.turn = turn;

        provinces.add(new Province(ProvinceName.BLUE, new Army(Player.BLUE), Player.BLUE));
        provinces.add(new Province(ProvinceName.NEUTRAL));
        provinces.add(new Province(ProvinceName.ORANGE, new Army(Player.ORANGE), Player.ORANGE));
    }

    public Board()
    {
        this(0, 0, 0);
    }

    public void display()
    {
        System.out.println("SETUP FOR " + getTurnName());
        for (int i = 0; i < 3; i++)
        {
            Province province = provinces.get(i);
            System.out.println("The " + province.getName() + " province is controlled by " + province.getControl() + " and is " +
                    (province.isOccupied() ? "occupied by " + province.getArmy().getPlayer() + ".": "not occupied." ));
        }
    }

    // TODO: old.Move to old.Game rather than old.Board to create a new old.Board using the old old.Board's parameters
    public void advanceTurn(ArrayList<String> ordersText)
    {
        turn++;
        centreX++;

        ArrayList<Order> orders = new ArrayList<>();
        for (String orderText : ordersText)
        {
            Order order;
            String[] words = orderText.split(" ");
            switch (words[1])
            {
                case "m":
                    try
                    {
                        order = new Move(mapProvince(words[0]), mapProvince(words[2]));
                        orders.add(order);
                    }
                    catch (InvalidOrderException e)
                    {
                        System.out.println("INVALID ORDER " + orderText);
                    }
                    break;
                case "h":
                    try
                    {
                        order = new Hold(mapProvince(words[0]));
                        orders.add(order);
                    }
                    catch (InvalidOrderException e) {
                        System.out.println("INVALID ORDER " + orderText);
                    }
                    break;
                case "s":
                    try
                    {
                        if (words.length > 3)
                        {
                            order = new Support(mapProvince(words[0]), mapProvince(words[2]), mapProvince(words[3]));
                        }
                        else
                        {
                            order = new Support(mapProvince(words[0]), mapProvince(words[2]), mapProvince(words[2]));
                        }
                        orders.add(order);
                    }
                    catch (InvalidOrderException e)
                    {
                        System.out.println("INVALID ORDER " + orderText);
                    }
                    break;
                default:
                    System.out.println("INVALID ORDER " + orderText);
            }
        }
    }

    public String getTurnName()
    {
        String turnName = turn % 2 == 0 ? "SPRING " : "FALL ";
        int gameYear = 1901 + turn / 2;
        turnName += gameYear;
        return turnName;
    }

    private Province mapProvince(String name) throws InvalidOrderException {
        switch (name)
        {
            case "blu":
                return provinces.get(0);
            case "neu":
                return provinces.get(1);
            case "ora":
                return provinces.get(2);
            default:
                System.out.println("INVALID ORDER - " + name + " is not a valid province name!");
                throw new InvalidOrderException();
        }
    }
}
