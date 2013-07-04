using System;
using System.Reflection;

namespace cmstar.RapidReflection.Emit
{
    public static class FieldAccessorGenerator
    {
        /// <summary>
        /// Creates a dynamic method for getting the value of the given field.
        /// </summary>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        public static Func<object, object> CreateGetter(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var declaringType = fieldInfo.DeclaringType;
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Get" + fieldInfo.Name,
                typeof(object),
                new[] { typeof(object) },
                declaringType);
            var il = dynamicMethod.GetILGenerator();

            if (fieldInfo.IsStatic)
            {
                il.Ldsfld(fieldInfo);
            }
            else
            {
                il.Ldarg_0();
                il.CastReference(declaringType);
                il.Ldfld(fieldInfo);
            }

            il.BoxIfNeeded(fieldInfo.FieldType);
            il.Ret();

            return (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given field.
        /// </summary>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        /// <remarks>
        /// In order to set a field on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var declaringType = fieldInfo.DeclaringType;
            var fieldType = fieldInfo.FieldType;
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Set" + fieldInfo.Name,
                null,
                new[] { typeof(object), typeof(object) },
                declaringType);
            var il = dynamicMethod.GetILGenerator();

            //copy the value to a local variable
            il.DeclareLocal(fieldType);
            il.Ldarg_1();
            il.CastValue(fieldType);
            il.Stloc_0();

            if (fieldInfo.IsStatic)
            {
                il.Ldloc_0();
                il.Stsfld(fieldInfo);
            }
            else
            {
                il.Ldarg_0();
                il.CastReference(declaringType);
                il.Ldloc_0();
                il.Stfld(fieldInfo);
            }

            il.Ret();
            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }
    }
}
