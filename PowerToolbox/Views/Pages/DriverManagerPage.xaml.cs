using Microsoft.UI.Xaml.Controls;
using PowerToolbox.Extensions.DataType.Class;
using PowerToolbox.Extensions.DataType.Enums;
using PowerToolbox.Helpers.Root;
using PowerToolbox.Models;
using PowerToolbox.Services.Root;
using PowerToolbox.Views.Dialogs;
using PowerToolbox.Views.NotificationTips;
using PowerToolbox.Views.Windows;
using PowerToolbox.WindowsAPI.PInvoke.Kernel32;
using PowerToolbox.WindowsAPI.PInvoke.NewDev;
using PowerToolbox.WindowsAPI.PInvoke.Setupapi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 驱动管理页面
    /// </summary>
    public sealed partial class DriverManagerPage : Page, INotifyPropertyChanged
    {
        private readonly string AddingDriverFailedString = ResourceService.DriverManagerResource.GetString("AddingDriverFailed");
        private readonly string AddingDriverString = ResourceService.DriverManagerResource.GetString("AddingDriver");
        private readonly string AddingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("AddingDriverSuccessfully");
        private readonly string AddInstallingDriverFailedString = ResourceService.DriverManagerResource.GetString("AddInstallingDriverFailed");
        private readonly string AddInstallingDriverString = ResourceService.DriverManagerResource.GetString("AddInstallingDriver");
        private readonly string AddInstallingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("AddInstallingDriverSuccessfully");
        private readonly string DeletingDriverFailedString = ResourceService.DriverManagerResource.GetString("DeletingDriverFailed");
        private readonly string DeletingDriverString = ResourceService.DriverManagerResource.GetString("DeletingDriver");
        private readonly string DeletingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("DeletingDriverSuccessfully");
        private readonly string DriverFilterConditionString = ResourceService.DriverManagerResource.GetString("DriverFilterCondition");
        private readonly string DriverInformationString = ResourceService.DriverManagerResource.GetString("DriverInformation");
        private readonly string DriverEmptyDescriptionString = ResourceService.DriverManagerResource.GetString("DriverEmptyDescription");
        private readonly string DriverEmptyWithConditionDescriptionString = ResourceService.DriverManagerResource.GetString("DriverEmptyWithConditionDescription");
        private readonly string ForceDeletingDriverFailedString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriverFailed");
        private readonly string ForceDeletingDriverString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriver");
        private readonly string ForceDeletingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriverSuccessfully");
        private readonly string RestartPCString = ResourceService.DriverManagerResource.GetString("RestartPC");
        private readonly string SelectFileString = ResourceService.DriverManagerResource.GetString("SelectFile");
        private readonly string UnknownDeviceNameString = ResourceService.DriverManagerResource.GetString("UnknownDeviceName");
        private readonly string UnknownString = ResourceService.DriverManagerResource.GetString("Unknown");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isInitialized;

        private static Dictionary<string, DEVPROPKEY> DevPropKeyDict { get; } = new()
        {
            { "DEVPKEY_NAME", new DEVPROPKEY() { fmtid = new ("B725F130-47EF-101A-A5F1-02608C9EEBAC"), pid = 10 }},
            { "DEVPKEY_Device_DeviceDesc", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 2 }},
            { "DEVPKEY_Device_HardwareIds", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 3 }},
            { "DEVPKEY_Device_CompatibleIds", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 4 }},
            { "DEVPKEY_Device_Service", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 6 }},
            { "DEVPKEY_Device_Class", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 9 }},
            { "DEVPKEY_Device_ClassGuid", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 10 }},
            { "DEVPKEY_Device_Driver", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 11 }},
            { "DEVPKEY_Device_ConfigFlags", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 12 }},
            { "DEVPKEY_Device_Manufacturer", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 13 }},
            { "DEVPKEY_Device_FriendlyName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 14 }},
            { "DEVPKEY_Device_LocationInfo", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 15 }},
            { "DEVPKEY_Device_PDOName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 16 }},
            { "DEVPKEY_Device_Capabilities", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 17 }},
            { "DEVPKEY_Device_UINumber", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 18 }},
            { "DEVPKEY_Device_UpperFilters", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 19 }},
            { "DEVPKEY_Device_LowerFilters", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 20 }},
            { "DEVPKEY_Device_BusTypeGd", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 21 }},
            { "DEVPKEY_Device_LegacyBusType", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 22 }},
            { "DEVPKEY_Device_BusNumber", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 23 }},
            { "DEVPKEY_Device_EnumeratorName", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 24 }},
            { "DEVPKEY_Device_Security", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 25 }},
            { "DEVPKEY_Device_SecuritySDS", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 26 }},
            { "DEVPKEY_Device_DevType", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 27 }},
            { "DEVPKEY_Device_Exclusive", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 28 }},
            { "DEVPKEY_Device_Characteristics", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 29 }},
            { "DEVPKEY_Device_Address", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 30 }},
            { "DEVPKEY_Device_UINumberDescFormat", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 31 }},
            { "DEVPKEY_Device_PowerData", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 32 }},
            { "DEVPKEY_Device_RemovalPolicy", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 33 }},
            { "DEVPKEY_Device_RemovalPolicyDefault", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 34 }},
            { "DEVPKEY_Device_RemovalPolicyOverride", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 35 }},
            { "DEVPKEY_Device_InstallState", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 36 }},
            { "DEVPKEY_Device_LocationPaths", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 37 }},
            { "DEVPKEY_Device_BaseContainerId", new DEVPROPKEY() { fmtid = new ("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 38 }},
            { "DEVPKEY_Device_DevNodeStatus", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 2 }},
            { "DEVPKEY_Device_ProblemCode", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 3 }},
            { "DEVPKEY_Device_EjectionRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 4 }},
            { "DEVPKEY_Device_RemovalRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 5 }},
            { "DEVPKEY_Device_PowerRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 6 }},
            { "DEVPKEY_Device_BusRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 7 }},
            { "DEVPKEY_Device_Parent", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 8 }},
            { "DEVPKEY_Device_Children", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 9 }},
            { "DEVPKEY_Device_Siblings", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 10 }},
            { "DEVPKEY_Device_TransportRelations", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 11 }},
            { "DEVPKEY_Device_ProblemStatus", new DEVPROPKEY() { fmtid = new ("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 12 }},
            { "DEVPKEY_Device_Reported", new DEVPROPKEY() { fmtid = new ("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 2 }},
            { "DEVPKEY_Device_Legacy", new DEVPROPKEY() { fmtid = new ("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 3 }},
            { "DEVPKEY_Device_ContainerId", new DEVPROPKEY() { fmtid = new ("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 2 }},
            { "DEVPKEY_Device_InLocalMachineContainer", new DEVPROPKEY() { fmtid = new ("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 4 }},
            { "DEVPKEY_Device_ModelId", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 2 }},
            { "DEVPKEY_Device_FriendlyNameAttributes", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 3 }},
            { "DEVPKEY_Device_ManufacturerAttributes", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 4 }},
            { "DEVPKEY_Device_PresenceNotForDevice", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 5 }},
            { "DEVPKEY_Device_SignalStrength", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 6 }},
            { "DEVPKEY_Device_IsAssociateableByUserAction", new DEVPROPKEY() { fmtid = new ("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 7 }},
            { "DEVPKEY_Numa_Proximity_Domain", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 1 }},
            { "DEVPKEY_Device_DHP_Rebalance_Policy", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 2 }},
            { "DEVPKEY_Device_Numa_Node", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 3 }},
            { "DEVPKEY_Device_BusReportedDeviceDesc", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 4 }},
            { "DEVPKEY_Device_IsPresent", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 5 }},
            { "DEVPKEY_Device_HasProblem", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 6 }},
            { "DEVPKEY_Device_ConfigurationId", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 7 }},
            { "DEVPKEY_Device_ReportedDeviceIdsHash", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 8 }},
            { "DEVPKEY_Device_PhysicalDeviceLocation", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 9 }},
            { "DEVPKEY_Device_BiosDeviceName", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 10 }},
            { "DEVPKEY_Device_DriverProblemDesc", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 11 }},
            { "DEVPKEY_Device_DebuggerSafe", new DEVPROPKEY() { fmtid = new ("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 12 }},
            { "DEVPKEY_Device_SessionId", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 6 }},
            { "DEVPKEY_Device_InstallDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 100 }},
            { "DEVPKEY_Device_FirstInstallDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 101 }},
            { "DEVPKEY_Device_LastArrivalDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 102 }},
            { "DEVPKEY_Device_LastRemovalDate", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 103 }},
            { "DEVPKEY_Device_DriverDate", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 2 }},
            { "DEVPKEY_Device_DriverVersion", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 3 }},
            { "DEVPKEY_Device_DriverDesc", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 4 }},
            { "DEVPKEY_Device_DriverInfPath", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 5 }},
            { "DEVPKEY_Device_DriverInfSection", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 6 }},
            { "DEVPKEY_Device_DriverInfSectionExt", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 7 }},
            { "DEVPKEY_Device_MatchingDeviceId", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 8 }},
            { "DEVPKEY_Device_DriverProvider", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 9 }},
            { "DEVPKEY_Device_DriverPropPageProvider", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 10 }},
            { "DEVPKEY_Device_DriverCoInstallers", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 11 }},
            { "DEVPKEY_Device_ResourcePickerTags", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 12 }},
            { "DEVPKEY_Device_ResourcePickerExceptions", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 13 }},
            { "DEVPKEY_Device_DriverRank", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 14 }},
            { "DEVPKEY_Device_DriverLogoLevel", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 15 }},
            { "DEVPKEY_Device_NoConnectSound", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 17 }},
            { "DEVPKEY_Device_GenericDriverInstalled", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 18 }},
            { "DEVPKEY_Device_AdditionalSoftwareRequested", new DEVPROPKEY() { fmtid = new ("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 19 }},
            { "DEVPKEY_Device_SafeRemovalRequired", new DEVPROPKEY() { fmtid = new ("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 2 }},
            { "DEVPKEY_Device_SafeRemovalRequiredOverride", new DEVPROPKEY() { fmtid = new ("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 3 }},
            { "DEVPKEY_DrvPkg_Model", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 2 }},
            { "DEVPKEY_DrvPkg_VendorWebSite", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 3 }},
            { "DEVPKEY_DrvPkg_DetailedDescription", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 4 }},
            { "DEVPKEY_DrvPkg_DocumentationLink", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 5 }},
            { "DEVPKEY_DrvPkg_Icon", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 6 }},
            { "DEVPKEY_DrvPkg_BrandingIcon", new DEVPROPKEY() { fmtid = new ("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 7 }},
            { "DEVPKEY_DeviceClass_UpperFilters", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 19 }},
            { "DEVPKEY_DeviceClass_LowerFilters", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 20 }},
            { "DEVPKEY_DeviceClass_Security", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 25 }},
            { "DEVPKEY_DeviceClass_SecuritySDS", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 26 }},
            { "DEVPKEY_DeviceClass_DevType", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 27 }},
            { "DEVPKEY_DeviceClass_Exclusive", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 28 }},
            { "DEVPKEY_DeviceClass_Characteristics", new DEVPROPKEY() { fmtid = new ("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 29 }},
            { "DEVPKEY_DeviceClass_Name", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassName", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 3 }},
            { "DEVPKEY_DeviceClass_Icon", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 4 }},
            { "DEVPKEY_DeviceClass_ClassInstaller", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 5 }},
            { "DEVPKEY_DeviceClass_PropPageProvider", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 6 }},
            { "DEVPKEY_DeviceClass_NoInstallClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 7 }},
            { "DEVPKEY_DeviceClass_NoDisplayClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 8 }},
            { "DEVPKEY_DeviceClass_SilentInstall", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 9 }},
            { "DEVPKEY_DeviceClass_NoUseClass", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 10 }},
            { "DEVPKEY_DeviceClass_DefaultService", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 11 }},
            { "DEVPKEY_DeviceClass_IconPath", new DEVPROPKEY() { fmtid = new ("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 12 }},
            { "DEVPKEY_DeviceClass_DHPRebalanceOptOut", new DEVPROPKEY() { fmtid = new ("D14D3EF3-66CF-4BA2-9D38-0DDB37AB4701"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassCoInstallers", new DEVPROPKEY() { fmtid = new ("713D1703-A2E2-49F5-9214-56472EF3DA5C"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_FriendlyName", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_Enabled", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 3 }},
            { "DEVPKEY_DeviceInterface_ClassGuid", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 4 }},
            { "DEVPKEY_DeviceInterface_ReferenceString", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 5 }},
            { "DEVPKEY_DeviceInterface_Restricted", new DEVPROPKEY() { fmtid = new ("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 6 }},
            { "DEVPKEY_DeviceInterfaceClass_DefaultInterface", new DEVPROPKEY() { fmtid = new ("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 2 }},
            { "DEVPKEY_DeviceInterfaceClass_Name", new DEVPROPKEY() { fmtid = new ("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 3 }},
            { "DEVPKEY_Device_Model", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 39 }},
            { "DEVPKEY_DeviceContainer_Address", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 51 }},
            { "DEVPKEY_DeviceContainer_DiscoveryMethod", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 52 }},
            { "DEVPKEY_DeviceContainer_IsEncrypted", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 53 }},
            { "DEVPKEY_DeviceContainer_IsAuthenticated", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 54 }},
            { "DEVPKEY_DeviceContainer_IsConnected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 55 }},
            { "DEVPKEY_DeviceContainer_IsPaired", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 56 }},
            { "DEVPKEY_DeviceContainer_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 57 }},
            { "DEVPKEY_DeviceContainer_Version", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 65 }},
            { "DEVPKEY_DeviceContainer_Last_Seen", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 66 }},
            { "DEVPKEY_DeviceContainer_Last_Connected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 67 }},
            { "DEVPKEY_DeviceContainer_IsShowInDisconnectedState", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 68 }},
            { "DEVPKEY_DeviceContainer_IsLocalMachine", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 70 }},
            { "DEVPKEY_DeviceContainer_MetadataPath", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 71 }},
            { "DEVPKEY_DeviceContainer_IsMetadataSearchInProgress", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 72 }},
            { "DEVPKEY_DeviceContainer_MetadataChecksum", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 73 }},
            { "DEVPKEY_DeviceContainer_IsNotInterestingForDisplay", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 74 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageOnDeviceConnect", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 76 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageFromExplorer", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 77 }},
            { "DEVPKEY_DeviceContainer_BaselineExperienceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 78 }},
            { "DEVPKEY_DeviceContainer_IsDeviceUniquelyIdentifiable", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 79 }},
            { "DEVPKEY_DeviceContainer_AssociationArray", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 80 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription1", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 81 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription2", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 82 }},
            { "DEVPKEY_DeviceContainer_HasProblem", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 83 }},
            { "DEVPKEY_DeviceContainer_IsSharedDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 84 }},
            { "DEVPKEY_DeviceContainer_IsNetworkDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 85 }},
            { "DEVPKEY_DeviceContainer_IsDefaultDevice", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 86 }},
            { "DEVPKEY_DeviceContainer_MetadataCabinet", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 87 }},
            { "DEVPKEY_DeviceContainer_RequiresPairingElevation", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 88 }},
            { "DEVPKEY_DeviceContainer_ExperienceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 89 }},
            { "DEVPKEY_DeviceContainer_Category", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 90 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Singular", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 91 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Plural", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 92 }},
            { "DEVPKEY_DeviceContainer_Category_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 93 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Desc", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 94 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Icon", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 95 }},
            { "DEVPKEY_DeviceContainer_PrimaryCategory", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 97 }},
            { "DEVPKEY_DeviceContainer_UnpairUninstall", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 98 }},
            { "DEVPKEY_DeviceContainer_RequiresUninstallElevation", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 99 }},
            { "DEVPKEY_DeviceContainer_DeviceFunctionSubRank", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 100 }},
            { "DEVPKEY_DeviceContainer_AlwaysShowDeviceAsConnected", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 101 }},
            { "DEVPKEY_DeviceContainer_ConfigFlags", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 105 }},
            { "DEVPKEY_DeviceContainer_PrivilegedPackageFamilyNames", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 106 }},
            { "DEVPKEY_DeviceContainer_CustomPrivilegedPackageFamilyNames", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 107 }},
            { "DEVPKEY_DeviceContainer_IsRebootRequired", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 108 }},
            { "DEVPKEY_Device_InstanceId", new DEVPROPKEY() { fmtid = new ("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 256 }},
            { "DEVPKEY_DeviceContainer_FriendlyName", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 12288 }},
            { "DEVPKEY_DeviceContainer_Manufacturer", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8192 }},
            { "DEVPKEY_DeviceContainer_ModelName", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8194 }},
            { "DEVPKEY_DeviceContainer_ModelNumber", new DEVPROPKEY() { fmtid = new ("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8195 }},
            { "DEVPKEY_DeviceContainer_InstallInProgress", new DEVPROPKEY() { fmtid = new ("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 9 }}
        };

        private string _driverDescription = string.Empty;

        public string DriverDescription
        {
            get { return _driverDescription; }

            set
            {
                if (!string.Equals(_driverDescription, value))
                {
                    _driverDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DriverDescription)));
                }
            }
        }

        private DriverPaneKind _driverPaneKind;

        public DriverPaneKind DriverPaneKind
        {
            get { return _driverPaneKind; }

            set
            {
                if (!Equals(_driverPaneKind, value))
                {
                    _driverPaneKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DriverPaneKind)));
                }
            }
        }

        private DriverResultKind _driverResultKind;

        public DriverResultKind DriverResultKind
        {
            get { return _driverResultKind; }

            set
            {
                if (!Equals(_driverResultKind, value))
                {
                    _driverResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DriverResultKind)));
                }
            }
        }

        private string _driverFailedContent;

        public string DriverFailedContent
        {
            get { return _driverFailedContent; }

            set
            {
                if (!string.Equals(_driverFailedContent, value))
                {
                    _driverFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DriverFailedContent)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private DriverSortRuleKind _selectedRule = DriverSortRuleKind.DeviceName;

        public DriverSortRuleKind SelectedRule
        {
            get { return _selectedRule; }

            set
            {
                if (!Equals(_selectedRule, value))
                {
                    _selectedRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRule)));
                }
            }
        }

        private List<DriverModel> DriverList { get; } = [];

        private ObservableCollection<DriverModel> DriverCollection { get; } = [];

        private ObservableCollection<DriverOperationModel> DriverOperationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DriverManagerPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await GetDriverAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第一部分：ExecuteCommand 命令调用时挂载的事件

        /// <summary>
        /// 点击选中驱动项
        /// </summary>
        private void OnCheckBoxExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        private void OnOpenFolderExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string driverLocation)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Process.Start(File.Exists(driverLocation) ? Path.GetDirectoryName(driverLocation) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(OnOpenFolderExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async void OnDeleteDriverExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driver && File.Exists(driver.DriverLocation))
            {
                DriverResultKind = DriverResultKind.Operating;
                Guid driverOperationGuid = Guid.NewGuid();
                DriverOperationCollection.Add(new DriverOperationModel
                {
                    DriverInfName = driver.DriverInfName,
                    DriverOperation = DeletingDriverString,
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                });

                // 删除驱动
                (bool operationResult, Win32Exception win32Exception) = await Task.Run(() =>
                {
                    bool result = SetupapiLibrary.SetupUninstallOEMInf(driver.DriverOEMInfName, SUOI_Flags.SUOI_NONE, IntPtr.Zero);
                    return result ? ValueTuple.Create<bool, Win32Exception>(result, null) : ValueTuple.Create(result, new Win32Exception(Kernel32Library.GetLastError()));
                });

                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                {
                    if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                    {
                        driverOperationItem.IsOperating = false;
                        driverOperationItem.DriverOperation = operationResult ? DeletingDriverSuccessfullyString : string.Format(DeletingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        break;
                    }
                }

                if (operationResult)
                {
                    synchronizationContext.Send(async (_) =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteDriverSuccessfully));
                    }, null);

                    await GetDriverAsync();
                }
                else
                {
                    DriverResultKind = DriverResultKind.Successfully;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async void OnForceDeleteDriverExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driver && File.Exists(driver.DriverLocation))
            {
                DriverResultKind = DriverResultKind.Operating;
                Guid driverOperationGuid = Guid.NewGuid();
                DriverOperationCollection.Add(new DriverOperationModel
                {
                    DriverInfName = driver.DriverInfName,
                    DriverOperation = ForceDeletingDriverString,
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                });

                // 强制删除驱动
                bool needReboot = false;
                (bool operationResult, Win32Exception win32Exception) = await Task.Run(() =>
                {
                    bool result = NewDevLibrary.DiUninstallDriver(IntPtr.Zero, driver.DriverLocation, DIURFLAG.NO_REMOVE_INF, out needReboot) && SetupapiLibrary.SetupUninstallOEMInf(driver.DriverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, IntPtr.Zero);
                    return result ? ValueTuple.Create<bool, Win32Exception>(result, null) : ValueTuple.Create(result, new Win32Exception(Kernel32Library.GetLastError()));
                });

                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                {
                    if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                    {
                        driverOperationItem.IsOperating = false;
                        driverOperationItem.DriverOperation = operationResult ? ForceDeletingDriverSuccessfullyString : string.Format(ForceDeletingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        break;
                    }
                }

                if (operationResult)
                {
                    synchronizationContext.Send(async (_) =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ForceDeleteDriverSuccessfully));
                    }, null);

                    if (needReboot)
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.UnInstallDriver));

                            await Task.Run(() =>
                            {
                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                }
                            });
                        }, null);
                    }

                    await GetDriverAsync();
                }
                else
                {
                    DriverResultKind = DriverResultKind.Successfully;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ForceDeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        private void OnRemoveTaskExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DriverOperationModel driverOperation)
            {
                DriverOperationCollection.Remove(driverOperation);
            }
        }

        #endregion 第一部分：ExecuteCommand 命令调用时挂载的事件

        #region 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 以管理员身份运行
        /// </summary>

        private void OnRunAsAdministratorClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = "--elevated",
                        FileName = System.Windows.Forms.Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
            });
        }

        /// <summary>
        /// 了解驱动管理的具体的使用说明
        /// </summary>
        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            await Task.Delay(300);
            if (!DriverSplitView.IsPaneOpen)
            {
                DriverPaneKind = DriverPaneKind.UseInstruction;
                DriverSplitView.OpenPaneLength = 320;
                DriverSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 打开设备管理器
        /// </summary>
        private void OnOpenDeviceManagementClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("devmgmt.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(OnOpenDeviceManagementClicked), 1, e);
                }
            });
        }

        /// <summary>
        /// 清空驱动操作任务（只允许清理未正进行的驱动操作任务）
        /// </summary>

        private void OnClearDriverOperationTaskClicked(object sender, RoutedEventArgs args)
        {
            for (int index = DriverOperationCollection.Count - 1; index >= 0; index--)
            {
                if (!DriverOperationCollection[index].IsOperating)
                {
                    DriverOperationCollection.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 点击关闭按钮关闭任务管理
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (DriverSplitView.IsPaneOpen)
            {
                DriverSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 搜索驱动名称
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && DriverResultKind is not DriverResultKind.Loading && DriverList.Count > 0)
            {
                DriverResultKind = DriverResultKind.Loading;
                DriverCollection.Clear();
                foreach (DriverModel driverItem in DriverList)
                {
                    if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                    {
                        driverItem.IsSelected = false;
                        DriverCollection.Add(driverItem);
                    }
                }

                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.Successfully;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyWithConditionDescriptionString : string.Empty;
                DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 搜索驱动名称内容发生变化事件
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && DriverResultKind is not DriverResultKind.Loading && DriverList.Count > 0)
            {
                DriverResultKind = DriverResultKind.Loading;
                DriverCollection.Clear();
                foreach (DriverModel driverItem in DriverList)
                {
                    driverItem.IsSelected = false;
                    DriverCollection.Add(driverItem);
                }

                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.Successfully;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyWithConditionDescriptionString : string.Empty;
                DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DriverModel driverItem in DriverCollection)
            {
                driverItem.IsSelected = true;
            }

            DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (DriverModel driverItem in DriverCollection)
            {
                driverItem.IsSelected = false;
            }

            DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
        }

        /// <summary>
        /// 打开任务管理
        /// </summary>
        private void OnTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            if (!DriverSplitView.IsPaneOpen)
            {
                DriverPaneKind = DriverPaneKind.TaskManager;
                DriverSplitView.OpenPaneLength = 400;
                DriverSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                IsIncrease = Convert.ToBoolean(tag);
                GetMatchedDrivers();
            }
        }

        /// <summary>
        /// 排序规则
        /// </summary>
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is DriverSortRuleKind driverSortRuleKind)
            {
                SelectedRule = driverSortRuleKind;
                GetMatchedDrivers();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetDriverAsync();
        }

        /// <summary>
        /// 添加驱动
        /// </summary>
        private async void OnAddDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                OpenFileDialog openFileDialog = new()
                {
                    Multiselect = true,
                    Title = SelectFileString,
                    Filter = DriverFilterConditionString,
                };

                if (openFileDialog.ShowDialog() is DialogResult.OK)
                {
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Loading;
                    List<Task> addDriverTaskList = [];
                    Dictionary<Guid, (string fileName, bool result, string driverOperation)> addDriverResultDict = [];
                    object addDriverResultDictObject = new();

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();
                        DriverOperationCollection.Add(new DriverOperationModel
                        {
                            DriverInfName = Path.GetFileName(fileName),
                            DriverOperation = AddingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        });

                        addDriverTaskList.Add(Task.Run(() =>
                        {
                            StringBuilder stringBuilder = new(260);
                            uint bufferSize = 0;
                            bool result = SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, IntPtr.Zero);
                            Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                            string driverOperation = result ? AddingDriverSuccessfullyString : string.Format(AddingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);

                            lock (addDriverResultDictObject)
                            {
                                addDriverResultDict.Add(driverOperationGuid, ValueTuple.Create(fileName, result, driverOperation));
                            }

                            synchronizationContext.Send((_) =>
                            {
                                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                                {
                                    if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                                    {
                                        driverOperationItem.DriverOperation = driverOperation;
                                        driverOperationItem.IsOperating = false;
                                        break;
                                    }
                                }
                            }, null);
                        }));
                    }

                    await Task.WhenAll([.. addDriverTaskList]);

                    if (addDriverResultDict.Values.Any(item => item.result))
                    {
                        if (addDriverResultDict.Values.All(item => item.result))
                        {
                            synchronizationContext.Send(async (_) =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddDriverAllSuccessfully));
                            }, null);
                        }
                        else
                        {
                            synchronizationContext.Send(async (_) =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddDriverPartialSuccessfully));
                            }, null);
                        }

                        await GetDriverAsync();
                    }
                    else
                    {
                        DriverResultKind = oldDriverResultKind;
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddDriverFailed));
                    }
                }
            }
        }

        /// <summary>
        /// 添加并安装驱动
        /// </summary>
        private async void OnAddInstallDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                OpenFileDialog openFileDialog = new()
                {
                    Multiselect = true,
                    Title = SelectFileString,
                    Filter = DriverFilterConditionString,
                };

                if (openFileDialog.ShowDialog() is DialogResult.OK)
                {
                    bool needReboot = false;
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Loading;
                    List<Task> addInstallDriverTaskList = [];
                    Dictionary<Guid, (string fileName, bool result, string driverOperation)> addInstallDriverResultDict = [];
                    object addInstallDriverResultDictObject = new();

                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();

                        DriverOperationCollection.Add(new DriverOperationModel
                        {
                            DriverInfName = Path.GetFileName(fileName),
                            DriverOperation = AddInstallingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        });

                        addInstallDriverTaskList.Add(Task.Run(() =>
                        {
                            StringBuilder stringBuilder = new(260);
                            uint bufferSize = 0;
                            bool result = NewDevLibrary.DiInstallDriver(IntPtr.Zero, fileName, 0, out bool NeedReboot) && SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, IntPtr.Zero);
                            Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                            string driverOperation = result ? AddInstallingDriverSuccessfullyString : string.Format(AddInstallingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                            lock (addInstallDriverResultDictObject)
                            {
                                addInstallDriverResultDict.Add(driverOperationGuid, ValueTuple.Create(fileName, result, driverOperation));
                            }

                            synchronizationContext.Send((_) =>
                            {
                                foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                                {
                                    if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                                    {
                                        driverOperationItem.DriverOperation = driverOperation;
                                        driverOperationItem.IsOperating = false;
                                        break;
                                    }
                                }
                            }, null);
                        }));
                    }

                    await Task.WhenAll([.. addInstallDriverTaskList]);

                    if (addInstallDriverResultDict.Values.Any(item => item.result))
                    {
                        if (addInstallDriverResultDict.Values.All(item => item.result))
                        {
                            synchronizationContext.Send(async (_) =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddInstallDriverAllSuccessfully));
                            }, null);
                        }
                        else
                        {
                            synchronizationContext.Send(async (_) =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddInstallDriverPartialSuccessfully));
                            }, null);
                        }

                        if (needReboot)
                        {
                            synchronizationContext.Send(async (_) =>
                            {
                                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.InstallDriver));

                                await Task.Run(() =>
                                {
                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                    }
                                });
                            }, null);
                        }

                        await GetDriverAsync();
                    }
                    else
                    {
                        DriverResultKind = oldDriverResultKind;
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.AddInstallDriverFailed));
                    }
                }
            }
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async void OnDeleteDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                List<DriverModel> selectedDriverList = [.. DriverCollection.Where(item => item.IsSelected)];

                if (selectedDriverList.Count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectDriverEmpty));
                    return;
                }

                DriverResultKind oldDriverResultKind = DriverResultKind;
                DriverResultKind = DriverResultKind.Operating;
                List<Task> deleteDriverTaskList = [];
                Dictionary<Guid, (string driverInfName, bool result, string driverOperation)> deleteDriverResultDict = [];
                object deleteDriverResultDictLock = new();

                foreach (DriverModel driverItem in selectedDriverList)
                {
                    Guid driverOperationGuid = Guid.NewGuid();
                    DriverOperationCollection.Add(new DriverOperationModel
                    {
                        DriverInfName = driverItem.DriverInfName,
                        DriverOperation = DeletingDriverString,
                        DriverOperationGuid = driverOperationGuid,
                        IsOperating = true
                    });

                    deleteDriverTaskList.Add(Task.Run(() =>
                    {
                        bool result = SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_NONE, IntPtr.Zero);
                        Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                        string driverOperation = result ? DeletingDriverSuccessfullyString : string.Format(DeletingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        lock (deleteDriverResultDictLock)
                        {
                            deleteDriverResultDict.Add(driverOperationGuid, ValueTuple.Create(driverItem.DriverInfName, result, driverOperation));
                        }

                        synchronizationContext.Send((_) =>
                        {
                            foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                            {
                                if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                                {
                                    driverOperationItem.DriverOperation = driverOperation;
                                    driverOperationItem.IsOperating = false;
                                    break;
                                }
                            }
                        }, null);
                    }));
                }

                await Task.WhenAll([.. deleteDriverTaskList]);

                if (deleteDriverResultDict.Values.Any(item => item.result))
                {
                    if (deleteDriverResultDict.Values.All(item => item.result))
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteDriverAllSuccessfully));
                        }, null);
                    }
                    else
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteDriverPartialSuccessfully));
                        }, null);
                    }

                    await GetDriverAsync();
                }
                else
                {
                    DriverResultKind = oldDriverResultKind;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async void OnForceDeleteDriverClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                List<DriverModel> selectedDriverList = [.. DriverCollection.Where(item => item.IsSelected)];

                if (selectedDriverList.Count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectDriverEmpty));
                    return;
                }

                bool needReboot = false;
                DriverResultKind oldDriverResultKind = DriverResultKind;
                DriverResultKind = DriverResultKind.Operating;
                List<Task> forceDeleteDriverTaskList = [];
                Dictionary<Guid, (string driverInfName, bool result, string driverOperation)> forceDeleteDriverResultDict = [];
                object forceDeleteDriverResultDictLock = new();

                foreach (DriverModel driverItem in selectedDriverList)
                {
                    Guid driverOperationGuid = Guid.NewGuid();

                    DriverOperationCollection.Add(new DriverOperationModel
                    {
                        DriverInfName = driverItem.DriverInfName,
                        DriverOperation = ForceDeletingDriverString,
                        DriverOperationGuid = driverOperationGuid,
                        IsOperating = true
                    });

                    forceDeleteDriverTaskList.Add(Task.Run(() =>
                    {
                        bool result = NewDevLibrary.DiUninstallDriver(IntPtr.Zero, driverItem.DriverLocation, DIURFLAG.NO_REMOVE_INF, out bool NeedReboot) && SetupapiLibrary.SetupUninstallOEMInf(driverItem.DriverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, IntPtr.Zero);
                        Win32Exception win32Exception = new(Kernel32Library.GetLastError());
                        string driverOperation = result ? ForceDeletingDriverSuccessfullyString : string.Format(ForceDeletingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                        lock (forceDeleteDriverResultDictLock)
                        {
                            forceDeleteDriverResultDict.Add(driverOperationGuid, ValueTuple.Create(driverItem.DriverInfName, result, driverOperation));
                        }

                        if (NeedReboot)
                        {
                            needReboot = NeedReboot;
                        }

                        synchronizationContext.Send((_) =>
                        {
                            foreach (DriverOperationModel driverOperationItem in DriverOperationCollection)
                            {
                                if (Equals(driverOperationItem.DriverOperationGuid, driverOperationGuid))
                                {
                                    driverOperationItem.DriverOperation = driverOperation;
                                    driverOperationItem.IsOperating = false;
                                    break;
                                }
                            }
                        }, null);
                    }));
                }

                await Task.WhenAll([.. forceDeleteDriverTaskList]);

                if (forceDeleteDriverResultDict.Values.Any(item => item.result))
                {
                    if (forceDeleteDriverResultDict.Values.All(item => item.result))
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ForceDeleteDriverAllSuccessfully));
                        }, null);
                    }
                    else
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ForceDeleteDriverPartialSuccessfully));
                        }, null);
                    }

                    // 添加重启提示
                    if (needReboot)
                    {
                        synchronizationContext.Send(async (_) =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(DriverInstallKind.UnInstallDriver));

                            await Task.Run(() =>
                            {
                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                }
                            });
                        }, null);
                    }

                    await GetDriverAsync();
                }
                else
                {
                    DriverResultKind = oldDriverResultKind;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ForceDeleteDriverFailed));
                }
            }
        }

        /// <summary>
        /// 选择旧驱动
        /// </summary>
        private void OnSelectOldDriverClicked(object sender, RoutedEventArgs args)
        {
            IEnumerable<IGrouping<string, DriverModel>> driverInfGroupInfoList = DriverCollection.GroupBy(item => item.DriverInfName);

            foreach (IGrouping<string, DriverModel> driverInfGroupInfo in driverInfGroupInfoList)
            {
                if (driverInfGroupInfo.Count() <= 1)
                {
                    continue;
                }

                DateTime lastestDate = driverInfGroupInfo.Max(item => item.DriverDate);

                foreach (DriverModel driverItem in driverInfGroupInfo)
                {
                    if (driverItem.DriverDate < lastestDate)
                    {
                        driverItem.IsSelected = true;
                    }
                }
            }

            DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
        }

        #endregion 第二部分：驱动管理页面——挂载的事件

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async Task GetDriverAsync()
        {
            DriverResultKind = DriverResultKind.Loading;
            DriverList.Clear();
            DriverCollection.Clear();
            DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));

            List<DriverModel> driverList = await Task.Run(() =>
            {
                List<DriverModel> driverList = GetDriverInformationList();
                driverList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));
                return driverList;
            });

            DriverList.AddRange(driverList);

            if (DriverList.Count is 0)
            {
                DriverResultKind = DriverResultKind.Failed;
                DriverFailedContent = DriverEmptyDescriptionString;
            }
            else
            {
                if (Equals(SelectedRule, DriverSortRuleKind.DeviceName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DeviceName) : DriverList.OrderByDescending(item => item.DeviceName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.InfName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverInfName) : DriverList.OrderByDescending(item => item.DriverInfName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.OEMInfName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverOEMInfName) : DriverList.OrderByDescending(item => item.DriverOEMInfName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.DeviceType))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverType) : DriverList.OrderByDescending(item => item.DriverType))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Manufacturer))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverManufacturer) : DriverList.OrderByDescending(item => item.DriverManufacturer))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Manufacturer))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverVersion) : DriverList.OrderByDescending(item => item.DriverVersion))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Date))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverDate) : DriverList.OrderByDescending(item => item.DriverDate))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }

                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.Successfully;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyWithConditionDescriptionString : string.Empty;
                DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 检索符合条件的驱动
        /// </summary>
        private void GetMatchedDrivers()
        {
            if (DriverResultKind is not DriverResultKind.Loading && DriverList.Count > 0)
            {
                DriverResultKind = DriverResultKind.Loading;
                DriverCollection.Clear();

                if (Equals(SelectedRule, DriverSortRuleKind.DeviceName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DeviceName) : DriverList.OrderByDescending(item => item.DeviceName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.InfName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverInfName) : DriverList.OrderByDescending(item => item.DriverInfName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.OEMInfName))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverOEMInfName) : DriverList.OrderByDescending(item => item.DriverOEMInfName))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.DeviceType))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverType) : DriverList.OrderByDescending(item => item.DriverType))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Manufacturer))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverManufacturer) : DriverList.OrderByDescending(item => item.DriverManufacturer))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Manufacturer))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverVersion) : DriverList.OrderByDescending(item => item.DriverVersion))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }
                else if (Equals(SelectedRule, DriverSortRuleKind.Date))
                {
                    foreach (DriverModel driverItem in IsIncrease ? DriverList.OrderBy(item => item.DriverDate) : DriverList.OrderByDescending(item => item.DriverDate))
                    {
                        driverItem.IsSelected = false;

                        if (string.IsNullOrEmpty(SearchText))
                        {
                            DriverCollection.Add(driverItem);
                        }
                        else
                        {
                            if (driverItem.DeviceName.Contains(SearchText) || driverItem.DriverInfName.Contains(SearchText) || driverItem.DriverOEMInfName.Contains(SearchText) || driverItem.DriverManufacturer.Contains(SearchText))
                            {
                                DriverCollection.Add(driverItem);
                            }
                        }
                    }
                }

                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.Successfully;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyWithConditionDescriptionString : string.Empty;
                DriverDescription = string.Format(DriverInformationString, DriverCollection.Count, DriverCollection.Count(item => item.IsSelected));
            }
        }

        /// <summary>
        /// 获取设备上所有的驱动信息
        /// </summary>
        private List<DriverModel> GetDriverInformationList()
        {
            List<DriverModel> driverList = [];

            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "pnputil.exe",
                    Arguments = @"/enum-drivers /format ""xml""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                Process process = Process.Start(startInfo);
                StreamReader streamReader = process.StandardOutput;
                string driverString = streamReader.ReadToEnd();
                process.WaitForExit();
                process.Dispose();

                XDocument xDocument = XDocument.Parse(driverString);
                List<PnpDriverInformation> pnpDriverInformationList = [];

                if (xDocument is not null)
                {
                    List<XElement> driverNodeList = [.. xDocument.Descendants("Driver")];

                    foreach (XElement driverNode in driverNodeList)
                    {
                        PnpDriverInformation pnpDriverInformation = new();

                        if (driverNode.Attribute("DriverName") is XAttribute driverNameAttribute)
                        {
                            pnpDriverInformation.DriverName = driverNameAttribute.Value;
                        }

                        if (driverNode.Element("OriginalName") is XElement driverNameElement)
                        {
                            pnpDriverInformation.OriginalName = driverNameElement.Value;
                        }

                        if (driverNode.Element("ProviderName") is XElement providerNameElement)
                        {
                            pnpDriverInformation.ProviderName = providerNameElement.Value;
                        }

                        if (driverNode.Element("ClassName") is XElement classNameElement)
                        {
                            pnpDriverInformation.ClassName = classNameElement.Value;
                        }

                        if (driverNode.Element("ClassGuid") is XElement classGuidElement)
                        {
                            pnpDriverInformation.ClassGuid = classGuidElement.Value;
                        }

                        if (driverNode.Element("DriverVersion") is XElement driverVersionElement)
                        {
                            string[] driverVersionArray = driverVersionElement.Value.Split(' ');

                            if (driverVersionArray.Length is 2)
                            {
                                pnpDriverInformation.DriverDate = driverVersionArray[0];
                                pnpDriverInformation.DriverVersion = driverVersionArray[1];
                            }
                        }

                        if (driverNode.Element("SignerName") is XElement signerNameElement)
                        {
                            pnpDriverInformation.SignerName = signerNameElement.Value;
                        }

                        pnpDriverInformationList.Add(pnpDriverInformation);
                    }
                }

                List<SystemDriverInformation> systemDriverInformationList = [];
                IntPtr deviceInfoSet = SetupapiLibrary.SetupDiGetClassDevs(Guid.Empty, null, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES);

                if (!deviceInfoSet.Equals(IntPtr.Zero))
                {
                    SP_DEVINFO_DATA deviceInfoData = new()
                    {
                        cbSize = Marshal.SizeOf<SP_DEVINFO_DATA>(),
                    };

                    // 枚举所有设备
                    for (int index = 0; SetupapiLibrary.SetupDiEnumDeviceInfo(deviceInfoSet, index, ref deviceInfoData); index++)
                    {
                        object deviceGuid = GetDevNodeProperty("DEVPKEY_Device_ClassGuid", deviceInfoData, deviceInfoSet);
                        object deviceDesc = GetDevNodeProperty("DEVPKEY_Device_DeviceDesc", deviceInfoData, deviceInfoSet);
                        object driverInfPath = GetDevNodeProperty("DEVPKEY_Device_DriverInfPath", deviceInfoData, deviceInfoSet);
                        object driverDate = GetDevNodeProperty("DEVPKEY_Device_DriverDate", deviceInfoData, deviceInfoSet);
                        object driverVersion = GetDevNodeProperty("DEVPKEY_Device_DriverVersion", deviceInfoData, deviceInfoSet);

                        if (deviceGuid is not null && deviceDesc is not null && driverInfPath is not null && driverDate is not null && driverVersion is not null)
                        {
                            systemDriverInformationList.Add(new SystemDriverInformation()
                            {
                                DeviceGuid = deviceGuid is null ? Guid.NewGuid() : Guid.TryParse(Convert.ToString(deviceGuid), out Guid guid) ? guid : Guid.NewGuid(),
                                Description = Convert.ToString(deviceDesc),
                                InfPath = Convert.ToString(driverInfPath),
                                Date = (DateTime)driverDate,
                                Version = new Version(Convert.ToString(driverVersion))
                            });
                        }
                    }

                    SetupapiLibrary.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }

                foreach (PnpDriverInformation pnpDriverInformationItem in pnpDriverInformationList)
                {
                    string driverLocation = GetDriverStoreLocation(pnpDriverInformationItem.DriverName);

                    DriverModel driverItem = new()
                    {
                        DriverInfName = pnpDriverInformationItem.OriginalName,
                        DriverOEMInfName = pnpDriverInformationItem.DriverName,
                        DriverManufacturer = pnpDriverInformationItem.ProviderName,
                        DriverDate = DateTime.Parse(pnpDriverInformationItem.DriverDate),
                        DriverVersion = new Version(pnpDriverInformationItem.DriverVersion),
                        DriverSize = string.IsNullOrEmpty(driverLocation) ? "0B" : VolumeSizeHelper.ConvertVolumeSizeToString(GetFolderSize(Path.GetDirectoryName(driverLocation))),
                        DriverLocation = driverLocation,
                        DriverType = pnpDriverInformationItem.ClassName,
                        SignatureName = string.IsNullOrEmpty(pnpDriverInformationItem.SignerName) ? UnknownString : pnpDriverInformationItem.SignerName,
                    };

                    Guid dismDriverPackageItemGuid = new(pnpDriverInformationItem.ClassGuid);

                    foreach (SystemDriverInformation systemDriverInformation in systemDriverInformationList)
                    {
                        if (Equals(dismDriverPackageItemGuid, systemDriverInformation.DeviceGuid) && string.Equals(driverItem.DriverOEMInfName, systemDriverInformation.InfPath, StringComparison.OrdinalIgnoreCase) && driverItem.DriverDate.Equals(systemDriverInformation.Date) && driverItem.DriverVersion.Equals(systemDriverInformation.Version))
                        {
                            driverItem.DeviceName = string.IsNullOrEmpty(systemDriverInformation.Description) ? UnknownDeviceNameString : systemDriverInformation.Description;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(driverItem.DeviceName))
                    {
                        driverItem.DeviceName = UnknownDeviceNameString;
                    }

                    driverList.Add(driverItem);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetDriverInformationList), 1, e);
            }

            return driverList;
        }

        /// <summary>
        /// 检索设备实例属性
        /// </summary>
        private object GetDevNodeProperty(string devPropKey, SP_DEVINFO_DATA deviceInfoData, IntPtr deviceInfoSet)
        {
            object value = null;

            if (DevPropKeyDict.TryGetValue(devPropKey, out DEVPROPKEY devPropKeyItem))
            {
                SetupapiLibrary.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref devPropKeyItem, out _, IntPtr.Zero, 0, out int bufferSize, 0);
                IntPtr propertyBufferPtr = Marshal.AllocHGlobal(bufferSize);

                if (SetupapiLibrary.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref devPropKeyItem, out DEVPROP_TYPE devPropertyType, propertyBufferPtr, bufferSize, out bufferSize, 0))
                {
                    // 空值
                    switch (devPropertyType)
                    {
                        case DEVPROP_TYPE.DEVPROP_TYPE_EMPTY: break;
                        case DEVPROP_TYPE.DEVPROP_TYPE_NULL: break;
                        case DEVPROP_TYPE.DEVPROP_TYPE_SBYTE:
                            {
                                value = Convert.ToSByte(Marshal.ReadByte(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BYTE:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT16:
                            {
                                value = Marshal.ReadInt16(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT16:
                            {
                                value = Convert.ToUInt16(Marshal.ReadInt16(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT32:
                            {
                                value = Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT32:
                            {
                                value = Convert.ToUInt32(Marshal.ReadInt32(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_INT64:
                            {
                                value = Marshal.ReadInt64(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_UINT64:
                            {
                                value = Convert.ToUInt64(Marshal.ReadInt64(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_FLOAT:
                            {
                                byte[] byteBuffer = new byte[sizeof(float)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = BitConverter.ToSingle(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DOUBLE:
                            {
                                byte[] byteBuffer = new byte[sizeof(double)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = BitConverter.ToDouble(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DECIMAL:
                            {
                                byte[] byteBuffer = new byte[sizeof(decimal)];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = Convert.ToDecimal(BitConverter.ToDouble(byteBuffer, 0));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_GUID:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<Guid>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = new Guid(byteBuffer);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_CURRENCY:
                            {
                                value = Marshal.ReadInt64(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DATE:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<System.Runtime.InteropServices.ComTypes.FILETIME>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = DateTime.FromOADate(BitConverter.ToDouble(byteBuffer, 0)).Date;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_FILETIME:
                            {
                                long fileTime = Marshal.ReadInt64(propertyBufferPtr);
                                value = DateTime.FromFileTime(fileTime).Date;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BOOLEAN:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr) is not 0;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_STRING:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR:
                            {
                                byte[] byteBuffer = new byte[Marshal.SizeOf<System.Runtime.InteropServices.ComTypes.FILETIME>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = new RawSecurityDescriptor(byteBuffer, 0);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPKEY:
                            {
                                DEVPROPKEY key = new();

                                byte[] byteBuffer = new byte[Marshal.SizeOf<Guid>()];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                key.fmtid = new Guid(byteBuffer);
                                key.pid = Convert.ToUInt32(Marshal.ReadInt32(propertyBufferPtr, 16));
                                value = key;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPTYPE:
                            {
                                value = (DEVPROP_TYPE)Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_ERROR:
                            {
                                value = Marshal.GetExceptionForHR(Marshal.ReadInt32(propertyBufferPtr));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_NTSTATUS:
                            {
                                value = Marshal.ReadInt32(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_STRING_INDIRECT:
                            {
                                value = devPropertyType.HasFlag(DEVPROP_TYPE.DEVPROP_TYPEMOD_LIST) ? Marshal.PtrToStringUni(propertyBufferPtr, bufferSize / 2).Split(['\0'], StringSplitOptions.RemoveEmptyEntries) : Marshal.PtrToStringUni(propertyBufferPtr);
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPEMOD_ARRAY:
                            {
                                byte[] byteBuffer = new byte[bufferSize];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = byteBuffer;
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_BINARY:
                            {
                                value = Marshal.ReadByte(propertyBufferPtr);
                                break;
                            }
                        default:
                            {
                                byte[] byteBuffer = new byte[bufferSize];
                                for (int index = 0; index < byteBuffer.Length; index++)
                                {
                                    byteBuffer[index] = Marshal.ReadByte(propertyBufferPtr + index);
                                }

                                value = byteBuffer;
                                break;
                            }
                    }
                }

                Marshal.FreeHGlobal(propertyBufferPtr);
            }

            return value;
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        public long GetFolderSize(string directoryPath)
        {
            long totalSize = 0;

            if (Directory.Exists(directoryPath))
            {
                try
                {
                    DirectoryInfo directoryInfo = new(directoryPath);
                    long fileAllSize = 0;

                    // 获取当前目录所有文件大小
                    try
                    {
                        FileInfo[] filesArray = directoryInfo.GetFiles();

                        Parallel.ForEach(filesArray, file =>
                        {
                            try
                            {
                                long fileSize = file.Length;
                                Interlocked.Add(ref fileAllSize, fileSize);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 1, e);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 2, e);
                    }

                    totalSize += fileAllSize;

                    // 获取当前文件夹包含的所有子目录大小
                    long folderAllSize = 0;

                    try
                    {
                        DirectoryInfo[] directoryArray = directoryInfo.GetDirectories();

                        Parallel.ForEach(directoryArray, directory =>
                        {
                            try
                            {
                                long folderSize = GetFolderSize(directory.FullName);
                                Interlocked.Add(ref folderAllSize, folderSize);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 3, e);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 4, e);
                    }

                    totalSize += folderAllSize;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(EventLevel.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 5, e);
                }
            }

            return totalSize;
        }

        /// <summary>
        /// 获取驱动存放的实际路径
        /// </summary>
        private static string GetDriverStoreLocation(string oemInfName)
        {
            SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, IntPtr.Zero, IntPtr.Zero, null, 0, out int bufferSize);

            if (bufferSize > 0)
            {
                StringBuilder stringBuilder = new(bufferSize);
                return SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity, out _) ? Convert.ToString(stringBuilder) : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取加载驱动是否成功
        /// </summary>
        private Visibility GetDriverSuccessfullyState(DriverResultKind driverResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? driverResultKind is DriverResultKind.Successfully || driverResultKind is DriverResultKind.Operating ? Visibility.Visible : Visibility.Collapsed : driverResultKind is DriverResultKind.Successfully || driverResultKind is DriverResultKind.Operating ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索驱动是否成功
        /// </summary>
        private Visibility CheckDriverState(DriverResultKind driverResultKind, DriverResultKind comparedDriverResultKind)
        {
            return Equals(driverResultKind, comparedDriverResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查驱动管理浮出面板状态
        /// </summary>
        private Visibility CheckDriverPaneKindState(DriverPaneKind driverPaneKind, DriverPaneKind comparedDriverPaneKind)
        {
            return Equals(driverPaneKind, comparedDriverPaneKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>
        private bool GetIsLoadingOrOperating(DriverResultKind driverResultKind)
        {
            return !(driverResultKind is DriverResultKind.Loading || driverResultKind is DriverResultKind.Operating);
        }

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>
        private bool GetElevatedIsLoadingOrOperating(DriverResultKind driverResultKind)
        {
            return RuntimeHelper.IsElevated && !(driverResultKind is DriverResultKind.Loading || driverResultKind is DriverResultKind.Operating);
        }

        /// <summary>
        /// 获取是否正在操作中
        /// </summary>
        private bool GetIsOperating(DriverResultKind driverResultKind)
        {
            return driverResultKind is not DriverResultKind.Operating;
        }
    }
}
