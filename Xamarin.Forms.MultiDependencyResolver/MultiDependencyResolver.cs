using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
    public class MultiDependencyResolver : IMultiDependencyResolver
    {
        private readonly IDictionary<Type, IList<Type>> dependencyImplementations = new Dictionary<Type, IList<Type>>();

        public void Register<TSharedInterface, TImpl>() where TSharedInterface : class where TImpl : class, TSharedInterface
        {
            Type type = typeof(TSharedInterface);
            Type implementorType = typeof(TImpl);

            IList<Type> implementationTypes;
            if (!this.dependencyImplementations.TryGetValue(type, out implementationTypes))
            {
                implementationTypes = new List<Type>();
                this.dependencyImplementations.Add(type, implementationTypes);
            }

            implementationTypes.Add(implementorType);
        }

        public IEnumerable<TSharedInterface> Get<TSharedInterface>(DependencyFetchTarget fetchTarget = DependencyFetchTarget.GlobalInstance) where TSharedInterface : class
        {
            Type type = typeof(TSharedInterface);

            IList<Type> implementationTypes;
            if (!this.dependencyImplementations.TryGetValue(type, out implementationTypes))
                return null;

            var dependencyGetMi = typeof(DependencyService).GetMethod(nameof(DependencyService.Get), BindingFlags.Public | BindingFlags.Static);
            return implementationTypes.Select(t =>
            {
                var dependencyGetSpecificTypeMi = dependencyGetMi.MakeGenericMethod(t);
                return dependencyGetSpecificTypeMi.Invoke(null, new object[1]) as TSharedInterface;
            })
            .Where(t => t != null)
            .ToList();
        }

        public void RegisterDependencyServiceResolver()
        {
            DependencyResolver.ResolveUsing(t =>
            {
                if (t.IsGenericType && typeof(IEnumerable).IsAssignableFrom(t))
                {
                    var type = t.GenericTypeArguments.FirstOrDefault();
                    if (type != null && this.dependencyImplementations.ContainsKey(type))
                    {
                        var multiDependencyGetMi = typeof(MultiDependencyResolver).GetMethod(nameof(MultiDependencyResolver.Get));
                        var multiDependencyGetSpecificTypeMi = multiDependencyGetMi.MakeGenericMethod(type);
                        return multiDependencyGetSpecificTypeMi.Invoke(this, new object[1]);
                    }
                }
                return null;
            });
        }
    }
}
