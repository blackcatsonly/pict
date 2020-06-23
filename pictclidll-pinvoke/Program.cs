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
    using System.ComponentModel;

    /// <summary>
    /// Class <see cref="Program"/> p/invokes PICT in-proc.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the <see cref="Program"/>,
        /// invokes <see cref="NativeMethods.Execute(string[])"/> to run PICT
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
            int exitCode;
            try
            {
                string output = NativeMethods.Execute(args);
                Console.WriteLine(output);
                exitCode = 0;
            }
            catch (Win32Exception e)
            {
                exitCode = e.ErrorCode;
            }

            return exitCode;
        }
    }
}
