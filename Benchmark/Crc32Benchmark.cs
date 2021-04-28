// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Ez.Crc32;
using System;

namespace Benchmark
{

    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    public class Crc32Benchmark
    {
        private byte[] data;

        [Params(1073741824)]
        public int DataSize;

        [GlobalSetup]
        public void Setup()
        {
            data = new byte[DataSize];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public uint EzCrc32() => ManagedCrc32.Crc32.Compute(data);

        [Benchmark]
        public uint EzCrc32C() => ManagedCrc32.Crc32C.Compute(data);

        [Benchmark]
        public uint EzCrc32C_X86() => NativeCrc32C.X86.Compute(data);

        [Benchmark]
        public uint EzCrc32C_X64() => NativeCrc32C.X64.Compute(data);

        [Benchmark(Baseline = true)]
        public uint EzCrc32C_Auto() => AutoCrc32.Crc32C.Compute(data);

        [Benchmark]
        public uint Crc32CNET() => Crc32C.Crc32CAlgorithm.Compute(data);

        [Benchmark]
        public uint ForceCrc32NET() => Force.Crc32.Crc32CAlgorithm.Compute(data);

        [Benchmark]
        public uint K4osCrc32() => K4os.Hash.Crc.Crc32.DigestOf(K4os.Hash.Crc.Crc32Table.Default, data);
    }
}
