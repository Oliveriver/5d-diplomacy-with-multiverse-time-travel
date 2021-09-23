package old;

public class Province
{
    private final ProvinceName name;
    private Army occupyingArmy;
    private Player controllingPlayer;

    public Province(ProvinceName name)
    {
        this.name = name;
        controllingPlayer = Player.NONE;
    }

    public Province(ProvinceName name, Army army)
    {
        this(name);
        occupyingArmy = army;
    }

    public Province(ProvinceName name, Army army, Player player)
    {
        this(name, army);
        controllingPlayer = player;
    }

    public ProvinceName getName()
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
