﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Lucky {
    [TestClass]
    public class DependencyObjectTests {
        [TestMethod]
        public void DependencyObjectTest() {
            DependencyObject obj = null;
            Task.Factory.StartNew(() => {
                obj = new DependencyObject();
            }).Wait();
            Assert.IsNotNull(obj);
            Assert.IsFalse(obj.Dispatcher.CheckAccess());
        }

        [TestMethod]
        public void DependencyObjectTest1() {
            DependencyObject obj = new DependencyObject();
            Assert.IsTrue(obj.Dispatcher.CheckAccess());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependencyObjectTest2() {
            DependencyObject obj = null;
            Task.Factory.StartNew(() => {
                obj = new DependencyObject();
            }).Wait();
            obj.GetValue(FrameworkElement.WidthProperty);
        }

        [TestMethod]
        public void UIThreadTest() {
            DependencyObject obj = null;
            Task.Factory.StartNew(() => {
                Lucky.UIThread.InitializeWithDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
                obj = new DependencyObject();
            }).Wait();
            Assert.IsNotNull(obj);
            Assert.IsFalse(obj.Dispatcher.CheckAccess());
            Assert.IsFalse(Lucky.UIThread.CheckAccess());
        }

        [TestMethod]
        public void UIThreadTest1() {
            DependencyObject obj = null;
            Lucky.UIThread.InitializeWithDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
            Task.Factory.StartNew(() => {
                obj = new DependencyObject();
            }).Wait();
            Assert.IsNotNull(obj);
            Assert.IsFalse(obj.Dispatcher.CheckAccess());
            Assert.IsTrue(Lucky.UIThread.CheckAccess());
        }

        [TestMethod]
        public void UIThreadTest2() {
            Lucky.UIThread.InitializeWithDispatcher(System.Windows.Threading.Dispatcher.CurrentDispatcher);
            DependencyObject obj = new DependencyObject();
            Assert.IsTrue(obj.Dispatcher.CheckAccess());
            Assert.IsTrue(Lucky.UIThread.CheckAccess());
        }
    }
}
