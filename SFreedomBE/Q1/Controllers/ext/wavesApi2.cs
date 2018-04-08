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

namespace HakWave.Controllers
{
    public class wavesApiNew
    {
        /*1) получить профиль по алиасу
2) получить твиты по алиасу
3) получить лайки по твиту L
4) получить подписчиков по алиаусу R
5) получить подписки по алиасу*/
        SHA256Managed SHA256 = new SHA256Managed();
        protected SMPL.Helpers.IJson json = SMPL.Helpers.Creator.json;
        Node node = new Node("http://nodes.unblock.wavesnodes.com:6869");
        string nodeString = "http://nodes.unblock.wavesnodes.com:6869";
        byte[] seed = Encoding.ASCII.GetBytes("digitaloctoberhackathon64");
        long fee = 10000000;
        string keyMain = "3NJLSu2of4jeDRnTAS68pXP41gHFka3epqP";
        PrivateKeyAccount privateKeyAccount = new PrivateKeyAccount(Encoding.ASCII.GetBytes("digitaloctoberhackathon64"), 'U', 0);
        public string setData(string key, string data)
        {



            return node.PutData(privateKeyAccount, new Dictionary<string, object>() { { key, Encoding.ASCII.GetBytes(data) } }, fee);

        }

        /// <summary>
        /// Получение лайков по алиасу
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ResponseModel getLikeReadersWhomReadByAlias(string alias, int type)
        {
            string address = getAddressAcountByAlias(alias);
            List<commonModel> listjson = new List<commonModel>();
            using (var webClient = new WebClient())
            {
                string bred = webClient.DownloadString($"{nodeString}/transactions/address/{address}/limit/50").Replace("[ [", "[").Replace("] ]", "]");
                listjson = json.deserializeModel<List<commonModel>>(bred);
            }
            //словарь для хранения хэша пкоммента,кол-во лайков
            Dictionary<byte[], int> dictLike = new Dictionary<byte[], int>();
            //кол-во подписчиков
            int countReaders = 0;
            //лист на кого подписан
            List<byte[]> listWhomReader = new List<byte[]>();

            foreach (commonModel model in listjson.Where(x => x.type == type))
            {
                //TODO проверку на лайкающего 
                //l-hash-bool
                string[] arrayAttachment = model.attachment.Split('-');
                byte[] hashLikeFromModel = Encoding.ASCII.GetBytes(arrayAttachment[1]);
                //лайки 
                if (arrayAttachment[0] == "L")
                {
                    if (dictLike.ContainsKey(hashLikeFromModel))
                    {
                        //лайки либо плюсуем либо отнимаем
                        dictLike[hashLikeFromModel] = bool.Parse(arrayAttachment[2]) ? +1 : -1;
                    }
                    else dictLike.Add(hashLikeFromModel, bool.Parse(arrayAttachment[2]) ? 1 : 0);
                }

                else if (arrayAttachment[0] == "R")
                {
                    //подписчики
                    if (model.recipient == address)
                    {
                        countReaders = (bool.Parse(arrayAttachment[1])) ? +1 : -1;
                    }
                    //подписки
                    else if (model.sender == address)
                    {
                        byte[] recepientByte = Encoding.ASCII.GetBytes(model.recipient);
                        if (!listWhomReader.Contains(recepientByte))
                        {
                            if (bool.Parse(arrayAttachment[1]))
                            {
                                listWhomReader.Add(recepientByte);
                            }
                        }
                        else
                            if (bool.Parse(arrayAttachment[1]))
                        {
                            listWhomReader.Remove(recepientByte);
                        }
                    }
                }
            }
            countReadersLikesFromAccount countReadersLikes = new countReadersLikesFromAccount()
            {
                countReaders = countReaders,
                dictLike = dictLike,
                listWhomReader = listWhomReader
            };
            return new ResponseModel() { data = countReadersLikes };
        }

        /// <summary>
        /// возврат твитов по адресу
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ResponseModel getTweetByAddress(string address)
        {
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
                    if (listjson.FirstOrDefault(x => Encoding.UTF8.GetBytes(x.key) == hashedAddress) != null)
                    {
                        hashedAddress = SHA256.ComputeHash(hashedAddress, 0, adressByte.Length);
                        t_hash.Add(hashedAddress, listjson.FirstOrDefault(x => Encoding.UTF8.GetBytes(x.key) == hashedAddress).value); ishashValue = true; break;
                    }
                    else break;
                }
            }
            return new ResponseModel() { data = t_hash };
        }

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
                    if (listjson.FirstOrDefault(x => Encoding.UTF8.GetBytes(x.key) == hashedAddress) != null)
                    {
                        hashedAddress = SHA256.ComputeHash(hashedAddress, 0, adressByte.Length);
                        t_hash.Add(hashedAddress, listjson.FirstOrDefault(x => Encoding.UTF8.GetBytes(x.key) == hashedAddress).value); ishashValue = true; break;
                    }
                    else break;
                }
            }
            return new ResponseModel() { data = t_hash };
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
            public string attachment { get; set; }
            public class Datum
            {
                public string key { get; set; }
                public string type { get; set; }
                public string value { get; set; }
            }

        }

        /// <summary>
        /// Модель для возврата иныы по кол-ву лайков, подписчиков и подписок
        /// </summary>
        public class countReadersLikesFromAccount
        {
            public List<byte[]> listWhomReader { get; set; }
            public int countReaders { get; set; }
            public Dictionary<byte[], int> dictLike { get; set; }
        }
    }
}
