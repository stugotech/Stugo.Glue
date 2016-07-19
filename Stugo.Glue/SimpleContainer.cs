using Stugo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stugo.Glue
{
    public sealed class SimpleContainer : IContainer, IContainerRegistration
    {
        private static readonly ILog logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<Type, Func<Type, object>> resolvers = new Dictionary<Type, Func<Type, object>>();


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
            try
            {
                logger.Trace($"Resolve type {abstractType.FullName}");
                object implementation;

                if (abstractType.IsAbstract)
                {
                    implementation = resolvers[abstractType](abstractType);
                    logger.Trace($"Type {abstractType.FullName} resolved to {implementation?.GetType()?.FullName ?? "null"}");
                }
                else
                {
                    implementation = Construct(abstractType);
                    logger.Trace($"Type {abstractType.FullName} constructed");
                }

                return implementation;
            }
            catch (Exception e)
            {
                logger.Error($"Error while resolving type {abstractType.FullName}", e);
                throw;
            }
        }


        /// <summary>
        /// Registers the specified concrete type to be resolved for the given abstract type.
        /// </summary>
        public void Register<TAbstract, TConcrete>(bool singleton = false)
            where TConcrete : class, TAbstract
        {
            logger.Trace($"Register {(singleton?"singleton ":"")} implementation {typeof(TConcrete).FullName} for interface {typeof(TAbstract).FullName}");
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
            logger.Trace($"Register singleton instance {instance?.GetType()?.FullName ?? "null"} for interface {typeof(TAbstract).FullName}");
            Register<TAbstract>(t => instance);
        }


        /// <summary>
        /// Registers the given resolver function to be used for the specified type.
        /// </summary>
        public void Register<TAbstract>(Func<Type, object> resolver)
        {
            resolvers[typeof(TAbstract)] = resolver;
        }


        /// <summary>
        /// Constructs the specified type by resolving the parameter types.
        /// </summary>
        private TConcrete Construct<TConcrete>()
            where TConcrete : class
        {
            return (TConcrete)Construct(typeof(TConcrete));
        }


        /// <summary>
        /// Constructs the specified type by resolving the parameter types.
        /// </summary>
        private object Construct(Type type)
        {
            logger.Trace($"Constructing type {type.FullName}");
            var constructors = type.GetConstructors();
            logger.Trace($"Type has {constructors.Length} constructors");

            if (constructors.Length != 1)
            {
                logger.Error($"Can't construct type {type.FullName} because it has {constructors.Length} constructors");
                throw new ArgumentException($"Can't construct type {type.FullName} because it has {constructors.Length} constructors");
            }

            var constructor = constructors.Single();
            logger.Trace($"Using constructor {constructor}");
            var args = constructor.GetParameters().Select(param => Resolve(param.ParameterType));
            return Activator.CreateInstance(type, args.ToArray());
        }
    }
}
