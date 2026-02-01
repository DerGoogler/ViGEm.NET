using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.DualSense;

namespace Nefarius.ViGEm.Client.Targets;

internal partial class DualSenseEdgeController
{
    public ref byte LeftTrigger => ref _nativeReport.bTriggerL;

    public ref byte RightTrigger => ref _nativeReport.bTriggerR;

    public ref byte LeftThumbX => ref _nativeReport.bThumbLX;

    public ref byte LeftThumbY => ref _nativeReport.bThumbLY;

    public ref byte RightThumbX => ref _nativeReport.bThumbRX;

    public ref byte RightThumbY => ref _nativeReport.bThumbRY;

    public void SetButtonState(DualSenseButton button, bool pressed)
    {
        switch (button)
        {
            case DualSenseSpecialButton specialButton:
                if (pressed)
                {
                    _nativeReport.bSpecial |= (byte)specialButton.Value;
                }
                else
                {
                    _nativeReport.bSpecial &= (byte)~specialButton.Value;
                }

                break;
            case DualSenseButton normalButton:
                if (pressed)
                {
                    _nativeReport.wButtons |= normalButton.Value;
                }
                else
                {
                    _nativeReport.wButtons &= (ushort)~normalButton.Value;
                }

                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetDPadDirection(DualSenseDPadDirection direction)
    {
        _nativeReport.wButtons &= unchecked((ushort)~0xF);
        _nativeReport.wButtons |= direction.Value;

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetAxisValue(DualSenseAxis axis, byte value)
    {
        switch (axis.Name)
        {
            case "LeftThumbX":
                _nativeReport.bThumbLX = value;
                break;
            case "LeftThumbY":
                _nativeReport.bThumbLY = value;
                break;
            case "RightThumbX":
                _nativeReport.bThumbRX = value;
                break;
            case "RightThumbY":
                _nativeReport.bThumbRY = value;
                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetSliderValue(DualSenseSlider slider, byte value)
    {
        switch (slider.Name)
        {
            case "LeftTrigger":
                _nativeReport.bTriggerL = value;
                break;
            case "RightTrigger":
                _nativeReport.bTriggerR = value;
                break;
        }

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetButtonsFull(ushort buttons)
    {
        _nativeReport.wButtons = buttons;

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    public void SetSpecialButtonsFull(byte buttons)
    {
        _nativeReport.bSpecial = buttons;

        if (AutoSubmitReport)
        {
            SubmitNativeReport(_nativeReport);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public void SubmitRawReport(byte[] buffer)
    {
        if (buffer.Length > 63)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Supplied buffer has invalid size.");
        }

        ViGEmClient.VIGEM_ERROR error =
            ViGEmClient.vigem_target_ds_update_ex(Client.NativeHandle, NativeHandle, buffer, (uint)buffer.Length);

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
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NOT_SUPPORTED:
                throw new VigemNotSupportedException();
            default:
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public byte[] AwaitRawOutputReport()
    {
        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds_await_output_report(Client.NativeHandle,
            NativeHandle,
            ref _outputBuffer);

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
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NOT_SUPPORTED:
                throw new VigemNotSupportedException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_PARAMETER:
                throw new VigemInvalidParameterException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_IS_DISPOSING:
                throw new VigemIsDisposingException();
            default:
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return _outputBuffer.Buffer;
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public byte[] AwaitRawOutputReport(uint timeout)
    {
        ViGEmClient.VIGEM_ERROR error = ViGEmClient.vigem_target_ds_await_output_report_timeout(Client.NativeHandle,
            NativeHandle,
            timeout, ref _outputBuffer);

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
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NOT_SUPPORTED:
                throw new VigemNotSupportedException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_PARAMETER:
                throw new VigemInvalidParameterException();
            case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_TIMED_OUT:
                throw new TimeoutException("Timeout occurred while waiting for output report");
            default:
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return _outputBuffer.Buffer;
    }

    /// <inheritdoc />
    public Task<byte[]> AwaitRawOutputReportAsync()
    {
        return Task.FromResult(AwaitRawOutputReport());
    }
}