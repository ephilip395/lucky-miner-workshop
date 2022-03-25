using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyJsonRpc
{
    public class MiningParams
    {
        public string Endpoint { get; private set; }
        public string Wallet { get; private set; }
        public string Coin { get; private set; }
        public string Miner { get; private set; }

        public MiningParams(string endpoint, string wallet, string coin, string miner)
        {
            Endpoint = endpoint;
            Wallet = wallet;
            Coin = coin;
            Miner = miner;
        }

        public void Deconstruct(out string endpoint, out string wallet, out string coin, out string miner)
        {
            endpoint = Endpoint;
            wallet = Wallet;
            coin = Coin;
            miner = Miner;
        }
    }
}
