// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Runtime.CompilerServices;

#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
#endif

namespace Ez.Crc32
{
    /// <summary>
    /// Provide native <see cref="ICrc32"/> implementations using intrinsics.
    /// </summary>
    public static class NativeCrc32C
    {
        /// <summary>
        /// <see cref="ICrc32"/> implementation that uses x86 intrinsics.
        /// </summary>
        public static readonly ICrc32 X86 = new NativeX86();

        /// <summary>
        /// <see cref="ICrc32"/> implementation that uses x86 intrinsics.
        /// </summary>
        public static readonly ICrc32 X64 = new NativeX64();

        /// <summary>
        /// <see cref="ICrc32"/> implementation that uses x64 intrinsics when supported, otherwise x86.
        /// </summary>
        public static readonly ICrc32 Default = X64.IsSupported ? X64 : X86;

        private class NativeX86 : ICrc32
        {
            public bool IsSupported =>
#if NETCOREAPP3_0_OR_GREATER
                Sse42.IsSupported;
#else
                false;
#endif
            public uint Polynomial => ManagedCrc32.Crc32CPolynomial;
            public NativeX86()
            {

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(uint previousCrc, void* data, ulong length)
            {
#if NETCOREAPP3_0_OR_GREATER
                if (!Sse42.IsSupported)
                    throw new NotSupportedException();
                
                uint* current = (uint*)data;
                void* max = (byte*)data + length;
                void* maxUint = (byte*)data + length - (length % (sizeof(uint) * 2));

                uint crc = ~previousCrc;
                
                while (current < maxUint)
                    crc = Sse42.Crc32(Sse42.Crc32(crc, *current++), *current++);

                byte* currentChar = (byte*)current;

                // remaining bytes
                while (currentChar < max)
                    crc = Sse42.Crc32((uint)crc, *currentChar++);

                return ~(uint)crc;
#else
            throw new NotSupportedException();
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, ReadOnlySpan<byte> data)
            {
                unsafe
                {
                    fixed (void* ptr = data)
                        return Compute(previousCrc32, (byte*)ptr, (ulong)data.Length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, IntPtr data, IntPtr length)
            {
                unsafe
                {
                    return Compute(previousCrc32, (void*)data, (ulong)length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(void* data, ulong length) =>
                Compute(0, data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(IntPtr data, IntPtr length) =>
                Compute(0, data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(ReadOnlySpan<byte> data) =>
                Compute(0, data);
        }

        private class NativeX64 : ICrc32
        {
            public bool IsSupported =>
#if NETCOREAPP3_0_OR_GREATER
                Sse42.X64.IsSupported;
#else
                false;
#endif

            public uint Polynomial => ManagedCrc32.Crc32CPolynomial;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(uint previousCrc, void* data, ulong length)
            {
#if NETCOREAPP3_0_OR_GREATER
                if (!Sse42.X64.IsSupported)
                    throw new NotSupportedException();

                ulong* current = (ulong*)data;
                void* max = (byte*)data + length;
                void* maxUlong = (byte*)data + length - (length % (sizeof(ulong) * 3));

                ulong crc = ~previousCrc;
                
                while (current < maxUlong)
                {
                    var one = current++;
                    var two = current++;
                    var three = current++;
                    crc = Sse42.X64.Crc32(Sse42.X64.Crc32(Sse42.X64.Crc32(crc, *one), *two), *three);
                }

                byte* currentChar = (byte*)current;

                // remaining bytes
                while (currentChar < max)
                    crc = Sse42.Crc32((uint)crc, *currentChar++);

                return ~(uint)crc;

#else
            throw new NotSupportedException();
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, ReadOnlySpan<byte> data)
            {
                unsafe
                {
                    fixed (void* ptr = data)
                        return Compute(previousCrc32, ptr, (ulong)data.Length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(uint previousCrc32, IntPtr data, IntPtr length)
            {
                unsafe
                {
                    return Compute(previousCrc32, (void*)data, (ulong)length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe uint Compute(void* data, ulong length) =>
                Compute(0, data, length);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(IntPtr data, IntPtr length)
            {
                unsafe
                {
                    return Compute(0, (void*)data, (ulong)length);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Compute(ReadOnlySpan<byte> data)
            {
                unsafe
                {
                    fixed (void* ptr = data)
                        return Compute(0, ptr, (ulong)data.Length);
                }
            }
        }
    }
}
