#include <iostream>
#include <wiringPi.h>

#include "include/led.h"
#include "include/pin.h"
#include "include/pin_pad.h"

using namespace std;

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
        pin(6));
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