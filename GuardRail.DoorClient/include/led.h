#pragma once
#include "pin.h"

class led
{
	pin pin_;

public:
	explicit led(pin);

	void turn_on() const;
	void turn_off() const;
};