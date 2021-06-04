using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Payments : MonoBehaviour
{
   

	public void OnClickRazorPay(){

		string customerId ="";

		string number = "9999999999";

		string razorPayMerchantName ="Company online pvt ltd";

		string currency = "INR";

		string imageUrl = "";

		int amt = 1000;
		decimal amount = amt;  

		string order_id = "";

		string email_id = null;

		AndroidRazorPayCashIn.AndroidRazorPayCashInParams requestParam = new AndroidRazorPayCashIn.AndroidRazorPayCashInParams (amount,email_id,number,order_id,customerId,razorPayMerchantName,currency, imageUrl);

		AndroidRazorPayCashIn.makeRazorPayInTransaction(requestParam);
	}

}
