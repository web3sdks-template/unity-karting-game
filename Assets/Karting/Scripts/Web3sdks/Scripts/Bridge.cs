
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Web3sdks
{
    public class Bridge
    {
        [System.Serializable]
        private struct Result<T>
        {
            public T result;
        }

        [System.Serializable]
        private struct RequestMessageBody
        {
            public RequestMessageBody(string[] arguments)
            {
                this.arguments = arguments;
            }
            public string[] arguments;
        }

        private static Dictionary<string, TaskCompletionSource<string>> taskMap = new Dictionary<string, TaskCompletionSource<string>>();

        [AOT.MonoPInvokeCallback(typeof(Action<string, string, string>))]
        private static void jsCallback(string taskId, string result, string error)
        {
            if (taskMap.ContainsKey(taskId))
            {
                if (error != null)
                {
                    taskMap[taskId].TrySetException(new Exception(error));
                }
                else
                {
                    taskMap[taskId].TrySetResult(result);
                }
                taskMap.Remove(taskId);
            }
        }

        public static void Initialize(string chainOrRPC, Web3sdksSDK.Options options)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Initializing the web3sdks SDK is not supported in the editor. Please build and run the app instead.");
                return;
            }
            Web3sdksInitialize(chainOrRPC, Utils.ToJson(options));
        }

        public static async Task<string> Connect(WalletConnection walletConnection)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Connecting wallets is not supported in the editor. Please build and run the app instead.");
                return Utils.AddressZero;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            Web3sdksConnect(taskId, walletConnection.provider.ToString(), walletConnection.chainId, jsCallback);
            string result = await task.Task;
            return result;
        }

        public static async Task Disconnect()
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Disconnecting wallets is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            Web3sdksDisconnect(taskId, jsCallback);
            await task.Task;
        }

        public static async Task SwitchNetwork(int chainId)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Switching networks is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            Web3sdksSwitchNetwork(taskId, chainId, jsCallback);
            await task.Task;
        }

        public static async Task<T> InvokeRoute<T>(string route, string[] body)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Interacting with the web3sdks SDK is not supported in the editor. Please build and run the app instead.");
                return default(T);
            }
            var msg = Utils.ToJson(new RequestMessageBody(body));
            string taskId = Guid.NewGuid().ToString();
            var task = new TaskCompletionSource<string>();
            taskMap[taskId] = task;
            Web3sdksInvoke(taskId, route, msg, jsCallback);
            string result = await task.Task;
            // Debug.Log($"InvokeRoute Result: {result}");
            return JsonConvert.DeserializeObject<Result<T>>(result).result;
        }

        public static async Task FundWallet(FundWalletOptions payload)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Interacting with the web3sdks SDK is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var msg = Utils.ToJson(payload);
            string taskId = Guid.NewGuid().ToString();
            var task = new TaskCompletionSource<string>();
            taskMap[taskId] = task;
            Web3sdksFundWallet(taskId, msg, jsCallback);
            await task.Task;
        }

        [DllImport("__Internal")]
        private static extern string Web3sdksInvoke(string taskId, string route, string payload, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string Web3sdksInitialize(string chainOrRPC, string options);
        [DllImport("__Internal")]
        private static extern string Web3sdksConnect(string taskId, string wallet, int chainId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string Web3sdksDisconnect(string taskId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string Web3sdksSwitchNetwork(string taskId, int chainId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string Web3sdksFundWallet(string taskId, string payload, Action<string, string, string> cb);
    }
}
