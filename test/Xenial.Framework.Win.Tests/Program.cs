using System;
using System.Threading.Tasks;

using static Xenial.Tasty;

namespace Xenial.Framework.Win.Tests
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            It("Runs", () => true);

            return await Run(args);
        }
    }
}
