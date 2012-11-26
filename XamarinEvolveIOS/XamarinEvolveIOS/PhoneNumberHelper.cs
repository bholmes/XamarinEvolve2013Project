using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace XamarinEvolveIOS
{
	public class PhoneNumberHelper
	{
		static public bool ShoudChange (UITextField textField, NSRange range, string toString)
		{			int length = getLength (textField.Text);
			
			if(length == 10)
			{
				if(range.Length == 0)
					return false;
			}
			
			if(length == 3)
			{
				string num = formatNumber(textField.Text);
				textField.Text = string.Format ("({0}) ", num);
				if(range.Length > 0)
					textField.Text = string.Format ("{0}", num.Substring(0, 3));
			}
			else if(length == 6)
			{
				string num = formatNumber(textField.Text);
				
				textField.Text = string.Format ("({0}) {1}-",num.Substring (0, 3) ,num.Substring (3));
				if(range.Length > 0)
					textField.Text = string.Format ("({0}) {1}",num.Substring (0, 3) ,num.Substring (3));
			}

			return true;
		}
		
		static string formatNumber (string mobileNumber)
		{
			
			mobileNumber = mobileNumber.Replace ("(" , "");
			mobileNumber = mobileNumber.Replace (")" , "");
			mobileNumber = mobileNumber.Replace (" " , "");
			mobileNumber = mobileNumber.Replace ("-" , "");
			mobileNumber = mobileNumber.Replace ("+" , "");
			
			int length = mobileNumber.Length;
			
			if(length > 10)
			{
				mobileNumber = mobileNumber.Substring (length-10);
			}
			
			return mobileNumber;
		}
		
		static int getLength (string mobileNumber)
		{
			mobileNumber = mobileNumber.Replace ("(" , "");
			mobileNumber = mobileNumber.Replace (")" , "");
			mobileNumber = mobileNumber.Replace (" " , "");
			mobileNumber = mobileNumber.Replace ("-" , "");
			mobileNumber = mobileNumber.Replace ("+" , "");
			
			return mobileNumber.Length;
		}
	}
}

