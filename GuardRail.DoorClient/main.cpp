/*
	Title --- 16keys2.cpp [examples]

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

#include <iostream>
#include <memory>

// Include Rpi-hw headers
#include "include/rpi-hw/keypad/matrix.hpp"

// Use Rpi-hw namespace
using namespace rpihw;

/*
	  (14, 15, 18, 23)   colums = 4
			||||
   ----------------------
   | (1)  (2)  (3)  (A) |
   |                    |
   | (4)  (5)  (6)  (B) |
   |                    |
   | (7)  (8)  (9)  (C) |
   |                    |
   | (*)  (0)  (#)  (D) |
   ----------------------
			||||
	   (24, 25, 8, 7)  rows = 4
*/

class my_app {

public:

	// Define the keymap
	std::vector<uint8_t> keymap = {
		'1', '2', '3', 'A',
		'4', '5', '6', 'B',
		'7', '8', '9', 'C',
		'*', '0', '#', 'D'
	};

	/** Constructor method **/
	my_app()
    : m_keypad_(
		new keypad::matrix(
			// cols
			{ 7, 8, 25, 24 },
			// rows
			{ 21, 20, 16, 12 },
			keymap))
    {
        const keypad::t_event_listener listener = std::bind(&my_app::event_listener, this, std::placeholders::_1);

		// Add the keypad event listener
		m_keypad_->add_event_listener(listener);
	}

	my_app(my_app&) = delete;
	my_app(const my_app&) = delete;
	my_app(my_app&&) = delete;
	my_app& operator=(my_app&) = delete;
	my_app& operator=(const my_app&) = delete;
	my_app& operator=(const my_app&&) = delete;

	~my_app() = default;

    /** A simple keypad event listener **/
	void event_listener(keypad::keypad_base& dev)
    {
		const auto& keystate = dev.key_state();
		for (auto c : keystate)
		{
			std::cout << static_cast<char>(c) << std::flush;
		}

        std::cout << std::string(keystate.size(), '\b');
	}

	static void run()
	{
		while (true)
		{
		}
	}

private:

	// The keypad instance.
	std::unique_ptr< keypad::matrix > m_keypad_{};
};

int main(int argc, char* args[]) {

	my_app app;

    my_app::run();

	return 0;
}
