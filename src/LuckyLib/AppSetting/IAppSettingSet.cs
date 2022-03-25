using System.Collections.Generic;

namespace Lucky.AppSetting {
    public interface IAppSettingSet {
        bool TryGetAppSetting(string key, out IAppSetting appSetting);
        IEnumerable<IAppSetting> AsEnumerable();
    }
}
