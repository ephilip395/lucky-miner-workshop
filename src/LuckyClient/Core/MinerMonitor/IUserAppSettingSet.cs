using Lucky.User;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor {
    public interface IUserAppSettingSet {
        void Init(List<UserAppSettingData> userAppSettings);
        bool TryGetAppSetting(string key, out IUserAppSetting appSetting);
        IEnumerable<IUserAppSetting> AsEnumerable();
    }
}
