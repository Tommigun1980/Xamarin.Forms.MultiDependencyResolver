# Xamarin.Forms.MultiDependencyResolver
*Adds support for multiple implementations to Xamarin.Forms's DI system*

NuGet package available at https://www.nuget.org/packages/Xamarin.Forms.MultiDependencyResolver/

## Intro

Xamarin.Forms's DI system is quite limited, where one of the major missing features is support for registering multiple implementations for an interface.
ASP.NET Core (https://edi.wang/post/2018/12/28/dependency-injection-with-multiple-implementations-in-aspnet-core) and many other implementations support this feature.

This package adds support for multiple implementations to Xamarin.Forms's built-in DI system by extending its existing functionality.

## Usage

### Setup

1) Register IMultiDependencyResolver with Xamarin's DI system:

```c#
DependencyService.Register<MultiDependencyResolver>();
```

2) Create a shared interface for the relevant services:

```c#
interface IMySharedInterface { }
```

And implement it in your implementations normally:

```c#
class MyService1 : IMySharedInterface { }
class MyService2 : IMySharedInterface { }
```

3) Register your dependencies with Xamarin's DI system normally:

```c#
DependencyService.Register<MyService1>();
DependencyService.Register<MyService2>();
```

4) Register your implementations with the multi dependency resolver:

```c#
var multiDependencyResolver = DependencyService.Get<IMultiDependencyResolver>();

multiDependencyResolver.Register<IMySharedInterface, MyService1>();
multiDependencyResolver.Register<IMySharedInterface, MyService2>();
```

### Resolving implementations

There are two ways of getting the enumeration of implementations for any registered interface.

1) Get them from the multi dependency resolver directly:

```c#
var implementations = DependencyService.Get<IMultiDependencyResolver>().Get<IMySharedInterface>();
```

2) Get them from the Xamarin.Forms's built-in DI container:

```c#
var implementations = DependencyService.Resolve<IEnumerable<IMySharedInterface>>();
```

Note that you have to use DependencyService.Resolve<Type>(); DependencyService.Get<Type>() won't work.

In order to use this second method, you have to enable it by calling this in your code once:

```c#
DependencyService.Get<IMultiDependencyResolver>().RegisterDependencyServiceResolver();
```

This registers a Xamarin.Forms DependencyResolver (to allow piggybacking on the DependencyService), so if your app requires the DependencyResolver for something else, method 1 is recommended.
