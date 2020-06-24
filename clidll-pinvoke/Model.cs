//---------------------------------------------------------------------------
// <summary>
// Convenience methods to generate and parse PICT models.
// </summary>
// <copyright file="Model.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

namespace PICT
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Class <see cref="Model"/> defines convenience methods to generate
    /// and parse PICT models.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Gets the path to the <see cref="Model"/> file.
        /// </summary>
        private string filePath;

        /// <summary>
        /// Initiaiizes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="categories">
        /// The groups of values to be written to the model.
        /// </param>
        public Model(IEnumerable<IGrouping<string, object>> categories)
        {
            this.filePath = Path.GetTempFileName();
            this.GenerateModelFile(categories);
        }

        /// <summary>
        /// Executes PICT in-proc with the specified command-line <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The PICT command-line arguments.
        /// </param>
        /// <returns>
        /// The output produced by PICT.
        /// </returns>
        /// <exception cref="Win32Exception">
        /// PICT did not return ERROR_SUCCESS.
        /// </exception>
        public static string Execute(params string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(paramName: nameof(args));
            }

            const int ExtraExpectedParameterCount = 1;
            string[] input = new string[args.Length + ExtraExpectedParameterCount];
            input[0] = "pict"; // This value does not matter but is expected by PICT.
            Array.ConstrainedCopy(
                sourceArray: args,
                sourceIndex: 0,
                destinationArray: input,
                destinationIndex: ExtraExpectedParameterCount,
                length: args.Length);

            StringBuilder output = new StringBuilder(8192);
            int ret = NativeMethods.Execute(input.Length, input, output, output.Capacity);
            switch (ret)
            {
                case 0:
                    return output.ToString();
                default:
                    throw new Win32Exception(ret);
            }
        }

        /// <summary>
        /// Executes PICT on this <see cref="Model"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{IEnumerable}"/> list of the
        /// <see cref="IEnumerable{Tuple}"/> rows of output produced by PICT.
        /// </returns>
        /// <exception cref="Win32Exception">
        /// PICT did not return ERROR_SUCCESS.
        /// </exception>
        public IEnumerable<IEnumerable<Tuple<string, string>>> Execute()
        {
            string output = Execute(this.filePath);

            const char Separator = '\t';
            IEnumerable<string[]> lines = output
                .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(Separator).Select(value => value.Trim()).ToArray());
            string[] categories = lines.First();
            IEnumerable<string[]> tuples = lines.Skip(1);
            return tuples
                .Select(tuple => tuple.Select((value, index) => new Tuple<string, string>(categories[index], value)));
        }

        /// <summary>
        /// Write the <paramref name="categories"/> to a <see cref="PICT"/> <see cref="Model"/> file.
        /// </summary>
        /// <param name="categories">
        /// The category names and values to be written to the <see cref="Model"/>.
        /// </param>
        private void GenerateModelFile(IEnumerable<IGrouping<string, object>> categories)
        {
            File.WriteAllText(this.filePath, "# Auto-generated PICT Model");
            File.WriteAllLines(
                this.filePath,
                categories.Select(this.GenerateCategoryLine));
        }

        /// <summary>
        /// Serialize the <paramref name="category"/> to a line of a <see cref="PICT"/>
        /// <see cref="Model"/> file.
        /// </summary>
        /// <param name="category">
        /// The category name and values to be written to the <see cref="Model"/> file line.
        /// </param>
        private string GenerateCategoryLine(IGrouping<string, object> category) => string.Format(
            CultureInfo.InvariantCulture,
            "{0}: {1}",
            category.Key,
            string.Join(",", category.Select(value => value.ToString())));
    }
}
