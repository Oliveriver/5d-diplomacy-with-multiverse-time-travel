public class Support extends Order
{
    private final Province supportedUnit;
    private final Province destination;

    public Support(Province location, Province supportedUnit, Province destination) {
        super(location);
        this.supportedUnit = supportedUnit;
        this.destination = destination;
    }
}
