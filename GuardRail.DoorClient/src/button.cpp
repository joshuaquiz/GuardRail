#include <wiringPi.h>
#include <mutex>
#include "../include/button.h"
#include "../include/pin.h"

button::button(const pin pos_pin, const pin neg_pin)
	: pos_pin_(pos_pin),
	  neg_pin_(neg_pin),
	  currently_on_(false)
{
}

void button::on_button_down(const std::function<void()> func)
{
	button_down_func_ = func;
}

void button::button_click(const std::function<void()> func)
{
	button_click_func_ = func;
}

void button::button_up(const std::function<void()> func)
{
	button_up_func_ = func;
}

void button::setup()
{
	pos_pin_.set_write_mode();
	pos_pin_.turn_on();
	currently_on_ = false;
	neg_pin_.set_change_mode(true);
	neg_pin_.on_change<button>(on_button_pressed);
}

void button::on_button_pressed()
{
	static unsigned long button_pressed_timestamp;
	const auto is_pin_on = neg_pin_.is_on();
	if (is_pin_on) {
		button_pressed_timestamp = millis();
		if (button_down_func_)
		{
			button_down_func_();
		}
		
		currently_on_ = true;
	}
	else {
		const auto duration = millis() - button_pressed_timestamp;
		if (button_up_func_)
		{
			button_up_func_();
		}
		
		button_pressed_timestamp = millis();
		if (duration < 100) {
			return;
		}

		if (button_click_func_)
		{
			button_click_func_();
		}
		
		currently_on_ = false;
	}
}