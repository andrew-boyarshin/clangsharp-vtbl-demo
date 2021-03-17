using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TerraFX.Interop;
using static TerraFX.Interop.Windows;

unsafe
{
    static void ThrowIfFailed(string methodName, HRESULT hr)
    {
        if (hr < 0)
            Marshal.ThrowExceptionForHR(hr);
    }

    static int SetOffsetX(IDCompositionVisual* visual, float offsetX)
    {
        // TerraFX has lpVtbl[3] instead.
        // http://web.archive.org/web/20141206094130/http://support.microsoft.com/kb/131104
        // llvm-project/clang/lib/AST/VTableBuilder.cpp: GroupNewVirtualOverloads
        return ((delegate* unmanaged<IDCompositionVisual*, float, int>) visual->lpVtbl[4])(visual, offsetX);
    }

    using ComPtr<ID3D11Device> device11 = default;
    using ComPtr<IDXGIDevice> deviceDxgi = default;
    using ComPtr<IDCompositionDevice> deviceComposition = default;
    using ComPtr<IDCompositionVisual> visual = default;
    D3D_FEATURE_LEVEL featureLevel;
    var iid = IID_IDCompositionDevice;

    ThrowIfFailed(
        nameof(D3D11CreateDevice),
        D3D11CreateDevice(
            null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, IntPtr.Zero,
            (uint) D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT,
            null, 0, D3D11_SDK_VERSION, device11.GetAddressOf(), &featureLevel, null
        )
    );

    Debug.Assert(device11.Get() != null);

    ThrowIfFailed(
        nameof(IDXGIDevice),
        device11.As(&deviceDxgi)
    );

    Debug.Assert(deviceDxgi.Get() != null);

    ThrowIfFailed(
        nameof(DCompositionCreateDevice),
        DCompositionCreateDevice(deviceDxgi, &iid, (void**) deviceComposition.GetAddressOf())
    );

    Debug.Assert(deviceComposition.Get() != null);

    ThrowIfFailed(
        nameof(IDCompositionDevice.CreateVisual),
        deviceComposition.Get()->CreateVisual(visual.GetAddressOf())
    );

    Debug.Assert(visual.Get() != null);

    SetOffsetX(visual.Get(), 15.5f);

    visual.Get()->SetOffsetX(10.15f);
}