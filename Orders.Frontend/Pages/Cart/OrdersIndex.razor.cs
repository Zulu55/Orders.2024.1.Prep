using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class OrdersIndex
    {
        [Inject] private IRepository repository { get; set; } = null!;

        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        private int currentPage = 1;
        private int totalPages;

        public List<Order>? Orders { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string Page { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/orders?page={page}";
            var response = await repository.GetAsync<List<Order>>(url);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            Orders = response.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/orders/totalPages";
            var response = await repository.GetAsync<int>(url);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPages = response.Response;
        }
    }
}