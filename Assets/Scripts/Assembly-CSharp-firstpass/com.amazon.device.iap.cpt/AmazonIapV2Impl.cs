using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using com.amazon.device.iap.cpt.json;
using UnityEngine;

namespace com.amazon.device.iap.cpt
{
	public abstract class AmazonIapV2Impl : MonoBehaviour, IAmazonIapV2
	{
		private abstract class AmazonIapV2Base : AmazonIapV2Impl
		{
			private static readonly object startLock = new object();

			private static volatile bool startCalled = false;

			public AmazonIapV2Base()
			{
				logger = new AmazonLogger(GetType().Name);
			}

			protected void Start()
			{
				if (startCalled)
				{
					return;
				}
				lock (startLock)
				{
					if (!startCalled)
					{
						Init();
						RegisterCallback();
						RegisterEventListener();
						RegisterCrossPlatformTool();
						startCalled = true;
					}
				}
			}

			protected abstract void Init();

			protected abstract void RegisterCallback();

			protected abstract void RegisterEventListener();

			protected abstract void RegisterCrossPlatformTool();

			public override void UnityFireEvent(string jsonMessage)
			{
				FireEvent(jsonMessage);
			}

			public override RequestOutput GetUserData()
			{
				Start();
				return RequestOutput.CreateFromJson(GetUserDataJson("{}"));
			}

			private string GetUserDataJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeGetUserDataJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetUserDataJson(string jsonMessage);

			public override RequestOutput Purchase(SkuInput skuInput)
			{
				Start();
				return RequestOutput.CreateFromJson(PurchaseJson(skuInput.ToJson()));
			}

			private string PurchaseJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativePurchaseJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativePurchaseJson(string jsonMessage);

			public override RequestOutput GetProductData(SkusInput skusInput)
			{
				Start();
				return RequestOutput.CreateFromJson(GetProductDataJson(skusInput.ToJson()));
			}

			private string GetProductDataJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeGetProductDataJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetProductDataJson(string jsonMessage);

			public override RequestOutput GetPurchaseUpdates(ResetInput resetInput)
			{
				Start();
				return RequestOutput.CreateFromJson(GetPurchaseUpdatesJson(resetInput.ToJson()));
			}

			private string GetPurchaseUpdatesJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeGetPurchaseUpdatesJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetPurchaseUpdatesJson(string jsonMessage);

			public override void NotifyFulfillment(NotifyFulfillmentInput notifyFulfillmentInput)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(NotifyFulfillmentJson(notifyFulfillmentInput.ToJson())) as Dictionary<string, object>);
			}

			private string NotifyFulfillmentJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeNotifyFulfillmentJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeNotifyFulfillmentJson(string jsonMessage);

