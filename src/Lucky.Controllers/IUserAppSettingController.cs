using Lucky.User;

namespace Lucky.Controllers {
    public interface IUserAppSettingController {
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetAppSetting(DataRequest<UserAppSettingData> request);
    }
}
