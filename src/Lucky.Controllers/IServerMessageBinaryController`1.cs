using Lucky.Core.MinerServer;

namespace Lucky.Controllers {
    // 为了解除对HttpResponseMessage所在的程序集的引用所以整了个类型参数T1
    public interface IServerMessageBinaryController<T1> {
        T1 ServerMessages(ServerMessagesRequest request);
    }
}
