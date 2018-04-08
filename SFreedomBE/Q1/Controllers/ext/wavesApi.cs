using SMPL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WavesCS;
using System.Security.Cryptography;
using System.IO;
using System.Numerics;
using System.Web.Script.Serialization;
using Ipfs.Api;

namespace NodeAPI.Controllers.waves
{
    public class wavesApi
    {
        /*1) получить профиль по алиасу
2) получить твиты по алиасу

3) получить лайки по твиту

4) получить подписчиков по алиаусу
5) получить подписки по алиасу*/
        IpfsClient ipfs = new IpfsClient();
        SHA256Managed SHA256 = new SHA256Managed();
        protected SMPL.Helpers.IJson json = SMPL.Helpers.Creator.json;
        Node node = new Node("http://nodes.unblock.wavesnodes.com:6869");
        string nodeString = "http://nodes.unblock.wavesnodes.com:6869";
        byte[] seed = Encoding.ASCII.GetBytes("digitaloctoberhackathon64");
        long fee = 10000000;
        string keyMain = "T";
        PrivateKeyAccount privateKeyAccount = new PrivateKeyAccount(Encoding.ASCII.GetBytes("digitaloctoberhackathon64"), 'U', 0);
        public string setData(string key, string data)
        {



            return node.PutData(privateKeyAccount, new Dictionary<string, object>() { { key, Encoding.ASCII.GetBytes(data) } }, fee);

        }

        public string getDate()
        {
            PrivateKeyAccount privateKeyAccount = new PrivateKeyAccount(seed, 'U', 0);

            return node.Alias(privateKeyAccount, "fierymaria", 'U', fee);

        }

        public ResponseModel getlistjson(string address)
        {
            List<jsonModel> listjson = new List<jsonModel>();
            using (var webClient = new WebClient())
            {

                listjson = json.deserializeModel<List<jsonModel>>(webClient.DownloadString($"{nodeString}/addresses/data/{address}"));
            }

            var userModel = new UserModel();
            userModel.avatar = listjson.Where(x => x.key == "avatar").FirstOrDefault().value;
            userModel.address = listjson.Where(x => x.key == "name").FirstOrDefault().value;
            userModel.address = listjson.Where(x => x.key == "lastName").FirstOrDefault().value;
            List<string> t_hash = new List<string>();

            bool ishashValue = false;
            while (ishashValue)
            {
                var adressByte = Encoding.ASCII.GetBytes(address);
                //получаем хэш адреса первый
                var hashedAddress = SHA256.ComputeHash(adressByte, 0, adressByte.Length);

                foreach (jsonModel model in listjson)
                {
                    if (model.key == BitConverter.ToString(hashedAddress).Replace("-", "")) { t_hash.Add(model.value); ishashValue = true; break; }
                }
            }
            return new ResponseModel() { data = userModel };
        }


        //public ResponseModel getLikeByAlias(string alias)
        //{

        //}

        /// <summary>
        /// возврат твитов по алиасу
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ResponseModel getTweetByAlias(string alias)
        {
            string address = getAddressAcountByAlias(alias);
            List<jsonModel> listjson = new List<jsonModel>();
            using (var webClient = new WebClient())
            {

                listjson = json.deserializeModel<List<jsonModel>>(webClient.DownloadString($"{nodeString}/addresses/data/{address}"));
            }

            Dictionary<byte[], string> t_hash = new Dictionary<byte[], string>();

            bool ishashValue = false;
            while (ishashValue)
            {
                var adressByte = Encoding.ASCII.GetBytes(address);
                //получаем хэш адреса первый
                var hashedAddress = SHA256.ComputeHash(adressByte, 0, adressByte.Length);

                while (true)
                {
                    if (listjson.FirstOrDefault(x => Encoding.ASCII.GetBytes(x.key) == hashedAddress) != null)
                    {
                        hashedAddress = SHA256.ComputeHash(hashedAddress, 0, adressByte.Length);
                        t_hash.Add(hashedAddress, listjson.FirstOrDefault(x => Encoding.ASCII.GetBytes(x.key) == hashedAddress).value); ishashValue = true; break;
                    }
                    else break;
                }
            }
            return new ResponseModel() { data = t_hash };
        }

