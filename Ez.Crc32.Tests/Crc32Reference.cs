// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Ez.Crc32 contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;

namespace Ez.Crc32.Tests
{
    public static class Crc32Reference
    {

        public static uint Compute(uint polynomial, uint previous, ReadOnlySpan<byte> data)
        {
            uint crc = previous ^ 0xFFFFFFFF;

            int current = 0;
            int length = data.Length;

            while(length-- > 0)
            {
                crc ^= data[current++];

                for (uint j = 0; j < 8; j++)
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc = crc >> 1;
            }

            return crc ^ 0xFFFFFFFF;
        }
    }
}
