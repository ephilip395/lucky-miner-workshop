using System;

namespace Lucky.Core.MinerServer
{
    [DataSchemaId("39BDF505-64AF-4F1B-B6F5-549A378EBE8D")]
    public class LuckyWalletData : ILuckyWallet, IDbEntity<Guid>
    {
        public LuckyWalletData() { }

        public Guid GetId()
        {
            return this.Id;
        }

        public Guid Id { get; set; }
        public string CoinCode { get; set; }
        public string Wallet { get; set; }
    }
}
