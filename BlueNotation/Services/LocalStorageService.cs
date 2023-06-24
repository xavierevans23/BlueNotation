using Microsoft.JSInterop;

namespace BlueNotation.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveData(string data)
    {
        await _jsRuntime.InvokeVoidAsync("saveLocalStorage", "data", data);
    }

    public async Task<string?> LoadData()
    {
        return await _jsRuntime.InvokeAsync<string?>("loadLocalStorage", "data");
    }

    public async Task SavePresets(string data)
    {
        await _jsRuntime.InvokeVoidAsync("saveLocalStorage", "presets", data);
    }

    public async Task<string?> LoadPresets()
    {
        return await _jsRuntime.InvokeAsync<string?>("loadLocalStorage", "presets");
    }
}
