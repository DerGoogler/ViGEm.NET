using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.DualSense;
using Nefarius.ViGEm.Client.Utilities;

namespace Nefarius.ViGEm.Client.Targets;

/// <inheritdoc cref="ViGEmTarget" />
/// <summary>
///     Represents an emulated wired Sony DualSense Controller.
/// </summary>
internal partial class DualSenseController : ViGEmTarget, IDualSenseController
{
    private static readonly List<DualSenseButton> ButtonMap = new()
    {
        DualSenseButton.ThumbRight,
        DualSenseButton.ThumbLeft,
        DualSenseButton.Options,
        DualSenseButton.Create,
        DualSenseButton.TriggerRight,
        DualSenseButton.TriggerLeft,
        DualSenseButton.ShoulderRight,
        DualSenseButton.ShoulderLeft,
        DualSenseButton.Triangle,
        DualSenseButton.Circle,
        DualSenseButton.Cross,
        DualSenseButton.Square,
        DualSenseSpecialButton.Ps,
        DualSenseSpecialButton.Touchpad,
        DualSenseSpecialButton.Mute
    };

    private static readonly List<DualSenseAxis> AxisMap = new()
    {
        DualSenseAxis.LeftThumbX, DualSenseAxis.LeftThumbY, DualSenseAxis.RightThumbX, DualSenseAxis.RightThumbY
    };

    private static readonly List<DualSenseSlider> SliderMap = new()
    {
        DualSenseSlider.LeftTrigger, DualSenseSlider.RightTrigger
    };

    private ViGEmClient.DS_REPORT _nativeReport;

    private ViGEmClient.PVIGEM_DS_NOTIFICATION _notificationCallback;

    private ViGEmClient.DS_OUTPUT_BUFFER _outputBuffer;

    /// <inheritdoc />
    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.DualSenseController" /> class bound
    ///     to a <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" />.
    /// </summary>
    /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
    public DualSenseController(ViGEmClient client) : base(client)
    {
        NativeHandle = ViGEmClient.vigem_target_ds_alloc();

        ResetReport();
    }

    /// <inheritdoc />
    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Nefarius.ViGEm.Client.Targets.DualSenseController" /> class bound
    ///     to a <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> overriding the default Vendor and Product IDs with the
    ///     provided values.
    /// </summary>
    /// <param name="client">The <see cref="T:Nefarius.ViGEm.Client.ViGEmClient" /> this device is attached to.</param>
    /// <param name="vendorId">The Vendor ID to use.</param>
    /// <param name="productId">The Product ID to use.</param>
    public DualSenseController(ViGEmClient client, ushort vendorId, ushort productId) : this(client)
    {
        VendorId = vendorId;
        ProductId = productId;
    }

    public override void Connect()
    {
        base.Connect();

        //
        // Callback to event
        // 
        _notificationCallback = (client, target, motor, smallMotor, color, userData) => FeedbackReceived?.Invoke(this,
            new DualSenseFeedbackReceivedEventArgs(motor, smallMotor,
                new LightbarColor(color.Red, color.Green, color.Blue)));

        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds_register_notification(Client.NativeHandle,
            NativeHandle,
            _notificationCallback);

        switch (error)
        {
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
                throw new VigemBusNotFoundException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
                throw new VigemInvalidTargetException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_CALLBACK_ALREADY_REGISTERED:
                throw new VigemCallbackAlreadyRegisteredException();
        }
    }

    public override void Disconnect()
    {
        ViGEmClient.vigem_target_ds_unregister_notification(NativeHandle);

        base.Disconnect();
    }

    public int ButtonCount => ButtonMap.Count;

    public int AxisCount => AxisMap.Count;

    public int SliderCount => SliderMap.Count;

    public void SetButtonState(int index, bool pressed)
    {
        SetButtonState(ButtonMap[index], pressed);
    }

    public void SetAxisValue(int index, short value)
    {
        SetAxisValue(AxisMap[index],
            (byte)MathUtil.ConvertRange(
                short.MinValue,
                short.MaxValue,
                byte.MinValue,
                byte.MaxValue,
                value
            )
        );
    }

    public void SetSliderValue(int index, byte value)
    {
        SetSliderValue(SliderMap[index], value);
    }

    public bool AutoSubmitReport { get; set; } = true;

    public void ResetReport()
    {
        _nativeReport = default;

        _nativeReport.wButtons &= unchecked((ushort)~0xF);
        _nativeReport.wButtons |= 0x08; // resting HAT switch position
        _nativeReport.bThumbLX = 0x80; // centered axis value
        _nativeReport.bThumbLY = 0x80; // centered axis value
        _nativeReport.bThumbRX = 0x80; // centered axis value
        _nativeReport.bThumbRY = 0x80; // centered axis value
    }

    public void SubmitReport()
    {
        SubmitNativeReport(_nativeReport);
    }

    public event DualSenseFeedbackReceivedEventHandler FeedbackReceived;

    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    private void SubmitNativeReport(ViGEmClient.DS_REPORT report)
    {
        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds_update(Client.NativeHandle, NativeHandle, report);

        switch (error)
        {
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
                break;
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_INVALID_HANDLE:
                throw new VigemBusInvalidHandleException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
                throw new VigemInvalidTargetException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
                throw new VigemBusNotFoundException();
            default:
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}