using IOITWebApp.Models.EF;
using Firebase.Database;
using log4net;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Data
{
    public class Firebase
    {
        private CNTTVNWebContext db = new CNTTVNWebContext();
        private static readonly ILog log = LogMaster.GetLogger("firebase", "firebase");
        private static FirebaseClient client;

        //string domainFile = ConfigurationManager.AppSettings["domainFile"].ToString();
        private static readonly string firebase_secret = "pcXk8hVMPEiInvbzo4A3v6QbRESqXMRZ7KkNwVW0";
        private static readonly string firebase_url = "https://autionkoi.firebaseio.com";
        //private static readonly string firebase_secret = ConfigurationManager.AppSettings["firebase_secret"].ToString();
        //private static readonly string firebase_url = ConfigurationManager.AppSettings["firebase_url"].ToString();

        public static async void sendAction(EF.Action action)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("Action/" + action.UserId + "/" + action.ActionId);

            try
            {
                await root.PutAsync(JsonConvert.SerializeObject(new EF.Action()
                {
                    ActionId = (int)action.ActionId,
                    ActionName = action.ActionName,
                    ActionType = action.ActionType,
                    TargetId = action.TargetId,
                    TargetType = action.TargetType,
                    CreatedAt = action.CreatedAt,
                }));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void pushAction(EF.Action action)
        {
            if (action.ActionId > 0)
            {
                var tasks = new[]
                {
                    Task.Run(() => sendAction(action))
                };
            }
        }

        public static async void sendAution(EF.Action action)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("Aution/" + action.TargetId + "/" + action.UserPushId + "/" + action.ActionId);

            try
            {
                await root.PutAsync(JsonConvert.SerializeObject(new EF.Action()
                {
                    ActionId = (int)action.ActionId,
                    ActionName = action.ActionName,
                    ActionType = action.ActionType,
                    TargetId = action.TargetId,
                    TargetType = action.TargetType,
                    CreatedAt = action.CreatedAt,
                }));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void pushAution(EF.Action action)
        {
            if (action.ActionId > 0)
            {
                var tasks = new[]
                {
                    Task.Run(() => sendAution(action))
                };
            }
        }

        public static async void deleteAction(int idUser)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("Action/" + idUser);

            try
            {
                await root.DeleteAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static async void deleteWarning(int idUser)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("Warning/" + idUser);

            try
            {
                await root.DeleteAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static async void sendUser(User user)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("User/" + user.UserId);

            try
            {
                await root.PutAsync(JsonConvert.SerializeObject(new User()
                {
                    UserId = user.UserId,
                    Password = Utils.GetMD5Hash(user.Password),
                    Status = user.Status,
                    RoleLevel = user.RoleLevel
                }));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static async void updateUser(User user)
        {
            if (client == null)
            {
                client = new FirebaseClient(
                                    firebase_url,
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(firebase_secret)
                                    });
            }
            var root = client.Child("User/" + user.UserId);
            try
            {
                await root.PatchAsync(JsonConvert.SerializeObject(new User()
                {
                    UserId = user.UserId,
                    Password = Utils.GetMD5Hash(user.Password),
                    Status = user.Status,
                    RoleMax = user.RoleMax,
                    RoleLevel=user.RoleLevel,
                }));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

    }
}