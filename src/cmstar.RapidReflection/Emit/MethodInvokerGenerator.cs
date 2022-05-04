using System;
using System.Reflection;

namespace cmstar.RapidReflection.Emit
{
    public static class MethodInvokerGenerator
    {
        /// <summary>
        /// Creates a dynamic method for invoking the method from the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">
        /// The instance of <see cref="MemberInfo"/> from which the dynamic method is to be created.
        /// </param>
        /// <returns>
        /// The delegate has two parameters: the first for the object instance (will be ignored 
        /// if the method is static), and the second for the arguments of the method (will be 
        /// ignored if the method has no arguments)/
        /// The return value of the delegate will be <c>null</c> if the method has no return value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo"/> is null.</exception>
        public static Func<object, object[], object> CreateDelegate(MethodInfo methodInfo)
        {
            return CreateDelegate(methodInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for invoking the method from the given <see cref="MethodInfo"/>
        /// and indicates whether to perform a arguments validation in the dynamic method.
        /// </summary>
        /// <param name="methodInfo">
        /// The instance of <see cref="MemberInfo"/> from which the dynamic method is to be created.
        /// </param>
        /// <param name="validateArguments">
        /// If <c>true</c>, the dynamic method will validate if the instance or the array of arguments 
        /// is null and check the length of the array to avoid the exceptions such as 
        /// <see cref="NullReferenceException"/> or <see cref="IndexOutOfRangeException"/>,
        /// an <see cref="ArgumentNullException"/> or <see cref="ArgumentException"/> will be thrown instead.
        /// </param>
        /// <returns>
        /// The delegate has two parameters: the first for the object instance (will be ignored 
        /// if the method is static), and the second for the arguments of the method (will be 
        /// ignored if the method has no arguments)/
        /// The return value of the delegate will be <c>null</c> if the method has no return value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo"/> is null.</exception>
        public static Func<object, object[], object> CreateDelegate(
            MethodInfo methodInfo, bool validateArguments)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var identity = new { methodInfo, validateArguments };
            var method = (Func<object, object[], object>)DelegateCache.GetOrAdd(
                identity, x => DoCreateDelegate(methodInfo, validateArguments));

            return method;
        }

        private static Func<object, object[], object> DoCreateDelegate(MethodInfo methodInfo, bool validateArguments)
        {
            var args = methodInfo.GetParameters();
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Call" + methodInfo.Name,
                typeof(object),
                new[] { typeof(object), typeof(object[]) },
                methodInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            var labelValidationCompleted = il.DefineLabel();
            if (!validateArguments || (methodInfo.IsStatic && args.Length == 0))
            {
                il.Br_S(labelValidationCompleted); //does not need validation
            }
            else
            {
                var labelCheckArgumentsRef = il.DefineLabel();
                var labelCheckArgumentsLength = il.DefineLabel();

                //check if the instance is null
                if (!methodInfo.IsStatic)
                {
                    // if (instance == null) throw new ArgumentNullException("instance");
                    il.Ldarg_0();
                    il.Brtrue_S(args.Length > 0 ? labelCheckArgumentsRef : labelValidationCompleted);

                    il.ThrowArgumentsNullException("instance");
                }

                //check the arguments
                if (args.Length > 0)
                {
                    // if (arguments == null) throw new ArgumentNullException("arguments");
                    il.MarkLabel(labelCheckArgumentsRef);
                    il.Ldarg_1();
                    il.Brtrue_S(labelCheckArgumentsLength);

                    il.ThrowArgumentsNullException("arguments");

                    // if (arguments.Length < $(args.Length)) throw new ArgumentNullException(msg, "arguments");
                    il.MarkLabel(labelCheckArgumentsLength);
                    il.Ldarg_1();
                    il.Ldlen();
                    il.Conv_I4();
                    il.LoadInt32(args.Length);
                    il.Bge_S(labelValidationCompleted);

                    il.ThrowArgumentsException("Not enough arguments in the argument array.", "arguments");
                }
            }

            il.MarkLabel(labelValidationCompleted);
            if (!methodInfo.IsStatic)
            {
                il.Ldarg_0();
                il.CastReference(methodInfo.DeclaringType);
            }

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    il.Ldarg_1();
                    il.LoadInt32((short)i);
                    il.Ldelem_Ref();
                    il.CastValue(args[i].ParameterType);
                }
            }

            il.CallMethod(methodInfo);
            if (methodInfo.ReturnType == typeof(void))
            {
                il.Ldc_I4_0(); //return null
            }
            else
            {
                il.BoxIfNeeded(methodInfo.ReturnType);
            }
            il.Ret();

            var methodDelegate = dynamicMethod.CreateDelegate(typeof(Func<object, object[], object>));
            return (Func<object, object[], object>)methodDelegate;
        }
    }
}
