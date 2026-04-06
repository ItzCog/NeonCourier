public class Modifier
{
    public enum Type { Flat, Percent }
    public Type type;
    public int amount;
    
    public Modifier(Type type, int amount = 1)
    {
        this.type = type;
        this.amount = amount;
    }
}