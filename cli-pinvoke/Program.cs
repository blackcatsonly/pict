//---------------------------------------------------------------------------
// <summary>
// Sample program  p/Invoke declarations for the PICT unmanaged utilities.
// </summary>
// <copyright file="NativeMethods.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

namespace PICT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Class <see cref="Program"/> p/invokes PICT in-proc.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the <see cref="Program"/>,
        /// invokes <see cref="Model.Execute(string[])"/> to run PICT
        /// with the specified <paramref name="args"/>.
        /// </summary>
        /// <param name="args">
        /// The command-line arguments that should be passed to PICT.
        /// </param>
        /// <returns>
        /// The <see cref="Environment.ExitCode"/> produced by PICT.
        /// </returns>
        private static int Main(string[] args)
        {
#if DEBUG
            TestModel();
#endif

            int exitCode;
            try
            {
                string output = Model.Execute(args);
                Console.WriteLine(output);
                exitCode = 0;
            }
            catch (Win32Exception e)
            {
                exitCode = e.ErrorCode;
            }

            return exitCode;
        }

        private static void TestModel()
        {
            Tuple<string, object>[] featureGates = new Tuple<string, object>[]
               {
                new Tuple<string, object>("Feature.Gate.1", true),
                new Tuple<string, object>("Feature.Gate.1", false),
                new Tuple<string, object>("Feature.Gate.2", true),
                new Tuple<string, object>("Feature.Gate.2", false)
               };
            Model model = new Model(featureGates.GroupBy(
                keySelector: gate => gate.Item1,
                elementSelector: gate => gate.Item2));
            IEnumerable<IEnumerable<Tuple<string, string>>> matrix = model.Execute();
            IEnumerable<Tuple<string, string>> firstRow = matrix.First();
            Tuple<string, string> firstCell = firstRow.First();
            System.Diagnostics.Debug.Assert(featureGates.First().Item1.Equals(firstCell.Item1, StringComparison.Ordinal));
            System.Diagnostics.Debug.Assert(bool.TrueString.Equals(firstCell.Item2, StringComparison.Ordinal));
            Tuple<string, string> lastCell = firstRow.Last();
            System.Diagnostics.Debug.Assert(featureGates.Last().Item1.Equals(lastCell.Item1, StringComparison.Ordinal));
            System.Diagnostics.Debug.Assert(bool.FalseString.Equals(lastCell.Item2, StringComparison.Ordinal));
        }
    }
}
