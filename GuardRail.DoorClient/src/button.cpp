#include "../include/button.h"
#include "../include/pin.h"

button::button(const pin pos_pin, const pin neg_pin)
	: pos_pin_(pos_pin),
	  neg_pin(neg_pin),
	  currently_on(false)
{
}

void button::setup(void(* func)()) const
{
	pos_pin_.set_write_mode();
	pos_pin_.turn_on();
	neg_pin.set_change_mode(true);
	neg_pin.on_change(func);
}