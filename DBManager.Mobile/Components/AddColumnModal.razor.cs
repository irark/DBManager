using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using DBManager.Mobile.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DBManager.Mobile.Components
{
    public partial class AddColumnModal : ComponentBase
    {
        [CascadingParameter] public BlazoredModalInstance BlazoredModal { get; set; }
        [Parameter] public string TableName { get; set; }
        [Inject] public ColumnService ColumnService { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        private Column Column { get; set; } = new();
        private string _availableValues;

        private EditContext _editContext;
        private ValidationMessageStore _messageStore;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(Column);
            _messageStore = new ValidationMessageStore(_editContext);
            _editContext.OnValidationRequested += (s, e) =>
            {
                _messageStore.Clear();
                if (Column.Type == ColumnType.Enum && !Column.AvailableValues.Any() && _editContext.IsModified(_editContext.Field("AvailableValues")))
                {
                    _messageStore.Add(() => Column.AvailableValues, "Available values cannot be empty");
                }
            };
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
                Column.AvailableValues.Clear();
                Column.AvailableValues.AddRange(_availableValues?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>());
                var isValid = _editContext.Validate();
                if (!isValid)
                    return;
                var result = await ColumnService.CreateColumn(TableName, Column);
                if (result != null)
                {
                    ToastService.ShowSuccess("Column successfully created");
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
    }
}