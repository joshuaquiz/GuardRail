#pragma once
#include <functional>
#include "pin.h"

class button
{
	pin pos_pin_;

public:
	button(pin pos_pin, pin neg_pin);

	void setup(void(*)()) const;

	pin neg_pin;
	bool currently_on;
	std::function<void()> button_down_func;
	std::function<void()> button_click_func;
	std::function<void()> button_up_func;
};