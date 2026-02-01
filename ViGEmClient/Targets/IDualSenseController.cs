using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Nefarius.ViGEm.Client.Targets.DualSense;

namespace Nefarius.ViGEm.Client.Targets;

[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
public interface IDualSenseController : IVirtualGamepad, IDisposable
{
    ref byte LeftTrigger { get; }

    ref byte RightTrigger { get; }

    ref byte LeftThumbX { get; }

    ref byte LeftThumbY { get; }

    ref byte RightThumbX { get; }

    ref byte RightThumbY { get; }

    void SetButtonState(DualSenseButton button, bool pressed);
    
    void SetDPadDirection(DualSenseDPadDirection direction);
    
    void SetAxisValue(DualSenseAxis axis, byte value);
    
    void SetSliderValue(DualSenseSlider slider, byte value);
    
    event DualSenseFeedbackReceivedEventHandler FeedbackReceived;
    
    void SetButtonsFull(ushort buttons);
    
    void SetSpecialButtonsFull(byte buttons);

    /// <summary>
    ///     Submits the full input report to the device.
    /// </summary>
    /// <param name="buffer">The input report.</param>
    void SubmitRawReport(byte[] buffer);

    /// <summary>
    ///     Awaits until a pending output report is available and returns it as byte array.
    /// </summary>
    /// <returns>The output report buffer.</returns>
    Task<byte[]> AwaitRawOutputReportAsync();

    /// <summary>
    ///     Awaits until a pending output report is available and returns it as byte array.
    /// </summary>
    /// <returns>The output report buffer.</returns>
    byte[] AwaitRawOutputReport();

    /// <summary>
    ///     Awaits until a pending output report is available and returns it as byte array.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The output report buffer.</returns>
    byte[] AwaitRawOutputReport(uint timeout);
}

public delegate void DualSenseFeedbackReceivedEventHandler(object sender, DualSenseFeedbackReceivedEventArgs e);