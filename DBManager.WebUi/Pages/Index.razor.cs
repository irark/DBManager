using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using DBManager.WebUi.Components;
using DBManager.WebUi.Models;
using DBManager.WebUi.Services.HttpServices;
using Microsoft.AspNetCore.Components;

namespace DBManager.WebUi.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] private IModalService ModalService { get; set; }
        [Inject] public DatabaseService DatabaseService { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private List<DatabaseViewModel> AllDatabases { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await ReloadDatabases();
        }

        private async Task ReloadDatabases()
        {
            try
            {
                AllDatabases = await DatabaseService.GetAllDatabases() ?? new List<DatabaseViewModel>();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(string.IsNullOrEmpty(ex.Message)
                    ? "Something went wrong"
                    : ex.Message);
            }
        }

        private void OpenDatabase(string path)
        {
            NavigationManager.NavigateTo($"database/{Uri.EscapeUriString(path)}");
        }

        private async Task Delete(DatabaseViewModel database)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Message", $"Are you sure you want to delete database {database.Name}?");
                var modal = ModalService.Show<ConfirmationModal>("Confirmation", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || !(bool) modalResult.Data)
                    return;
                var res = await DatabaseService.DeleteDataBaseByPath(Uri.EscapeUriString(database.Path));
                if (res)
                {
                    ToastService.ShowSuccess("Database successfully deleted");
                    await ReloadDatabases();
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


        private async Task UploadDatabase()
        {
            var modal = ModalService.Show<OpenDatabaseModal>("Upload database");
            var modalResult = await modal.Result;
            if (modalResult.Cancelled || string.IsNullOrEmpty(modalResult.Data.ToString()))
                return;
            OpenDatabase(modalResult.Data.ToString());
        }

        private async Task Create()
        {
            var modal = ModalService.Show<CreateDatabaseModal>("Create database");
            var modalResult = await modal.Result;
            if (modalResult.Cancelled || !(bool) modalResult.Data)
                return;
            await ReloadDatabases();
        }
    }
}