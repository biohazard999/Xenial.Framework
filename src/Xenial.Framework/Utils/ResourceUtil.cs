using System;
using System.IO;

namespace Xenial.Framework.Utils
{
    /// <summary>   Class ResourceUtil. </summary>
    public static class ResourceUtil
    {
        /// <summary>   Gets the resource stream. </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="ResourceNotFoundException">    . </exception>
        ///
        /// <param name="type"> The type. </param>
        /// <param name="path"> The path. </param>
        ///
        /// <returns>   Stream. </returns>

        public static Stream GetResourceStream(Type type, string path)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var assembly = type.Assembly;
            var name = type.Assembly.GetName().Name;

#if NET5
            path = path
                .Replace("/", ".", StringComparison.InvariantCultureIgnoreCase)
                .Replace("\\", ".", StringComparison.InvariantCultureIgnoreCase);
#else
            path = path.Replace("/", ".").Replace("\\", ".");
#endif

            var fullPath = $"{name}.{path}";
            var stream = assembly.GetManifestResourceStream(fullPath);

            _ = stream ?? throw new ResourceNotFoundException(assembly, path);

            return stream;
        }

        /// <summary>   Gets the resource string. </summary>
        ///
        /// <param name="type"> The type. </param>
        /// <param name="path"> The path. </param>
        ///
        /// <returns>   System.String. </returns>

        public static string GetResourceString(Type type, string path)
        {
            using var resourceStream = GetResourceStream(type, path);
            using var reader = new StreamReader(resourceStream);
            var resourceString = reader.ReadToEnd();
            return resourceString;
        }
    }
}
