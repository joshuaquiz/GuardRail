#include "../include/pin_pad.h"
#include "../include/button.h"

pin_pad::pin_pad(
    const pin col_1_pin,
	const pin col_2_pin,
	const pin col_3_pin,
	const pin col_4_pin,

	const pin row_1_pin,
	const pin row_2_pin,
	const pin row_3_pin,
	const pin row_4_pin,

	button* b,
	void(*func)())
	: one_(row_1_pin, col_1_pin),
	two_(row_1_pin, col_2_pin),
	three_(row_1_pin, col_3_pin),
	a_(row_1_pin, col_4_pin),

	four_(row_2_pin, col_1_pin),
	five_(row_2_pin, col_2_pin),
	six_(row_2_pin, col_3_pin),
	b_(row_2_pin, col_4_pin),

	seven_(row_3_pin, col_1_pin),
	eight_(row_3_pin, col_2_pin),
	nine_(row_3_pin, col_3_pin),
	c_(row_3_pin, col_4_pin),

	star_(row_4_pin, col_1_pin),
	zero_(row_4_pin, col_2_pin),
	pound_(row_4_pin, col_3_pin),
	d_(row_4_pin, col_4_pin)
{
	one_.button_click_func = [this, &b]() {
		b = &one_;
		this->func_('1');
	};
	one_.setup(func);
	two_.button_click_func = [this]() {
		this->func_('2');
	};
	two_.setup(func);
	three_.button_click_func = [this]() {
		this->func_('3');
	};
	three_.setup(func);
	a_.button_click_func = [this]() {
		this->func_('A');
	};
	a_.setup(func);
	four_.button_click_func = [this]() {
		this->func_('4');
	};
	four_.setup(func);
	five_.button_click_func = [this]() {
		this->func_('5');
	};
	five_.setup(func);
	six_.button_click_func = [this]() {
		this->func_('6');
	};
	six_.setup(func);
	b_.button_click_func = [this]() {
		this->func_('B');
	};
	b_.setup(func);
	seven_.button_click_func = [this]() {
		this->func_('7');
	};
	seven_.setup(func);
	eight_.button_click_func = [this]() {
		this->func_('8');
	};
	eight_.setup(func);
	nine_.button_click_func = [this]() {
		this->func_('9');
	};
	nine_.setup(func);
	c_.button_click_func = [this]() {
		this->func_('c');
	};
	c_.setup(func);
	star_.button_click_func = [this]() {
		this->func_('*');
	};
	star_.setup(func);
	zero_.button_click_func = [this]() {
		this->func_('0');
	};
	zero_.setup(func);
	pound_.button_click_func = [this]() {
		this->func_('#');
	};
	pound_.setup(func);
	d_.button_click_func = [this]() {
		this->func_('D');
	};
	d_.setup(func);
}

void pin_pad::on_button_press(const std::function<void(char)> func)
{
	func_ = func;
}