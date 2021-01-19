#include "../include/led.h"
#include "../include/pin.h"

led::led(pin pin)
	: pin_(pin)
{
	pin_.set_write_mode();
	pin.turn_off();
}

void led::turn_on() const
{
	pin_.turn_on();
}

void led::turn_off() const
{
	pin_.turn_off();
}