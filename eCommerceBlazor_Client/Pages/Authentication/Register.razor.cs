using Microsoft.AspNetCore.Components;
using eCommerceBlazor_Models;
using eCommerceBlazor_Client.Service.IService;

namespace eCommerceBlazor_Client.Pages.Authentication
{
    public partial class Register
    {
        private SignUpRequestDTO SignUpRequest = new();
        public bool IsProcessing { get; set; } = false;
        public bool ShowRegistrationErrors { get; set; } 
        public IEnumerable<string> Errors { get; set; }

        [Inject]
        public IAuthenticationService _authService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private async Task RegisterUser()
        {
            ShowRegistrationErrors = false;
            IsProcessing = true;
            var result = await _authService.RegisterUser(SignUpRequest);
            if (result.IsRegistrationSuccessful)
            {
                //Registration is successful, redirect to login
                _navigationManager.NavigateTo("/login");
            }
            else
            {
                //Registration is not successful, show errors
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
            IsProcessing = false;
        }
    }
}
