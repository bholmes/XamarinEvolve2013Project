using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace XamarinEvolveIOS
{
	public class NameValueCell : CustomUITableViewCellSubView
	{	
		public UILabel NameLabel {get;private set;}
		public UILabel ValueLabel {get;private set;}
		public UITextField ValueTextField {get;private set;}
		Func<string> _valueGet;
		Action<string> _valueSet;
		
		public NameValueCell (string name, Func<string> valueGet, Action<string> valueSet)
		{
			this.BackgroundColor = UIColor.Clear;
			this.Frame = new RectangleF (0, 0, 100, 100);
			
			int labelXPos = 10;
			int label2XPos = 75;
			NameLabel = new UILabel (new RectangleF (
				labelXPos, 11,  label2XPos-(labelXPos+5), 21));
			NameLabel.AutoresizingMask = UIViewAutoresizing.None;
			NameLabel.BackgroundColor = UIColor.Clear;
			NameLabel.Font = UIFont.SystemFontOfSize (14);
			NameLabel.AdjustsFontSizeToFitWidth = true;
			NameLabel.TextAlignment = UITextAlignment.Right;
			NameLabel.Text = name;
			NameLabel.TextColor = UIColor.Blue;
			this.Add (NameLabel);
			
			ValueLabel = new UILabel (new RectangleF (
				label2XPos, 11,  this.Frame.Width-(label2XPos+10), 21));
			ValueLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ValueLabel.BackgroundColor = UIColor.Clear;
			ValueLabel.Font = UIFont.BoldSystemFontOfSize (17);
			ValueLabel.AdjustsFontSizeToFitWidth = true;
			ValueLabel.Text = valueGet.Invoke ();
			this.Add (ValueLabel);
			
			ValueTextField = new UITextField (new RectangleF (
				label2XPos, 11,  this.Frame.Width-(label2XPos+10), 21));
			
			ValueTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ValueTextField.BackgroundColor = UIColor.Clear;
			ValueTextField.Font = UIFont.BoldSystemFontOfSize (17);
			ValueTextField.AdjustsFontSizeToFitWidth = true;
			ValueTextField.Text = valueGet.Invoke ();
			ValueTextField.ReturnKeyType = UIReturnKeyType.Done;
			ValueTextField.ClearButtonMode = UITextFieldViewMode.Always;
			ValueTextField.ShouldReturn = delegate {
				ValueTextField.ResignFirstResponder ();
				return true;
			};

			ValueTextField.EditingDidEnd += delegate {
				_valueSet.Invoke (ValueTextField.Text);
			}; 

			this.Add (ValueTextField);
			
			_valueGet = valueGet;
			_valueSet = valueSet;
		}
		
		protected override void EditStyleChanged (bool editing, bool animated)
		{
			base.EditStyleChanged (editing, animated);
			
			if (editing)
			{
				ValueTextField.Hidden = false;
				ValueLabel.Hidden = true;
			}
			else
			{
				_valueSet.Invoke (ValueTextField.Text);
				ValueLabel.Text = _valueGet.Invoke ();
				ValueTextField.ResignFirstResponder ();
				ValueLabel.Hidden = false;
				ValueTextField.Hidden = true;
			}
		}
	}
}

