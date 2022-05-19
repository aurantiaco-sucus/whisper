using Whisper.Structure;

namespace Whisper.Interpreter;

public class EnvFrame
{
    public EnvFrame? Parent { get; }
    public Dictionary<string, Element> Bindings { get; }

    public EnvFrame(EnvFrame? parent, Dictionary<string, Element>? bindings)
    {
        Parent = parent;
        Bindings = bindings ?? new Dictionary<string, Element>();
    }
    
    /**
     * TODO: (!) This function has a problem which may prevent compilation mechanisms to work correctly. (!)
     * This type of simple way to iterate through frame chains actually breaks the environment model by assuming
     * only one environment exists all the time and calling functions just derive the caller's frame. This allows
     * the callee to mutate the variables in the scope of caller and cause major issues.
     */
    public Binding? Lookup(string name)
    {
        var current = this;
        while (current != null)
        {
            if (current.Bindings.TryGetValue(name, out var element))
            {
                return new Binding(
                    () => current.Bindings[name], 
                    (value) => current.Bindings[name] = value);
            }
            current = current.Parent;
        }
        return null;
    }
}