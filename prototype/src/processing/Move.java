package processing;

public class Move extends Order {
    private int destinationX;
    private int destinationY;
    private int destinationZ;
    private int strength = 1;
    private boolean success;

    public void addStrength(int support)
    {
        strength += support;
    }

    public int getStrength()
    {
        return strength;
    }

    public void setDestination(String[] destination)
    {
        destinationX = Integer.parseInt(destination[0]);
        destinationY = Integer.parseInt(destination[1]);
        destinationZ = Integer.parseInt(destination[2]);
    }

    public void setDestination(int[] destination)
    {
        destinationX = destination[0];
        destinationY = destination[1];
        destinationZ = destination[2];
    }

    public int[] getDestination()
    {
        return new int[] {destinationX, destinationY, destinationZ};
    }

    public String toString()
    {
        String successValue = success ? " SUCCEEDS" : "";
        return player + " (" + locationX + "," + locationY + "," + locationZ + ") MOVE ("
                + destinationX + "," + destinationY + "," + destinationZ + ") STRENGTH " + strength + successValue;
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
