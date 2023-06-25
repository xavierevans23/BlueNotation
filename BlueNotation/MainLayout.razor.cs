namespace BlueNotation;

public partial class MainLayout
{
    private bool _ready = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ConductorService.Start();
            _ready = true;
            await InvokeAsync(StateHasChanged);
        }
    }
}
