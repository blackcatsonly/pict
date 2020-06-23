//---------------------------------------------------------------------------
// <summary>
// Wraps p/Invoke declarations for the PICT unmanaged utilities.
// </summary>
// <copyright file="NativeMethods.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

namespace PICT
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Class <see cref="NativeMethods"/> declares the p/Invoke syntax for PICT
    /// usage from .NET consumers.
    /// </summary>
    public static class NativeMethods
    {
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
        public static string Execute(string[] args)
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
            int ret = Execute(input.Length, input, output, output.Capacity);
            switch (ret)
            {
                case 0:
                    return output.ToString();
                default:
                    throw new Win32Exception(ret);
            }
        }

        /// <summary>
        /// Execute is the entry point to the command-line PICT, and it behaves accordingly.
        /// To use it, we need to mimic the runtime's handling of the console apps.  The second
        /// argument is a path to the model file.
        /// </summary>
        /// <param name="argCount">
        /// The number of arguments being passed to PICT
        /// </param>
        /// <param name="args">
        /// The PICT command-line arguments. The first argument is the name of the program but in
        /// this case, the actual value doesn't matter.
        /// </param>
        /// <param name="output">
        /// Gets set to the output produced by PICT.
        /// </param>
        /// <param name="outputLength">
        /// The length of the <paramref name="output"/> buffer.
        /// </param>
        /// <returns>
        /// The Error Code resulting from executing PICT with the command-line <paramref name="args"/>.
        /// </returns>
        /// <remarks>
        /// pict.dll exports this method, make sure you have pict.dll next to this executable when running.
        /// </remarks>
        [DllImport("pict.dll", EntryPoint = "executeNet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern int Execute(
            [In]int argCount,
            [In][MarshalAs(UnmanagedType.LPArray)] string[] args,
            [In][Out]StringBuilder output,
            [In]int outputLength);
    }
}
