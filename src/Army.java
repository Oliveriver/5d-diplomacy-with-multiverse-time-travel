public class Army {
    private final int xPos;
    private final int yPos;
    private final int zPos;
    private final Player owner;

    public Army(int x, int y, int z, Player player)
    {
        xPos = x;
        yPos = y;
        zPos = z;
        owner = player;
    }

    public Army(int[] location, Player player)
    {
        xPos = location[0];
        yPos = location[1];
        zPos = location[2];
        owner = player;
    }

    public int[] getLocation()
    {
        return new int[] {xPos, yPos, zPos};
    }

    public Player getOwner()
    {
        return owner;
    }

    public String toString()
    {
        return owner + " army at (" + xPos + "," + yPos + "," + zPos + ")";
    }
}
