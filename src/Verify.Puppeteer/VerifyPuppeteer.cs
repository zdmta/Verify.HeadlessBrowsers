﻿using PuppeteerSharp;

namespace VerifyTests;

public static class VerifyPuppeteer
{
    public static bool Initialized { get; private set; }

    [Obsolete("Use Initialize()")]
    public static void Enable() =>
        Initialize();

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter<ElementHandle>(ElementToImage);
        VerifierSettings.RegisterFileConverter<Page>(PageToImage);
    }

    static async Task<ConversionResult> PageToImage(Page page, IReadOnlyDictionary<string, object> context)
    {
        var screenshot = page.ScreenshotStreamAsync();
        var html = page.GetContentAsync();

        return new(
            null,
            new List<Target>
            {
                new("html", await html),
                new("png", await screenshot)
            }
        );
    }

    static async Task<ConversionResult> ElementToImage(ElementHandle element, IReadOnlyDictionary<string, object> context)
    {
        var screenshot = await element.ScreenshotStreamAsync();
        var html = await element.EvaluateFunctionAsync<string>("element => element.innerHTML");

        return new(
            null,
            new List<Target>
            {
                new("html", html),
                new("png", screenshot)
            }
        );
    }
}