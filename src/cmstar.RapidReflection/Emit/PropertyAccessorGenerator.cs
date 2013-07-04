using System;
using System.Reflection;

namespace cmstar.RapidReflection.Emit
{
    public static class PropertyAccessorGenerator
    {
        /// <summary>
        /// Creates a dynamic method for getting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer or 
        /// the get accessor method from <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo)
        {
            return CreateGetter(propertyInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for getting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property getter method.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer or 
        /// the get accessor method from <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException(
                   "Cannot create a dynamic getter for an indexed property.",
                   "propertyInfo");
            }

            var getMethod = propertyInfo.GetGetMethod(nonPublic);
            if (getMethod == null)
            {
                if (nonPublic)
                {
                    throw new ArgumentException(
                        "The property does not have a get method.", "propertyInfo");
                }

                throw new ArgumentException(
                    "The property does not have a publice get method.", "propertyInfo");
            }

            var declaringType = propertyInfo.DeclaringType;
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Get" + propertyInfo.Name,
                typeof(object),
                new[] { typeof(object) },
                declaringType);
            var il = dynamicMethod.GetILGenerator();

            if (!getMethod.IsStatic)
            {
                il.Ldarg_0();
                il.CastReference(declaringType);
            }

            il.CallMethod(getMethod);
            il.BoxIfNeeded(propertyInfo.PropertyType);
            il.Ret();

            return (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer or
        /// the set accessor method from the <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        /// <remarks>
        /// In order to set a property value on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(PropertyInfo propertyInfo)
        {
            return CreateSetter(propertyInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property setter method.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer or
        /// the set accessor method from the <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        /// <remarks>
        /// In order to set a property value on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException(
                   "Cannot create a dynamic setter for an indexed property.",
                   "propertyInfo");
            }

            var setMethod = propertyInfo.GetSetMethod(nonPublic);
            if (setMethod == null)
            {
                if (nonPublic)
                {
                    throw new ArgumentException(
                        "The property does not have a set method.", "propertyInfo");
                }

                throw new ArgumentException(
                    "The property does not have a publice set method.", "propertyInfo");
            }

            var propType = propertyInfo.PropertyType;
            var declaringType = propertyInfo.DeclaringType;
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Set" + propertyInfo.Name,
                null,
                new[] { typeof(object), typeof(object) },
                declaringType);
            var il = dynamicMethod.GetILGenerator();

            //copy the value to a local variable
            il.DeclareLocal(propType);
            il.Ldarg_1();
            il.CastValue(propType);
            il.Stloc_0();

            if (!setMethod.IsStatic)
            {
                il.Ldarg_0();
                il.CastReference(declaringType);
            }

            il.Ldloc_0();
            il.CallMethod(setMethod);
            il.Ret();

            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }
    }
}
