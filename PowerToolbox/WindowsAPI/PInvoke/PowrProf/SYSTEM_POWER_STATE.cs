namespace PowerToolbox.WindowsAPI.PInvoke.PowrProf
{
    public enum SYSTEM_POWER_STATE
    {
        /// <summary>
        /// 未指定的系统电源状态。
        /// </summary>
        PowerSystemUnspecified = 0,

        /// <summary>
        /// 指定系统电源状态 S0。
        /// </summary>
        PowerSystemWorking = 1,

        /// <summary>
        /// 指定系统电源状态 S1。
        /// </summary>
        PowerSystemSleeping1 = 2,

        /// <summary>
        /// 指定系统电源状态 S2。
        /// </summary>
        PowerSystemSleeping2 = 3,

        /// <summary>
        /// 指定系统电源状态 S3。
        /// </summary>
        PowerSystemSleeping3 = 4,

        /// <summary>
        /// 指定系统电源状态 S4 (HIBERNATE) 。
        /// </summary>
        PowerSystemHibernate = 5,

        /// <summary>
        /// 指定系统电源状态 S5 (OFF) 。
        /// </summary>
        PowerSystemShutdown = 6,

        /// <summary>
        /// 指定最大枚举值。
        /// </summary>
        PowerSystemMaximum = 7
    }
}
