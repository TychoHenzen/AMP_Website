namespace NidoSuave.Services;

public class TaglineService
{
    private static readonly string[] Taglines =
    [
        "een moment voor jou…",
        "Een plek waar je even niets hoeft",
        "Rust is geen luxe. Het is een noodzaak.",
        "Een zachte plek voor groot en klein",
        "Een goede massage begint vóórdat je op de tafel ligt.",
        "afgestemd op jouw lichaam en jouw moment."
    ];

    public string Current { get; } = Taglines[Random.Shared.Next(Taglines.Length)];
}
