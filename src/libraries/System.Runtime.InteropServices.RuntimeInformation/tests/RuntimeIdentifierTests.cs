// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Runtime.InteropServices.RuntimeInformationTests
{
    public class RuntimeIdentifierTests
    {
        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyOSRid()
        {
            Assert.NotNull(RuntimeInformation.RuntimeIdentifier);
            Assert.Same(RuntimeInformation.RuntimeIdentifier, RuntimeInformation.RuntimeIdentifier);
            Assert.EndsWith(RuntimeInformation.ProcessArchitecture.ToString(), RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyEnvironmentVariable()
        {
            RemoteInvokeOptions options = new RemoteInvokeOptions();
            options.StartInfo.EnvironmentVariables.Add("DOTNET_RUNTIME_ID", "overridenFromEnv-rid");

            RemoteExecutor.Invoke(() =>
            {
                Assert.Equal("overridenFromEnv-rid", RuntimeInformation.RuntimeIdentifier);
            }, options).Dispose();
        }

        [Fact]
        public void VerifyAppContextVariable()
        {
            RemoteExecutor.Invoke(() =>
            {
                AppDomain.CurrentDomain.SetData("RUNTIME_IDENTIFIER", "overriden-rid");

                Assert.Equal("overriden-rid", RuntimeInformation.RuntimeIdentifier);
            }).Dispose();
        }

        [Fact]
        public void VerifyAppContextVariableUnknown()
        {
            RemoteExecutor.Invoke(() =>
            {
                AppDomain.CurrentDomain.SetData("RUNTIME_IDENTIFIER", null);

                Assert.Equal("unknown", RuntimeInformation.RuntimeIdentifier);
            }).Dispose();

            RemoteExecutor.Invoke(() =>
            {
                AppDomain.CurrentDomain.SetData("RUNTIME_IDENTIFIER", new object());

                Assert.Equal("unknown", RuntimeInformation.RuntimeIdentifier);
            }).Dispose();
        }

        [Fact, PlatformSpecific(TestPlatforms.Windows)]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyWindowsRid()
        {
            Assert.StartsWith("win", RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.Linux)]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyLinuxRid()
        {
            string expectedOSName = File.ReadAllLines("/etc/os-release")
                .First(line => line.StartsWith("ID=", StringComparison.OrdinalIgnoreCase))
                .Substring("ID=".Length)
                .Trim();

            Assert.StartsWith(expectedOSName, RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.FreeBSD)]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyFreeBSDRid()
        {
            Assert.StartsWith("freebsd", RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        [Fact, PlatformSpecific(TestPlatforms.OSX)]
        [ActiveIssue("https://github.com/dotnet/runtime/issues/26780")] // need a new testhost
        public void VerifyOSXRid()
        {
            Assert.StartsWith("osx", RuntimeInformation.RuntimeIdentifier, StringComparison.OrdinalIgnoreCase);
        }
    }
}