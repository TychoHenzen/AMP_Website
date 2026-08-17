using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace NidoSuave.Services;

public class TaglineService : IDisposable
{
    private static readonly string[] Taglines =
    [
        "Een moment voor jou…",
        "Een plek waar je even niets hoeft",
        "Rust is geen luxe.\nHet is een noodzaak.",
        "Een zachte plek\nvoor groot en klein",
        "Een goede massage begint vóórdat je op de tafel ligt.",
        "Afgestemd op jouw lichaam en jouw moment.",
        "Waar je even niets hoeft.",
        "Gewoon terugzakken in je eigen lijf.",
        "Jij verdient de ruimte.",
        "Zonder masker, zonder verwachting, zonder rol.\nGewoon jij.",
        "Aanraking kan helen\nniet alleen fysiek, maar ook emotioneel.",
    ];

    public string Current { get; private set; }
    public event Action? OnChange;

    private readonly NavigationManager _nav;

    public TaglineService(NavigationManager nav)
    {
        _nav = nav;
        Current = Pick();
        _nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Current = Pick();
        OnChange?.Invoke();
    }

    private static string Pick() => Taglines[Random.Shared.Next(Taglines.Length)];

    public void Dispose() => _nav.LocationChanged -= OnLocationChanged;
}
