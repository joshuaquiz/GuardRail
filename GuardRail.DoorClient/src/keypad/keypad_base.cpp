/* 
    Title --- keypad/base.cpp

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

#ifndef RPI_HW_KEYPAD_BASE_CPP
#define RPI_HW_KEYPAD_BASE_CPP

#include "../../include/rpi-hw/keypad/keypad_base.hpp"

namespace rpihw
{
	namespace keypad
	{
		keypad_base::keypad_base(
			const size_t total,
			const std::initializer_list<uint8_t> pins)
			: m_number_of_keys_(total),
			m_input_(new iface::input(pins, PULL_UP)),
			m_keystate_(m_number_of_keys_, false),
			m_pressed_(m_number_of_keys_, false),
			m_released_(m_number_of_keys_, false),
			m_frequency_(10.0),
			m_thread_(new std::thread(&keypad_base::update, this)),
			m_mutex_(new std::mutex)
		{
		}

		keypad_base::keypad_base(
			const size_t total,
			const std::initializer_list<uint8_t> pins,
			const std::vector<uint8_t>& keymap)
			: m_number_of_keys_(total),
			m_input_(new iface::input(pins, PULL_UP)),
			m_keystate_(m_number_of_keys_, false),
			m_pressed_(m_number_of_keys_, false),
			m_released_(m_number_of_keys_, false),
			m_frequency_(10.0),
			m_thread_(new std::thread([this]()
				{
					update();
				})),
			m_mutex_(new std::mutex())
		{
			// Set the keymap
			keypad_base::set_keymap(keymap);
		}

		keypad_base::keypad_base(
			const size_t total,
			const std::vector<uint8_t>& pins)
			: m_number_of_keys_(total),
			m_input_(new iface::input(pins, PULL_UP)),
			m_keystate_(m_number_of_keys_, false),
			m_pressed_(m_number_of_keys_, false),
			m_released_(m_number_of_keys_, false),
			m_frequency_(10.0),
			m_thread_(new std::thread(&keypad_base::update, this)),
			m_mutex_(new std::mutex)
		{
		}

		keypad_base::keypad_base(
			const size_t total,
			const std::vector<uint8_t>& pins,
			const std::vector<uint8_t>& keymap)
			: m_number_of_keys_(total),
			m_input_(new iface::input(pins, PULL_UP)),
			m_keystate_(m_number_of_keys_, false),
			m_pressed_(m_number_of_keys_, false),
			m_released_(m_number_of_keys_, false),
			m_frequency_(10.0),
			m_thread_(new std::thread(&keypad_base::update, this)),
			m_mutex_(new std::mutex)
		{
			// Set the keymap
			keypad_base::set_keymap(keymap);
		}

		keypad_base::~keypad_base()
		{
			// Destroy the interfaces
			delete m_input_;

			// Destroy the thread and mutex instances 
			delete m_thread_;
			delete m_mutex_;
		}

		void keypad_base::set_keymap(const std::vector<uint8_t>& keymap)
		{
			// Check the keymap
			if (keymap.size() != m_number_of_keys_)
			{
				throw exception(utils::format("(Fatal) `keypad::setKeymap`: bad keymap\n"));
			}

			// Store the keymap
			uint8_t index = 0;
			for (auto& key : keymap)
			{
				m_keymap_[key] = index++;
			}
		}

		bool keypad_base::state(const size_t index) const
		{
			// Check if the button exists
			if (index >= m_number_of_keys_)
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::state`: keypad %p has only %lu buttons\n",
						this,
						static_cast<unsigned long>(m_number_of_keys_)));
			}

			// Return the button state
			return m_keystate_[index];
		}

		bool keypad_base::pressed(const size_t index) const
		{
			// Check if the button exists
			if (index >= m_number_of_keys_)
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::pressed`: keypad %p has only %lu buttons\n",
						this,
						static_cast<unsigned long>(m_number_of_keys_)));
			}

			// Return `true` if the button is pressed
			return m_pressed_[index];
		}

		bool keypad_base::released(const size_t index) const
		{
			// Check if the button exists
			if (index >= m_number_of_keys_)
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::released`: keypad %p has only %lu buttons\n",
						this,
						static_cast<unsigned long>(m_number_of_keys_)));
			}

			// Return `true` if the button is released
			return m_released_[index];
		}

		bool keypad_base::key_state(const uint8_t key) const
		{
			// Check if the key exists
			const auto it = m_keymap_.find(key);
			if (it == m_keymap_.end())
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::state`: keypad %p doesn't have key '%c'\n",
						this,
						static_cast<char>(key)));
			}

			// Return the button state
			return m_keystate_[it->second];
		}

		bool keypad_base::key_pressed(const uint8_t key) const
		{
			// Check if the key exists
			const auto it = m_keymap_.find(key);
			if (it == m_keymap_.end())
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::pressed`: keypad %p doesn't have key '%c'\n",
						this,
						static_cast<char>(key)));
			}

			// Return `true` if the button is pressed
			return m_pressed_[it->second];
		}

		bool keypad_base::key_released(const uint8_t key) const
		{
			// Check if the key exists
			const auto it = m_keymap_.find(key);
			if (it == m_keymap_.end())
			{
				throw exception(
					utils::format(
						"(Fatal) `keypad::released`: keypad %p doesn't have key '%c'\n",
						this,
						static_cast<char>(key)));
			}

			// Return `true` if the button is released
			return m_released_[it->second];
		}

		std::vector<uint8_t> keypad_base::key_state() const
		{
			// List of pressed keys
			std::vector<uint8_t> keys;
			keys.reserve(m_number_of_keys_);

			// Find pressed keys
			for (auto& key : m_keymap_)
			{
				if (m_keystate_[key.second])
				{
					keys.push_back(key.first);
				}
			}

			// Return the list of pressed keys
			return keys;
		}

	}
}

#endif /* RPI_HW_KEYPAD_BASE_CPP */
