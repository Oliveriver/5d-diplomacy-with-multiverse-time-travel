import java.util.ArrayList;
import java.util.HashMap;

public class Board
{
    private final int turn;
    private ArrayList<Province> provinces = new ArrayList<>();
    private final int centreX;
    private final int centreY;

    public Board(int centreX, int centreY, int turn)
    {
        this.centreX = centreX;
        this.centreY = centreY;
        this.turn = turn;

        provinces.add(new Province("Blue", new Army(Player.BLUE), Player.BLUE));
        provinces.add(new Province("Neutral"));
        provinces.add(new Province("Orange", new Army(Player.ORANGE), Player.ORANGE));
    }

    public Board()
    {
        this(0, 0, 0);
    }

    public void display()
    {
        System.out.println(getTurnName());
        for (int i = 0; i < 3; i++)
        {
            Province province = provinces.get(i);
            System.out.println("The " + province.getName() + " province is controlled by " + province.getControl() + " and is " +
                    (province.isOccupied() ? "occupied by " + province.getArmy().getPlayer() + ".": "not occupied." ));
        }
    }

    public Board generateNextBoard()
    {
        return new Board(centreX + 1, centreY, turn + 1);
    }

    public String getTurnName()
    {
        String turnName = turn % 2 == 0 ? "SPRING " : "FALL ";
        int gameYear = 1901 + turn / 2;
        turnName += gameYear;
        return turnName;
    }
}
