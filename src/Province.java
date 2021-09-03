public class Province
{
    private final String name;
    private Army occupyingArmy;
    private Player controllingPlayer;

    public Province(String name)
    {
        this.name = name;
        controllingPlayer = Player.NONE;
    }

    public Province(String name, Army army)
    {
        this(name);
        occupyingArmy = army;
    }

    public Province(String name, Army army, Player player)
    {
        this(name, army);
        controllingPlayer = player;
    }

    public String getName()
    {
        return name;
    }

    public boolean isOccupied()
    {
        return occupyingArmy != null;
    }

    public Player getControl()
    {
        return controllingPlayer;
    }

    public Army getArmy()
    {
        return occupyingArmy;
    }
}
