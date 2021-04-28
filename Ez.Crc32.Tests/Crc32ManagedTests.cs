// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Xunit;

namespace Ez.Crc32.Tests
{
    public class Crc32X86Tests
    {
        private readonly ICrc32 _x86;
        private readonly ICrc32 _x64;
        private readonly byte[] _data;

        public Crc32X86Tests()
        {
            _x86 = NativeCrc32C.X86;
            _x64 = NativeCrc32C.X64;
            _data = new byte[65535];
            new Random().NextBytes(_data);
        }

        [Fact]
        public void X64Compute()
        {
            var reference = Crc32Reference.Compute(ManagedCrc32.Crc32CPolynomial, 0, _data);
            var ezcrc32 = _x64.Compute(_data);
            Assert.Equal(reference, ezcrc32);
        }

        [Fact]
        public void X86Compute()
        {
            var reference = Crc32Reference.Compute(ManagedCrc32.Crc32CPolynomial, 0, _data);
            var ezcrc32 = _x86.Compute(_data);
            Assert.Equal(reference, ezcrc32);
        }
    }
}
