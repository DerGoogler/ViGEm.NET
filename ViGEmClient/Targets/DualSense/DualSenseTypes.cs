using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nefarius.ViGEm.Client.Targets.DualSense;

/// <summary>
///     Describes a modifiable property of a <see cref="DualSenseController" /> object.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public abstract class DualSenseProperty : IComparable
{
    protected DualSenseProperty(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    ///     Property ID/Index.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets all properties of the provided type.
    /// </summary>
    /// <typeparam name="T">The <see cref="DualSenseProperty" />-type to query for.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="T" />.</returns>
    public static IEnumerable<T> GetAll<T>() where T : DualSenseProperty =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is not DualSenseProperty otherValue)
            return false;

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(object other)
    {
        return Id.CompareTo(((DualSenseProperty)other).Id);
    }
}

/// <summary>
///     Possible identifiers for digital (two-state) buttons on a <see cref="DualSenseController" /> surface.
/// </summary>
public abstract class DualSenseButton : DualSenseProperty
{
    public static readonly DualSenseButton ThumbRight = new ThumbRightButton();
    public static readonly DualSenseButton ThumbLeft = new ThumbLeftButton();
    public static readonly DualSenseButton Options = new OptionsButton();
    public static readonly DualSenseButton Create = new CreateButton();
    public static readonly DualSenseButton TriggerRight = new TriggerRightButton();
    public static readonly DualSenseButton TriggerLeft = new TriggerLeftButton();
    public static readonly DualSenseButton ShoulderRight = new ShoulderRightButton();
    public static readonly DualSenseButton ShoulderLeft = new ShoulderLeftButton();
    public static readonly DualSenseButton Triangle = new TriangleButton();
    public static readonly DualSenseButton Circle = new CircleButton();
    public static readonly DualSenseButton Cross = new CrossButton();
    public static readonly DualSenseButton Square = new SquareButton();

    protected DualSenseButton(int id, string name, ushort value)
        : base(id, name)
    {
        Value = value;
    }

    [IgnoreDataMember]
    public ushort Value { get; }

    private class ThumbRightButton : DualSenseButton
    {
        public ThumbRightButton() : base(0, "ThumbRight", 1 << 15)
        {
        }
    }

    private class ThumbLeftButton : DualSenseButton
    {
        public ThumbLeftButton() : base(1, "ThumbLeft", 1 << 14)
        {
        }
    }

    private class OptionsButton : DualSenseButton
    {
        public OptionsButton() : base(2, "Options", 1 << 13)
        {
        }
    }

    private class CreateButton : DualSenseButton
    {
        public CreateButton() : base(3, "Create", 1 << 12)
        {
        }
    }

    private class TriggerRightButton : DualSenseButton
    {
        public TriggerRightButton() : base(4, "TriggerRight", 1 << 11)
        {
        }
    }

    private class TriggerLeftButton : DualSenseButton
    {
        public TriggerLeftButton() : base(5, "TriggerLeft", 1 << 10)
        {
        }
    }

    private class ShoulderRightButton : DualSenseButton
    {
        public ShoulderRightButton() : base(6, "ShoulderRight", 1 << 9)
        {
        }
    }

    private class ShoulderLeftButton : DualSenseButton
    {
        public ShoulderLeftButton() : base(7, "ShoulderLeft", 1 << 8)
        {
        }
    }

    private class TriangleButton : DualSenseButton
    {
        public TriangleButton() : base(8, "Triangle", 1 << 7)
        {
        }
    }

    private class CircleButton : DualSenseButton
    {
        public CircleButton() : base(9, "Circle", 1 << 6)
        {
        }
    }

    private class CrossButton : DualSenseButton
    {
        public CrossButton() : base(10, "Cross", 1 << 5)
        {
        }
    }

    private class SquareButton : DualSenseButton
    {
        public SquareButton() : base(11, "Square", 1 << 4)
        {
        }
    }
}

public abstract class DualSenseSpecialButton : DualSenseButton
{
    public static readonly DualSenseSpecialButton Ps = new PsButton();
    public static readonly DualSenseSpecialButton Touchpad = new TouchpadButton();
    public static readonly DualSenseSpecialButton Mute = new MuteButton();

    private DualSenseSpecialButton(int id, string name, ushort value) : base(id, name, value)
    {
    }

