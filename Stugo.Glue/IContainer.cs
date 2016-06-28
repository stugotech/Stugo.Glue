using System;

namespace Stugo.Glue
{
    public interface IContainer
    {
        TAbstract Resolve<TAbstract>();
        object Resolve(Type abstractType);
    }
}