			public override void AddGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate)
			{
				Start();
				string key = "getUserDataResponse";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new GetUserDataResponseDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new GetUserDataResponseDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate)
			{
				Start();
				string key = "getUserDataResponse";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (GetUserDataResponseDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddPurchaseResponseListener(PurchaseResponseDelegate responseDelegate)
			{
				Start();
				string key = "purchaseResponse";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new PurchaseResponseDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new PurchaseResponseDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemovePurchaseResponseListener(PurchaseResponseDelegate responseDelegate)
			{
				Start();
				string key = "purchaseResponse";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (PurchaseResponseDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate)
			{
				Start();
				string key = "getProductDataResponse";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new GetProductDataResponseDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new GetProductDataResponseDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate)
			{
				Start();
				string key = "getProductDataResponse";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (GetProductDataResponseDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate)
			{
				Start();
				string key = "getPurchaseUpdatesResponse";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new GetPurchaseUpdatesResponseDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new GetPurchaseUpdatesResponseDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate)
			{
				Start();
				string key = "getPurchaseUpdatesResponse";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (GetPurchaseUpdatesResponseDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}
		}

		private class AmazonIapV2Default : AmazonIapV2Base
		{
			protected override void Init()
			{
			}

			protected override void RegisterCallback()
			{
			}

			protected override void RegisterEventListener()
			{
			}

			protected override void RegisterCrossPlatformTool()
			{
			}

			protected override string NativeGetUserDataJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativePurchaseJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeGetProductDataJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeGetPurchaseUpdatesJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeNotifyFulfillmentJson(string jsonMessage)
			{
				return "{}";
			}
		}

		private abstract class AmazonIapV2DelegatesBase : AmazonIapV2Base
		{
			private const string CrossPlatformTool = "XAMARIN";

			protected CallbackDelegate callbackDelegate;

			protected CallbackDelegate eventDelegate;

			protected override void Init()
			{
				NativeInit();
			}

			protected override void RegisterCallback()
			{
				callbackDelegate = callback;
				NativeRegisterCallback(callbackDelegate);
			}

			protected override void RegisterEventListener()
			{
				eventDelegate = FireEvent;
				NativeRegisterEventListener(eventDelegate);
			}

			protected override void RegisterCrossPlatformTool()
			{
				NativeRegisterCrossPlatformTool("XAMARIN");
			}

			public override void UnityFireEvent(string jsonMessage)
			{
				throw new NotSupportedException("UnityFireEvent is not supported");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCallback(CallbackDelegate callback);

			protected abstract void NativeRegisterEventListener(CallbackDelegate callback);

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);
		}

		private class Builder
		{
			internal static readonly IAmazonIapV2 instance;

			static Builder()
			{
				instance = AmazonIapV2UnityAndroid.Instance;
			}
		}

		private class AmazonIapV2UnityAndroid : AmazonIapV2UnityBase
		{
			public new static AmazonIapV2UnityAndroid Instance
			{
				get
				{
					return AmazonIapV2UnityBase.getInstance<AmazonIapV2UnityAndroid>();
				}
			}

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeRegisterCallbackGameObject(string name);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeInit();

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetUserDataJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativePurchaseJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetProductDataJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetPurchaseUpdatesJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeNotifyFulfillmentJson(string jsonMessage);

			protected override void NativeInit()
			{
				nativeInit();
			}

			protected override void RegisterCallback()
			{
				nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void RegisterEventListener()
			{
				nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void NativeRegisterCrossPlatformTool(string crossPlatformTool)
			{
			}

			protected override string NativeGetUserDataJson(string jsonMessage)
			{
				return nativeGetUserDataJson(jsonMessage);
			}

			protected override string NativePurchaseJson(string jsonMessage)
			{
				return nativePurchaseJson(jsonMessage);
			}

			protected override string NativeGetProductDataJson(string jsonMessage)
			{
				return nativeGetProductDataJson(jsonMessage);
			}

			protected override string NativeGetPurchaseUpdatesJson(string jsonMessage)
			{
				return nativeGetPurchaseUpdatesJson(jsonMessage);
			}

			protected override string NativeNotifyFulfillmentJson(string jsonMessage)
			{
				return nativeNotifyFulfillmentJson(jsonMessage);
			}
		}

		private abstract class AmazonIapV2UnityBase : AmazonIapV2Base
		{
			private const string CrossPlatformTool = "UNITY";

			private static AmazonIapV2UnityBase instance;

			private static Type instanceType;

			private static volatile bool quit;

			private static object initLock;

			static AmazonIapV2UnityBase()
			{
				quit = false;
				initLock = new object();
			}

			public static T getInstance<T>() where T : AmazonIapV2UnityBase
			{
				if (quit)
				{
					return (T)null;
				}
				if (instance != null)
				{
					return (T)instance;
				}
				lock (initLock)
				{
					Type typeFromHandle = typeof(T);
					assertTrue(instance == null || (instance != null && instanceType == typeFromHandle), "Only 1 instance of 1 subtype of AmazonIapV2UnityBase can exist.");
					if (instance == null)
					{
						instanceType = typeFromHandle;
						GameObject gameObject = new GameObject();
						instance = gameObject.AddComponent<T>();
						gameObject.name = typeFromHandle.ToString() + "_Singleton";
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
					return (T)instance;
				}
			}

			public void OnDestroy()
			{
				quit = true;
			}

			private static void assertTrue(bool statement, string errorMessage)
			{
				if (!statement)
				{
					throw new AmazonException("FATAL: An internal error occurred", new InvalidOperationException(errorMessage));
				}
			}

			protected override void Init()
			{
				NativeInit();
			}

			protected override void RegisterCrossPlatformTool()
			{
				NativeRegisterCrossPlatformTool("UNITY");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);
		}

		protected delegate void CallbackDelegate(string jsonMessage);

		private static AmazonLogger logger;

		private static readonly Dictionary<string, IDelegator> callbackDictionary = new Dictionary<string, IDelegator>();

		private static readonly object callbackLock = new object();

		private static readonly Dictionary<string, List<IDelegator>> eventListeners = new Dictionary<string, List<IDelegator>>();

		private static readonly object eventLock = new object();

		public static IAmazonIapV2 Instance
		{
			get
			{
				return Builder.instance;
			}
		}

		private AmazonIapV2Impl()
		{
		}

		public static void callback(string jsonMessage)
		{
			Dictionary<string, object> dictionary = null;
			try
			{
				logger.Debug("Executing callback");
				dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string callerId = dictionary["callerId"] as string;
				Dictionary<string, object> response = dictionary["response"] as Dictionary<string, object>;
				callbackCaller(response, callerId);
			}
			catch (KeyNotFoundException inner)
			{
				logger.Debug("callerId not found in callback");
				throw new AmazonException("Internal Error: Unknown callback id", inner);
			}
			catch (AmazonException ex)
			{
				logger.Debug("Async call threw exception: " + ex.ToString());
			}
		}

		private static void callbackCaller(Dictionary<string, object> response, string callerId)
		{
			IDelegator delegator = null;
			try
			{
				Jsonable.CheckForErrors(response);
				lock (callbackLock)
				{
					delegator = callbackDictionary[callerId];
					callbackDictionary.Remove(callerId);
					delegator.ExecuteSuccess(response);
				}
			}
			catch (AmazonException e)
			{
				lock (callbackLock)
				{
					if (delegator == null)
					{
						delegator = callbackDictionary[callerId];
					}
					callbackDictionary.Remove(callerId);
					delegator.ExecuteError(e);
				}
			}
		}

		public static void FireEvent(string jsonMessage)
		{
			try
			{
				logger.Debug("eventReceived");
				Dictionary<string, object> dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string key = dictionary["eventId"] as string;
				Dictionary<string, object> dictionary2 = null;
				if (dictionary.ContainsKey("response"))
				{
					dictionary2 = dictionary["response"] as Dictionary<string, object>;
					Jsonable.CheckForErrors(dictionary2);
				}
				lock (eventLock)
				{
					foreach (IDelegator item in eventListeners[key])
					{
						if (dictionary2 != null)
						{
							item.ExecuteSuccess(dictionary2);
						}
						else
						{
							item.ExecuteSuccess();
						}
					}
				}
			}
			catch (AmazonException ex)
			{
				logger.Debug("Event call threw exception: " + ex.ToString());
			}
		}

		public abstract RequestOutput GetUserData();

		public abstract RequestOutput Purchase(SkuInput skuInput);

		public abstract RequestOutput GetProductData(SkusInput skusInput);

		public abstract RequestOutput GetPurchaseUpdates(ResetInput resetInput);

		public abstract void NotifyFulfillment(NotifyFulfillmentInput notifyFulfillmentInput);

		public abstract void UnityFireEvent(string jsonMessage);

		public abstract void AddGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		public abstract void RemoveGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		public abstract void AddPurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		public abstract void RemovePurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		public abstract void AddGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		public abstract void RemoveGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		public abstract void AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);

		public abstract void RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);
	}
}
