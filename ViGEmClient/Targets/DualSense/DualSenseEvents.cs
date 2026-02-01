using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.ViGEm.Client.Targets.DualSense;

/// <summary>
///     Represents an RGB color value.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class LightbarColor : IEquatable<LightbarColor>
{
    /// <summary>
    ///     Creates a new <see cref="LightbarColor" /> object.
    /// </summary>
    /// <param name="red">Red component.</param>
    /// <param name="green">Green component.</param>
    /// <param name="blue">Blue component.</param>
    public LightbarColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    /// <summary>
    ///     Red component.
    /// </summary>
    public byte Red { get; set; }

    /// <summary>
    ///     Green component.
    /// </summary>
    public byte Green { get; set; }

    /// <summary>
    ///     Blue component.
    /// </summary>
    public byte Blue { get; set; }

    public bool Equals(LightbarColor other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Red == other.Red && Green == other.Green && Blue == other.Blue;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is LightbarColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Red.GetHashCode();
            hashCode = (hashCode * 397) ^ Green.GetHashCode();
            hashCode = (hashCode * 397) ^ Blue.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(LightbarColor left, LightbarColor right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LightbarColor left, LightbarColor right)
    {
        return !Equals(left, right);
    }
}

/// <summary>
///     Represents force feedback event arguments for DualSense controllers.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class DualSenseFeedbackReceivedEventArgs : EventArgs
{
    internal DualSenseFeedbackReceivedEventArgs(byte largeMotor, byte smallMotor, LightbarColor lightbarColor)
    {
        LargeMotor = largeMotor;
        SmallMotor = smallMotor;
        LightbarColor = lightbarColor;
    }

    /// <summary>
    ///     Intensity of the large rumble motor.
    /// </summary>
    public byte LargeMotor { get; }

    /// <summary>
    ///     Intensity of the small rumble motor.
    /// </summary>
    public byte SmallMotor { get; }

    /// <summary>
    ///     Color of the lightbar.
    /// </summary>
    public LightbarColor LightbarColor { get; }
}