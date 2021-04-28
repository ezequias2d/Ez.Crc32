// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Xunit;

namespace Ez.Crc32.Tests
{
    public class ManagedCrc32Tests
    {
        private readonly ICrc32 _instance;
        private readonly byte[] _data;
        private readonly uint Crc32;

        public ManagedCrc32Tests()
        {
            _instance = ManagedCrc32.Crc32;
            _data = new byte[65535];
            new Random().NextBytes(_data);
            Crc32 = Crc32Reference.Compute(ManagedCrc32.Crc32Polynomial, 0, _data);
        }

        [Fact]
        public void ComputeSpan()
        {
            var ezcrc32 = _instance.Compute(_data);
            Assert.Equal(Crc32, ezcrc32);
        }

        [Fact]
        public void ComputeIntPtr()
        {
            unsafe
            {
                fixed (void* ptr = _data)
                {
                    var ezcrc32 = _instance.Compute((IntPtr)ptr, (IntPtr)_data.Length);
                    Assert.Equal(Crc32, ezcrc32);
                }
            }
        }

        [Fact]
        public void ComputePtr()
        {
            unsafe
            {
                fixed (void* ptr = _data)
                {
                    var ezcrc32 = _instance.Compute(ptr, (ulong)_data.Length);
                    Assert.Equal(Crc32, ezcrc32);
                }
            }
        }
    }
}
