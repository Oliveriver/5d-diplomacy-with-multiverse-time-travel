public class Board {
    private final int xPos;
    private final int yPos;
    private boolean active;

    public Board(int x, int y)
    {
        xPos = x;
        yPos = y;
        active = true;
    }

    public void setActive(boolean isActive)
    {
        active = isActive;
    }

    public int[] getPosition()
    {
        return new int[] {xPos, yPos};
    }

    public String toString()
    {
        return (active ? "ACTIVE" : "INACTIVE" ) + " board at (" + xPos + "," + yPos + ")";
    }

    public boolean isActive()
    {
        return active;
    }
}
