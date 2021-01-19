#include "../include/pin_pad.h"
#include "../include/button.h"

pin_pad::pin_pad(
	pin col_1_pin,
	pin col_2_pin,
	pin col_3_pin,
	pin col_4_pin,
	pin row_1_pin,
	pin row_2_pin,
	pin row_3_pin,
	pin row_4_pin)
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
	one_.button_click(
		[this]() {
			this->func_('1');
		});
	two_.button_click(
		[this]() {
			this->func_('2');
		});
	three_.button_click(
		[this]() {
			this->func_('3');
		});
	a_.button_click(
		[this]() {
			this->func_('A');
		});
	four_.button_click(
		[this]() {
			this->func_('4');
		});
	five_.button_click(
		[this]() {
			this->func_('5');
		});
	six_.button_click(
		[this]() {
			this->func_('6');
		});
	b_.button_click(
		[this]() {
			this->func_('B');
		});
	seven_.button_click(
		[this]() {
			this->func_('7');
		});
	eight_.button_click(
		[this]() {
			this->func_('8');
		});
	nine_.button_click(
		[this]() {
			this->func_('9');
		});
	c_.button_click(
		[this]() {
			this->func_('c');
		});
	star_.button_click(
		[this]() {
			this->func_('*');
		});
	zero_.button_click(
		[this]() {
			this->func_('0');
		});
	pound_.button_click(
		[this]() {
			this->func_('#');
		});
	d_.button_click(
		[this]() {
			this->func_('D');
		});
}

void pin_pad::on_button_press(const std::function<void(char)> func)
{
	func_ = func;
}