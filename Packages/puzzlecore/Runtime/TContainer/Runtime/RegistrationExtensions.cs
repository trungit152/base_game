using System;

namespace trungnhd.puzzlecore.TContainer
{
    public static class RegistrationExtensions 
    {
        public static void Register<TImpl>(this IContainerBuilder b, Lifetime l)
            => b.Add(new Registration{ServiceType=typeof(TImpl),ImplementationType=typeof(TImpl),Lifetime=l});

        public static void Register<TFace,TImpl>(this IContainerBuilder b, Lifetime l) where TImpl:TFace
            => b.Add(new Registration{ServiceType=typeof(TFace),ImplementationType=typeof(TImpl),Lifetime=l});

        public static void Register<T>(this IContainerBuilder b, Func<IObjectResolver,T> f, Lifetime l)
            => b.Add(new Registration{ServiceType=typeof(T),Factory=r=>f(r),Lifetime=l});

        public static void RegisterInstance<T>(this IContainerBuilder b, T instance)
            => b.Add(new Registration{ServiceType=typeof(T),Instance=instance,Lifetime=Lifetime.Singleton});

        public static void RegisterEntryPoint<T>(this IContainerBuilder b)
            => b.Add(new Registration{ServiceType=typeof(T),ImplementationType=typeof(T),
                Lifetime=Lifetime.Singleton,IsEntryPoint=true});
    }
}