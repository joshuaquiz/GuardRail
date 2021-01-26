/*
    Title --- keypad/base-inl.hpp

    Copyright (C) 2013 Giacomo Trudu - wicker25[at]gmail[dot]com

    This file is part of Rpi-hw.

    Rpi-hw is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation version 3 of the License.

    Rpi-hw is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Rpi-hw. If not, see <http://www.gnu.org/licenses/>.
*/

#ifndef RPI_HW_KEYPAD_BASE_INL_HPP
#define RPI_HW_KEYPAD_BASE_INL_HPP

namespace rpihw
{
    namespace keypad
    {
        inline void keypad_base::add_event_listener(const t_event_listener listener)
        {
            // Set the keypad event listener
            m_event_listener_ = listener;
        }

        inline void keypad_base::set_refresh_rate(const float frequency)
        {
            // Set the frequency with which buttons are read
            m_frequency_ = frequency;
        }

        inline float keypad_base::get_refresh_rate() const
        {
            // Returns the frequency with which buttons are read
            return m_frequency_;
        }

        inline const std::vector<bool>& keypad_base::state() const
        {
            // Return the list of button states
            return m_keystate_;
        }

        inline size_t keypad_base::num_of_keys() const
        {
            // Return the number of keys
            return m_number_of_keys_;
        }
    }
}

#endif /* RPI_HW_KEYPAD_BASE_INL_HPP */
