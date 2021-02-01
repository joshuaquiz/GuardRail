/* 
    Title --- gpio.cpp

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

#ifndef RPI_HW_GPIO_CPP
#define RPI_HW_GPIO_CPP

#include "../../include/rpi-hw/gpio.hpp"

namespace rpihw
{
	gpio& gpio::get()
	{
		// Return the singleton instance
		static gpio instance;
		return instance;
	}

	gpio::gpio()
		: m_bcm2835_(new driver::bcm2835)
	{
	}

	gpio::~gpio()
	{
		// Destroy the BCM2835 controller
		delete m_bcm2835_;
	}

	expander_slot& gpio::find_expander(const uint8_t pin)
	{
		// Find the I/O expander with the specific pin index
		for (auto it = m_expanders_.rbegin(); it != m_expanders_.rend(); ++it)
		{
			if (pin >= it->pin_base)
			{
				return *it;
			}
		}

		// Else throw an exception
		throw exception(
			utils::format(
				"(Fatal) `gpio::findExpander`: there's not I/O expander for pin %u\n",
				pin));
	}

	void gpio::setup(
		const uint8_t pin,
		const uint8_t mode,
		const uint8_t pull_mode)
	{
		// Set the mode of a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setup(pin, mode, pull_mode);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setup(pin - slot.pin_base, mode, pull_mode);
		}
	}

	void gpio::write(const uint8_t pin, const bool value)
	{
		// Set the value of a output pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->write(pin, value);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->write(pin - slot.pin_base, value);
		}
	}

	bool gpio::read(const uint8_t pin)
	{
		// Return the value of a input pin
		if (pin <= reserved_pins)
		{
			return m_bcm2835_->read(pin);
		}

		auto& slot = find_expander(pin);
		return slot.expander->read(pin - slot.pin_base);
	}

	bool gpio::check_event(const uint8_t pin)
	{
		// Return the event state of a GPIO pin
		if (pin <= reserved_pins)
		{
			return m_bcm2835_->checkEvent(pin);
		}

		auto& slot = find_expander(pin);
		return slot.expander->checkEvent(pin - slot.pin_base);
	}

	void gpio::set_rising_event(const uint8_t pin, const bool enabled)
	{
		// Enable/disable the rising edge event on a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setRisingEvent(pin, enabled);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setRisingEvent(pin - slot.pin_base, enabled);
		}
	}

	void gpio::set_falling_event(const uint8_t pin, const bool enabled)
	{
		// Enable/disable the falling edge event on a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setFallingEvent(pin, enabled);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setFallingEvent(pin - slot.pin_base, enabled);
		}
	}

	void gpio::set_high_event(const uint8_t pin, const bool enabled)
	{
		// Enable/disable the high event on a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setHighEvent(pin, enabled);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setHighEvent(pin - slot.pin_base, enabled);
		}
	}

	void gpio::set_low_event(const uint8_t pin, const bool enabled)
	{
		// Enable/disable the low event on a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setLowEvent(pin, enabled);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setLowEvent(pin - slot.pin_base, enabled);
		}
	}

	void gpio::set_pull_up_down(const uint8_t pin, const uint8_t mode)
	{
		// Enable/disable the pull-up/down control on a GPIO pin
		if (pin <= reserved_pins)
		{
			m_bcm2835_->setPullUpDown(pin, mode);
		}
		else
		{
			auto& slot = find_expander(pin);
			slot.expander->setPullUpDown(pin - slot.pin_base, mode);
		}
	}
}

#endif /* RPI_HW_GPIO_CPP */
