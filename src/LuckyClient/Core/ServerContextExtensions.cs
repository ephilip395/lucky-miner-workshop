using Lucky.Repositories;
using System;

namespace Lucky.Core {
    public static class ServerContextExtensions {
        #region CreateRepository
        /// <summary>
        /// 创建组合仓储，组合仓储由ServerDb和ProfileDb层序组成。
        /// 如果是开发者则访问ServerDb且只访问GlobalDb，否则将ServerDb和ProfileDb并起来访问且不能修改删除GlobalDb。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<T> CreateCompositeRepository<T>(this IServerContext _) where T : class, ILevelEntity<Guid> {
            return new HierarchicalRepository<T>(CreateServerRepository<T>(_), CreateLocalRepository<T>(_));
        }

        public static IRepository<T> CreateLocalRepository<T>(this IServerContext _) where T : class, IDbEntity<Guid> {
            if (!LuckyContext.IsJsonLocal) {
                return new LiteDbReadWriteRepository<T>(HomePath.LocalDbFileFullName);
            }
            else {
                return new JsonReadOnlyRepository<T>(LuckyContext.LocalJsonDb);
            }
        }

        public static IRepository<T> CreateServerRepository<T>(this IServerContext _) where T : class, IDbEntity<Guid> {
            if (!LuckyContext.IsJsonServer) {
                return new LiteDbReadWriteRepository<T>(HomePath.ServerDbFileFullName);
            }
            else {
                return new JsonReadOnlyRepository<T>(LuckyContext.ServerJsonDb);
            }
        }
        #endregion
    }
}
