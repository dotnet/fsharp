using System;

namespace Ninject.Planning.Bindings
{
    public interface IRequest
    {
    }

    public interface IBinding
    {
        Type Service { get; }
        Func<IRequest, bool> Condition { get; set; }
    }

    public class Binding : IBinding
    {
        public Type Service { get; private set; }
        public Func<IRequest, bool> Condition { get; set; }
    }

    public class Request : IRequest
    {
    }
}
