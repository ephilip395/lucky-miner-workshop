using Lucky.Services.OSS;

namespace Lucky.Services {
    public class OSSService {
        public readonly AliyunOSSService AliyunOSSService = new AliyunOSSService();

        internal OSSService() { }
    }
}
