using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AndroidRazorPayCashIn : MonoBehaviour
{
	public enum RazorPayCashInStatus
	{
		NONE = -1,
		PAYMENT_CANCELED = 0,
		TRANSACTION_SUCCESS = 1,
		NETWORK_ERROR = 2,
		INVALID_OPTIONS= 3,
		TLS_ERROR = 6
	}

	public class AndroidRazorPayCashInParams
	{
		public decimal mAmount;
		public string mEmailId;
		public string mMobileNumber;
		public string mOrderId;
		public string mCustomerId;
		public string mMerchantName;
		public string mCurrencyType;
		public string mImageUrl;


		public AndroidRazorPayCashInParams (decimal inAmount,string inEmailId, string inMobileNumber, string inOrderId,string inCustomerId, String inMerchantName, String inCurrencyType, String inImageUrl)
		{
			mAmount = inAmount;
			mEmailId = inEmailId;
			mMobileNumber = inMobileNumber;
			mOrderId = inOrderId;
			mCustomerId = inCustomerId;
			mMerchantName = inMerchantName;
			mCurrencyType = inCurrencyType;
			mImageUrl = inImageUrl;

		}

	}

	public class RazorPayTransactionResponse
	{
		public string PAYMENTID { get; set; }

		public string SIGNATURE { get; set; }

		public string ORDERID { get; set; }
	}

	public class AndroidRazorPayCashInResponse
	{
		public RazorPayCashInStatus status_code { get; set; }

		public RazorPayTransactionResponse response { get; set; }

		public string error { get; set; }
	}


	public delegate void delegateRazorPayCashInStatus (AndroidRazorPayCashInResponse cashInCallback);

	public static event delegateRazorPayCashInStatus RazorPayCashInTransactionCallback;

	private static bool mRazorPayCashInInitiated = false;
	private static bool mRazorPayCashInCompleted = false;


	public static void makeRazorPayInTransaction (AndroidRazorPayCashInParams inParams)
	{
		decimal amountToPay = inParams.mAmount;

		int amount = (int)amountToPay * 100;

		Debug.Log(amountToPay + "--amountToPay");

		mRazorPayCashInInitiated = false;

		mRazorPayCashInCompleted = false;

		AndroidJavaClass androidPaytmCashInClass;
		AndroidJavaObject androidPaytmCashInObject;
		androidPaytmCashInClass = new AndroidJavaClass ("com.example.payments_plugin.razor_pay.UnityRazorPayActivity");

		if (androidPaytmCashInClass != null)
		{
			androidPaytmCashInObject = androidPaytmCashInClass.CallStatic<AndroidJavaObject> ("getInstance");

			if (androidPaytmCashInObject != null)
			{
				mRazorPayCashInInitiated = true;

				androidPaytmCashInObject.Call ("StartRazorPayActivity",amount, inParams.mEmailId, inParams.mMobileNumber,inParams.mOrderId,inParams.mCustomerId, inParams.mMerchantName, inParams.mCurrencyType, inParams.mImageUrl);

			}
		}

	}



	public void OnRazorPayCallback (string inResponse)
	{
		Debug.Log("OnRazorPayCallback Response : " + inResponse);

		mRazorPayCashInCompleted = true;

		AndroidRazorPayCashInResponse responseDict = null;

		try
		{
            //responseDict = Newtonsoft.Json.JsonConvert.DeserializeObject<AndroidRazorPayCashIn.AndroidRazorPayCashInResponse>(inResponse); //search for Photon
        }
		catch
		{

		}

		if (RazorPayCashInTransactionCallback != null)
		{
			RazorPayCashInTransactionCallback (responseDict);
		}

		RazorPayCashInTransactionCallback = null;

		mRazorPayCashInInitiated = false;

		mRazorPayCashInCompleted = false;

	}

	void OnApplicationPause (bool pause)
	{
		if (pause)
		{

		}
		else
		{
			StartCoroutine (CheckForCallbackAfterTwoSeconds ());
		}
	}

	IEnumerator CheckForCallbackAfterTwoSeconds ()
	{
		yield return new WaitForSeconds (3f);

		if (mRazorPayCashInInitiated && !mRazorPayCashInCompleted)
		{
			if (RazorPayCashInTransactionCallback != null)
			{
				RazorPayCashInTransactionCallback (null);
			}
		}

		mRazorPayCashInInitiated = false;

		mRazorPayCashInCompleted = false;
	}

}
