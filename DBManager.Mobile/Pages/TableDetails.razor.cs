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
    public partial class TableDetails : ComponentBase
    {
        [Parameter] public string TableName { get; set; }
        [Inject] private IModalService ModalService { get; set; }
        [Inject] public IToastService ToastService { get; set; }
        [Inject] public TableService TableService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public RowService RowService { get; set; }
        [Inject] public ColumnService ColumnService { get; set; }
        public Table Table { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await ReloadTable();
        }

        private async Task ReloadTable()
        {
            try
            {
                Table = await TableService.GetTableByName(TableName) ?? new Table();
            }
            catch (Exception ex)
            {
                NavigationManager.NavigateTo("/");
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
                parameters.Add("Name", Table.Name);
                var modal = ModalService.Show<EditNameModal>("Edit name", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || string.IsNullOrEmpty(modalResult.Data.ToString()))
                    return;
                var res = await TableService.RenameTable(TableName, modalResult.Data.ToString());
                if (res)
                {
                    ToastService.ShowSuccess("Table successfully renamed");
                    Table.Name = modalResult.Data.ToString();
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

        private async Task EditColumnName(Column column)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Name", column.Name);
                var modal = ModalService.Show<EditNameModal>("Edit name", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || string.IsNullOrEmpty(modalResult.Data.ToString()))
                    return;
                var res = await ColumnService.RenameColumn(TableName, column.Name, modalResult.Data.ToString());
                if (res)
                {
                    ToastService.ShowSuccess("Column successfully renamed");
                    column.Name = modalResult.Data.ToString();
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

        private async Task CellValueChanged(int columnIndex, int rowIndex, ChangeEventArgs args)
        {
            try
            {
                if (args.Value == null)
                    return;
                var res = await RowService.EditCell(TableName, columnIndex, rowIndex, args.Value.ToString());
                if (res)
                {
                    ToastService.ShowSuccess("Cell successfully edited");
                    Table.Rows[rowIndex].Values[columnIndex] = args.Value.ToString();
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
        private async Task CreateColumn()
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("TableName", Table.Name);
                var modal = ModalService.Show<AddColumnModal>("Add column", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || !(bool) modalResult.Data)
                    return;
                await ReloadTable();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(string.IsNullOrEmpty(ex.Message)
                    ? "Something went wrong"
                    : ex.Message);
            }
        }
        private async Task CreateRow()
        {
            try
            {
                var res = await RowService.CreateRow(TableName);
                if (res != null)
                {
                    ToastService.ShowSuccess("Row successfully added");
                    await ReloadTable();
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
        
        private async Task DeleteRow(int rowIndex)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Message", $"Are you sure you want to delete row?");
                var modal = ModalService.Show<ConfirmationModal>("Confirmation", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || !(bool) modalResult.Data)
                    return;
                var res = await RowService.DeleteRow(TableName, rowIndex);
                if (res)
                {
                    ToastService.ShowSuccess("Row successfully deleted");
                    await ReloadTable();
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
        
        private async Task DeleteColumn(int columnIndex)
        {
            try
            {
                var parameters = new ModalParameters();
                parameters.Add("Message", $"Are you sure you want to delete column?");
                var modal = ModalService.Show<ConfirmationModal>("Confirmation", parameters);
                var modalResult = await modal.Result;
                if (modalResult.Cancelled || !(bool) modalResult.Data)
                    return;
                var res = await ColumnService.DeleteColumn(TableName, columnIndex);
                if (res)
                {
                    ToastService.ShowSuccess("Column successfully deleted");
                    await ReloadTable();
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