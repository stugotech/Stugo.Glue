using System;

namespace Stugo.Glue
{
    public interface IContainerRegistration
    {
        void Register<TAbstract, TConcrete>(bool singleton = false) where TConcrete : class, TAbstract;
        void Register<TAbstract>(TAbstract instance);
        void Register<TAbstract>(Func<Type, object> resolver);
    }
}
