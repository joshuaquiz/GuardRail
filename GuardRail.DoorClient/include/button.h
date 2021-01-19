#pragma once
#include <mutex>
#include "pin.h"

class button
{
	pin pos_pin_;
	pin neg_pin_;
	bool currently_on_;
	std::function<void()> button_down_func_;
	std::function<void()> button_click_func_;
	std::function<void()> button_up_func_;

public:
	button(pin pos_pin, pin neg_pin);
	
	void on_button_down(std::function<void()>);
	void button_click(std::function<void()>);
	void button_up(std::function<void()>);

private:
	void setup();
	void on_button_pressed();
};