using System;

namespace SolitaireCore;

public abstract class CoreId
{
    private readonly string _value;

    protected CoreId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    //public static implicit operator string(CoreId userId) => userId._value;
    //public static explicit operator CoreId(string value) => new CoreId(value);

    public override bool Equals(object? obj)
    {
        if (obj is CoreId other)
        {
            return _value == other._value;
        }

        return false;
    }

    public static bool operator == (CoreId? left, CoreId? right)
    {
        if (ReferenceEquals(left, right)) return true; // Same reference or both null
        if (left is null || right is null) return false; // One is null

        return left.Equals(right);
    }

    public static bool operator != (CoreId? left, CoreId? right)
    {
        if (left is null || right is null) return false; // One is null

        return !(left.Equals(right));
    }

    public override int GetHashCode() => _value.GetHashCode();
}

public class CardId : CoreId
{
    public CardId(string value) : base(value) { }
}

public class CardPileId : CoreId
{
    public CardPileId(string value) : base(value) { }
}