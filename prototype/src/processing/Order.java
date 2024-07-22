package processing;

public class Order {
    protected int locationX;
    protected int locationY;
    protected int locationZ;
    protected Player player;

    public void setLocation(String[] location)
    {
        locationX = Integer.parseInt(location[0]);
        locationY = Integer.parseInt(location[1]);
        locationZ = Integer.parseInt(location[2]);
    }

    public void setLocation(int[] location)
    {
        locationX = location[0];
        locationY = location[1];
        locationZ = location[2];
    }

    public void setPlayer(Player owner)
    {
        player = owner;
    }

    public int[] getLocation()
    {
        return new int[] {locationX, locationY, locationZ};
    }

    public Player getPlayer()
    {
        return player;
    }
}
