
//================================================================================
//
//  Copyright (c) objectnation GmbH
//
//================================================================================
//
//  Author          simon.wilson
//  Creation Date   13.09.2002
//  Creation Time   16:47:52
//  IDE Version     Microsoft Development Environment Enterprise Edition 7.00
//  OS Version      Microsoft Windows NT 5.1.2600.0
//
//================================================================================

#region Copyright � objectnation GmbH

// This software is provided 'as-is'. While the greatest care has been taken to 
// ensure that the software functions correctly, no guarantee can be made to this effect.
//
// objectnation grants permission to anyone to use this software for any purpose as long as 
// it is in compliance with the following restrictions:
//
// 1.	This software is the intellectual property and copyright of objectnation GmbH. 
//    Under no circumstances may the origin of the software be misrepresented.
//
// 2.	Should this software be used in the development of a commercial product, 
//    then an acknowledgement in that product's documentation is a requirement. 
//    The following text should be used:
//
//    This product contains code generated by objectnation SP/Invoke, 
//    copyright � objectnation GmbH (http://www.objectnation.com).
//
// 3.	The software may not be redistributed without the express written permission of 
//    the copyright holder, namely objectnation GmbH, Switzerland.

#endregion

namespace Microsoft.VisualStudio.Extensibility
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public abstract class BaseCodeGenerator : IVsSingleFileGenerator, IDisposable
    {
        protected BaseCodeGenerator()
        {
            this.codeFileNameSpace = string.Empty;
            this.codeFilePath = string.Empty;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.codeGeneratorProgress = null;
        }

        ~BaseCodeGenerator()
        {
            this.Dispose(false);
        }

        public void Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, out IntPtr rgbOutputFileContents, out int pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            if (bstrInputFileContents == null)
            {
                throw new ArgumentNullException(bstrInputFileContents);
            }
            this.codeFilePath = wszInputFilePath;
            this.codeFileNameSpace = wszDefaultNamespace;
            this.codeGeneratorProgress = pGenerateProgress;
            byte[] buffer1 = this.GenerateCode(wszInputFilePath, bstrInputFileContents);
            if (buffer1 == null)
            {
                rgbOutputFileContents = IntPtr.Zero;
                pcbOutput = 0;
            }
            else
            {
                pcbOutput = buffer1.Length;
                rgbOutputFileContents = Marshal.AllocCoTaskMem(pcbOutput);
                Marshal.Copy(buffer1, 0, rgbOutputFileContents, pcbOutput);
            }
        }

        protected abstract byte[] GenerateCode(string inputFileName, string inputFileContent);
        protected virtual void GeneratorErrorCallback(bool warning, int level, string message, int line, int column)
        {
            IVsGeneratorProgress progress1 = this.CodeGeneratorProgress;
            if (progress1 != null)
            {
                if (line > 0)
                {
                    line--;
                }
                progress1.GeneratorError(warning, level, message, line, column);
            }
        }

        public abstract string GetDefaultExtension();

        internal IVsGeneratorProgress CodeGeneratorProgress
        {
            [DebuggerStepThrough]
            get
            {
                return this.codeGeneratorProgress;
            }
        }

        protected string FileNameSpace
        {
            [DebuggerStepThrough]
            get
            {
                return this.codeFileNameSpace;
            }
        }

        protected string InputFilePath
        {
            [DebuggerStepThrough]
            get
            {
                return this.codeFilePath;
            }
        }


        private string codeFileNameSpace;
        private string codeFilePath;
        private IVsGeneratorProgress codeGeneratorProgress;
    }
}

