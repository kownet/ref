using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ref.Shared.Extensions
{
    public static class AssemblyExt
    {
        public static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
            => assembly.GetTypesAssignableFrom(typeof(T));

        public static List<Type> GetTypesAssignableFrom(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (compareType.IsAssignableFrom(type) && compareType != type)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }
    }
}