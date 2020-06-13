using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms
{
    /**
     * Adds multi implementation support to Xamarin.Form's DI system.
     * 
     * Usage:
     * Register IMultiDependencyResolver with Xamarin's DI system:
     * DependencyService.Register<MultiDependencyResolver>();
     * 
     * Create a shared interface for the relevant services:
     * interface IMySharedInterface { }
     * And implement it in your implementations normally:
     * class MyService1 : IMySharedInterface { }
     * class MyService2 : IMySharedInterface { }
     * 
     * Register your dependencies with Xamarin's DI system normally:
     * DependencyService.Register<MyService1>();
     * DependencyService.Register<MyService2>();
     * 
     * Register your implementations with the multi dependency resolver:
     * DependencyService.Get<IMultiDependencyResolver>().Register<IMySharedInterface, MyService1>();
     * DependencyService.Get<IMultiDependencyResolver>().Register<IMySharedInterface, MyService2>();
     * 
     * To get implementations, call:
     * var implementations = DependencyService.Get<IMultiDependencyResolver>().Get<IMySharedInterface>();
     * 
     * Or alternatively, call this once in your code:
     * DependencyService.Get<IMultiDependencyResolver>().RegisterDependencyServiceResolver();
     * 
     * ... after which you can get the implementations directly from Xamarin's DI system like so:
     * var implementations = DependencyService.Resolve<IEnumerable<IMySharedInterface>>();
     * */
    public interface IMultiDependencyResolver
    {
        void Register<TSharedInterface, TImpl>() where TSharedInterface : class where TImpl : class, TSharedInterface;
        IEnumerable<TSharedInterface> Get<TSharedInterface>(DependencyFetchTarget fetchTarget = DependencyFetchTarget.GlobalInstance) where TSharedInterface : class;

        void RegisterDependencyServiceResolver();
    }
}
