#pragma once
class pin
{
	int wiring_pi_pin_number_;

public:
	/**
	 * \brief Represents a single pin.
	 * \param wiring_pi_pin_number The number of the pin using wiringPi numbering.
	 */
	explicit pin(int wiring_pi_pin_number);
	
	void set_read_mode() const;
	void set_write_mode() const;
	void set_change_mode(bool start_low = true) const;
	void turn_on() const;
	void turn_off() const;
	bool is_on() const;
	template<typename T>
	void on_change(void (T::*)()) const;
};