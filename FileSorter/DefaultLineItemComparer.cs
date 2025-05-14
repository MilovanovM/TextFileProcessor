namespace FileSorter;

/// <summary>
/// Default implementation of ILineItemComparer that compares line items
/// by text and then by number
/// </summary>
public class DefaultLineItemComparer : ILineItemComparer
{
    public int Compare(LineItem x, LineItem y)
    {
        var textComparison = StringComparer.Ordinal.Compare(x.Text, y.Text);
        return textComparison != 0 ? textComparison : x.Number.CompareTo(y.Number);
    }
}
