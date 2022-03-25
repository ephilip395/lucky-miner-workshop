﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Lucky {
    [TestClass]
    public class LuckyDataSchemasTests {
        [TestMethod]
        public void TypesTest() {
            var assembly = typeof(IData).Assembly;
            int skipCount = 0;
            var types = assembly.GetTypes();
            foreach (var type in types) {
                if (!type.IsClass || type.IsAbstract || typeof(Attribute).IsAssignableFrom(type) || type.Namespace == null || !type.Namespace.StartsWith(nameof(Lucky))) {
                    Console.WriteLine(type.FullName);
                    skipCount++;
                    continue;
                }
                if (type.IsDefined(typeof(IsNotDataSchemaAttribute), false)) {
                    continue;
                }
                var ctors = type.GetConstructors();
                Assert.IsTrue(ctors.Length != 0, type.FullName);
                if (ctors.All(a => a.IsStatic)) {
                    Console.WriteLine(type.FullName);
                    skipCount++;
                    continue;
                }
                // 1 确保有默认构造函数
                Assert.IsTrue(ctors.Any(a => a.IsPublic && a.GetParameters().Length == 0), type.FullName);
                // 2 所有属性都是公共的可读写的，除非标记了JsonIgnore
                Assert.IsTrue(type.GetProperties().Where(a=> !a.IsDefined(typeof(JsonIgnoreAttribute), false)).All(a => a.CanWrite && a.CanRead && a.GetMethod.IsPublic && a.SetMethod.IsPublic), type.FullName);
            }
            Console.WriteLine($"共{types.Length.ToString()}个类型，以上类型被跳过，因为它们不是类型或者是抽象类型或者是静态类型或者是Attribute类型或者不是Lucky定义的类型，共跳过{skipCount.ToString()}条");
        }
    }
}
