using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using DBManager.Mobile.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DBManager.Mobile.Components
{
    public partial class CreateDatabaseModal : ComponentBase
    {
        [CascadingParameter] public BlazoredModalInstance BlazoredModal { get; set; }
        [Inject] public DatabaseService DatabaseService { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        private Database Database { get; set; } = new();

        private EditContext _editContext;
        private ValidationMessageStore _messageStore;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(Database);
            _messageStore = new ValidationMessageStore(_editContext);
            _editContext.OnValidationRequested += (s, e) => _messageStore.Clear();
            _editContext.OnFieldChanged += (s, e) =>
            {
                _messageStore.Clear(e.FieldIdentifier);
                _editContext.GetValidationMessages(e.FieldIdentifier);
            };
        }

        private void Cancel()
        {
            BlazoredModal.Close(ModalResult.Cancel());
        }

        private async Task Create()
        {
            try
            {
                var isValid = _editContext.Validate();
                if (!isValid)
                    return;
                var result = await DatabaseService.CreateDatabase(Database);
                if (result != null)
                {
                    ToastService.ShowSuccess("Database successfully created");
                    BlazoredModal.Close(ModalResult.Ok(true));
                }
                else
                    ToastService.ShowError("Something went wrong");
            }
            catch (Exception ex)
            {
                ToastService.ShowError(string.IsNullOrEmpty(ex.Message)
                    ? "Something went wrong"
                    : ex.Message);
            }
        }

        private void HandlePath(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (!Regex.Match(file.ContentType, @$"^text/").Success)
                {
                    ToastService.ShowError("Incorrect format");
                    return;
                }

                Database.Path = Path.GetFullPath(file.Name);
            }
            catch (Exception ex)
            {
                
                ToastService.ShowError(string.IsNullOrEmpty(ex.Message)
                    ? "Something went wrong"
                    : ex.Message);
            }
        }
    }
}