package processing;

public enum Player {
    BLUE,
    ORANGE,
    NEUTRAL;

    public Player opposite()
    {
        if (this == Player.BLUE)
            return Player.ORANGE;
        if (this == Player.ORANGE)
            return Player.BLUE;
        return Player.NEUTRAL;
    }
}
