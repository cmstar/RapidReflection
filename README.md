# RapidReflection

[![NuGet](https://img.shields.io/nuget/v/cmstar.RapidReflection.svg)](https://www.nuget.org/packages/cmstar.RapidReflection)

Simply accesses type memebers with dynamic methods created using Reflection.Emit.

Supported .net versions:
- .net Framework 3.5
- .net Framework 4.x
- Other versions that are compliant with .net Standard 2.0, such as .net Core 2/3, .net 5/6

## Install

Install via Package Manager:
```
Install-Package cmstar.RapidReflection
```

or via dotnet-cli:
```
dotnet add package cmstar.RapidReflection
```

## Usage

The class for test

```csharp
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
```

### Property Access

```csharp
PropertyInfo property = typeof(TestClass).GetProperty("Integer");
TestClass instance = new TestClass { Integer = 12345 };

Func<object, object> getter = PropertyAccessorGenerator.CreateGetter(property);
Console.WriteLine(getter(instance)); // -> 12345

Action<object, object> setter = PropertyAccessorGenerator.CreateSetter(property);
setter(instance, 54321);
Console.WriteLine(instance.Integer); // -> 54321
```

### Field Access

```csharp
FieldInfo field = typeof(TestClass).GetField(
    "_integer", BindingFlags.Instance | BindingFlags.NonPublic);
TestClass instance = new TestClass { Integer = 12345 };

Func<object, object> getter = FieldAccessorGenerator.CreateGetter(field);
Console.WriteLine(getter(instance)); // -> 12345

Action<object, object> setter = FieldAccessorGenerator.CreateSetter(field);
setter(instance, 54321);
Console.WriteLine(instance.Integer); // -> 54321
```

For static property/field access, use *null* as the first argument:

```csharp
getter(null);
setter(null, value);
```

### Constructor Invoking

```csharp
ConstructorInfo constructor
    = typeof(TestClass).GetConstructor(new Type[] { typeof(int) });
Func<object[], object> func
    = ConstructorInvokerGenerator.CreateDelegate(constructor);

object[] arguments = new object[] { 123 };
TestClass instance = (TestClass)func(arguments);
Console.WriteLine(instance.Integer); // -> 123
```

or use the parameterless constructor directly:
```csharp
Func<object> func 
    = ConstructorInvokerGenerator.CreateDelegate(typeof(TestClass));
TestClass instance = (TestClass)func();
```

### Method Invoking

```csharp
MethodInfo method = typeof(TestClass).GetMethod("GetHello");
Func<object, object[], object> caller
    = MethodInvokerGenerator.CreateDelegate(method);

TestClass instance = new TestClass();
object[] arguments = new object[] { "World" };
string result = (string)caller(instance, arguments);
Console.WriteLine(result); // -> "Hello World!"
```

To call a static method, use *null* as the instance:
```csharp
caller(null, arguments);
```

If the method has no argument, either is OK:
```csharp
caller(instanc, null);
caller(instanc, new object[0]);
```

If the method has no return value, the delegate returns null.

## Extention Methods

A set of extention methods is provided so the code below
```csharp
ILGenerator il = dynamicMethod.GetILGenerator();
il.Emit(OpCodes.Ldloc_0);
il.Emit(OpCodes.Ldlen);
il.Emit(OpCodes.Conv_I4);
```

can be written in this form:
```csharp
ILGenerator il = dynamicMethod.GetILGenerator();
il.Ldarg_0().Ldlen().Conv_I4();
```

*In the current version, the extention methods does not include operations for all opcodes.*
