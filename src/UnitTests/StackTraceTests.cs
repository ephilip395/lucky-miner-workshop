﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Lucky {
    [TestClass]
    public class StackTraceTests {
        [TestMethod]
        public void StackTraceTest() {
            int count = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < count; i++) {
                MethodA();
            }
            Console.WriteLine("MethodA "+ Lucky.NTStopwatch.Stop());
            NTStopwatch.Start();
            for (int i = 0; i < count; i++) {
                MethodB();
            }
            Console.WriteLine("MethodB " + Lucky.NTStopwatch.Stop());
        }

        private void MethodA() {
            GetMessagePathLocation(null);
        }

        private void MethodB() {

        }

        private static Type GetMessagePathLocation(Type borderType) {
            if (borderType == null) {
                borderType = typeof(StackTraceTests);
            }
            StackTrace ss = new StackTrace(false);
            int index = 1;
            Type location = ss.GetFrame(index).GetMethod().DeclaringType;
            while (location != borderType) {
                index++;
                if (index == ss.FrameCount) {
                    throw new InvalidProgramException("到底了");
                }
                if (index > 10) {
                    throw new InvalidProgramException("不可能这么深");
                }
                location = ss.GetFrame(index).GetMethod().DeclaringType;
            }
            while (location == borderType) {
                index++;
                if (index == ss.FrameCount) {
                    throw new InvalidProgramException("到底了");
                }
                if (index > 10) {
                    throw new InvalidProgramException("不可能这么深");
                }
                location = ss.GetFrame(index).GetMethod().DeclaringType;
            }
            return location;
        }
    }
}
