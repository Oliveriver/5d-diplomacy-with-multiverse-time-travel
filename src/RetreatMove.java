public class RetreatMove extends Retreat {
    private int destinationX;
    private int destinationY;
    private int destinationZ;
    private boolean success;

    public void setDestination(String[] destination)
    {
        destinationX = Integer.parseInt(destination[0]);
        destinationY = Integer.parseInt(destination[1]);
        destinationZ = Integer.parseInt(destination[2]);
    }

    public int[] getDestination()
    {
        return new int[] {destinationX, destinationY, destinationZ};
    }

    public String toString()
    {
        String successValue = success ? " SUCCEEDS" : "";
        return player + " (" + locationX + "," + locationY + "," + locationZ + ") MOVE ("
                + destinationX + "," + destinationY + "," + destinationZ + ")";
    }

    public void setSuccessful(boolean successful)
    {
        success = successful;
    }

    public boolean isSuccessful()
    {
        return success;
    }
}
