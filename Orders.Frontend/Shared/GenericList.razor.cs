using Microsoft.AspNetCore.Components;

namespace Orders.Frontend.Shared
{
    public partial class GenericList<Titem>
    {
        [EditorRequired]
        [Parameter]
        public List<Titem> MyList { get; set; } = null!;
    }
}