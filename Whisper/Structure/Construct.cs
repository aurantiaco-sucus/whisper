namespace Whisper.Structure;

public class Construct : Element.MutableBoxElement<Tuple<Element, Element>>
{
    public override Tuple<Element, Element> Data => Tuple.Create(Car, Cdr);
    
    public Element Car { get; private set; }
    public Element Cdr { get; private set; }

    public Construct(Element car, Element cdr)
    {
        Car = car;
        Cdr = cdr;
    }

    public override void Mutate(Tuple<Element, Element> data)
    {
        (Car, Cdr) = data;
    }
    
    public void MutateCar(Element car)
    {
        Car = car;
    }
    
    public void MutateCdr(Element cdr)
    {
        Cdr = cdr;
    }
}