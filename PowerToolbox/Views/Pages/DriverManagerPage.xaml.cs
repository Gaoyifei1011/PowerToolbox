using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PowerToolbox.Extensions.DataType.Classes;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

// 抑制 CA1806，CA1822，IDE0060 警告
#pragma warning disable CA1806,CA1822,IDE0060

namespace PowerToolbox.Views.Pages
{
    /// <summary>
    /// 驱动管理页面
    /// </summary>
    internal sealed partial class DriverManagerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AddingDriverFailedString = ResourceService.DriverManagerResource.GetString("AddingDriverFailed");
        private readonly string AddingDriverString = ResourceService.DriverManagerResource.GetString("AddingDriver");
        private readonly string AddingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("AddingDriverSuccessfully");
        private readonly string AddInstallingDriverFailedString = ResourceService.DriverManagerResource.GetString("AddInstallingDriverFailed");
        private readonly string AddInstallingDriverString = ResourceService.DriverManagerResource.GetString("AddInstallingDriver");
        private readonly string AddInstallingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("AddInstallingDriverSuccessfully");
        private readonly string DeletingDriverFailedString = ResourceService.DriverManagerResource.GetString("DeletingDriverFailed");
        private readonly string DeletingDriverString = ResourceService.DriverManagerResource.GetString("DeletingDriver");
        private readonly string DeletingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("DeletingDriverSuccessfully");
        private readonly string DriverCountInfoString = ResourceService.DriverManagerResource.GetString("DriverCountInfo");
        private readonly string DriverEmptyDescriptionString = ResourceService.DriverManagerResource.GetString("DriverEmptyDescription");
        private readonly string DriverFilterConditionString = ResourceService.DriverManagerResource.GetString("DriverFilterCondition");
        private readonly string ForceDeletingDriverFailedString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriverFailed");
        private readonly string ForceDeletingDriverString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriver");
        private readonly string ForceDeletingDriverSuccessfullyString = ResourceService.DriverManagerResource.GetString("ForceDeletingDriverSuccessfully");
        private readonly string NotAvailableString = ResourceService.DriverManagerResource.GetString("NotAvailable");
        private readonly string RestartPCString = ResourceService.DriverManagerResource.GetString("RestartPC");
        private readonly string SelectFileString = ResourceService.DriverManagerResource.GetString("SelectFile");
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;
        private bool isInitialized;

