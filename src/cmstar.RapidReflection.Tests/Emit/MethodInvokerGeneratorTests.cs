using System;
using System.Reflection;
using NUnit.Framework;

namespace cmstar.RapidReflection.Emit
{
    [TestFixture]
    public class MethodInvokerGeneratorTests
    {
#pragma warning disable IDE0051 // Remove unused private members
        // ReSharper disable UnusedMember.Local,UnusedMember.Global
        private class InnerClass
        {
            public int Value;
            public virtual void Act(int value) { Value = value; }
            protected virtual int IntFunc() { return 1; }
            private string StrFunc() { return string.Empty; }
            public static int StaticFunc(int x, int y) { return x + y; }
        }
        // ReSharper restore UnusedMember.Local,UnusedMember.Global
#pragma warning restore IDE0051 // Remove unused private members

        private class InnerClassDerived : InnerClass
        {
            public override void Act(int value) { Value = -value; }
            protected override int IntFunc() { return 2; }
        }

        [Test]
        public void CallInstanceMethods()
        {
            var mAct = typeof(InnerClass).GetMethod("Act");
            var act = MethodInvokerGenerator.CreateDelegate(mAct);
            var instance = new InnerClass();
            var result = act(instance, new object[] { 123 });
            Assert.IsNull(result);
            Assert.AreEqual(123, instance.Value);

            result = act(instance, new object[] { 123, "argument to be ignored" });
            Assert.IsNull(result);
            Assert.AreEqual(123, instance.Value);

            var mIntFunc = typeof(InnerClass).GetMethod("IntFunc", BindingFlags.Instance | BindingFlags.NonPublic);
            var intFunc = MethodInvokerGenerator.CreateDelegate(mIntFunc);
            result = intFunc(instance, null);
            Assert.AreEqual(1, result);

            var mStrFunc = typeof(InnerClass).GetMethod("StrFunc", BindingFlags.Instance | BindingFlags.NonPublic);
            var strFunc = MethodInvokerGenerator.CreateDelegate(mStrFunc);
            result = strFunc(instance, null);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void CallOverrideMethods()
        {
            var mAct = typeof(InnerClassDerived).GetMethod("Act");
            var act = MethodInvokerGenerator.CreateDelegate(mAct);
            var instance = new InnerClassDerived();
            act(instance, new object[] { 123 });
            Assert.AreEqual(-123, instance.Value);

            var mIntFunc = typeof(InnerClass).GetMethod("IntFunc", BindingFlags.Instance | BindingFlags.NonPublic);
            var intFunc = MethodInvokerGenerator.CreateDelegate(mIntFunc);
            Assert.AreEqual(2, intFunc(instance, null));
        }

        [Test]
        public void CallMethodsThrowsExceptionWithArgumentValidation()
        {
            var mAct = typeof(InnerClass).GetMethod("Act");
            var act = MethodInvokerGenerator.CreateDelegate(mAct);
            var instance = new InnerClass();

            Assert.Throws<ArgumentNullException>(() => act(null, new object[] { 1 }));
            Assert.Throws<ArgumentNullException>(() => act(instance, null));
            Assert.Throws<ArgumentException>(() => act(instance, new object[0]));
            Assert.Throws<InvalidCastException>(() => act(instance, new object[] { "string" }));
        }

        [Test]
        public void CallMethodsThrowsExceptionWithoutArgumentValidation()
        {
            var mAct = typeof(InnerClass).GetMethod("Act");
            var act = MethodInvokerGenerator.CreateDelegate(mAct, false);
            var instance = new InnerClass();

            Assert.Throws<NullReferenceException>(() => act(null, new object[] { 1 }));
            Assert.Throws<NullReferenceException>(() => act(instance, null));
            Assert.Throws<IndexOutOfRangeException>(() => act(instance, new object[0]));
        }

        [Test]
        public void CallStaticMethods()
        {
            var mStaticFunc = typeof(InnerClass).GetMethod("StaticFunc", BindingFlags.Public | BindingFlags.Static);
            var staticFunc = MethodInvokerGenerator.CreateDelegate(mStaticFunc);
            var result = staticFunc(null, new object[] { 1, 2 });
            Assert.AreEqual(3, result);
        }
    }
}