        public class TweetModel
        {
            public int id { get; set; }
            public byte[] hash { get; set; }
            public string value { get; set; }
        }
        /// <summary>
        /// возврат твитов по адресу
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public List<TweetModel> getTweetByAddressAsync(string address)
        {
            List<jsonModel> listjson = new List<jsonModel>();
            using (var webClient = new WebClient())
            {

                listjson = json.deserializeModel<List<jsonModel>>(webClient.DownloadString($"{nodeString}/addresses/data/{address}"));
            }

            List<TweetModel> t_hash = new List<TweetModel>();

            bool ishashValue = true;
            int i = 0;
            var adressByte = Encoding.UTF8.GetBytes(address);
            //получаем хэш адреса первый
            var hashedAddress = SHA256.ComputeHash(adressByte, 0, adressByte.Length);
            while (true)
            {

                if (listjson.FirstOrDefault(x => x.key == BitConverter.ToString(hashedAddress).Replace("-", "")) != null)
                {
                   
                    string value = listjson.FirstOrDefault(x => x.key == BitConverter.ToString(hashedAddress).Replace("-", "")).value;
                    try
                    {
                        string has123h = Encoding.Unicode.GetString(Base58.Decode(value));
                        string text = ipfs.FileSystem.ReadAllTextAsync(convertXXXL(BitConverter.ToString(Base58.Decode(value)))).Result;
                        t_hash.Add(new TweetModel()
                        {
                            id = i,
                            hash = hashedAddress,
                            value = text
                        });
                        hashedAddress = SHA256.ComputeHash(hashedAddress, 0, hashedAddress.Length);
                        ishashValue = true;
                    }
                    catch(Exception ex)
                    {
                        hashedAddress = SHA256.ComputeHash(hashedAddress, 0, hashedAddress.Length);

                    }

                }
                else break;
                i++;

            }
            return t_hash;
        }


        public string convertXXXL(string tetx)
        {
            string xxxl = "";
            string[] mass = tetx.Split('-');
            for(int i = 0; i < mass.Length; i++)
            {
                xxxl += Convert.ToChar(Convert.ToInt32(int.Parse(mass[i], System.Globalization.NumberStyles.HexNumber)));
            }
            return xxxl;
        }
        /// <summary>
        /// возврат инфы по всему аккаунту по адресу
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public ResponseModel getAccountByAddress(string address)
        {
            List<jsonModel> listjson = new List<jsonModel>();
            using (var webClient = new WebClient())
            {
                listjson = json.deserializeModel<List<jsonModel>>(webClient.DownloadString($"http://nodes.unblock.wavesnodes.com:6869/addresses/data/{address}"));
            }

            var userModel = new UserModel();
            userModel.avatar = listjson.Where(x => x.key == "avatar").FirstOrDefault()?.value ?? null;
            userModel.name = listjson.Where(x => x.key == "name").FirstOrDefault()?.value ?? null;
            userModel.lastName = listjson.Where(x => x.key == "lastName").FirstOrDefault()?.value ?? null;
            return new ResponseModel() { data = userModel };
        }

        /// <summary>
        /// возврат адреса по псевдониму
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ResponseModel getAccountByAlias(string alias)
        {
            addressModel addressJson = new addressModel();
            using (var webClient = new WebClient())
            {
                addressJson = json.deserializeModel<addressModel>(webClient.DownloadString($"http://nodes.unblock.wavesnodes.com:6869/alias/by-alias/{alias}"));
            }
            //возвращаем профиль аккаунта
            return new ResponseModel() { data = getAccountByAddress(addressJson.address) };
        }

        /// <summary>
        /// возврат адреса по псевдониму
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string getAddressAcountByAlias(string alias)
        {
            addressModel addressJson = new addressModel();
            using (var webClient = new WebClient())
            {
                addressJson = json.deserializeModel<addressModel>(webClient.DownloadString($"http://nodes.unblock.wavesnodes.com:6869/alias/by-alias/{alias}"));
            }
            //возвращаем профиль аккаунта
            return addressJson.address;
        }

        /// <summary>
        /// статус ноды
        /// </summary>
        /// <returns></returns>
        public ResponseModel getStatusNode()
        {
            string nodeStatus;
            using (var webClient = new WebClient())
            {
                nodeStatus = webClient.DownloadString($"{nodeString}/node/status");
            }
            return new ResponseModel() { data = nodeStatus };
        }


        /// <summary>
        /// Модель для аккаунта
        /// </summary>
        public class UserModel
        {
            public string address { get; set; }
            public string avatar { get; set; }
            public string name { get; set; }
            public string lastName { get; set; }
            //TODO точно стринга? не кол-во не список?
            public string readingTweet { get; set; }
            public List<string> t_hash { get; set; }
        }
        /// <summary>
        /// модель для данных в аккаунте
        /// </summary>
        public class jsonModel
        {
            public string key { get; set; }
            public string type { get; set; }
            public string value { get; set; }
        }

        /// <summary>
        /// для адреса
        /// </summary>
        public class addressModel
        {
            public string address { get; set; }
        }

        /// <summary>
        /// общая модель по транзакциям
        /// </summary>
        public class commonModel
        {
            public int type { get; set; }
            public string id { get; set; }
            public string sender { get; set; }
            public string senderPublicKey { get; set; }
            public int fee { get; set; }
            public object timestamp { get; set; }
            public List<string> proofs { get; set; }
            public int version { get; set; }
            public List<Datum> data { get; set; }
            public int height { get; set; }
            public string signature { get; set; }
            public string alias { get; set; }
            public string recipient { get; set; }
            public long? amount { get; set; }
            public class Datum
            {
                public string key { get; set; }
                public string type { get; set; }
                public string value { get; set; }
            }

        }
    }
}
