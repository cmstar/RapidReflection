using System;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
namespace cmstar.RapidReflection.Emit
{
    [TestFixture]
    public class ConstructorInvokerGeneratorTests
    {
        private class InternalClassWithNoParameterlessConstructor
        {
            public InternalClassWithNoParameterlessConstructor(int i) { }
            public InternalClassWithNoParameterlessConstructor(int i, string s) { }
            public InternalClassWithNoParameterlessConstructor(int i, string s, double d) { }
        }

        [Test]
        public void CreateDelegateFromType()
        {
            Assert.Throws<ArgumentException>(
                () => ConstructorInvokerGenerator.CreateDelegate(typeof(IInternalInterface)));
            Assert.Throws<ArgumentException>(
                () => ConstructorInvokerGenerator.CreateDelegate(typeof(Type)));
            Assert.Throws<ArgumentException>(
                () => ConstructorInvokerGenerator.CreateDelegate(typeof(InternalClassWithNoParameterlessConstructor)));

            var func = ConstructorInvokerGenerator.CreateDelegate(typeof(InternalClass));
            Assert.IsInstanceOf<InternalClass>(func());

            func = ConstructorInvokerGenerator.CreateDelegate(typeof(InternalStruct));
            Assert.IsInstanceOf<InternalStruct>(func());
        }

        [Test]
        public void CreateDelegateFromConstructorInfoForClass()
        {
            var ctor = typeof(InternalClass).GetConstructor(Type.EmptyTypes);
            var func = ConstructorInvokerGenerator.CreateDelegate(ctor);
            Assert.IsInstanceOf<InternalClass>(func(null));

            ctor = typeof(InternalClassWithNoParameterlessConstructor)
                .GetConstructor(new[] { typeof(int) });
            func = ConstructorInvokerGenerator.CreateDelegate(ctor);
            Assert.IsInstanceOf<InternalClassWithNoParameterlessConstructor>(func(new object[] { 1 }));

            ctor = typeof(InternalClassWithNoParameterlessConstructor)
                .GetConstructor(new[] { typeof(int), typeof(string) });
            func = ConstructorInvokerGenerator.CreateDelegate(ctor);
            Assert.IsInstanceOf<InternalClassWithNoParameterlessConstructor>(func(new object[] { 1, "s" }));
            Assert.IsInstanceOf<InternalClassWithNoParameterlessConstructor>(func(new object[] { 1, "s", 123 })); //the third para will be ignored

            ctor = typeof(InternalClassWithNoParameterlessConstructor)
                .GetConstructor(new[] { typeof(int), typeof(string), typeof(double) });
            func = ConstructorInvokerGenerator.CreateDelegate(ctor);
            Assert.IsInstanceOf<InternalClassWithNoParameterlessConstructor>(func(new object[] { 1, "s", 0.123 }));
        }

        [Test]
        public void CreateDelegateFromConstructorInfoForClassThrowsException()
        {
            var ctor = typeof(InternalClassWithNoParameterlessConstructor)
                .GetConstructor(new[] { typeof(int) });

            //with argument validation
            var func = ConstructorInvokerGenerator.CreateDelegate(ctor);
            Assert.Throws<ArgumentNullException>(() => func(null));
            Assert.Throws<ArgumentException>(() => func(new object[0]));
            Assert.Throws<InvalidCastException>(() => func(new object[] { "1" }));

            //without argument validation
            func = ConstructorInvokerGenerator.CreateDelegate(ctor, false);
            Assert.Throws<NullReferenceException>(() => func(null));
            Assert.Throws<IndexOutOfRangeException>(() => func(new object[0]));
        }
    }
}