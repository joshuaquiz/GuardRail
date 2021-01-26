/* 
    Title --- keypad/matrix.cpp

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


#ifndef RPI_HW_KEYPAD_MATRIX_CPP
#define RPI_HW_KEYPAD_MATRIX_CPP
#include "../../include/rpi-hw/keypad/matrix.hpp"

namespace rpihw { // Begin main namespace

	namespace keypad { // Begin keypads namespace

		matrix::matrix(
			const std::initializer_list<uint8_t> cols,
			const std::initializer_list<uint8_t> rows)
			: keypad_base(cols.size()* rows.size(), rows),
			m_output_(new iface::output(cols))
		{
		}

		matrix::matrix(
			const std::initializer_list<uint8_t> cols,
			const std::initializer_list<uint8_t> rows,
			const std::vector<uint8_t>& keymap)
			: keypad_base(cols.size()* rows.size(), rows, keymap),
			m_output_(new iface::output(cols))
		{
		}

		matrix::~matrix()
		{
			// Destroy the interfaces
			delete m_input_;
		}

		void matrix::update()
		{
			// Get the size of the keypad
			const auto cols = m_output_->size();
			const auto rows = m_input_->size();

			// Updating loop
			for (;;)
			{
				// Activate all columns
				m_output_->write(0);

				// Check if some buttons have been pressed
				if (~m_input_->read() == 0)
				{
					continue;
				}

				// Deactivate all columns
				m_output_->write(0xFFFF);

				// Update state of buttons
				for (uint8_t j = 0; j < cols; ++j)
				{
					// Activate the j-th column
					m_output_->write(j, LOW);
					for (uint8_t i = 0; i < rows; ++i)
					{
						// Look for connection with i-th row
						const auto state = !m_input_->read(i);

						// Update the button registers
						const auto index = static_cast<size_t>(j) + static_cast<size_t>(i) * static_cast<size_t>(cols);

						m_mutex_->lock();
						m_pressed_[index] = !m_keystate_[index] && state;
						m_released_[index] = m_keystate_[index] && !state;
						m_keystate_[index] = state;
						m_mutex_->unlock();
					}

					// Deactivate the j-th column
					m_output_->write(j, HIGH);
				}

				// Call the event listener
				if (m_event_listener_)
				{
					m_mutex_->lock();
					m_event_listener_(*this);
					m_mutex_->unlock();
				}

				// Wait some time
				time::usleep(math::floor(1000000.0 / m_frequency_));
			}
		}
	}
}

#endif /* RPI_HW_KEYPAD_MATRIX_CPP */
