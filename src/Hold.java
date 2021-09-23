public class Hold extends Order {
    private int strength = 1;

    public void addStrength(int support)
    {
        strength += support;
    }

    public int getStrength()
    {
        return strength;
    }

    public String toString()
    {
        return player + " (" + locationX + "," + locationY + "," + locationZ + ") HOLD STRENGTH " + strength;
    }
}
