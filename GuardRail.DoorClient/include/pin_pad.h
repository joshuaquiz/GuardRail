#pragma once
#include "button.h"
#include "pin.h"

class pin_pad
{
	button one_;
	button two_;
	button three_;
	button a_;

	button four_;
	button five_;
	button six_;
	button b_;

	button seven_;
	button eight_;
	button nine_;
	button c_;

	button star_;
	button zero_;
	button pound_;
	button d_;

	std::function<void(char)> func_;

public:
	pin_pad(
		pin col_1_pin,
		pin col_2_pin,
		pin col_3_pin,
		pin col_4_pin,

		pin row_1_pin,
		pin row_2_pin,
		pin row_3_pin,
		pin row_4_pin,

		button*,
		void(*)());

	void on_button_press(std::function<void(char)>);
};