using System;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using DBManager.Mobile.Components;
using DBManager.Mobile.Services;
using Microsoft.AspNetCore.Components;

namespace DBManager.Mobile.Pages
{
    public partial class DatabaseDetails : ComponentBase
    {
        [Parameter] public string DatabasePath { get; set; }
        [Inject] private IModalService ModalService { get; set; }
        [Inject] public DatabaseService DatabaseService { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        [Inject] public TableService TableService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        public Database Database { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await ReloadDatabase();
        }

        private async Task ReloadDatabase()
        {
            try
            {
                Database = await DatabaseService.OpenDatabase(DatabasePath) ?? new Database();
            }
            catch (Exception ex)
            {
                NavigationManager.NavigateTo("/");
                ToastService.ShowError(string.IsNullOrEmpty(ex.Message)
                    ? "Something went wrong"
                    : ex.Message);
            }
        }

        private async Task Difference()
        {
            var parameters = new ModalParameters();
            parameters.Add("Tables", Database.Tables.ToList());
            var modal = ModalService.Show<TablesDifferenceModal>("Tables difference", parameters);
            await modal.Result;
        }

        private async Task Create()
        {
            var modal = ModalService.Show<CreateTableModal>("Create table");
            var modalResult = await modal.Result;
            if (modalResult.Cancelled || !(bool) modalResult.Data)
                return;
            await ReloadDatabase();
        }

        private async Task Delete(string name)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Message", $"Are you sure you want to delete table {name}?");
                var modal = ModalService.Show<ConfirmationModal>("Confirmation", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || !(bool) modalResult.Data)
                    return;
                var res = await TableService.DeleteTable(name);
                if (res)
                {
                    ToastService.ShowSuccess("Table successfully deleted");
                    await ReloadDatabase();
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

        private async Task EditName()
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Name", Database.Name);
                var modal = ModalService.Show<EditNameModal>("Edit name", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || string.IsNullOrEmpty(modalResult.Data.ToString()))
                    return;
                var res = await DatabaseService.RenameDatabase(modalResult.Data.ToString());
                if (res)
                {
                    ToastService.ShowSuccess("Database successfully renamed");
                    Database.Name = modalResult.Data.ToString();
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

        private void Open(string tableName)
        {
            NavigationManager.NavigateTo($"tableDetails/{tableName}");
        }
    }
}