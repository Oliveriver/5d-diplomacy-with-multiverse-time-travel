public class Move extends Order
{
    private final Province destination;

    public Move(Province location, Province destination) {
        super(location);
        this.destination = destination;
    }
}
