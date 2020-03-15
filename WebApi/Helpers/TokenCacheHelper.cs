using System.IO;
using Microsoft.Identity.Client;

namespace YukiDrive.Helpers
{
    /// <summary>
    /// 缓存 Token
    /// </summary>
    static class TokenCacheHelper
        {
            public static void EnableSerialization(ITokenCache tokenCache)
            {
                tokenCache.SetBeforeAccess(BeforeAccessNotification);
                tokenCache.SetAfterAccess(AfterAccessNotification);
            }

            /// <summary>
            /// Path to the token cache
            /// </summary>
            public static readonly string CacheFilePath = Path.Combine(Directory.GetCurrentDirectory(),"TokenCache.bin");

            private static readonly object FileLock = new object();


            private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
            {
                lock (FileLock)
                {
                    args.TokenCache.DeserializeMsalV3(System.IO.File.Exists(CacheFilePath)
                            ? System.IO.File.ReadAllBytes(CacheFilePath)
                            : null);
                }
            }

            private static void AfterAccessNotification(TokenCacheNotificationArgs args)
            {
                // if the access operation resulted in a cache update
                if (args.HasStateChanged)
                {
                    lock (FileLock)
                    {
                        // reflect changesgs in the persistent store
                        System.IO.File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
                    }
                }
            }
        }

}