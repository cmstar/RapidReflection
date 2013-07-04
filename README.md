# RapidReflection

Simply accesses type memebers with dynamic methods created using Reflection.Emit.

## Dynamic Methods
The class for test

    class TestClass
    {
        private int _integer;

        public TestClass() { }
        public TestClass(int integer) { _integer = integer; }

        public int Integer { get { return _integer; } set { _integer = value; } }

        public string GetHello(string target)
        {
            return string.Format("Hello {0}!", target); 
        }
    }


### Property Access

    PropertyInfo property = typeof(TestClass).GetProperty("Integer");
    TestClass instance = new TestClass { Integer = 12345 };

    Func<object, object> getter = PropertyAccessorGenerator.CreateGetter(property);
    Console.WriteLine(getter(instance)); // -> 12345

    Action<object, object> setter = PropertyAccessorGenerator.CreateSetter(property);
    setter(instance, 54321);
    Console.WriteLine(instance.Integer); // -> 54321


### Field Access

    FieldInfo field = typeof(TestClass).GetField(
        "_integer", BindingFlags.Instance | BindingFlags.NonPublic);
    TestClass instance = new TestClass { Integer = 12345 };

    Func<object, object> getter = FieldAccessorGenerator.CreateGetter(field);
    Console.WriteLine(getter(instance)); // -> 12345

    Action<object, object> setter = FieldAccessorGenerator.CreateSetter(field);
    setter(instance, 54321);
    Console.WriteLine(instance.Integer); // -> 54321

For static property/field access, use *null* as the first argument:

	getter(null);
	setter(null, value);


### Constructor Invoking

    ConstructorInfo constructor
        = typeof(TestClass).GetConstructor(new Type[] { typeof(int) });
    Func<object[], object> func
        = ConstructorInvokerGenerator.CreateDelegate(constructor);

    object[] arguments = new object[] { 123 };
    TestClass instance = (TestClass)func(arguments);
    Console.WriteLine(instance.Integer); // -> 123

or use the parameterless constructor directly:

    Func<object> func 
		= ConstructorInvokerGenerator.CreateDelegate(typeof(TestClass));
    TestClass instance = (TestClass)func();


### Method Invoking

    MethodInfo method = typeof(TestClass).GetMethod("GetHello");
    Func<object, object[], object> caller
        = MethodInvokerGenerator.CreateDelegate(method);

    TestClass instance = new TestClass();
    object[] arguments = new object[] { "World" };
    string result = (string)caller(instance, arguments);
    Console.WriteLine(result); // -> "Hello World!"

To call a static method, use *null* as the instance:

    caller(null, arguments);


If the method has no argument, either is OK:

    caller(instanc, null);
    caller(instanc, new object[0]);

If the method has no return value, the delegate returns null.

## Extention Methods
A set of extention methods is provided so the code below

    ILGenerator il = dynamicMethod.GetILGenerator();
    il.Emit(OpCodes.Ldloc_0);
    il.Emit(OpCodes.Ldlen);
    il.Emit(OpCodes.Conv_I4);

can be written in this form:

    ILGenerator il = dynamicMethod.GetILGenerator();
    il.Ldarg_0().Ldlen().Conv_I4();

*In the current version, the extention methods does not include operations for all opcodes.*

## Tasks in the future
- Allows specifying the types in the delegates, such as Func&lt;TInstance, TRet&gt; against Func&lt;object, object&gt;.