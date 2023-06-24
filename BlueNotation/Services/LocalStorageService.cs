using Microsoft.JSInterop;

namespace BlueNotation.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task Save(string data)
    {
        await _jsRuntime.InvokeVoidAsync("saveLocalStorage", "data", data);
    }

    public async Task<string> Load()
    {
        return await _jsRuntime.InvokeAsync<string>("loadLocalStorage", "data");
    }
}
