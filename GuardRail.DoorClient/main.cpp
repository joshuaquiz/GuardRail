#include <iostream>
#include <memory>
#include "include/rpi-hw/keypad/matrix.hpp"

#include "include/nfc/nfc.h"
#include "libnfc/chips/pn53x.h"

using namespace rpihw;

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
			{ 17, 27, 22, 5 },
			// rows
			{ 23, 24, 25, 16 },
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
		auto pressed = false;
		std::string line = "";
		for (auto i = 0; i < 4; i++)
		{
            const auto r = i * 4;
			if (dev.pressed(r % 4 + r) || dev.pressed(r % 4 + 1 + r) || dev.pressed(r % 4 + 2 + r) || dev.pressed(r % 4 + 3 + r))
			{
				pressed = true;
				for (auto j = 0; j < 4; j++)
				{
					if (!dev.pressed(r + j))
					{
						line += keymap[r + j];
						break;
					}
				}
			}
		}

		if (pressed)
		{
			std::cout << line << std::endl;
		}
	}

	static void run()
	{
		while (true)
		{
		}
	}

private:

	// The keypad instance.
	std::unique_ptr<keypad::matrix> m_keypad_{};
};

int main(int argc, char* args[]) {

	my_app app;
	nfc_device* pnd;
	nfc_target nt;
	nfc_context* context;
	nfc_init(&context);
	if (context == NULL)
	{
		exit(0);
		return 0;
	}

	pnd = nfc_open(context, NULL);
	if (pnd == NULL)
	{
		nfc_exit(context);
		return 0;
	}

	int i, res;
	for (i = 0; i < 10; i++) {
		if ((res = pn53x_write_register(pnd, PN53X_SFR_P3, _BV(P32), 0)) < 0)
		{
			nfc_close(pnd);
			return 0;
		}
		sleep(1);
		if ((res = pn53x_write_register(pnd, PN53X_SFR_P3, _BV(P32), _BV(P32))) < 0)
		{
			nfc_close(pnd);
			return 0;
		}
		sleep(1);
	}

    my_app::run();

	return 0;
}
