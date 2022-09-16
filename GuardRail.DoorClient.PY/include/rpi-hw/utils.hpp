/* 
    Title --- utils.hpp

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

#ifndef RPI_HW_UTILS_HPP
#define RPI_HW_UTILS_HPP

#include <cstdio>
#include <cstdarg>
#include <cstdint>
#include <cstddef>

#include <string>

#include "types.hpp"

namespace rpihw
{
    /*!
	    @namespace rpihw::utils
	    @brief Namespace of the utils functions.
    */
	namespace utils
    {
    	//! Parameters of text.
		enum text_flags {
			align_left = 0x01,
			align_center = 0x02,
			align_right = 0x04,
			word_wrap = 0x08,
			word_break = 0x10
		};

		/*!
			@brief Sets a bit in a variable.
			@param[in] buffer The buffer containing the data.
			@param[in] offset The offset position in the buffer.
			@param[in] index The bit position.
			@param[in] value The bit value.
		*/
		template<typename T>
		void set_bit(T& buffer, size_t offset, uint8_t index, bool value = 1);

		/*!
			@brief Clears a bit in a variable.
			@param[in] buffer The buffer containing the data.
			@param[in] offset The offset position in the buffer.
			@param[in] index The bit position.
		*/
		template<typename T>
		void clear_bit(T& buffer, size_t offset, uint8_t index);

		/*!
			@brief Flips a bit value in a variable.
			@param[in] buffer The buffer containing the data.
			@param[in] offset The offset position in the buffer.
			@param[in] index The bit position.
		*/
		template<typename T>
		void flip_bit(T& buffer, size_t offset, uint8_t index);

		/*!
			@brief Returns a bit value in a variable.
			@param[in] buffer The buffer containing the data.
			@param[in] offset The offset position in the buffer.
			@param[in] index The bit position.
			@return The bit value.
		*/
		template<typename T>
		bool get_bit(T& buffer, size_t offset, uint8_t index);

		//! Returns a formatted string like `printf`.
		std::string format(const char* format, ...);

		/*!
			@brief Aligns a text.
			@param[in] text The string to align.
			@param[in] width The width of the text.
			@param[in] flags The parameters of the text.
			@return The aligned text.
		*/
		template <typename T>
		std::basic_string<T> align(
			const std::basic_string<T>& text,
			size_t width,
			uint8_t flags = align_left);
	}
}

// Include inline methods 
#include "utils-inl.hpp"

#endif /* RPI_HW_UTILS_HPP */