    private class PsButton : DualSenseSpecialButton
    {
        public PsButton() : base(0, "PS", 1 << 0)
        {
        }
    }

    private class TouchpadButton : DualSenseSpecialButton
    {
        public TouchpadButton() : base(1, "Touchpad", 1 << 1)
        {
        }
    }

    private class MuteButton : DualSenseSpecialButton
    {
        public MuteButton() : base(2, "Mute", 1 << 2)
        {
        }
    }
}

public abstract class DualSenseDPadDirection : DualSenseProperty
{
    public static readonly DualSenseDPadDirection None = new NoneDirection();
    public static readonly DualSenseDPadDirection Northwest = new NorthwestDirection();
    public static readonly DualSenseDPadDirection West = new WestDirection();
    public static readonly DualSenseDPadDirection Southwest = new SouthwestDirection();
    public static readonly DualSenseDPadDirection South = new SouthDirection();
    public static readonly DualSenseDPadDirection Southeast = new SoutheastDirection();
    public static readonly DualSenseDPadDirection East = new EastDirection();
    public static readonly DualSenseDPadDirection Northeast = new NortheastDirection();
    public static readonly DualSenseDPadDirection North = new NorthDirection();

    protected DualSenseDPadDirection(int id, string name, byte value)
        : base(id, name)
    {
        Value = value;
    }

    [IgnoreDataMember]
    public byte Value { get; }

    private class NoneDirection : DualSenseDPadDirection
    {
        public NoneDirection() : base(0, "None", 0x8) { }
    }

    private class NorthwestDirection : DualSenseDPadDirection
    {
        public NorthwestDirection() : base(1, "Northwest", 0x7) { }
    }

    private class WestDirection : DualSenseDPadDirection
    {
        public WestDirection() : base(2, "West", 0x6) { }
    }

    private class SouthwestDirection : DualSenseDPadDirection
    {
        public SouthwestDirection() : base(3, "Southwest", 0x5) { }
    }

    private class SouthDirection : DualSenseDPadDirection
    {
        public SouthDirection() : base(4, "South", 0x4) { }
    }

    private class SoutheastDirection : DualSenseDPadDirection
    {
        public SoutheastDirection() : base(5, "Southeast", 0x3) { }
    }

    private class EastDirection : DualSenseDPadDirection
    {
        public EastDirection() : base(6, "East", 0x2) { }
    }

    private class NortheastDirection : DualSenseDPadDirection
    {
        public NortheastDirection() : base(7, "Northeast", 0x1) { }
    }

    private class NorthDirection : DualSenseDPadDirection
    {
        public NorthDirection() : base(8, "North", 0x0) { }
    }
}

public abstract class DualSenseAxis : DualSenseProperty
{
    public static readonly DualSenseAxis LeftThumbX = new LeftThumbXAxis();
    public static readonly DualSenseAxis LeftThumbY = new LeftThumbYAxis();
    public static readonly DualSenseAxis RightThumbX = new RightThumbXAxis();
    public static readonly DualSenseAxis RightThumbY = new RightThumbYAxis();

    protected DualSenseAxis(int id, string name)
        : base(id, name)
    {
    }

    private class LeftThumbXAxis : DualSenseAxis
    {
        public LeftThumbXAxis() : base(0, "LeftThumbX") { }
    }

    private class LeftThumbYAxis : DualSenseAxis
    {
        public LeftThumbYAxis() : base(1, "LeftThumbY") { }
    }

    private class RightThumbXAxis : DualSenseAxis
    {
        public RightThumbXAxis() : base(2, "RightThumbX") { }
    }

    private class RightThumbYAxis : DualSenseAxis
    {
        public RightThumbYAxis() : base(3, "RightThumbY") { }
    }
}

public abstract class DualSenseSlider : DualSenseProperty
{
    public static readonly DualSenseSlider LeftTrigger = new LeftTriggerSlider();
    public static readonly DualSenseSlider RightTrigger = new RightTriggerSlider();

    protected DualSenseSlider(int id, string name)
        : base(id, name)
    {
    }

    private class LeftTriggerSlider : DualSenseSlider
    {
        public LeftTriggerSlider() : base(0, "LeftTrigger") { }
    }

    private class RightTriggerSlider : DualSenseSlider
    {
        public RightTriggerSlider() : base(1, "RightTrigger") { }
    }
}