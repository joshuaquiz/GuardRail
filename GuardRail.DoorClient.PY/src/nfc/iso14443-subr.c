/*-
 * Free/Libre Near Field Communication (NFC) library
 *
 * Libnfc historical contributors:
 * Copyright (C) 2009      Roel Verdult
 * Copyright (C) 2009-2013 Romuald Conty
 * Copyright (C) 2010-2012 Romain Tartière
 * Copyright (C) 2010-2013 Philippe Teuwen
 * Copyright (C) 2012-2013 Ludovic Rousseau
 * See AUTHORS file for a more comprehensive list of contributors.
 * Additional contributors of this file:
 *
 * This program is free software: you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>
 */

 /**
 * @file iso14443-subr.c
 * @brief Defines some function extracted for ISO/IEC 14443
 */

#ifdef HAVE_CONFIG_H
#include "../../include/nfc/config.h"
#endif // HAVE_CONFIG_H

#include <stdio.h>
#include <string.h>

#include "../../include/nfc/nfc.h"
#include "../../include/nfc/nfc-internal.h"


 /**
  * @brief CRC_A
  */
void iso14443a_crc(uint8_t* pbt_data, size_t sz_len, uint8_t* pbt_crc)
{
    uint32_t w_crc = 0x6363;
    do {
        uint8_t bt = *pbt_data++;
        bt = bt ^ (uint8_t)(w_crc & 0x00FF);
        bt = bt ^ bt << 4;
        w_crc = w_crc >> 8 ^ (uint32_t)bt << 8 ^ (uint32_t)bt << 3 ^ (uint32_t)bt >> 4;
    } while (--sz_len);

    *pbt_crc++ = (uint8_t)(w_crc & 0xFF);
    *pbt_crc = (uint8_t)(w_crc >> 8 & 0xFF);
}

/**
 * @brief Append CRC_A
 */
void iso14443a_crc_append(uint8_t* pbtData, size_t szLen)
{
    iso14443a_crc(pbtData, szLen, pbtData + szLen);
}

/**
 * @brief CRC_B
 */
void iso14443b_crc(uint8_t* pbt_data, size_t sz_len, uint8_t* pbt_crc)
{
    uint32_t w_crc = 0xFFFF;
    do {
        uint8_t bt = *pbt_data++;
        bt = bt ^ (uint8_t)(w_crc & 0x00FF);
        bt = bt ^ bt << 4;
        w_crc = w_crc >> 8 ^ (uint32_t)bt << 8 ^ (uint32_t)bt << 3 ^ (uint32_t)bt >> 4;
    } while (--sz_len);
    w_crc = ~w_crc;
    *pbt_crc++ = (uint8_t)(w_crc & 0xFF);
    *pbt_crc = (uint8_t)(w_crc >> 8 & 0xFF);
}

/**
 * @brief Append CRC_B
 */
void iso14443b_crc_append(uint8_t* pbt_data, const size_t szLen)
{
    iso14443b_crc(pbt_data, szLen, pbt_data + szLen);
}

/**
 * @brief Locate historical bytes
 * @see ISO/IEC 14443-4 (5.2.7 Historical bytes)
 */
uint8_t* iso14443a_locate_historical_bytes(uint8_t* pbt_ats, const size_t sz_ats, size_t* psz_tk)
{
    if (sz_ats) {
        size_t offset = 1;
        // TA
        if (pbt_ats[0] & 0x10)
        {
            offset++;
        }

        // TB
        if (pbt_ats[0] & 0x20)
        {
            offset++;
        }

        // TC
        if (pbt_ats[0] & 0x40)
        {
            offset++;
        }

        if (sz_ats > offset)
        {
            *psz_tk = sz_ats - offset;
            return pbt_ats + offset;
        }
    }

    *psz_tk = 0;
    return NULL;
}

/**
 * @brief Add cascade tags (0x88) in UID
 * @see ISO/IEC 14443-3 (6.4.4 UID contents and cascade levels)
 */
void iso14443_cascade_uid(const uint8_t abt_uid[], const size_t sz_uid, uint8_t* pbt_cascaded_uid, size_t* psz_cascaded_uid)
{
    switch (sz_uid) {
        case 7:
            pbt_cascaded_uid[0] = 0x88;
            memcpy(pbt_cascaded_uid + 1, abt_uid, 7);
            *psz_cascaded_uid = 8;
            break;

        case 10:
            pbt_cascaded_uid[0] = 0x88;
            memcpy(pbt_cascaded_uid + 1, abt_uid, 3);
            pbt_cascaded_uid[4] = 0x88;
            memcpy(pbt_cascaded_uid + 5, abt_uid + 3, 7);
            *psz_cascaded_uid = 12;
            break;

        case 4:
        default:
            memcpy(pbt_cascaded_uid, abt_uid, sz_uid);
            *psz_cascaded_uid = sz_uid;
            break;
    }
}
