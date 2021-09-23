public class Support extends Order {
    private int supportLocationX;
    private int supportLocationY;
    private int supportLocationZ;
    private int supportDestinationX;
    private int supportDestinationY;
    private int supportDestinationZ;

    public void setSupportLocation(String[] location)
    {
        supportLocationX = Integer.parseInt(location[0]);
        supportLocationY = Integer.parseInt(location[1]);
        supportLocationZ = Integer.parseInt(location[2]);
    }

    public void setSupportDestination(String[] destination)
    {
        supportDestinationX = Integer.parseInt(destination[0]);
        supportDestinationY = Integer.parseInt(destination[1]);
        supportDestinationZ = Integer.parseInt(destination[2]);
    }

    public int[] getSupportLocation()
    {
        return new int[] {supportLocationX, supportLocationY, supportLocationZ};
    }

    public int[] getSupportDestination()
    {
        return new int[] {supportDestinationX, supportDestinationY, supportDestinationZ};
    }

    public String toString()
    {
        return player + " (" + locationX + "," + locationY + "," + locationZ + ") SUPPORT ("
                + supportLocationX + "," + supportLocationY + "," + supportLocationZ + ") TO ("
                + supportDestinationX + "," + supportDestinationY + "," + supportDestinationZ + ")";
    }
}
