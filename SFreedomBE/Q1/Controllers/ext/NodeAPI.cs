
using SMPL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WavesCS;
using Ipfs;
using NodeAPI.Controllers.waves;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using Ipfs.Api;

namespace LkFinance.Controllers.ext
{
    public class NodeAPI
    {
        IpfsClient ipfs = new IpfsClient();
        SHA256Managed SHA256 = new SHA256Managed();
        wavesApi wavesApi = new wavesApi();
        protected SMPL.Helpers.IJson json = SMPL.Helpers.Creator.json;
        Node node = new Node("http://nodes.unblock.wavesnodes.com:6869");
        byte[] seed = Encoding.ASCII.GetBytes("digitaloctoberhackathon64");
        long fee = 10000000;
        string keyMain = "3NJLSu2of4jeDRnTAS68pXP41gHFka3epqP";
        PrivateKeyAccount privateKeyAccount = new PrivateKeyAccount(Encoding.ASCII.GetBytes("claw congress island ankle option pipe quick two smooth sick cliff powder tobacco plunge snack"), 'U', 0);
        public ResponseModel addPost(String msg)
        {

            var list = wavesApi.getTweetByAddressAsync(privateKeyAccount.Address);
            byte[] hash = null;
            if (list.Count > 0)
            {
                hash = list[list.Count - 1].hash;
                hash = SHA256.ComputeHash(hash, 0, hash.Length);

            }
            else
            {
                var adressByte = Encoding.UTF8.GetBytes(privateKeyAccount.Address);
                hash = SHA256.ComputeHash(adressByte, 0, adressByte.Length);
            }



                var hashModel = ipfs.FileSystem.AddTextAsync(msg).Result;
           


           
            Dictionary<string, object> listValue = new Dictionary<string, object>();
            MultiHash s = hashModel.Id.Hash;
            listValue.Add(BitConverter.ToString(hash).Replace("-", "").Replace("-", ""), Encoding.UTF8.GetBytes(s.ToBase58()));
            node.PutData(privateKeyAccount, listValue, 1000000);
            return new ResponseModel() { };
        }
        public ResponseModel getMyTweet()
        {
            return new ResponseModel()
            {
                data = wavesApi.getTweetByAddressAsync(privateKeyAccount.Address).OrderByDescending(x=>x.id).ToList()
            };
        }
        public ResponseModel getTweet(string address)
        {
            return new ResponseModel()
            {
                data = wavesApi.getTweetByAddressAsync(address).OrderByDescending(x => x.id).ToList()
            };
        }

    }
}