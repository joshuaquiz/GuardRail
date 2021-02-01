#include <inttypes.h>
#include <iostream>
#include <memory>
#include "include/rpi-hw/keypad/matrix.hpp"

#include "include/nfc/nfc.h"
#include "include/nfc/target-subr.h"
#include "include/nfc/chips/pn53x.h"

using namespace rpihw;

class my_app {

public:

	// Define the keymap
	std::vector<uint8_t> keymap = {
		'1', '2', '3', 'A',
		'4', '5', '6', 'B',
		'7', '8', '9', 'C',
		'*', '0', '#', 'D'
	};

	/** Constructor method **/
	my_app()
    : m_keypad_(
		new keypad::matrix(
			// cols
			{ 17, 27, 22, 5 },
			// rows
			{ 23, 24, 25, 16 },
			keymap))
    {
        const keypad::t_event_listener listener = std::bind(&my_app::event_listener, this, std::placeholders::_1);

		// Add the keypad event listener
		m_keypad_->add_event_listener(listener);
	}

	my_app(my_app&) = delete;
	my_app(const my_app&) = delete;
	my_app(my_app&&) = delete;
	my_app& operator=(my_app&) = delete;
	my_app& operator=(const my_app&) = delete;
	my_app& operator=(const my_app&&) = delete;

	~my_app() = default;

    /** A simple keypad event listener **/
	void event_listener(keypad::keypad_base& dev)
    {
		auto pressed = false;
		std::string line = "";
		for (auto i = 0; i < 4; i++)
		{
            const auto r = i * 4;
			if (dev.pressed(r % 4 + r) || dev.pressed(r % 4 + 1 + r) || dev.pressed(r % 4 + 2 + r) || dev.pressed(r % 4 + 3 + r))
			{
				pressed = true;
				for (auto j = 0; j < 4; j++)
				{
					if (!dev.pressed(r + j))
					{
						line += keymap[r + j];
						break;
					}
				}
			}
		}

		if (pressed)
		{
			std::cout << line << std::endl;
		}
	}

	static void run()
	{
		while (true)
		{
		}
	}

private:

	// The keypad instance.
	std::unique_ptr<keypad::matrix> m_keypad_{};
};
#define MAX_DEVICE_COUNT 16

static nfc_device* pnd = NULL;
static nfc_context* context;

static void stop_polling(int sig)
{
	(void)sig;
	if (pnd != NULL)
		nfc_abort_command(pnd);
	else {
		nfc_exit(context);
		exit(EXIT_FAILURE);
	}
}

static void
print_usage(const char* progname)
{
	printf("usage: %s [-v]\n", progname);
	printf("  -v\t verbose display\n");
}

int
main(int argc, const char* argv[])
{
	my_app app;

	bool verbose = false;

	signal(SIGINT, stop_polling);

	// Display libnfc version
	const char* acLibnfcVersion = nfc_version();

	printf("%s uses libnfc %s\n", argv[0], acLibnfcVersion);
	if (argc != 1) {
		if ((argc == 2) && (0 == strcmp("-v", argv[1]))) {
			verbose = true;
		}
		else {
			print_usage(argv[0]);
			exit(EXIT_FAILURE);
		}
	}

	const uint8_t uiPollNr = 20;
	const uint8_t uiPeriod = 2;
	const nfc_modulation nmModulations[6] = {
	  {.nmt = NMT_ISO14443A, .nbr = NBR_106 },
	  {.nmt = NMT_ISO14443B, .nbr = NBR_106 },
	  {.nmt = NMT_FELICA, .nbr = NBR_212 },
	  {.nmt = NMT_FELICA, .nbr = NBR_424 },
	  {.nmt = NMT_JEWEL, .nbr = NBR_106 },
	  {.nmt = NMT_ISO14443BICLASS, .nbr = NBR_106 },
	};
	const size_t szModulations = 6;

	nfc_target nt;
	int res = 0;

	nfc_init(&context);
	if (context == NULL) {
		exception("Unable to init libnfc (malloc)");
		exit(EXIT_FAILURE);
	}

	pnd = nfc_open(context, NULL);

	if (pnd == NULL) {
		exception("%s", "Unable to open NFC device.");
		nfc_exit(context);
		exit(EXIT_FAILURE);
	}

	if (nfc_initiator_init(pnd) < 0) {
		nfc_perror(pnd, "nfc_initiator_init");
		nfc_close(pnd);
		nfc_exit(context);
		exit(EXIT_FAILURE);
	}

	printf("NFC reader: %s opened\n", nfc_device_get_name(pnd));
	printf("NFC device will poll during %ld ms (%u pollings of %lu ms for %" PRIdPTR " modulations)\n", (unsigned long)uiPollNr * szModulations * uiPeriod * 150, uiPollNr, (unsigned long)uiPeriod * 150, szModulations);
	if ((res = nfc_initiator_poll_target(pnd, nmModulations, szModulations, uiPollNr, uiPeriod, &nt)) < 0) {
		nfc_perror(pnd, "nfc_initiator_poll_target");
		nfc_close(pnd);
		nfc_exit(context);
		exit(EXIT_FAILURE);
	}

	if (res > 0) {
		print_nfc_target(&nt, verbose);
		printf("Waiting for card removing...");
		fflush(stdout);
		while (0 == nfc_initiator_target_is_present(pnd, NULL)) {}
		nfc_perror(pnd, "nfc_initiator_target_is_present");
		printf("done.\n");
	}
	else {
		printf("No target found.\n");
	}

	nfc_close(pnd);
	nfc_exit(context);
	exit(EXIT_SUCCESS);
	my_app::run();
}
