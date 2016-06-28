using System;
using System.Collections.Generic;
using System.Linq;

namespace Stugo.Glue
{
    public sealed class SimpleContainer : IContainer, IContainerRegistration
    {
        private readonly Dictionary<Type, Func<Type, object>> resolvers = new Dictionary<Type, Func<Type, object>>();


        /// <summary>
        /// Constructs the specified type by resolving the parameter types.
        /// </summary>
        public TConcrete Construct<TConcrete>()
            where TConcrete : class
        {
            return (TConcrete)Construct(typeof(TConcrete));
        }


        /// <summary>
        /// Constructs the specified type by resolving the parameter types.
        /// </summary>
        public object Construct(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    string.Format("The type '{0}' does not have exactly one public constructor",
                        type.FullName));
            }

            var constructor = constructors.Single();
            var args = constructor.GetParameters().Select(param => Resolve(param.ParameterType));
            return Activator.CreateInstance(type, args.ToArray());
        }


        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        public TAbstract Resolve<TAbstract>()
        {
            return (TAbstract)Resolve(typeof(TAbstract));
        }


        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        public object Resolve(Type abstractType)
        {
            if (abstractType.IsAbstract)
                return GetResolver(abstractType)(abstractType);
            else
                return Construct(abstractType);
        }


        /// <summary>
        /// Registers the specified concrete type to be resolved for the given abstract type.
        /// </summary>
        public void Register<TAbstract, TConcrete>(bool singleton = false)
            where TConcrete : class, TAbstract
        {
            Func<Type, object> resolver;

            if (singleton)
            {
                TConcrete instance = null;
                object monitor = new object();

                resolver = t =>
                {
                    lock (monitor)
                    {
                        if (instance == null)
                            instance = Construct<TConcrete>();
                        return instance;
                    }
                };
            }
            else
            {
                resolver = t => Construct<TConcrete>();
            }

            Register<TAbstract>(resolver);
        }


        /// <summary>
        /// Registers the given instance to be resolved for the specified type.
        /// </summary>
        public void Register<TAbstract>(TAbstract instance)
        {
            Register<TAbstract>(t => instance);
        }


        /// <summary>
        /// Registers the given resolver function to be used for the specified type.
        /// </summary>
        public void Register<TAbstract>(Func<Type, object> resolver)
        {
            resolvers[typeof(TAbstract)] = resolver;
        }


        private Func<Type, object> GetResolver(Type type)
        {
            return resolvers[type];
        }
    }
}
