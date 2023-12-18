using System;
using System.Text.RegularExpressions;

using Shouldly;

using Xenial.Framework.Utils.Slugger;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Utils.Slugger
{
    /// <summary>   A sluger facts. </summary>
    public static class SlugerFacts
    {
        /// <summary>   Slugger tests. </summary>
        public static void SluggerTests() => Describe(nameof(Slugger), () =>
        {
            It("empty config", () =>
            {
                var config = new SlugifierConfig();

                config.ShouldSatisfyAllConditions(
                    () => config.ForceLowerCase.ShouldBeTrue(),
                    () => config.CollapseWhiteSpace.ShouldBeTrue(),
                    () => config.StringReplacements.Count.ShouldBe(1),
                    () => config.DeniedCharactersRegex.ShouldNotBeEmpty(),
                    () => config.StringReplacements.Count.ShouldBe(1),
                    () => config.StringReplacements[" "].ShouldBe("-")
                );
            });

            It("Slugifier with null ctor throws",
                () => Should.Throw<ArgumentNullException>(() => new Slugifier(null!))
            );

            Describe("default settings", () =>
            {

                var helper = new Slugifier();

                It("enforces lowercase values", () =>
                {
                    const string original = "AbCdE";
                    const string expected = "abcde";

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("collapses whitespace", () =>
                {
                    const string original = "a  b    \n  c   \t    d";
                    const string expected = "a-b-c-d";

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("removes diacritics", () =>
                {
                    const string withDiacritics = "ñáîùëÓ";
                    const string withoutDiacritics = "naiueo";

                    helper.GenerateSlug(withDiacritics).ShouldBe(withoutDiacritics);
                });

                It("removes denied chars", () =>
                {
                    const string original = "!#$%&/()=";
                    const string expected = "";

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("colapses dashes", () =>
                {
                    const string original = "foo & bar";
                    const string expected = "foo-bar";

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("colapses more than two dashes", () =>
                {
                    const string original = "foo & bar & & & Jazz&&&&&&&&";
                    const string expected = "foo-bar-jazz";

                    helper.GenerateSlug(original).ShouldBe(expected);
                });
            });

            Describe("special settings", () =>
            {
                It("not collapses whitespace", () =>
                {
                    const string original = "a  b    \n  c   \t    d";
                    const string expected = "a-b-c-d";

                    var helper = new Slugifier(new SlugifierConfig
                    {
                        CollapseWhiteSpace = false
                    });

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("replaces configured chars", () =>
                {
                    const string original = "abcde";
                    const string expected = "xyzde";

                    var config = new SlugifierConfig();
                    config.StringReplacements.Add("a", "x");
                    config.StringReplacements.Add("b", "y");
                    config.StringReplacements.Add("c", "z");

                    var helper = new Slugifier(config);

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("does not colapse dashes", () =>
                {
                    const string original = "foo & bar";
                    const string expected = "foo--bar";

                    var helper = new Slugifier(new SlugifierConfig
                    {
                        CollapseDashes = false
                    });

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("trims whitespace", () =>
                {
                    const string original = "  foo & bar  ";
                    const string expected = "foo-bar";

                    var helper = new Slugifier(new SlugifierConfig
                    {
                        TrimWhitespace = true
                    });

                    helper.GenerateSlug(original).ShouldBe(expected);
                });

                It("provides unicode support", () =>
                {
                    const string original = "unicode ♥ support";
                    const string expected = "unicode-support";

                    var helper = new Slugifier(new SlugifierConfig
                    {
                        TrimWhitespace = true,
                        CollapseDashes = true
                    });

                    helper.GenerateSlug(original).ShouldBe(expected);
                });
            });

            Describe("functional tests", () =>
            {
                var testData = new[]
                {
                    ("E¢Ðƕtoy  mÚÄ´¨ss¨sïuy   !  Pingüiño", "etoy-muasssiuy-pinguino"),
                    ("QWE dfrewf# $%& asd", "qwe-dfrewf-asd"),
                    ("You can't have any pudding if you don't eat your meat!", "you-cant-have-any-pudding-if-you-dont-eat-your-meat"),
                    ("El veloz murciélago hindú", "el-veloz-murcielago-hindu"),
                    ("Médicos sin medicinas medican meditando", "medicos-sin-medicinas-medican-meditando"),
                };

                var helper = new Slugifier();

                foreach (var (given, expected) in testData)
                {
                    It($"replaces '{given}' to be '{expected}'", () =>
                    {
                        helper.GenerateSlug(given).ShouldBe(expected);
                    });
                }
            });
        });
    }
}
