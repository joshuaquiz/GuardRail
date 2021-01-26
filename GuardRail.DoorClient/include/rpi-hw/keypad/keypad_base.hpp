/* 
    Title --- keypad/base.hpp

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

#ifndef RPI_HW_KEYPAD_BASE_HPP
#define RPI_HW_KEYPAD_BASE_HPP

#include <functional>
#include <vector>
#include <map>

#include <thread>
#include <mutex>

#include "../types.hpp"
#include "../exception.hpp"
#include "../math.hpp"
#include "../time.hpp"
#include "../consts.hpp"
#include "../utils.hpp"

#include "../iface/iface_base.hpp"
#include "../iface/input.hpp"

namespace rpihw
{
	/*!
		@namespace rpihw::keypad
		@brief Namespace of the keypads.
	*/
	namespace keypad
	{
		// Prototypes
		class keypad_base;

		//! The type of the keypad event listener.
		typedef std::function<void(keypad_base&)> t_event_listener;

		/*!
			@class keypad_base
			@brief Generic keypad controller.
		*/
		class keypad_base
		{

		public:

			//! The keymap type.
			typedef std::map<uint8_t, uint8_t> t_keymap;

			/*!
				@brief Constructor method.
				@param[in] total Number of the buttons.
				@param[in] pins Sequence of `uint8_t` containing the input GPIOs.
			*/
			keypad_base(
				size_t total,
				std::initializer_list<uint8_t> pins);

			/*!
				@brief Constructor method.
				@param[in] total Number of the buttons.
				@param[in] pins Sequence of `uint8_t` containing the input GPIOs.
				@param[in] keymap The keymap vector.
			*/
			keypad_base(
				size_t total,
				std::initializer_list<uint8_t> pins,
				const std::vector<uint8_t>& keymap);

			/*!
				@brief Constructor method.
				@param[in] total Number of the buttons.
				@param[in] pins Vector containing the input GPIO pins.
			*/
			keypad_base(
				size_t total,
				const std::vector<uint8_t>& pins);

			/*!
				@brief Constructor method.
				@param[in] total Number of the buttons.
				@param[in] pins Vector containing the input GPIO pins.
				@param[in] keymap The keymap vector.
			*/
			keypad_base(
				size_t total,
				const std::vector<uint8_t>& pins,
				const std::vector<uint8_t>& keymap);

			keypad_base() = delete;
			keypad_base(keypad_base&) = delete;
			keypad_base(const keypad_base&) = delete;
			keypad_base(keypad_base&&) = delete;
			keypad_base& operator=(keypad_base&) = delete;
			keypad_base& operator=(const keypad_base&) = delete;
			keypad_base& operator=(const keypad_base&&) = delete;

			//! Destructor method.
			virtual ~keypad_base();

			/*!
				@brief Sets the keymap.
				@param[in] keymap The keymap vector.
			*/
			virtual void set_keymap(const std::vector<uint8_t>& keymap);

			/*!
				@brief Sets the keypad event listener.
				@param[in] listener The event listener.
			*/
			virtual void add_event_listener(t_event_listener listener);

			/*!
				@brief Sets the frequency with which buttons are read.
				@param[in] frequency The refresh rate in Hz.
			*/
			virtual void set_refresh_rate(float frequency);

			//! Returns the frequency with which buttons are read.
			virtual float get_refresh_rate() const;

			/*!
				@brief Returns a button state.
				@param[in] index The index position of the input pin.
				@return The state of the button.
			*/
			virtual bool state(size_t index) const;

			/*!
				@brief Checks if a button is pressed.
				@param[in] index The index position of the input pin.
				@return Return \c true if button is pressed.
			*/
			virtual bool pressed(size_t index) const;

			/*!
				@brief Checks if a button is released.
				@param[in] index The index position of the input pin.
				@return Return \c true if button is released.
			*/
			virtual bool released(size_t index) const;

			//! Returns the list of button states.
			virtual const std::vector<bool>& state() const;

			/*!
				@brief Returns a key state.
				@param[in] key The key button.
				@return The state of the button.
			*/
			virtual bool key_state(uint8_t key) const;

			/*!
				@brief Checks if a key is pressed.
				@param[in] key The key button.
				@return Return \c true if button is pressed.
			*/
			virtual bool key_pressed(uint8_t key) const;

			/*!
				@brief Checks if a key is released.
				@param[in] key The key button.
				@return Return \c true if button is released.
			*/
			virtual bool key_released(uint8_t key) const;

			//! Returns the list of pressed keys.
			virtual std::vector< uint8_t > key_state() const;

			//! Returns the number of keys.
			virtual size_t num_of_keys() const;

		protected:

			//! Number of the keys.
			size_t m_number_of_keys_;

			//! Buttons input interface.
			iface::input* m_input_{};

			//! The keymap vector.
			t_keymap m_keymap_{};

			//! Button states (0 = up, 1 = down).
			std::vector< bool > m_keystate_{};

			//! Pressed buttons (0 = none, 1 = pressed).
			std::vector< bool > m_pressed_{};

			//! Pressed buttons (0 = none, 1 = released).
			std::vector< bool > m_released_{};

			//! The refresh rate.
			float m_frequency_;

			//! Updating thread.
			std::thread* m_thread_{};

			//! Mutex of the updating thread.
			std::mutex* m_mutex_;

			//! The keypad event listener.
			t_event_listener m_event_listener_;

			//! Updates the state of buttons (threading function).
			virtual void update() = 0;
		};
	}
}

// Include inline methods 
#include "keypad_base-inl.hpp"

#endif /* RPI_HW_KEYPAD_BASE_HPP */
