/* 
	Title --- gpio.hpp

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

#ifndef RPI_HW_GPIO_HPP
#define RPI_HW_GPIO_HPP

#include <vector>

#include "rpi.hpp"

#include "types.hpp"
#include "exception.hpp"
#include "utils.hpp"

#include "driver/bcm2835.hpp"
#include "driver/io-expander.hpp"

constexpr int reserved_pins = 30;

namespace rpihw
{
	/*!
		@class expander_slot
		@brief Slot to store the I/O expander.
	*/
	struct expander_slot
	{
		//! Constructor method.
		expander_slot(
			const uint8_t b,
			driver::io_expander* e)
			: pin_base(b),
			expander(e)
		{
		}

		//! The pin base for the I/O expander.
		int pin_base;

		//! Pointer to the I/O expander.
		driver::io_expander* expander;
	};

	/*!
		@class gpio
		@brief Virtual GPIO controller.
	*/
	class gpio final
	{
	public:

		//! Returns the singleton instance.
		static gpio& get();
		
		gpio(gpio&) = delete;
		gpio(const gpio&) = delete;
		gpio(gpio&&) = delete;
		gpio& operator=(gpio&) = delete;
		gpio& operator=(const gpio&) = delete;
		gpio& operator=(const gpio&&) = delete;

		//! Destructor method.
		~gpio();

		/*!
			@brief Adds a I/O expander to the standard GPIO connector.
			@param[in] pin_base The user-defined pin base for the I/O expander.
			@param[in] expander The I/O expander.
		*/
		template<typename T>
		void expand(uint8_t pin_base, T& expander);

		/*!
			@brief Sets the mode of a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] mode The GPIO mode.
			@param[in] pull_mode The pull resistor mode.
		*/
		void setup(uint8_t pin, uint8_t mode, uint8_t pull_mode = PULL_OFF);

		/*!
			@brief Sets the value of a output pin.
			@param[in] pin The output pin.
			@param[in] value The value of output pin.
		*/
		void write(uint8_t pin, bool value);

		/*!
			@brief Returns the value of a input pin.
			@param[in] pin The input pin.
			@return The value of input pin.
		*/
		bool read(uint8_t pin);

		/*!
			@brief Returns the event state of a GPIO pin.
			@param[in] pin The GPIO pin.
			@return The event state.
		*/
		bool check_event(uint8_t pin);

		/*!
			@brief Enables/disables the rising edge event on a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] enabled If \c true enable the rising edge event.
		*/
		void set_rising_event(uint8_t pin, bool enabled);

		/*!
			@brief Enables/disables the falling edge event on a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] enabled If \c true enable the falling edge event.
		*/
		void set_falling_event(uint8_t pin, bool enabled);

		/*!
			@brief Enables/disables the high event on a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] enabled If \c true enable the high event.
		*/
		void set_high_event(uint8_t pin, bool enabled);

		/*!
			@brief Enables/disables the low event on a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] enabled If \c true enable the low event.
		*/
		void set_low_event(uint8_t pin, bool enabled);

		/*!
			@brief Enables/disables the pull-up/down control on a GPIO pin.
			@param[in] pin The GPIO pin.
			@param[in] mode The pull resistor mode.
		*/
		void set_pull_up_down(uint8_t pin, uint8_t mode);

	private:

		//! Broadcom BCM2835 controller.
		driver::bcm2835* m_bcm2835_;

		//! List of I/O expanders.
		std::vector<expander_slot> m_expanders_{};

		/*!
			@brief Finds the I/O expander with a specific pin index.
			@param[in] pin The GPIO pin.
			@return A pointer to the I/O expander.
		*/
		expander_slot& find_expander(uint8_t pin);

		//! Constructor method.
		gpio();
	};
}

// Include inline methods 
#include "gpio-inl.hpp"

#endif /* RPI_HW_GPIO_HPP */
