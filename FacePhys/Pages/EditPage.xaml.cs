
using System.ComponentModel;

namespace FacePhys.Pages
{
	public partial class EditPage : ContentPage
	{
		public EditPage()
		{
			InitializeComponent();
			BindingContext = App.UserViewModel;
		}
	}
}