        private static Dictionary<string, DEVPROPKEY> DevPropKeyDict { get; } = new()
        {
            { "DEVPKEY_NAME", new() { fmtid = new("B725F130-47EF-101A-A5F1-02608C9EEBAC"), pid = 10 }},
            { "DEVPKEY_Device_DeviceDesc", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 2 }},
            { "DEVPKEY_Device_HardwareIds", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 3 }},
            { "DEVPKEY_Device_CompatibleIds", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 4 }},
            { "DEVPKEY_Device_Service", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 6 }},
            { "DEVPKEY_Device_Class", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 9 }},
            { "DEVPKEY_Device_ClassGuid", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 10 }},
            { "DEVPKEY_Device_Driver", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 11 }},
            { "DEVPKEY_Device_ConfigFlags", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 12 }},
            { "DEVPKEY_Device_Manufacturer", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 13 }},
            { "DEVPKEY_Device_FriendlyName", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 14 }},
            { "DEVPKEY_Device_LocationInfo", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 15 }},
            { "DEVPKEY_Device_PDOName", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 16 }},
            { "DEVPKEY_Device_Capabilities", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 17 }},
            { "DEVPKEY_Device_UINumber", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 18 }},
            { "DEVPKEY_Device_UpperFilters", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 19 }},
            { "DEVPKEY_Device_LowerFilters", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 20 }},
            { "DEVPKEY_Device_BusTypeGd", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 21 }},
            { "DEVPKEY_Device_LegacyBusType", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 22 }},
            { "DEVPKEY_Device_BusNumber", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 23 }},
            { "DEVPKEY_Device_EnumeratorName", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 24 }},
            { "DEVPKEY_Device_Security", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 25 }},
            { "DEVPKEY_Device_SecuritySDS", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 26 }},
            { "DEVPKEY_Device_DevType", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 27 }},
            { "DEVPKEY_Device_Exclusive", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 28 }},
            { "DEVPKEY_Device_Characteristics", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 29 }},
            { "DEVPKEY_Device_Address", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 30 }},
            { "DEVPKEY_Device_UINumberDescFormat", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 31 }},
            { "DEVPKEY_Device_PowerData", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 32 }},
            { "DEVPKEY_Device_RemovalPolicy", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 33 }},
            { "DEVPKEY_Device_RemovalPolicyDefault", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 34 }},
            { "DEVPKEY_Device_RemovalPolicyOverride", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 35 }},
            { "DEVPKEY_Device_InstallState", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 36 }},
            { "DEVPKEY_Device_LocationPaths", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 37 }},
            { "DEVPKEY_Device_BaseContainerId", new() { fmtid = new("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pid = 38 }},
            { "DEVPKEY_Device_DevNodeStatus", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 2 }},
            { "DEVPKEY_Device_ProblemCode", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 3 }},
            { "DEVPKEY_Device_EjectionRelations", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 4 }},
            { "DEVPKEY_Device_RemovalRelations", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 5 }},
            { "DEVPKEY_Device_PowerRelations", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 6 }},
            { "DEVPKEY_Device_BusRelations", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 7 }},
            { "DEVPKEY_Device_Parent", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 8 }},
            { "DEVPKEY_Device_Children", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 9 }},
            { "DEVPKEY_Device_Siblings", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 10 }},
            { "DEVPKEY_Device_TransportRelations", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 11 }},
            { "DEVPKEY_Device_ProblemStatus", new() { fmtid = new("4340A6C5-93FA-4706-972C-7B648008A5A7"), pid = 12 }},
            { "DEVPKEY_Device_Reported", new() { fmtid = new("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 2 }},
            { "DEVPKEY_Device_Legacy", new() { fmtid = new("80497100-8C73-48B9-AAD9-CE387E19C56E"), pid = 3 }},
            { "DEVPKEY_Device_ContainerId", new() { fmtid = new("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 2 }},
            { "DEVPKEY_Device_InLocalMachineContainer", new() { fmtid = new("8C7ED206-3F8A-4827-B3AB-AE9E1FAEFC6C"), pid = 4 }},
            { "DEVPKEY_Device_ModelId", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 2 }},
            { "DEVPKEY_Device_FriendlyNameAttributes", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 3 }},
            { "DEVPKEY_Device_ManufacturerAttributes", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 4 }},
            { "DEVPKEY_Device_PresenceNotForDevice", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 5 }},
            { "DEVPKEY_Device_SignalStrength", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 6 }},
            { "DEVPKEY_Device_IsAssociateableByUserAction", new() { fmtid = new("80D81EA6-7473-4B0C-8216-EFC11A2C4C8B"), pid = 7 }},
            { "DEVPKEY_Numa_Proximity_Domain", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 1 }},
            { "DEVPKEY_Device_DHP_Rebalance_Policy", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 2 }},
            { "DEVPKEY_Device_Numa_Node", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 3 }},
            { "DEVPKEY_Device_BusReportedDeviceDesc", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 4 }},
            { "DEVPKEY_Device_IsPresent", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 5 }},
            { "DEVPKEY_Device_HasProblem", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 6 }},
            { "DEVPKEY_Device_ConfigurationId", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 7 }},
            { "DEVPKEY_Device_ReportedDeviceIdsHash", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 8 }},
            { "DEVPKEY_Device_PhysicalDeviceLocation", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 9 }},
            { "DEVPKEY_Device_BiosDeviceName", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 10 }},
            { "DEVPKEY_Device_DriverProblemDesc", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 11 }},
            { "DEVPKEY_Device_DebuggerSafe", new() { fmtid = new("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pid = 12 }},
            { "DEVPKEY_Device_SessionId", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 6 }},
            { "DEVPKEY_Device_InstallDate", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 100 }},
            { "DEVPKEY_Device_FirstInstallDate", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 101 }},
            { "DEVPKEY_Device_LastArrivalDate", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 102 }},
            { "DEVPKEY_Device_LastRemovalDate", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 103 }},
            { "DEVPKEY_Device_DriverDate", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 2 }},
            { "DEVPKEY_Device_DriverVersion", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 3 }},
            { "DEVPKEY_Device_DriverDesc", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 4 }},
            { "DEVPKEY_Device_DriverInfPath", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 5 }},
            { "DEVPKEY_Device_DriverInfSection", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 6 }},
            { "DEVPKEY_Device_DriverInfSectionExt", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 7 }},
            { "DEVPKEY_Device_MatchingDeviceId", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 8 }},
            { "DEVPKEY_Device_DriverProvider", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 9 }},
            { "DEVPKEY_Device_DriverPropPageProvider", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 10 }},
            { "DEVPKEY_Device_DriverCoInstallers", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 11 }},
            { "DEVPKEY_Device_ResourcePickerTags", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 12 }},
            { "DEVPKEY_Device_ResourcePickerExceptions", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 13 }},
            { "DEVPKEY_Device_DriverRank", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 14 }},
            { "DEVPKEY_Device_DriverLogoLevel", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 15 }},
            { "DEVPKEY_Device_NoConnectSound", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 17 }},
            { "DEVPKEY_Device_GenericDriverInstalled", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 18 }},
            { "DEVPKEY_Device_AdditionalSoftwareRequested", new() { fmtid = new("A8B865DD-2E3D-4094-AD97-E593A70C75D6"), pid = 19 }},
            { "DEVPKEY_Device_SafeRemovalRequired", new() { fmtid = new("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 2 }},
            { "DEVPKEY_Device_SafeRemovalRequiredOverride", new() { fmtid = new("AFD97640-86A3-4210-B67C-289C41AABE55"), pid = 3 }},
            { "DEVPKEY_DrvPkg_Model", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 2 }},
            { "DEVPKEY_DrvPkg_VendorWebSite", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 3 }},
            { "DEVPKEY_DrvPkg_DetailedDescription", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 4 }},
            { "DEVPKEY_DrvPkg_DocumentationLink", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 5 }},
            { "DEVPKEY_DrvPkg_Icon", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 6 }},
            { "DEVPKEY_DrvPkg_BrandingIcon", new() { fmtid = new("CF73BB51-3ABF-44A2-85E0-9A3DC7A12132"), pid = 7 }},
            { "DEVPKEY_DeviceClass_UpperFilters", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 19 }},
            { "DEVPKEY_DeviceClass_LowerFilters", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 20 }},
            { "DEVPKEY_DeviceClass_Security", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 25 }},
            { "DEVPKEY_DeviceClass_SecuritySDS", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 26 }},
            { "DEVPKEY_DeviceClass_DevType", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 27 }},
            { "DEVPKEY_DeviceClass_Exclusive", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 28 }},
            { "DEVPKEY_DeviceClass_Characteristics", new() { fmtid = new("4321918B-F69E-470D-A5DE-4D88C75AD24B"), pid = 29 }},
            { "DEVPKEY_DeviceClass_Name", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassName", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 3 }},
            { "DEVPKEY_DeviceClass_Icon", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 4 }},
            { "DEVPKEY_DeviceClass_ClassInstaller", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 5 }},
            { "DEVPKEY_DeviceClass_PropPageProvider", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 6 }},
            { "DEVPKEY_DeviceClass_NoInstallClass", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 7 }},
            { "DEVPKEY_DeviceClass_NoDisplayClass", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 8 }},
            { "DEVPKEY_DeviceClass_SilentInstall", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 9 }},
            { "DEVPKEY_DeviceClass_NoUseClass", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 10 }},
            { "DEVPKEY_DeviceClass_DefaultService", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 11 }},
            { "DEVPKEY_DeviceClass_IconPath", new() { fmtid = new("259ABFFC-50A7-47CE-AF08-68C9A7D73366"), pid = 12 }},
            { "DEVPKEY_DeviceClass_DHPRebalanceOptOut", new() { fmtid = new("D14D3EF3-66CF-4BA2-9D38-0DDB37AB4701"), pid = 2 }},
            { "DEVPKEY_DeviceClass_ClassCoInstallers", new() { fmtid = new("713D1703-A2E2-49F5-9214-56472EF3DA5C"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_FriendlyName", new() { fmtid = new("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 2 }},
            { "DEVPKEY_DeviceInterface_Enabled", new() { fmtid = new("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 3 }},
            { "DEVPKEY_DeviceInterface_ClassGuid", new() { fmtid = new("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 4 }},
            { "DEVPKEY_DeviceInterface_ReferenceString", new() { fmtid = new("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 5 }},
            { "DEVPKEY_DeviceInterface_Restricted", new() { fmtid = new("026E516E-B814-414B-83CD-856D6FEF4822"), pid = 6 }},
            { "DEVPKEY_DeviceInterfaceClass_DefaultInterface", new() { fmtid = new("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 2 }},
            { "DEVPKEY_DeviceInterfaceClass_Name", new() { fmtid = new("14C83A99-0B3F-44B7-BE4C-A178D3990564"), pid = 3 }},
            { "DEVPKEY_Device_Model", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 39 }},
            { "DEVPKEY_DeviceContainer_Address", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 51 }},
            { "DEVPKEY_DeviceContainer_DiscoveryMethod", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 52 }},
            { "DEVPKEY_DeviceContainer_IsEncrypted", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 53 }},
            { "DEVPKEY_DeviceContainer_IsAuthenticated", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 54 }},
            { "DEVPKEY_DeviceContainer_IsConnected", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 55 }},
            { "DEVPKEY_DeviceContainer_IsPaired", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 56 }},
            { "DEVPKEY_DeviceContainer_Icon", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 57 }},
            { "DEVPKEY_DeviceContainer_Version", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 65 }},
            { "DEVPKEY_DeviceContainer_Last_Seen", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 66 }},
            { "DEVPKEY_DeviceContainer_Last_Connected", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 67 }},
            { "DEVPKEY_DeviceContainer_IsShowInDisconnectedState", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 68 }},
            { "DEVPKEY_DeviceContainer_IsLocalMachine", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 70 }},
            { "DEVPKEY_DeviceContainer_MetadataPath", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 71 }},
            { "DEVPKEY_DeviceContainer_IsMetadataSearchInProgress", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 72 }},
            { "DEVPKEY_DeviceContainer_MetadataChecksum", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 73 }},
            { "DEVPKEY_DeviceContainer_IsNotInterestingForDisplay", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 74 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageOnDeviceConnect", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 76 }},
            { "DEVPKEY_DeviceContainer_LaunchDeviceStageFromExplorer", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 77 }},
            { "DEVPKEY_DeviceContainer_BaselineExperienceId", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 78 }},
            { "DEVPKEY_DeviceContainer_IsDeviceUniquelyIdentifiable", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 79 }},
            { "DEVPKEY_DeviceContainer_AssociationArray", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 80 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription1", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 81 }},
            { "DEVPKEY_DeviceContainer_DeviceDescription2", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 82 }},
            { "DEVPKEY_DeviceContainer_HasProblem", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 83 }},
            { "DEVPKEY_DeviceContainer_IsSharedDevice", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 84 }},
            { "DEVPKEY_DeviceContainer_IsNetworkDevice", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 85 }},
            { "DEVPKEY_DeviceContainer_IsDefaultDevice", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 86 }},
            { "DEVPKEY_DeviceContainer_MetadataCabinet", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 87 }},
            { "DEVPKEY_DeviceContainer_RequiresPairingElevation", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 88 }},
            { "DEVPKEY_DeviceContainer_ExperienceId", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 89 }},
            { "DEVPKEY_DeviceContainer_Category", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 90 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Singular", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 91 }},
            { "DEVPKEY_DeviceContainer_Category_Desc_Plural", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 92 }},
            { "DEVPKEY_DeviceContainer_Category_Icon", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 93 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Desc", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 94 }},
            { "DEVPKEY_DeviceContainer_CategoryGroup_Icon", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 95 }},
            { "DEVPKEY_DeviceContainer_PrimaryCategory", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 97 }},
            { "DEVPKEY_DeviceContainer_UnpairUninstall", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 98 }},
            { "DEVPKEY_DeviceContainer_RequiresUninstallElevation", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 99 }},
            { "DEVPKEY_DeviceContainer_DeviceFunctionSubRank", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 100 }},
            { "DEVPKEY_DeviceContainer_AlwaysShowDeviceAsConnected", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 101 }},
            { "DEVPKEY_DeviceContainer_ConfigFlags", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 105 }},
            { "DEVPKEY_DeviceContainer_PrivilegedPackageFamilyNames", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 106 }},
            { "DEVPKEY_DeviceContainer_CustomPrivilegedPackageFamilyNames", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 107 }},
            { "DEVPKEY_DeviceContainer_IsRebootRequired", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 108 }},
            { "DEVPKEY_Device_InstanceId", new() { fmtid = new("78C34FC8-104A-4ACA-9EA4-524D52996E57"), pid = 256 }},
            { "DEVPKEY_DeviceContainer_FriendlyName", new() { fmtid = new("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 12288 }},
            { "DEVPKEY_DeviceContainer_Manufacturer", new() { fmtid = new("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8192 }},
            { "DEVPKEY_DeviceContainer_ModelName", new() { fmtid = new("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8194 }},
            { "DEVPKEY_DeviceContainer_ModelNumber", new() { fmtid = new("656A3BB3-ECC0-43FD-8477-4AE0404A96CD"), pid = 8195 }},
            { "DEVPKEY_DeviceContainer_InstallInProgress", new() { fmtid = new("83DA6326-97A6-4088-9453-A1923F573B29"), pid = 9 }}
        };

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、列表与事件

        private DriverPaneKind DriverPaneKind
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DriverPaneKind)));
                }
            }
        }

        private DriverResultKind DriverResultKind
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DriverResultKind)));
                }
            }
        }

        private int SelectedItemsCount
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItemsCount)));
                }
            }
        }

        private string DriverFailedContent
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(DriverFailedContent)));
                }
            }
        }

        private bool IsIncrease
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsIncrease)));
                }
            }
        }

        private DriverSortRuleKind SelectedDriverSortRuleKind
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedDriverSortRuleKind)));
                }
            }
        }

        private List<DriverModel> DriverList { get; } = [];

        private WinRTObservableCollection<DriverModel> DriverCollection { get; } = [];

        private WinRTObservableCollection<DriverOperationModel> DriverOperationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、列表与事件

        #region 第三部分：构造函数

        internal DriverManagerPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await GetDriverAsync(SelectedDriverSortRuleKind, IsIncrease);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 打开文件夹
        /// </summary>
        private void OnOpenFolderExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string driverLocation)
            {
                OpenDriverLocation(driverLocation);
            }
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async void OnDeleteDriverExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driver && File.Exists(driver.DriverLocation) && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating))
            {
                DriverResultKind oldDriverResultKind = DriverResultKind;
                DriverResultKind = DriverResultKind.Operating;
                Guid driverOperationGuid = Guid.NewGuid();
                DriverOperationModel driverOperation = new()
                {
                    DriverInfName = driver.DriverInfName,
                    DriverOperation = DeletingDriverString,
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                };
                DriverOperationCollection.Add(driverOperation);
                ReadOnlyCollection<(string, string, DriverOperationModel)> driverCollection = new([ValueTuple.Create(driver.DriverOEMInfName, driver.DriverLocation, driverOperation)]);
                (bool needReboot, int successfullyCount, int failedCount) = await DeleteDriverCollectionAsync(false, driverCollection);
                if (needReboot)
                {
                    ShowRebootNotification(DriverInstallKind.UnInstallDriver);
                }
                await UpdateDriverOperationResultAsync(DriverInstallKind.UnInstallDriver, false, false, successfullyCount, failedCount, oldDriverResultKind);
            }
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async void OnForceDeleteDriverExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is DriverModel driver && File.Exists(driver.DriverLocation) && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating))
            {
                DriverResultKind oldDriverResultKind = DriverResultKind;
                DriverResultKind = DriverResultKind.Operating;
                Guid driverOperationGuid = Guid.NewGuid();
                DriverOperationModel driverOperation = new()
                {
                    DriverInfName = driver.DriverInfName,
                    DriverOperation = ForceDeletingDriverString,
                    DriverOperationGuid = driverOperationGuid,
                    IsOperating = true
                };
                DriverOperationCollection.Add(driverOperation);
                ReadOnlyCollection<(string, string, DriverOperationModel)> driverCollection = new([ValueTuple.Create(driver.DriverOEMInfName, driver.DriverLocation, driverOperation)]);
                (bool needReboot, int successfullyCount, int failedCount) = await DeleteDriverCollectionAsync(true, driverCollection);
                if (needReboot)
                {
                    ShowRebootNotification(DriverInstallKind.UnInstallDriver);
                }
                await UpdateDriverOperationResultAsync(DriverInstallKind.UnInstallDriver, false, true, successfullyCount, failedCount, oldDriverResultKind);
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

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private void OnRunAsAdministratorClicked(object sender, RoutedEventArgs args)
        {
            RunAsAdministrator();
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
            OpenDeviceManagement();
        }

        /// <summary>
        /// 驱动管理结果列表控件选中项发生变化后触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating)
            {
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
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
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            if (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating)
            {
                DriverManagerListView.SelectAll();
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            if (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating)
            {
                DriverManagerListView.DeselectRange(new(0, (uint)DriverManagerListView.Items.Count));
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            if (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating)
            {
                ReadOnlyCollection<Microsoft.UI.Xaml.Data.ItemIndexRange> selectedRangeCollection = new([.. DriverManagerListView.SelectedRanges]);
                DriverManagerListView.SelectAll();
                foreach (Microsoft.UI.Xaml.Data.ItemIndexRange itemIndexRange in selectedRangeCollection)
                {
                    DriverManagerListView.DeselectRange(itemIndexRange);
                }
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
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
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is bool tag)
            {
                IsIncrease = tag;
                ReadOnlyCollection<DriverModel> sortedDriverCollection = await GetSortedDriversAsync(SelectedDriverSortRuleKind, IsIncrease, DriverList.AsReadOnly());
                foreach (DriverModel sortedDriverItem in sortedDriverCollection)
                {
                    DriverCollection.Add(sortedDriverItem);
                }
                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.HasResult;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyDescriptionString : string.Empty;
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 排序规则
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is DriverSortRuleKind driverSortRuleKind)
            {
                SelectedDriverSortRuleKind = driverSortRuleKind;
                ReadOnlyCollection<DriverModel> sortedDriverCollection = await GetSortedDriversAsync(SelectedDriverSortRuleKind, IsIncrease, DriverList.AsReadOnly());
                foreach (DriverModel sortedDriverItem in sortedDriverCollection)
                {
                    DriverCollection.Add(sortedDriverItem);
                }
                DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.HasResult;
                DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyDescriptionString : string.Empty;
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetDriverAsync(SelectedDriverSortRuleKind, IsIncrease);
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

                if (openFileDialog.ShowDialog() is DialogResult.OK && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating))
                {
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Operating;
                    List<(string, DriverOperationModel)> driverList = [];
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();
                        DriverOperationModel driverOperation = new()
                        {
                            DriverInfName = Path.GetFileName(fileName),
                            DriverOperation = AddingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        };
                        DriverOperationCollection.Add(driverOperation);
                        driverList.Add(ValueTuple.Create(fileName, driverOperation));
                    }
                    (bool needReboot, int successfullyCount, int failedCount) = await AddDriverCollectionAsync(false, driverList.AsReadOnly());
                    if (needReboot)
                    {
                        ShowRebootNotification(DriverInstallKind.InstallDriver);
                    }
                    await UpdateDriverOperationResultAsync(DriverInstallKind.InstallDriver, false, false, successfullyCount, failedCount, oldDriverResultKind);
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

                if (openFileDialog.ShowDialog() is DialogResult.OK && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is DriverResultKind.Operating))
                {
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Operating;
                    List<(string, DriverOperationModel)> driverList = [];
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();
                        DriverOperationModel driverOperation = new()
                        {
                            DriverInfName = Path.GetFileName(fileName),
                            DriverOperation = AddInstallingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        };
                        DriverOperationCollection.Add(driverOperation);
                        driverList.Add(ValueTuple.Create(fileName, driverOperation));
                    }
                    (bool needReboot, int successfullyCount, int failedCount) = await AddDriverCollectionAsync(true, driverList.AsReadOnly());
                    if (needReboot)
                    {
                        ShowRebootNotification(DriverInstallKind.InstallDriver);
                    }
                    await UpdateDriverOperationResultAsync(DriverInstallKind.InstallDriver, true, false, successfullyCount, failedCount, oldDriverResultKind);
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
                List<DriverModel> selectedDriverList = [];
                foreach (object item in DriverManagerListView.SelectedItems)
                {
                    if (item is DriverModel driverItem)
                    {
                        selectedDriverList.Add(driverItem);
                    }
                }

                if (selectedDriverList.Count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectDriverEmpty));
                    return;
                }

                if (RuntimeHelper.IsElevated && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating))
                {
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Operating;
                    List<(string, string, DriverOperationModel)> driverList = [];
                    foreach (DriverModel driverItem in selectedDriverList)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();
                        DriverOperationModel driverOperation = new()
                        {
                            DriverInfName = driverItem.DriverInfName,
                            DriverOperation = DeletingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        };
                        DriverOperationCollection.Add(driverOperation);
                        driverList.Add(ValueTuple.Create(driverItem.DriverOEMInfName, driverItem.DriverLocation, driverOperation));
                    }
                    (bool needReboot, int successfullyCount, int failedCount) = await DeleteDriverCollectionAsync(false, driverList.AsReadOnly());
                    if (needReboot)
                    {
                        ShowRebootNotification(DriverInstallKind.UnInstallDriver);
                    }
                    await UpdateDriverOperationResultAsync(DriverInstallKind.UnInstallDriver, false, false, successfullyCount, failedCount, oldDriverResultKind);
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
                List<DriverModel> selectedDriverList = [];

                foreach (object item in DriverManagerListView.SelectedItems)
                {
                    if (item is DriverModel driverItem)
                    {
                        selectedDriverList.Add(driverItem);
                    }
                }

                if (selectedDriverList.Count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectDriverEmpty));
                    return;
                }

                if (RuntimeHelper.IsElevated && (DriverResultKind is not DriverResultKind.Loading || DriverResultKind is not DriverResultKind.Operating))
                {
                    DriverResultKind oldDriverResultKind = DriverResultKind;
                    DriverResultKind = DriverResultKind.Operating;
                    List<(string, string, DriverOperationModel)> driverList = [];
                    foreach (DriverModel driverItem in selectedDriverList)
                    {
                        Guid driverOperationGuid = Guid.NewGuid();
                        DriverOperationModel driverOperation = new()
                        {
                            DriverInfName = driverItem.DriverInfName,
                            DriverOperation = ForceDeletingDriverString,
                            DriverOperationGuid = driverOperationGuid,
                            IsOperating = true
                        };
                        DriverOperationCollection.Add(driverOperation);
                        driverList.Add(ValueTuple.Create(driverItem.DriverOEMInfName, driverItem.DriverLocation, driverOperation));
                    }
                    (bool needReboot, int successfullyCount, int failedCount) = await DeleteDriverCollectionAsync(false, driverList.AsReadOnly());
                    if (needReboot)
                    {
                        ShowRebootNotification(DriverInstallKind.UnInstallDriver);
                    }
                    await UpdateDriverOperationResultAsync(DriverInstallKind.UnInstallDriver, false, true, successfullyCount, failedCount, oldDriverResultKind);
                }
            }
        }

        /// <summary>
        /// 选择旧驱动
        /// </summary>
        private void OnSelectOldDriverClicked(object sender, RoutedEventArgs args)
        {
            DriverManagerListView.SelectedItems.Clear();
            Dictionary<string, List<(int Index, DateTimeOffset DriverDate)>> driverInfGroupInfoList = [];
            for (int index = 0; index < DriverCollection.Count; index++)
            {
                DriverModel driverItem = DriverCollection[index];

                if (!driverInfGroupInfoList.TryGetValue(driverItem.DriverInfName, out List<(int Index, DateTimeOffset DriverDate)> driverList))
                {
                    driverList = [];
                    driverInfGroupInfoList.Add(driverItem.DriverInfName, driverList);
                }

                driverList.Add((index, driverItem.DriverDate));
            }

            List<int> selectedIndexList = [];
            foreach (List<(int Index, DateTimeOffset DriverDate)> driverList in driverInfGroupInfoList.Values)
            {
                if (driverList.Count <= 1)
                {
                    continue;
                }

                DateTimeOffset latestDate = driverList.Max(item => item.DriverDate);

                foreach ((int Index, DateTimeOffset DriverDate) in driverList)
                {
                    if (DriverDate < latestDate)
                    {
                        selectedIndexList.Add(Index);
                    }
                }
            }

            foreach (int index in selectedIndexList)
            {
                DriverManagerListView.SelectRange(new(index, 1));
            }
            SelectedItemsCount = selectedIndexList.Count;
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            IsIncrease = true;
            SelectedDriverSortRuleKind = DriverSortRuleKind.DeviceName;
        }

        /// <summary>
        /// 获取设备驱动信息
        /// </summary>
        private async Task GetDriverAsync(DriverSortRuleKind driverSortRuleKind, bool isIncrease)
        {
            if (DriverResultKind is not DriverResultKind.Loading)
            {
                DriverResultKind = DriverResultKind.Loading;
                DriverList.Clear();
                DriverCollection.Clear();
                SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
                ReadOnlyCollection<DriverModel> driverCollection = await GetDriverInformationCollectionAsync();

                if (driverCollection.Count is 0)
                {
                    DriverResultKind = DriverResultKind.Failed;
                    DriverFailedContent = DriverEmptyDescriptionString;
                }
                else
                {
                    DriverList.AddRange(driverCollection);
                    ReadOnlyCollection<DriverModel> sortedDriverCollection = await GetSortedDriversAsync(driverSortRuleKind, isIncrease, DriverList.AsReadOnly());
                    foreach (DriverModel sortedDriverItem in sortedDriverCollection)
                    {
                        DriverCollection.Add(sortedDriverItem);
                    }
                    DriverResultKind = DriverCollection.Count is 0 ? DriverResultKind.Failed : DriverResultKind.HasResult;
                    DriverFailedContent = DriverCollection.Count is 0 ? DriverEmptyDescriptionString : string.Empty;
                    SelectedItemsCount = DriverManagerListView.SelectedItems.Count;
                }
            }
        }

        /// <summary>
        /// 获取设备上所有的驱动信息
        /// </summary>
        private async Task<ReadOnlyCollection<DriverModel>> GetDriverInformationCollectionAsync()
        {
            return await Task.Run(() =>
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
                        ReadOnlyCollection<XElement> driverNodeList = new([.. xDocument.Descendants("Driver")]);

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
                    nint deviceInfoSet = SetupapiLibrary.SetupDiGetClassDevs(Guid.Empty, null, 0, DIGCF.DIGCF_ALLCLASSES);

                    if (deviceInfoSet is not 0)
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
                                systemDriverInformationList.Add(new()
                                {
                                    DeviceGuid = deviceGuid is null ? Guid.NewGuid() : Guid.TryParse(Convert.ToString(deviceGuid), out Guid guid) ? guid : Guid.NewGuid(),
                                    Description = Convert.ToString(deviceDesc),
                                    InfPath = Convert.ToString(driverInfPath),
                                    Date = (DateTimeOffset)driverDate,
                                    Version = new(Convert.ToString(driverVersion))
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
                            DriverDate = DateTimeOffset.TryParse(pnpDriverInformationItem.DriverDate, out DateTimeOffset driverDate) ? driverDate : DateTimeOffset.MinValue,
                            DriverVersion = new(pnpDriverInformationItem.DriverVersion),
                            DriverSize = string.IsNullOrEmpty(driverLocation) ? "0B" : VolumeSizeHelper.ConvertVolumeSizeToString(GetFolderSize(Path.GetDirectoryName(driverLocation))),
                            DriverLocation = driverLocation,
                            DriverType = pnpDriverInformationItem.ClassName,
                            SignatureName = string.IsNullOrEmpty(pnpDriverInformationItem.SignerName) ? NotAvailableString : pnpDriverInformationItem.SignerName,
                        };

                        Guid dismDriverPackageItemGuid = new(pnpDriverInformationItem.ClassGuid);

                        foreach (SystemDriverInformation systemDriverInformation in systemDriverInformationList)
                        {
                            if (Equals(dismDriverPackageItemGuid, systemDriverInformation.DeviceGuid) && string.Equals(driverItem.DriverOEMInfName, systemDriverInformation.InfPath, StringComparison.OrdinalIgnoreCase) && driverItem.DriverDate.Date.Equals(systemDriverInformation.Date.Date) && driverItem.DriverVersion.Equals(systemDriverInformation.Version))
                            {
                                driverItem.DeviceName = string.IsNullOrEmpty(systemDriverInformation.Description) ? NotAvailableString : systemDriverInformation.Description;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(driverItem.DeviceName))
                        {
                            driverItem.DeviceName = NotAvailableString;
                        }

                        driverList.Add(driverItem);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetDriverInformationCollectionAsync), 1, e);
                }

                driverList.Sort((item1, item2) => item1.DriverOEMInfName.CompareTo(item2.DriverOEMInfName));
                return driverList.AsReadOnly();
            });
        }

        /// <summary>
        /// 获取排序后的驱动
        /// </summary>
        private async Task<ReadOnlyCollection<DriverModel>> GetSortedDriversAsync(DriverSortRuleKind driverSortRuleKind, bool isIncrease, ReadOnlyCollection<DriverModel> driverCollection)
        {
            if (driverCollection is null || driverCollection.Count is 0)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                List<DriverModel> sortedDriverList = [];
                if (driverSortRuleKind is DriverSortRuleKind.DeviceName)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DeviceName)] : [.. driverCollection.OrderByDescending(item => item.DeviceName)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.InfName)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverInfName)] : [.. driverCollection.OrderByDescending(item => item.DriverInfName)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.OEMInfName)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverOEMInfName)] : [.. driverCollection.OrderByDescending(item => item.DriverOEMInfName)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.DeviceType)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverType)] : [.. driverCollection.OrderByDescending(item => item.DriverType)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.Manufacturer)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverManufacturer)] : [.. driverCollection.OrderByDescending(item => item.DriverManufacturer)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.Version)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverVersion)] : [.. driverCollection.OrderByDescending(item => item.DriverVersion)];
                }
                else if (driverSortRuleKind is DriverSortRuleKind.Date)
                {
                    sortedDriverList = isIncrease ? [.. driverCollection.OrderBy(item => item.DriverDate)] : [.. driverCollection.OrderByDescending(item => item.DriverDate)];
                }
                return sortedDriverList.AsReadOnly();
            });
        }

        /// <summary>
        /// 检索设备实例属性
        /// </summary>
        private object GetDevNodeProperty(string devPropKey, SP_DEVINFO_DATA deviceInfoData, nint deviceInfoSet)
        {
            if (string.IsNullOrEmpty(devPropKey))
            {
                return default;
            }

            object value = null;

            if (DevPropKeyDict.TryGetValue(devPropKey, out DEVPROPKEY devPropKeyItem))
            {
                SetupapiLibrary.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData, ref devPropKeyItem, out _, 0, 0, out int bufferSize, 0);
                nint propertyBufferPtr = Marshal.AllocHGlobal(bufferSize);

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

                                value = DateTimeOffset.FromUnixTimeSeconds(BitConverter.ToInt64(byteBuffer, 0));
                                break;
                            }
                        case DEVPROP_TYPE.DEVPROP_TYPE_FILETIME:
                            {
                                long fileTime = Marshal.ReadInt64(propertyBufferPtr);
                                value = DateTimeOffset.FromFileTime(fileTime);
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

                                key.fmtid = new(byteBuffer);
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
        private long GetFolderSize(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                return default;
            }

            long totalSize = 0;
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 1, e);
                        }
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 2, e);
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
                            LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 3, e);
                        }
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 4, e);
                }
                totalSize += folderAllSize;
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(GetFolderSize), 5, e);
            }
            return totalSize;
        }

        /// <summary>
        /// 获取驱动存放的实际路径
        /// </summary>
        private static string GetDriverStoreLocation(string oemInfName)
        {
            SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, 0, 0, null, 0, out int bufferSize);

            if (bufferSize > 0)
            {
                StringBuilder stringBuilder = new(bufferSize);
                return SetupapiLibrary.SetupGetInfDriverStoreLocation(oemInfName, 0, 0, stringBuilder, stringBuilder.Capacity, out _) ? Convert.ToString(stringBuilder) : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 添加并安装驱动
        /// </summary>
        private async Task<(bool, int, int)> AddDriverCollectionAsync(bool isAddandInstall, ReadOnlyCollection<(string, DriverOperationModel)> driverCollection)
        {
            if (driverCollection is null || driverCollection.Count is 0)
            {
                return default;
            }

            int successfullyCount = 0;
            int failedCount = 0;
            bool needReboot = false;
            foreach ((string fileName, DriverOperationModel driverOperationItem) in driverCollection)
            {
                (bool result, bool needRebootResult, Win32Exception win32Exception) = await AddDriverAsync(isAddandInstall, fileName);
                driverOperationItem.IsOperating = false;
                if (result)
                {
                    driverOperationItem.DriverOperation = isAddandInstall ? AddInstallingDriverSuccessfullyString : AddingDriverSuccessfullyString;
                    successfullyCount++;
                }
                else
                {
                    driverOperationItem.DriverOperation = string.Format(isAddandInstall ? AddingDriverFailedString : AddInstallingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                    failedCount++;
                }
                if (!needReboot && needRebootResult)
                {
                    needReboot = true;
                }
            }
            return ValueTuple.Create(needReboot, successfullyCount, failedCount);
        }

        /// <summary>
        /// 添加并安装驱动
        /// </summary>
        private async Task<(bool, bool, Win32Exception)> AddDriverAsync(bool isAddandInstall, string fileName)
        {
            return await Task.Run(() =>
            {
                bool result = false;
                bool needReboot = false;
                Win32Exception win32Exception = null;

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (isAddandInstall)
                    {
                        StringBuilder stringBuilder = new(260);
                        uint bufferSize = 0;
                        result = NewDevLibrary.DiInstallDriver(0, fileName, 0, out bool needRebootResult) && SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, 0);
                        if (!result)
                        {
                            win32Exception = new Win32Exception(Kernel32Library.GetLastError());
                        }
                        if (!needReboot && needRebootResult)
                        {
                            needReboot = true;
                        }
                    }
                    else
                    {
                        StringBuilder stringBuilder = new(260);
                        uint bufferSize = 0;
                        result = SetupapiLibrary.SetupCopyOEMInf(fileName, null, SPOST.SPOST_PATH, SP_COPY.SP_COPY_NONE, stringBuilder, (uint)stringBuilder.Capacity, ref bufferSize, 0);
                        if (!result)
                        {
                            win32Exception = new Win32Exception(Kernel32Library.GetLastError());
                        }
                    }
                }
                return ValueTuple.Create(result, needReboot, win32Exception);
            });
        }

        /// <summary>
        /// 删除驱动
        /// </summary>
        private async Task<(bool, int, int)> DeleteDriverCollectionAsync(bool isForce, ReadOnlyCollection<(string, string, DriverOperationModel)> driverCollection)
        {
            if (driverCollection is null || driverCollection.Count is 0)
            {
                return default;
            }

            int successfullyCount = 0;
            int failedCount = 0;
            bool needReboot = false;
            foreach ((string driverOEMInfName, string driverLocation, DriverOperationModel driverOperationItem) in driverCollection)
            {
                (bool result, bool needRebootResult, Win32Exception win32Exception) = await DeleteDriverAsync(isForce, driverOEMInfName, driverLocation);
                driverOperationItem.IsOperating = false;
                if (result)
                {
                    driverOperationItem.DriverOperation = isForce ? DeletingDriverSuccessfullyString : ForceDeletingDriverSuccessfullyString;
                    successfullyCount++;
                }
                else
                {
                    driverOperationItem.DriverOperation = string.Format(isForce ? DeletingDriverFailedString : ForceDeletingDriverFailedString, win32Exception.NativeErrorCode, win32Exception.HResult, win32Exception.Message);
                    failedCount++;
                }
                if (!needReboot && needRebootResult)
                {
                    needReboot = true;
                }
            }
            return ValueTuple.Create(needReboot, successfullyCount, failedCount);
        }

        /// <summary>
        /// 强制删除驱动
        /// </summary>
        private async Task<(bool, bool, Win32Exception)> DeleteDriverAsync(bool isForce, string driverOEMInfName, string driverLocation)
        {
            return await Task.Run(() =>
            {
                bool result = false;
                bool needReboot = false;
                Win32Exception win32Exception = null;

                if (isForce)
                {
                    if (!string.IsNullOrEmpty(driverOEMInfName) || !string.IsNullOrEmpty(driverLocation))
                    {
                        result = NewDevLibrary.DiUninstallDriver(0, driverLocation, DIURFLAG.NO_REMOVE_INF, out bool needRebootResult) && SetupapiLibrary.SetupUninstallOEMInf(driverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, 0);
                        if (!result)
                        {
                            win32Exception = new Win32Exception(Kernel32Library.GetLastError());
                        }
                        if (!needReboot && needRebootResult)
                        {
                            needReboot = true;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(driverOEMInfName))
                    {
                        result = SetupapiLibrary.SetupUninstallOEMInf(driverOEMInfName, SUOI_Flags.SUOI_FORCEDELETE, 0);
                        if (!result)
                        {
                            win32Exception = new Win32Exception(Kernel32Library.GetLastError());
                        }
                    }
                }
                return ValueTuple.Create(result, needReboot, win32Exception);
            });
        }

        /// <summary>
        /// 更新驱动操作结果
        /// </summary>
        private async Task UpdateDriverOperationResultAsync(DriverInstallKind driverInstallKind, bool isAddandInstall, bool isForceUninstall, int successfullyCount, int failedCount, DriverResultKind oldDriverResultKind)
        {
            OperationKind operationKind = OperationKind.NoOperation;
            if (driverInstallKind is DriverInstallKind.InstallDriver)
            {
                if (successfullyCount + failedCount is 0)
                {
                    operationKind = isAddandInstall ? OperationKind.AddInstallDriverFailed : OperationKind.AddDriverFailed;
                }
                else
                {
                    if (successfullyCount > 0 && failedCount > 0)
                    {
                        operationKind = isAddandInstall ? OperationKind.AddInstallDriverPartialSuccessfully : OperationKind.AddDriverPartialSuccessfully;
                    }
                    else if (successfullyCount > 0 && failedCount is 0)
                    {
                        operationKind = isAddandInstall ? OperationKind.AddInstallDriverAllSuccessfully : OperationKind.AddDriverAllSuccessfully;
                    }
                    else if (successfullyCount is 0 && failedCount > 0)
                    {
                        operationKind = isAddandInstall ? OperationKind.AddInstallDriverFailed : OperationKind.AddDriverFailed;
                    }
                }
            }
            else if (driverInstallKind is DriverInstallKind.UnInstallDriver)
            {
                if (successfullyCount + failedCount is 0)
                {
                    operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverFailed : OperationKind.DeleteDriverFailed;
                }
                else if (successfullyCount + failedCount is 1)
                {
                    if (successfullyCount is 1 && failedCount is 0)
                    {
                        operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverSuccessfully : OperationKind.DeleteDriverSuccessfully;
                    }
                    else if (successfullyCount is 0 && failedCount is 1)
                    {
                        operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverFailed : OperationKind.DeleteDriverFailed;
                    }
                }
                else
                {
                    if (successfullyCount > 0 && failedCount > 0)
                    {
                        operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverPartialSuccessfully : OperationKind.DeleteDriverPartialSuccessfully;
                    }
                    else if (successfullyCount > 0 && failedCount is 0)
                    {
                        operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverAllSuccessfully : OperationKind.DeleteDriverAllSuccessfully;
                    }
                    else if (successfullyCount is 0 && failedCount > 0)
                    {
                        operationKind = isForceUninstall ? OperationKind.ForceDeleteDriverFailed : OperationKind.DeleteDriverFailed;
                    }
                }
            }

            if (operationKind is not OperationKind.NoOperation)
            {
                synchronizationContext.Send(async (_) =>
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(operationKind));
                }, null);
            }

            if (successfullyCount > 0)
            {
                await GetDriverAsync(SelectedDriverSortRuleKind, IsIncrease);
            }
            else
            {
                DriverResultKind = oldDriverResultKind;
            }
        }

        /// <summary>
        /// 显示重新启动的消息通知
        /// </summary>
        private void ShowRebootNotification(DriverInstallKind driverInstallKind)
        {
            synchronizationContext.Send(async (_) =>
            {
                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(driverInstallKind));

                await Task.Run(() =>
                {
                    if (contentDialogResult is ContentDialogResult.Primary)
                    {
                        ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                    }
                });
            }, null);
        }

        /// <summary>
        /// 打开驱动存储的路径
        /// </summary>
        private void OpenDriverLocation(string driverLocation)
        {
            if (string.IsNullOrEmpty(driverLocation))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    Process.Start(File.Exists(driverLocation) ? Path.GetDirectoryName(driverLocation) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(OpenDriverLocation), 1, e);
                }
            });
        }

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private void RunAsAdministrator()
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
        /// 打开设备管理器
        /// </summary>
        private void OpenDeviceManagement()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("devmgmt.msc");
                }
                catch (Exception e)
                {
                    LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(DriverManagerPage), nameof(OpenDeviceManagement), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取加载驱动是否加载完成
        /// </summary>
        private Visibility GetDriverHasResultVisibility(DriverResultKind driverResultKind, bool isReverse)
        {
            return isReverse ? driverResultKind is DriverResultKind.HasResult || driverResultKind is DriverResultKind.Operating ? Visibility.Collapsed : Visibility.Visible : driverResultKind is DriverResultKind.HasResult || driverResultKind is DriverResultKind.Operating ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查搜索驱动结果状态
        /// </summary>
        private Visibility CheckDriverResultKindVisibility(DriverResultKind driverResultKind, DriverResultKind comparedDriverResultKind)
        {
            return Equals(driverResultKind, comparedDriverResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查驱动管理浮出面板状态
        /// </summary>
        private Visibility CheckDriverPaneKindVisibility(DriverPaneKind driverPaneKind, DriverPaneKind comparedDriverPaneKind)
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

        #endregion 第七部分：数据操作与业务逻辑
    }
}
