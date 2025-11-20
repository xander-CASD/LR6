namespace Core.COre;

public record User(string Login, string Email, int Age);

public static class Validator
{
    public static bool IsValidEmail(string email)
        => !string.IsNullOrWhiteSpace(email) && email.Contains('@') && email.Contains('.');

    public static void EnsureAdult(int age)
    {
        if (age < 18) throw new ArgumentException("User must be 18+");
    }
}

public static class TariffService
{
    public static decimal CalcPrice(string plan, int months)
    {
        if (months <= 0) throw new ArgumentOutOfRangeException(nameof(months));
        return plan switch
        {
            "basic" => 5m * months,
            "pro" => 12m * months,
            "enterprise" => 25m * months,
            _ => throw new ArgumentException("Unknown plan")
        };
    }
}

public interface IDiscountProvider
{
    decimal GetDiscount(string plan);
}

public class TariffWithDiscount
{
    private readonly IDiscountProvider _d;
    public TariffWithDiscount(IDiscountProvider d) => _d = d;

    public decimal Calc(string plan, int months, decimal baseMonthly)
    {
        var discount = _d.GetDiscount(plan);
        return months * baseMonthly * (1 - discount);
    }
}