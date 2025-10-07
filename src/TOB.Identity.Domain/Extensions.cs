namespace TOB.Identity.Domain;

public static class Extensions
{
    public static string ToYesOrNo(this bool isTrue)
    {
        return isTrue ? "Yes" : "No";
    }
}
