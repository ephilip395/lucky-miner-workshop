﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Lucky.Gpus.Adl {
    public static class AdlNativeMethods {
        [AttributeUsage(AttributeTargets.Property)]
        public class AdlAttribute : Attribute {
            public AdlAttribute() {
            }
        }

        internal delegate IntPtr ADL_Main_Memory_AllocCallback(int size);
        internal delegate AdlStatus ADL_Main_Control_CreateDelegate(ADL_Main_Memory_AllocCallback callback, int enumConnectedAdapters);
        internal delegate AdlStatus ADL2_Main_Control_CreateDelegate(ADL_Main_Memory_AllocCallback callback, int enumConnectedAdapters, out IntPtr context);
        internal delegate AdlStatus ADL_Adapter_AdapterInfo_GetDelegate(IntPtr info, int size);

        public delegate AdlStatus ADL_Main_Control_DestroyDelegate();
        public delegate AdlStatus ADL2_Main_Control_DestroyDelegate(IntPtr context);
        internal delegate AdlStatus ADL_Adapter_NumberOfAdapters_GetDelegate(ref int numAdapters);
        internal delegate AdlStatus ADL_Overdrive5_Temperature_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);
        internal delegate AdlStatus ADL2_OverdriveN_Temperature_GetDelegate(IntPtr context, int adapterIndex, ADLODNTemperatureType temperatureType, out int temperature);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_GetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeed_SetDelegate(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);
        internal delegate AdlStatus ADL_Overdrive5_FanSpeedToDefault_SetDelegate(int adapterIndex, int thermalControllerIndex);
        internal delegate AdlStatus ADL2_Overdrive5_FanSpeedToDefault_SetDelegate(IntPtr context, int adapterIndex, int thermalControllerIndex);
        internal delegate AdlStatus ADL_Overdrive_CapsDelegate(int adapterIndex, out int supported, out int enabled, out int version);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_GetDelegate(IntPtr context, int iAdapterIndex, out ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_OverdriveN_PowerLimit_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPowerLimitSetting lpODPowerLimit);
        internal delegate AdlStatus ADL2_Overdrive6_CurrentPower_GetDelegate(IntPtr context, int iAdapterIndex, ADLODNCurrentPowerType powerType, out int lpCurrentValue);
        internal delegate AdlStatus ADL2_New_QueryPMLogData_GetDelegate(IntPtr context, int adapterIndex, ref ADLPMLogDataOutput dataOutput);
        internal delegate AdlStatus ADL_Adapter_MemoryInfo_GetDelegate(int iAdapterIndex, ref ADLMemoryInfo lpMemoryInfo);
        internal delegate AdlStatus ADL2_Graphics_VersionsX2_GetDelegate(IntPtr context, ref ADLVersionsInfoX2 lpVersionsInfo);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryTimingLevel_GetDelegate(IntPtr context, int iAdapterIndex, out int lpSupport, out int lpCurrentValue, out int lpDefaultValue, out int lpNumberLevels, ref IntPtr lppLevelList);
        internal delegate AdlStatus ADL2_OverdriveN_MemoryTimingLevel_SetDelegate(IntPtr context, int iAdapterIndex, int currentValue);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_SystemClocksX2_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNPerformanceLevelsX2 lpODPerformanceLevels);
        internal delegate AdlStatus ADL2_OverdriveN_CapabilitiesX2_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLODNCapabilitiesX2 lpODCapabilities);
        internal delegate AdlStatus ADL2_Overdrive6_FanSpeed_ResetDelegate(IntPtr context, int iAdapterIndex);
        internal delegate AdlStatus ADL2_Overdrive8_Current_Setting_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLOD8CurrentSetting lpCurrentSetting);
        internal delegate AdlStatus ADL2_Overdrive8_Init_Setting_GetDelegate(IntPtr context, int iAdapterIndex, ref ADLOD8InitSetting lpInitSetting);
        internal delegate AdlStatus ADL2_Overdrive8_Setting_SetDelegate(IntPtr context, int iAdapterIndex, ref ADLOD8SetSetting lpSetSetting, ref ADLOD8CurrentSetting lpCurrentSetting);

        // 以下属性要求必须是外部可见的static，不能是private的
        // 注意属性名是EnterPoint，不要改名
        [Adl]
        internal static ADL_Main_Control_CreateDelegate ADL_Main_Control_Create { get; private set; }
        [Adl]
        internal static ADL2_Main_Control_CreateDelegate ADL2_Main_Control_Create { get; private set; }
        [Adl]
        internal static ADL_Adapter_AdapterInfo_GetDelegate ADL_Adapter_AdapterInfo_Get { get; private set; }
        [Adl]
        internal static ADL_Main_Control_DestroyDelegate ADL_Main_Control_Destroy { get; private set; }
        [Adl]
        internal static ADL2_Main_Control_DestroyDelegate ADL2_Main_Control_Destroy { get; private set; }
        [Adl]
        internal static ADL_Adapter_NumberOfAdapters_GetDelegate ADL_Adapter_NumberOfAdapters_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_Temperature_GetDelegate ADL_Overdrive5_Temperature_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_Temperature_GetDelegate ADL2_OverdriveN_Temperature_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeed_GetDelegate ADL_Overdrive5_FanSpeed_Get { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeed_SetDelegate ADL_Overdrive5_FanSpeed_Set { get; private set; }
        [Adl]
        internal static ADL_Overdrive5_FanSpeedToDefault_SetDelegate ADL_Overdrive5_FanSpeedToDefault_Set { get; private set; }
        [Adl]
        internal static ADL2_Overdrive5_FanSpeedToDefault_SetDelegate ADL2_Overdrive5_FanSpeedToDefault_Set { get; private set; }
        [Adl]
        internal static ADL_Overdrive_CapsDelegate ADL_Overdrive_Caps { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_PowerLimit_GetDelegate ADL2_OverdriveN_PowerLimit_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive6_CurrentPower_GetDelegate ADL2_Overdrive6_CurrentPower_Get { get; private set; }
        [Adl]
        internal static ADL2_New_QueryPMLogData_GetDelegate ADL2_New_QueryPMLogData_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_PowerLimit_SetDelegate ADL2_OverdriveN_PowerLimit_Set { get; private set; }
        [Adl]
        internal static ADL_Adapter_MemoryInfo_GetDelegate ADL_Adapter_MemoryInfo_Get { get; private set; }
        [Adl]
        internal static ADL2_Graphics_VersionsX2_GetDelegate ADL2_Graphics_VersionsX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryClocksX2_GetDelegate ADL2_OverdriveN_MemoryClocksX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryClocksX2_SetDelegate ADL2_OverdriveN_MemoryClocksX2_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryTimingLevel_GetDelegate ADL2_OverdriveN_MemoryTimingLevel_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_MemoryTimingLevel_SetDelegate ADL2_OverdriveN_MemoryTimingLevel_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_SystemClocksX2_GetDelegate ADL2_OverdriveN_SystemClocksX2_Get { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_SystemClocksX2_SetDelegate ADL2_OverdriveN_SystemClocksX2_Set { get; private set; }
        [Adl]
        internal static ADL2_OverdriveN_CapabilitiesX2_GetDelegate ADL2_OverdriveN_CapabilitiesX2_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive6_FanSpeed_ResetDelegate ADL2_Overdrive6_FanSpeed_Reset { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Current_Setting_GetDelegate ADL2_Overdrive8_Current_Setting_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Init_Setting_GetDelegate ADL2_Overdrive8_Init_Setting_Get { get; private set; }
        [Adl]
        internal static ADL2_Overdrive8_Setting_SetDelegate ADL2_Overdrive8_Setting_Set { get; private set; }

        internal static AdlStatus ADLMainControlCreate(out IntPtr context) {
            context = IntPtr.Zero;
            string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "atiadlxx.dll");
            string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "atiadlxy.dll");
            string path3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "atiadlxx.dll");
            string path4 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "atiadlxy.dll");
            if (!File.Exists(path1) && !File.Exists(path2) && !File.Exists(path3) && !File.Exists(path4)) {
                return AdlStatus.ADL_ERR;
            }
            AdlStatus r = AdlStatus.ADL_ERR;
            string dllName = "atiadlxx.dll";
            try {
                CreateDelegates(dllName);
                if (ADL_Main_Control_Create != null) {
                    r = ADL_Main_Control_Create(Marshal.AllocHGlobal, 1);
                }
                else {
                    return AdlStatus.ADL_ERR;
                }
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
                try {
                    dllName = "atiadlxy.dll";
                    CreateDelegates(dllName);
                    if (ADL_Main_Control_Create != null) {
                        r = ADL_Main_Control_Create(Marshal.AllocHGlobal, 1);
                    }
                    else {
                        return AdlStatus.ADL_ERR;
                    }
                }
                catch(Exception ex) {
                    Logger.ErrorDebugLine(ex);
                    r = AdlStatus.ADL_ERR;
                }
            }
            if (r < AdlStatus.ADL_OK) {
                LuckyConsole.DevError(() => $"{nameof(ADL_Main_Control_Create)} {r.ToString()} {dllName}");
            }
            ADL2MainControlCreate(out context);
            return r;
        }

        private static void ADL2MainControlCreate(out IntPtr context) {
            try {
                var r = ADL2_Main_Control_Create(Marshal.AllocHGlobal, 1, out context);
                if (r < AdlStatus.ADL_OK) {
                    LuckyConsole.DevError(() => $"{nameof(ADL2_Main_Control_Create)} {r.ToString()}");
                }
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                context = IntPtr.Zero;
            }
        }

        internal static AdlStatus ADLAdapterAdapterInfoGet(ADLAdapterInfo[] info) {
            Type elementType = typeof(ADLAdapterInfo);
            int elementSize = Marshal.SizeOf(elementType);
            int size = info.Length * elementSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            AdlStatus result = ADL_Adapter_AdapterInfo_Get(ptr, size);
            for (int i = 0; i < info.Length; i++) {
                info[i] = (ADLAdapterInfo)Marshal.PtrToStructure((IntPtr)((long)ptr + i * elementSize), elementType);
            }
            Marshal.FreeHGlobal(ptr);

            // the ADLAdapterInfo.VendorID field reported by ADL is wrong on 
            // Windows systems (parse error), so we fix this here
            Regex regex1 = new Regex("PCI_VEN_([A-Fa-f0-9]{1,4})&.*");
            Regex regex2 = new Regex("[0-9]+:[0-9]+:([0-9]+):[0-9]+:[0-9]+");
            for (int i = 0; i < info.Length; i++) {
                // try Windows UDID format
                Match m = regex1.Match(info[i].UDID);
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 16);
                    continue;
                }
                // if above failed, try Unix UDID format
                m = regex2.Match(info[i].UDID);
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 10);
                }
            }

            return result;
        }

        private static void SetDelegate(PropertyInfo property, string dllName) {
            DllImportAttribute attribute = new DllImportAttribute(dllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = property.Name
            };
            try {
                PInvokeDelegateFactory.CreateDelegate(attribute, property.PropertyType, out object newDelegate);
                property.SetValue(null, newDelegate, null);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void CreateDelegates(string dllName) {
            Type t = typeof(AdlNativeMethods);
            var properties = t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetProperty);
            foreach (var property in properties) {
                var attrs = property.GetCustomAttributes(typeof(AdlAttribute), inherit: false);
                if (attrs.Length == 0) {
                    continue;
                }
                SetDelegate(property, dllName);
            }
        }
    }
}
