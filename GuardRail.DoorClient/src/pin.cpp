#include "../include/pin.h"
#include <wiringPi.h>

pin::pin(const int wiring_pi_pin_number)
{
	wiring_pi_pin_number_ = wiring_pi_pin_number;
}

void pin::set_read_mode() const
{
	pinMode(wiring_pi_pin_number_, INPUT);
}

void pin::set_write_mode() const
{
	pinMode(wiring_pi_pin_number_, OUTPUT);
}

void pin::set_change_mode(const bool start_low) const
{
	pinMode(wiring_pi_pin_number_, OUTPUT);
	if (start_low)
	{
		turn_off();
	}
	else
	{
		turn_on();
	}
}

void pin::turn_on() const
{
	digitalWrite(wiring_pi_pin_number_, HIGH);
}

void pin::turn_off() const
{
	digitalWrite(wiring_pi_pin_number_, LOW);
}

bool pin::is_on() const
{
	return digitalRead(wiring_pi_pin_number_) == HIGH;
}

void pin::on_change(void(*func)()) const
{
	wiringPiISR(wiring_pi_pin_number_, INT_EDGE_BOTH, func);
}