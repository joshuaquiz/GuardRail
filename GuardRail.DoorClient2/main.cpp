#include <iostream>
#include <memory>

// Include Rpi-hw headers
#include "include/rpi-hw/keypad/base.hpp"
#include "include/rpi-hw/keypad/matrix.hpp"


// Use Rpi-hw namespace
using namespace rpihw;

/*
      (14, 15, 18, 23)   colums = 4
            ||||
   ----------------------
   | (1)  (2)  (3)  (A) |
   |                    |
   | (4)  (5)  (6)  (B) |
   |                    |
   | (7)  (8)  (9)  (C) |
   |                    |
   | (*)  (0)  (#)  (D) |
   ----------------------
            ||||
       (24, 25, 8, 7)  rows = 4
*/

/** The class of my application **/
class my_app {

public:

    // Define the keymap
    std::vector< uint8_t > keymap = {

        '1', '2', '3', 'A',
        '4', '5', '6', 'B',
        '7', '8', '9', 'C',
        '*', '0', '#', 'D'
    };

    /** Constructor method **/
    my_app() : m_keypad_(new keypad::matrix({ 14, 15, 18, 23 }, { 24, 25, 8, 7 }, keymap)) {
        const keypad::T_EventListener listener = std::bind(&my_app::event_listener, this, std::placeholders::_1);

        // Add the keypad event listener
        m_keypad_->addEventListener(listener);
    }

    /** Destructor method **/
    ~my_app() = default;

    /** A simple keypad event listener **/
    void event_listener(keypad::base& dev) {

        const auto& key_state = dev.keyState();

        for (auto c : key_state)
            std::cout << static_cast<char>(c) << std::flush;
    }

    /** Main loop **/
    static void run() {

        for (;; ) {

            /* ... */
        }
    }

private:

    // The keypad instance.
    std::unique_ptr< keypad::matrix > m_keypad_{};
};


int
main(int argc, char* args[]) {

    my_app app;

    my_app::run();

    return 0;
}

/*
 *using namespace std;
using namespace rpihw;

constexpr auto  row_1_pin_num = 1;
constexpr auto  row_2_pin_num = 4;
constexpr auto  row_3_pin_num = 5;
constexpr auto  row_4_pin_num = 6;

constexpr auto  col_1_pin_num = 26;
constexpr auto  col_2_pin_num = 27;
constexpr auto  col_3_pin_num = 28;
constexpr auto  col_4_pin_num = 29;

int main()
{
    //wiringPiSetup();

    /*auto buzz_pin_num = 7;
    auto* const buzz_pin = new pin(&buzz_pin_num);
    (*buzz_pin).set_write_mode();
    auto red_light_pin_num = 0;
    auto* const red_light_pin = new pin(&red_light_pin_num);
    const auto red_light = led(red_light_pin);
    auto green_light_pin_num = 2;
    auto* const green_light_pin = new pin(&green_light_pin_num);
    const auto green_light = led(green_light_pin);#1#

    const rpihw::keypad::matrix dev({ 26, 27, 28, 29 }, { 1, 4, 5, 6 });

    for (;; ) {

        // Check some keys state
        if (dev.pressed(0))
            std::cout << "You have pressed button 0!\n";

        if (dev.released(2))
            std::cout << "You have released button 2!\n";

        if (dev.pressed(1) && dev.pressed(4))
            std::cout << "You have pressed buttons 1 and 4!\n";

        // Wait some time
        rpihw::time::msleep(100);
    }

    /*auto col_1_pin_num = COL_1_PIN_NUM;
    auto COL_2_PIN_NUM = 27;
    auto COL_3_PIN_NUM = 28;
    auto COL_4_PIN_NUM = 29;

    auto row_1_pin_num = 1;
    auto row_2_pin_num = 4;
    auto row_3_pin_num = 5;
    auto row_4_pin_num = 6;

    auto* const col_1_pin = new pin(static_cast<new int>(COL_1_PIN_NUM));
    auto* const col_2_pin = new pin(&COL_2_PIN_NUM);
    auto* const col_3_pin = new pin(&COL_3_PIN_NUM);
    auto* const col_4_pin = new pin(&COL_4_PIN_NUM);

    auto* const row_1_pin = new pin(&row_1_pin_num);
    auto* const row_2_pin = new pin(&row_2_pin_num);
    auto* const row_3_pin = new pin(&row_3_pin_num);
    auto* const row_4_pin = new pin(&row_4_pin_num);

    one_ = new button(row_1_pin, col_1_pin, on_button_pressed);
    two_ = new button(row_1_pin, col_2_pin, on_button_pressed);
    three_ = new button(row_1_pin, col_3_pin, on_button_pressed);
    a_ = new button(row_1_pin, col_4_pin, on_button_pressed);

    four_ = new button(row_2_pin, col_1_pin, on_button_pressed);
    five_ = new button(row_2_pin, col_2_pin, on_button_pressed);
    six_ = new button(row_2_pin, col_3_pin, on_button_pressed);
    b_ = new button(row_2_pin, col_4_pin, on_button_pressed);

    seven_ = new button(row_3_pin, col_1_pin, on_button_pressed);
    eight_ = new button(row_3_pin, col_2_pin, on_button_pressed);
    nine_ = new button(row_3_pin, col_3_pin, on_button_pressed);
    c_ = new button(row_3_pin, col_4_pin, on_button_pressed);

    star_ = new button(row_4_pin, col_1_pin, on_button_pressed);
    zero_ = new button(row_4_pin, col_2_pin, on_button_pressed);
    pound_ = new button(row_4_pin, col_3_pin, on_button_pressed);
    d_ = new button(row_4_pin, col_4_pin, on_button_pressed);#1#

    /*buzz_pin->turn_on();
    delay(500);
    buzz_pin->turn_off();
    red_light.turn_on();
    delay(500);
    red_light.turn_off();
    green_light.turn_on();
    delay(500);
    green_light.turn_off();#1#
    
    return 0;
}*/