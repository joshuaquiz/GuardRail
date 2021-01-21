#include <wiringPi.h>

#include "include/led.h"
#include "include/pin.h"
#include "include/pin_pad.h"

using namespace std;

button *b = nullptr;

void on_button_pressed()
{
    static unsigned long button_pressed_timestamp;
    const auto is_pin_on = (*b).neg_pin.is_on();
    if (is_pin_on) {
        button_pressed_timestamp = millis();
        if ((*b).button_down_func)
        {
            (*b).button_down_func();
        }

        (*b).currently_on = true;
    }
    else {
        const auto duration = millis() - button_pressed_timestamp;
        if ((*b).button_up_func)
        {
            (*b).button_up_func();
        }

        button_pressed_timestamp = millis();
        if (duration < 100) {
            return;
        }

        if ((*b).button_click_func)
        {
            (*b).button_click_func();
        }

        (*b).currently_on = false;
    }
}

int main()
{
    wiringPiSetup();
    const pin buzz_pin(7);
    buzz_pin.set_write_mode();
    const auto red_light_pin = led(pin(0));
    const auto green_light_pin = led(pin(0));
    auto pad = pin_pad(
        pin(26),
        pin(27),
        pin(28),
        pin(29),

        pin(1),
        pin(4),
        pin(5),
        pin(6),

        b,
        on_button_pressed);
    pad.on_button_press([green_light_pin](const char c)
    {
	    if (c == '9') {
            green_light_pin.turn_on();
	    } else {
            green_light_pin.turn_off();
        }
    });

    buzz_pin.turn_on();
    delay(500);
    buzz_pin.turn_off();
    red_light_pin.turn_on();
    delay(500);
    red_light_pin.turn_off();
    green_light_pin.turn_on();
    delay(500);
    green_light_pin.turn_off();
	
    return 0;
}