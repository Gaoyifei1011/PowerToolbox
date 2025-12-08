using System;
using System.Device.Location;
using System.Diagnostics;
using ThemeSwitch.Services.Root;

namespace ThemeSwitch.Services.Position
{
    /// <summary>
    /// 设备位置服务
    /// </summary>
    public static class DevicePositionService
    {
        private static GeoCoordinateWatcher geoCoordinateWatcher = null;

        public static GeoPositionStatus GeoPositionStatus { get; private set; } = GeoPositionStatus.NoData;

        public static GeoPositionPermission Permission { get; private set; } = GeoPositionPermission.Unknown;

        public static double Longitude { get; private set; }

        public static double Latitude { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static bool IsLoaded { get; private set; }

        public static event Action StatusOrPositionChanged;

        /// <summary>
        /// 位置服务初始化
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (!IsInitialized && geoCoordinateWatcher is null)
                {
                    geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
                    {
                        MovementThreshold = 20
                    };
                    geoCoordinateWatcher.PositionChanged += OnPositionChanged;
                    geoCoordinateWatcher.StatusChanged += OnStatusChanged;
                    geoCoordinateWatcher.Start();
                    IsInitialized = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(DevicePositionService), nameof(Initialize), 1, e);
                IsInitialized = false;
            }
        }

        /// <summary>
        /// 位置服务卸载
        /// </summary>
        public static void UnInitialize()
        {
            try
            {
                if (IsInitialized && geoCoordinateWatcher is not null)
                {
                    geoCoordinateWatcher.PositionChanged -= OnPositionChanged;
                    geoCoordinateWatcher.StatusChanged -= OnStatusChanged;
                    geoCoordinateWatcher.Stop();
                    GeoPositionStatus = GeoPositionStatus.NoData;
                    Permission = GeoPositionPermission.Unknown;
                    Longitude = 0;
                    Latitude = 0;
                    geoCoordinateWatcher = null;
                    IsInitialized = false;
                    IsLoaded = false;
                }
            }
            catch (Exception e)
            {
                IsInitialized = false;
                IsLoaded = false;
                LogService.WriteLog(TraceEventType.Error, nameof(ThemeSwitch), nameof(DevicePositionService), nameof(UnInitialize), 1, e);
            }
        }

        /// <summary>
        /// GeoCoordinateWatcher 类状态发生变化后触发的事件
        /// </summary>
        private static void OnStatusChanged(object sender, GeoPositionStatusChangedEventArgs args)
        {
            switch (args.Status)
            {
                case GeoPositionStatus.Ready:
                    {
                        IsLoaded = true;
                        GeoPositionStatus = args.Status;
                        Permission = (sender as GeoCoordinateWatcher).Permission;
                        if (sender is GeoCoordinateWatcher geoCoordinateWatcher)
                        {
                            Latitude = geoCoordinateWatcher.Position.Location.Latitude;
                            Longitude = geoCoordinateWatcher.Position.Location.Longitude;
                        }

                        StatusOrPositionChanged?.Invoke();
                        break;
                    }

                case GeoPositionStatus.Initializing:
                    {
                        GeoPositionStatus = args.Status;
                        Permission = (sender as GeoCoordinateWatcher).Permission;
                        Latitude = 0;
                        Longitude = 0;
                        StatusOrPositionChanged?.Invoke();
                        break;
                    }
                case GeoPositionStatus.NoData:
                    {
                        GeoPositionStatus = args.Status;
                        Permission = (sender as GeoCoordinateWatcher).Permission;
                        Latitude = 0;
                        Longitude = 0;
                        StatusOrPositionChanged?.Invoke();
                        break;
                    }
                case GeoPositionStatus.Disabled:
                    {
                        GeoPositionStatus = args.Status;
                        Permission = (sender as GeoCoordinateWatcher).Permission;
                        Latitude = 0;
                        Longitude = 0;
                        StatusOrPositionChanged?.Invoke();
                        break;
                    }
                default:
                    {
                        GeoPositionStatus = GeoPositionStatus.NoData;
                        Permission = (sender as GeoCoordinateWatcher).Permission;
                        Latitude = 0;
                        Longitude = 0;
                        StatusOrPositionChanged?.Invoke();
                        break;
                    }
            }

            // 如果定位服务被禁用，直接卸载当前服务
            if (Permission is GeoPositionPermission.Denied || GeoPositionStatus is GeoPositionStatus.Disabled)
            {
                UnInitialize();
            }
        }

        /// <summary>
        /// 设备位置变化后触发的事件
        /// </summary>
        private static void OnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> args)
        {
            if (!args.Position.Location.IsUnknown)
            {
                Latitude = args.Position.Location.Latitude;
                Longitude = args.Position.Location.Longitude;
                StatusOrPositionChanged?.Invoke();
            }
        }
    }
}
