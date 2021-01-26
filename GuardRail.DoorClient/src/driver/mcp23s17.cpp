/* 
    Title --- driver/mcp23s17.cpp

    Copyright (C) 2013 Giacomo Trudu - wicker25[at]gmail[dot]com

    This file is part of Rpi-hw.

    Rpi-hw is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation version 3 of the License.

    Rpi-hw is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Rpi-hw. If not, see <http://www.gnu.org/licenses/>.
*/


#ifndef _RPI_HW_DRIVER_MCP23S17_CPP_
#define _RPI_HW_DRIVER_MCP23S17_CPP_

#include "../../include/rpi-hw/driver/mcp23s17.hpp"

namespace rpihw { // Begin main namespace

namespace driver { // Begin drivers namespace

mcp23s17::mcp23s17( const std::string &dev_path, uint8_t dev_id )

	: mcp23x17	( dev_path )
	, m_spi		( new driver::spi( dev_path, driver::spi::MODE_0, 8, 10000000 ) )
	, m_dev_id	( dev_id ) {

	// Initialize the expander
	init();
}

mcp23s17::~mcp23s17() {

	// Destroy the SPI controller
	delete m_spi;
}

void
mcp23s17::send( uint8_t reg, uint8_t data ) {

	// Build the buffer to send
	m_buffer[0] = WRITE | m_dev_id << 1;
	m_buffer[1] = reg;
	m_buffer[2] = data;

	// Send data to the device
	m_spi->transfer( m_buffer, 3 );
}

uint8_t
mcp23s17::receive( uint8_t reg ) {

	// Build the buffer to send
	m_buffer[0] = READ | m_dev_id << 1;
	m_buffer[1] = reg;

	// Receive data from the device
	m_spi->transfer( m_buffer, 3 );

	return m_buffer[2];
}

} // End of drivers namespace

} // End of main namespace

#endif /* _RPI_HW_DRIVER_MCP23S17_CPP_ */
